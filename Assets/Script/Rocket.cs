using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ConstantForce))]
public class Rocket : MonoBehaviour
{

    public float timeOut = 3.0f;
    private float f_damage = 20;
    public GameObject explosionParticle;

    public float explosionRadius = 2.0f;
    public float explosionForce = 100.0f;

    //Set Damage
    public void setDamage(float _damage)
    {
        f_damage = _damage;
    }
    //Get Damage
    public float getDamage()
    {
        return f_damage;
    }

    // Use this for initialization
    void Start()
    {
        Invoke("KillObject", timeOut);
    }
    public void OnCollisionEnter(Collision others)
    {
        ContactPoint contactPoint = others.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up,
                                                        contactPoint.normal);
        GameObject.Instantiate(explosionParticle, contactPoint.point, rotation);

        Vector3 v3_position = transform.position;
        Collider[] a_hits = Physics.OverlapSphere(v3_position, explosionRadius);

        Debug.Log(others.gameObject.name);

        if (others.gameObject.tag != "Player")
        {
            foreach (Collider c in a_hits)
            {
                if (c.tag == "Destructible")
                {
                    Rigidbody r = c.GetComponent<Rigidbody>();
                    if (r != null)
                    {
                        r.isKinematic = false;
                        r.AddExplosionForce(explosionForce, v3_position, explosionRadius);
                    }
                }
            }
        }

        //KillObject(0.0f);
        KillObject();
    }

    public void KillObject()
    {
        GameObject.Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
