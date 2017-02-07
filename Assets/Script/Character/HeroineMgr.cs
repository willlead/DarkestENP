using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 기본 제어 및 백 뷰 카메라로 사용됨.
public class HeroineMgr : MonoBehaviour {

    public CharacterController m_Ctl;
    private Animation     m_Animation;
    private AnimationClip m_Idle;
    private AnimationClip m_Walk;
    private AnimationClip m_Run;
    private AnimationClip m_Jump;
    private AnimationClip m_Fall;
    private Vector3 m_vMoveDirection = Vector3.zero;
    private float m_fJumpSpeed = 8.0f;
    private float m_fVerticalSpeed = 0.0f;
    private CollisionFlags m_CollisionFlags;
    private bool m_bJump = false;
    private bool m_bRun = false;
    void Awake()
    {
        m_Ctl = GetComponent<CharacterController>();
        m_Animation = GetComponent<Animation>();
        m_Idle = m_Animation.GetClip("Idle");
        m_Walk = m_Animation.GetClip("Walk");
        m_Run = m_Animation.GetClip("Run");
        m_Jump = m_Animation.GetClip("Jump");
        m_Fall = m_Animation.GetClip("Fall");
    }
    // Use this for initialization
    void Start () {
       m_Animation.CrossFade(m_Walk.name, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        Transform m_CameraTransform = Camera.main.transform;
        Vector3 vForward = m_CameraTransform.TransformDirection(Vector3.forward);
        vForward.y = 0;
        Vector3 vRight = new Vector3(vForward.z, 0, -vForward.x);
        float fHor = Input.GetAxis("Horizontal");
        float fVer = Input.GetAxis("Vertical");
        Vector3 vTargetDirection = (fHor * vRight) + (fVer * vForward);
        if (vTargetDirection != Vector3.zero)
        {
            Vector3 vMoveDirection = Vector3.Slerp(m_vMoveDirection, vTargetDirection,
                Time.deltaTime);
            m_vMoveDirection = vMoveDirection.normalized;
            m_Animation.CrossFade(m_Walk.name, 0.1f);
        }
        else
        {
            m_vMoveDirection = Vector3.zero;
        }

        bool bGround = (CollisionFlags.CollidedBelow & m_CollisionFlags ) != 0;
        if (bGround)
        {
            m_bJump = false;
            m_fVerticalSpeed = 0.0f;
            m_bRun = false;
            if ( Input.GetKey(KeyCode.LeftShift))
            {
                m_bRun = true;
            }
            if (Input.GetButton("Jump"))
            {
                m_fVerticalSpeed = m_fJumpSpeed;
                m_bJump = true;               
            }
        }
        else
        {
            m_fVerticalSpeed -= 20.0f * Time.deltaTime;
        }

        Vector3 vMovement = (m_vMoveDirection) + new Vector3(0, m_fVerticalSpeed, 0);
        vMovement *= Time.deltaTime;
        m_CollisionFlags = m_Ctl.Move(vMovement);

        if (m_bJump)
        {
            if (m_Ctl.velocity.y > 0.0f)
            {
                m_Animation.CrossFade(m_Jump.name, 0.1f);
            }
            else
            {
                m_Animation.CrossFade(m_Fall.name, 0.1f);
            }
        }
        else
        {
            if (m_Ctl.velocity.sqrMagnitude < 0.1f)
            {
                if (bGround == true )
                    m_Animation.CrossFade(m_Idle.name, 0.1f);
            }
            else
            {
                if (m_bRun)
                    m_Animation.CrossFade(m_Run.name, 0.1f);
                else
                    m_Animation.CrossFade(m_Walk.name, 0.1f);
            }
        }
        if (m_vMoveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(m_vMoveDirection);
        }
       
    }
    void LateUpdate()
    {
        Vector3 vForward = transform.TransformDirection(Vector3.forward);
        Vector3 vDir = transform.position  + (-vForward * 5.0f);
        Camera.main.transform.rotation = Quaternion.LookRotation(vForward);
        Camera.main.transform.position = new Vector3(vDir.x,Camera.main.transform.position.y,vDir.z);
    }        
}
