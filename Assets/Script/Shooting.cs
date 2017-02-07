using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TGAME
{
    public class Shooting : MonoBehaviour
    {
        float timer2;
        public float timeBetweenBullets2 = 0.15f;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButton("Fire1") && timer2 >= timeBetweenBullets2 && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot();
            }
        }

        void Shoot()
        {

        }
    }

}
