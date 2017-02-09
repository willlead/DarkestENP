using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGAME;
using UnityStandardAssets.Characters.FirstPerson;

public class ShotGun : MonoBehaviour {

    public CharacterController m_Ctl;
    public Animation m_Animation;
    private AnimationClip m_Shoot;
    public FirstPersonController m_Fpc;

    void Awake()
    {
        m_Ctl = GetComponent<CharacterController>();
        m_Animation = GetComponent<Animation>();
        m_Shoot = m_Animation.GetClip("shoot");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton("Fire1"))
        {
            if(m_Animation.isPlaying == false)
            {
                m_Fpc.m_iBullet -= 1;
                m_Animation.CrossFade(m_Shoot.name, 0.1f);
                AudioMgr.Instance.PlayEffect("shot1");
            }
        }
	}
}