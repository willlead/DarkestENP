using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ConstantForce))]
public class RocketLauncher : MonoBehaviour
{

    // rauncher
    public GameObject smoke;
    private GameObject smokeClone;
    public Transform smokePosition;
    public ConstantForce rocket;

    public float speed = 10F;
    public int ammoCount = 20;
    private float lastShot = 0.0F;

    public void Fire(float _reloadTime)
    {
        if (Time.time > (_reloadTime + lastShot) && ammoCount > 0)
        {
            ConstantForce rocketPrefab = ConstantForce.Instantiate(rocket, transform.position, transform.rotation) as ConstantForce;
            rocketPrefab.relativeForce = new Vector3(0, 0, speed);
            smokeClone = GameObject.Instantiate(smoke, smokePosition.position, smokePosition.rotation) as GameObject;
            lastShot = Time.time;
            //ammoCount--;
        }
    }
    public void Reload()
    {
        ammoCount = 20;
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
