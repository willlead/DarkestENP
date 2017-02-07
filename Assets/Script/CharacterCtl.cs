using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGAME;

[RequireComponent(typeof(CharacterController))]
public class CharacterCtl : MonoBehaviour {

    private AnimationClip m_IdleClip;
    private AnimationClip m_WalkClip;
    private AnimationClip m_RunClip;
    private AnimationClip m_JumpClip;
    private AnimationClip m_FallClip;
    private Animation m_Animation;

    private float m_fJumpAnimationSpeed = 4.0f;
    private float m_fFallAnimationSpeed = 0.1f;
    private float m_fRunAnimationSpeed = 1.5f;
    private float m_fWalkAnimationSpeed = 1.5f;
    private float m_fIdleAnimationSpeed = 0.5f;

    private float m_fSpeed = 2.0f;
    private float m_fRunSpeed = 5.0f;
    private float m_fJumpSpeed = 8.0f;
    private float m_fGravity = 20.0f;
    private CharacterController m_Controller;

    private float m_fVerticalSpeed = 0.0f;
    private float m_fMoveSpeed = 0.0f;
    private Vector3 m_vMoveDirection = Vector3.zero;
    private bool m_bRun;
    private bool b_isBackward;
    private bool m_bJumping;

    private Quaternion m_qCurrentRotation;
    private Quaternion m_qRot;
    private float m_fRotateSpeed = 1.0f;

    private Vector3 m_vForward;
    private Vector3 m_vRight;
    private CollisionFlags m_CollisionFlags;
    private float m_fAirTime = 0.0f;
    private float m_fAirStartTime = 0.0f;
    private float m_fMinAirTime = 0.15f;

    private float m_fLockCameraTimer = 0.0f;
    private bool m_bMovingBack = false;
    private bool m_bMoving = false;
    private bool m_bWasMoving = false;

    void GetPosition(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.transform.tag == "item")
            {
                GameObject obj = PrefabMgr.Instance("CharEffect4", "CharEffect4", hit.point.x, hit.point.y, hit.point.z);
                Destroy(hit.transform.gameObject, 0.2f);
            }
            GameObject obj2 = PrefabMgr.Instance("CharEffect4", "CharEffect4", hit.point.x, hit.point.y, hit.point.z);
        }
    }

    void Awake()
    {
        IGame.Init();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
