using UnityEngine;
using System.Collections;
using System.Linq;

public class Waypoints : MonoBehaviour
{

    public string m_IconName = "wayIcon.psd";
    public float m_fRadius = 1.0f;
    public bool m_bOrderDirection = false;

    public Transform[] m_Waypoints;
    private int m_iWayIndex;
    private int m_iNextIndex;
    private int m_iWayLength;
    private Vector3 m_vDirection;

    private bool b_isHitRadius;

    T GetComponentInChildrenByName<T>(string name) where T : Component
    {
        return (from i in GetComponentsInChildren<T>() where i.name == name select i).Single();
    }

    public void Awake()
    {
        m_Waypoints = gameObject.GetComponentsInChildren<Transform>();
        m_iWayLength = m_Waypoints.Length;
        m_iWayIndex = 0;
        m_iNextIndex = 1;

        if (m_bOrderDirection == false)
        {
            int int_randomWay = (int)Mathf.Floor(Random.value * m_iWayLength);
            if (m_iWayLength > 1)
            {
                while (m_iWayIndex == int_randomWay)
                {
                    int_randomWay = (int)Mathf.Floor(Random.value * m_iWayLength);
                }
            }
            m_iWayIndex = int_randomWay;
        }
        m_vDirection = Vector3.zero;
        b_isHitRadius = true;
    }

    public Vector3 StartPosition()
    {
        return m_Waypoints[0].position;
    }
    // 다음 웨이포인트로 가기 위한 이동 방향을 얻는다.
    public Vector3 GetDirection(Transform tAi)
    {
        // 웨이 포인트와 충돌 체크
        if (Vector3.Distance(tAi.position, m_Waypoints[m_iNextIndex].position) < m_fRadius)
        {
            if (!b_isHitRadius)
            {
                b_isHitRadius = true;
                m_iWayIndex = m_iNextIndex;
                if (m_bOrderDirection == true)
                {
                    // 다음 웨이포인트 얻는다.
                    m_iNextIndex = (m_iNextIndex + 1) % m_iWayLength;
                }
                else
                {
                    // 무작위 순서에 따라 이동
                    int int_randomWay = (int)Mathf.Floor(Random.value * m_iWayLength);
                    if (m_iWayLength > 1)
                    {
                        // 무작위 선정된 인덱스 사용.
                        while (m_iWayIndex == int_randomWay)
                        {
                            int_randomWay = (int)Mathf.Floor(Random.value * m_iWayLength);
                        }
                    }
                    m_iNextIndex = int_randomWay;
                }
            }
            else
            {
                b_isHitRadius = false;
            }
        }
        //  현재 위치에서 다음 웨이 포인트 방향을 구한다.
        Vector3 v3_currentPosition = new Vector3(tAi.position.x, m_Waypoints[m_iNextIndex].position.y, tAi.position.z);
        m_vDirection = (m_Waypoints[m_iNextIndex].position - v3_currentPosition).normalized;
        return m_vDirection;
    }
    // 현재 위치에서 부터 유저를 향하도록 계산한다.
    public Vector3 GetDirectionToPlayer(Transform ai, Transform m_Player)
    {
        float fAiHeight = m_Waypoints[m_iWayIndex].position.y;
        Vector3 v3_curentPosition = new Vector3(ai.position.x, fAiHeight, ai.position.z);
        Vector3 v3_playerPosition = new Vector3(m_Player.position.x, fAiHeight, m_Player.position.z);
        m_vDirection = (v3_playerPosition - v3_curentPosition).normalized;
        return m_vDirection;
    }
    // 유저를 향해 움지이고 있는 동안 다음 웨이포인트 간의 거리 계산한다.    
    public bool AwayFromWaypoint(Transform ai, float distance)
    {
        if (Vector3.Distance(ai.position, m_Waypoints[m_iNextIndex].position) >= distance)
        {
            return true;
        }
        return false;
    }

    public void OnDrawGizmos()
    {
        Transform[] waypointGizmos = gameObject.GetComponentsInChildren<Transform>();
        if (waypointGizmos != null)
        {
            if (m_bOrderDirection == true)
            {
                for (int i = 0; i < waypointGizmos.Length; i++)
                {
                    Gizmos.color = Color.red;
                    int n = (i + 1) % waypointGizmos.Length;
                    Gizmos.DrawLine(waypointGizmos[i].position, waypointGizmos[n].position);
                    Gizmos.DrawIcon(waypointGizmos[i].position, m_IconName);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(waypointGizmos[i].position, m_fRadius);
                }
            }
            else
            {
                for (int j = 0; j < waypointGizmos.Length; j++)
                {
                    for (int k = 0; k < waypointGizmos.Length; k++)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(waypointGizmos[j].position, waypointGizmos[k].position);
                    }
                    Gizmos.DrawIcon(waypointGizmos[j].position, m_IconName);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(waypointGizmos[j].position, m_fRadius);
                }
            }
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
