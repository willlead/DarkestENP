using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class WayPoint : MonoBehaviour {

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

    public Vector3 GetDirection(Transform tAi)
    {
        if (Vector3.Distance(tAi.position, m_Waypoints[m_iNextIndex].position) < m_fRadius)
        {
            if (!b_isHitRadius)
            {
                b_isHitRadius = true;
                m_iWayIndex = m_iNextIndex;
                if(m_bOrderDirection == true)
                {
                    m_iNextIndex = (m_iNextIndex + 1) % m_iWayLength;
                }
                else
                {
                    int int_randomWay = (int)Mathf.Floor(Random.value * m_iWayLength);
                    if (m_iWayLength >1 )
                    {
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
        Vector3 v3_currentPositioin = new Vector3(tAi.position.x, m_Waypoints[m_iNextIndex].position.y, tAi.position.z);
        m_vDirection = (m_Waypoints[m_iNextIndex].position - v3_currentPositioin).normalized;
        return m_vDirection;
    }


    public Vector3 GetDirectionToPlayer (Transform ai, Transform m_Player)
    {
        float fAiHeight = m_Waypoints[m_iWayIndex].position.y;
        Vector3 v3_currentPosition = new Vector3(ai.position.x, fAiHeight, ai.position.z);
        Vector3 v3_playerPosition = new Vector3(m_Player.position.x, fAiHeight, m_Player.position.z);
        m_vDirection = (v3_playerPosition - v3_currentPosition).normalized;
        return m_vDirection;
    }

    public bool AwayFromWaypoint(Transform ai, float distance)
    {
        if (Vector3.Distance(ai.position, m_Waypoints[m_iNextIndex].position)>= distance)
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
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
