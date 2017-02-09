using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public partial class EnemyMgr : MonoBehaviour
{
    private CharacterController m_Ctl;
    private CollisionFlags m_CollisionFlags;
    public Waypoints m_WayPoint;
    public RocketLauncher m_RocketLauncher;
    public Transform m_Player;
    public Animation m_Animation;
    public AnimationClip m_IdleAnimation;
    public AnimationClip m_WalkAnimation;
    public AnimationClip m_RunAnimation;
    public AnimationClip m_ShotAnimation;
    public AnimationClip m_DieAnimation;

    public float m_fWalkSpeed = 1.5f;
    public float m_fIdleSpeed = 1.0f;
    public float m_fRunSpeed = 2.0f;
    public float m_fShotSpeed = 0.5f;
    public int m_iRunSpeed = 6;
    public int m_iWalkSpeed = 2;
    public float m_fJumpSpeed = 8.0f;
    public float m_fGravity = 20.0f;
    // 발사 거리
    private float m_fShotRange = 15.0f;
    // 발사 거리에 더해 지는 추적 가능 거리
    public float m_fPlayerRange = 5.0f;
    // 웨이포인트로부터 벗어 날 수 있는 최대 거리
    public float m_fWaypointDistance = 10.0f;
    // 4초 동안 걸은 뒤에 멈춰서 생각하는 동작을 취한다.
    public float m_fWalkingTime = 4.0f;
    // 2초간 멈춰서 쉬고, 다시 걷기 시작한다.
    public float m_fThinkingTime = 2.0f;
    public float m_fHP = 100;
    private float m_fMaxHP = 100;
    private float m_fVerticalSpeed = 1.0f;
    private float m_fMoveSpeed = 0.0f;
    private Vector3 m_vMoveDirection = Vector3.zero;
    private bool m_bRun;
    private bool m_bAiming;
    private bool m_bJumping;
    private bool m_bStop;
    // 발사 
    private bool m_bPrepare = false;
    private bool m_bShot = false;
    private float m_fRotateSpeed = 1.0f;
    private float m_fLastTime = 0.0f;
    public GameObject m_Ragdoll;

    public ShotGun m_PlayerShotGun;
    public Camera fpsCam;
    public float weaponRange = 50f;

    private bool m_bDead = false;

    public float GetHpPercent()
    {
        return m_fHP / m_fMaxHP;
    }

    public void Awake()
    {
        m_Ctl = GetComponent<CharacterController>();
        m_bRun = false;
        m_bAiming = false;
        m_bJumping = false;
        m_fMoveSpeed = m_iWalkSpeed;
        m_CollisionFlags = CollisionFlags.CollidedBelow;
        m_fMoveSpeed = m_iWalkSpeed;
        // 특정 시점에 멈추게 한다.
        m_fLastTime = Time.time;
        m_bStop = false;
        m_fMaxHP = m_fHP;

        m_Animation[m_WalkAnimation.name].speed = m_fWalkSpeed;
        m_Animation[m_WalkAnimation.name].wrapMode = WrapMode.Loop;

        m_Animation[m_RunAnimation.name].speed = m_fRunSpeed;
        m_Animation[m_RunAnimation.name].wrapMode = WrapMode.Loop;

        m_Animation[m_IdleAnimation.name].speed = m_fIdleSpeed;
        m_Animation[m_IdleAnimation.name].wrapMode = WrapMode.Loop;
    }
    // Use this for initialization
    void Start()
    {
        transform.position = m_WayPoint.StartPosition();
    }
    public bool IsGrounded()
    {
        return (m_CollisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (StaticVars.b_isGameOver == false)
        {
            if (collision.transform.tag == "Rocket")
            {
                Rocket rocket = collision.gameObject.GetComponent<Rocket>();
                float f_damege = rocket.getDamage();
                //m_fHP = Mathf.Clamp(m_fHP - f_damege, 0, m_fMaxHP);
                m_fHP -= f_damege;
                // b_isGotHit = true;
                if (m_fHP <= 0)
                {
                    m_fHP = 0.0f;



                    GameObject obj_aiPrefab = GameObject.Instantiate(m_Ragdoll, transform.position, transform.rotation) as GameObject;

                    float f_force = 1000;
                    Rigidbody[] a_rigid = obj_aiPrefab.GetComponentsInChildren<Rigidbody>();

                    foreach (Rigidbody r in a_rigid)
                    {
                        r.AddExplosionForce(f_force, rocket.transform.position, 100.0f);
                    }
                    GameObject.Destroy(transform.parent.gameObject);

                }
            }
        }
    }

    // 점프 필요여부를 판정한다.
    public bool Jump(Vector3 _direction)
    {
        RaycastHit hit;
        Vector3 p1 = transform.position + m_Ctl.center + Vector3.up * (-m_Ctl.height * 0.5f);
        Vector3 p2 = p1 + Vector3.up * m_Ctl.height;

        if ((Physics.CapsuleCast(p1, p2, 0.1f, _direction, out hit)))
        {
            if (hit.transform.tag == "Wall")
            {
                return true;
            }
        }
        //// Ray 사용
        //RaycastHit hit;
        //Vector3 v3_leg = transform.position + m_Ctl.center + Vector3.up * (-m_Ctl.height * 0.5f);
        //float f_distance = m_Ctl.m_fRadius * 2;

        //if ((Physics.Raycast(v3_leg, _direction, out hit, f_distance))
        //    &&
        //    (m_CollisionFlags & CollisionFlags.Sides) != 0)
        //{
        //    if (hit.transform.tag == "Wall")
        //    {
        //        return true;
        //    }
        //}
        return false;
    }
    public bool Run()
    {
        // 추적 필요 확인
        if ((Vector3.Distance(transform.position, m_Player.position) <= (m_fPlayerRange + m_fShotRange))
            &&
            (Vector3.Distance(transform.position, m_Player.position) > m_fShotRange))
        {
            m_bRun = true;
        }
        else
        {
            m_bRun = false;
        }
        return m_bRun;
    }
    // 걷거나 쉬기 위한 시간을 계산
    public bool IsThinking()
    {
        float f_time;
        if (m_bStop)
        {
            f_time = m_fThinkingTime;
        }
        else
        {
            f_time = m_fWalkingTime;
        }
        if (Time.time >= (m_fLastTime + f_time))
        {
            if (m_bStop)
            {
                m_bStop = false;
            }
            else
            {
                m_bStop = true;
            }
            m_fLastTime = Time.time;
        }
        return m_bStop;
    }
    public void NormalMove(Vector3 vRocketDirection)
    {
        Vector3 v3_movement;
        Vector3 v3_targetDirection;
        // 추적 중이라면 방향을 계산
        if (m_bRun)
        {
            v3_targetDirection = m_WayPoint.GetDirectionToPlayer(transform, m_Player);
        }
        else
        {
            if (m_fThinkingTime > 0)
            {
                if (!IsThinking())
                {
                    // 목표 방향
                    v3_targetDirection = m_WayPoint.GetDirection(transform);
                }
                else
                {
                    v3_targetDirection = Vector3.zero;
                }
            }
            else
            {
                v3_targetDirection = m_WayPoint.GetDirection(transform);
            }
        }

        if (v3_targetDirection != Vector3.zero)
        {
            // 목표 방향을 향해 회전
            m_vMoveDirection = Vector3.Slerp(m_vMoveDirection, v3_targetDirection, m_fRotateSpeed * Time.deltaTime);
            m_vMoveDirection = m_vMoveDirection.normalized;
        }
        else
        {
            m_vMoveDirection = Vector3.zero;
        }

        if (!m_bJumping)
        {
            if (m_bRun)
            {
                m_bRun = true;
                m_fMoveSpeed = m_iRunSpeed;
            }
            else
            {
                m_bRun = false;
                m_fMoveSpeed = m_iWalkSpeed;
            }
            if ((Jump(m_vMoveDirection)) && (m_bJumping == false))
            {
                m_bJumping = true;
                m_fVerticalSpeed = m_fJumpSpeed;
            }
        }

        if (IsGrounded())
        {
            m_fVerticalSpeed = 0.0f;
            m_bJumping = false;
        }
        else
        {
            m_fVerticalSpeed -= m_fGravity * Time.deltaTime;
        }

        v3_movement = (m_vMoveDirection * m_fMoveSpeed) + new Vector3(0, m_fVerticalSpeed, 0);
        v3_movement *= Time.deltaTime;
        m_bPrepare = false;

        if (m_vMoveDirection != Vector3.zero)
        {
            if (m_fMoveSpeed == m_iWalkSpeed)
            {
                m_Animation.CrossFade(m_WalkAnimation.name);
            }
            else
            {
                m_Animation.CrossFade(m_RunAnimation.name);
            }
        }
        else
        {
            m_Animation.CrossFade(m_IdleAnimation.name);
        }

        m_CollisionFlags = m_Ctl.Move(v3_movement);
        if ((m_vMoveDirection != Vector3.zero) && (!m_bAiming))
        {
            transform.rotation = Quaternion.LookRotation(m_vMoveDirection);
        }
    }
    void Update()
    {
        if (m_bDead == true)
            return;

        if (Input.GetMouseButtonDown(0))
        {


            if (m_PlayerShotGun.m_Animation.isPlaying == false)
            {
                // Create a vector at the center of our camera's viewport
                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

                // Declare a raycast hit to store information about what our raycast has hit
                RaycastHit hit;

                // Check if our raycast has hit anything
                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
                {


                    m_fHP -= 20;

                    if (m_fHP <= 0.0f && m_bDead == false)
                    {
                        m_bDead = true;
                        m_Animation.CrossFade(m_DieAnimation.name);

                        Invoke("DestroyNow", 10.0f);

                    }


                    //int a = 1;

                    //// Set the end position for our laser line 
                    //laserLine.SetPosition(1, hit.point);

                    //// Get a reference to a health script attached to the collider we hit
                    //ShootableBox health = hit.collider.GetComponent<ShootableBox>();

                    //// If there was a health script attached
                    //if (health != null)
                    //{
                    //    // Call the damage function of that script, passing in our gunDamage variable
                    //    health.Damage(gunDamage);
                    //}

                    //// Check if the object we hit has a rigidbody attached
                    //if (hit.rigidbody != null)
                    //{
                    //    // Add force to the rigidbody we hit, in the direction from which it was hit
                    //    hit.rigidbody.AddForce(-hit.normal * hitForce);
                    //}
                }
            }
        }





        if ((m_Player != null) && (m_fHP > 0))
        {
            if (StaticVars.b_isGameOver == false)
            {
                Vector3 vPos = m_Player.position;
                vPos.y += 1.0f;
                Vector3 vRocketDirection = (vPos - transform.position).normalized;
                // 조준을 시작한 상태가 아니라면 추적, 
                if (true
                    //Shoot(vRocketDirection)
                    )
                {
                    Run();
                }
                else
                {
                    // 웨이포인트로부터 특정 거리 이상 떨어져 있는지 확인
                    // m_fWaypointDistance 이상 떨어져 있으면 추적이나 공격을 멈추고 
                    // 웨이포인트로 돌아간다.
                    if (m_WayPoint.AwayFromWaypoint(transform, m_fWaypointDistance))
                    {
                        m_bAiming = false;
                        m_bRun = false;
                    }
                }
                if (!m_bAiming)
                {
                    NormalMove(vRocketDirection);
                }
                else
                {
                    //AttackMove(vRocketDirection);
                }
            }
            else
            {
                m_Animation.CrossFade(m_IdleAnimation.name);
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (m_Player != null)
        {
            Gizmos.color = Color.blue;
            Vector3 vPos = m_Player.position;
            vPos.y += 1.0f;
            Gizmos.DrawLine(transform.position, vPos);
        }
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, (m_fPlayerRange + m_fShotRange));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_fShotRange);
    }


    public void DestroyNow()
    {
        //transform.DetachChildren();
        GameObject.Destroy(gameObject);
    }
}
