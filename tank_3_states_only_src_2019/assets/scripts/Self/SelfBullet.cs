using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfBullet : MonoBehaviour
{
    //public GameObject Explosion;
    public int ammoType { private get; set; }
    public int shootFromWhichFaction { private get; set; }
    public int shooter { get; set; }
    public GameObject bulletModel;
    public float damage { set; private get; }
    public float armorPiercing { set; private get; }
    public float speed { set; private get; }
    public float LifeTime = 3.0f;
    bool damageDone = false;
    float bulletRadius;
    // public float maxDistance { private get; set; }
    float movingDistance;
    // float movedDistance;
    Rigidbody rb;
    CapsuleCollider cc;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        bulletRadius = cc.radius;
        cc.enabled = false;
        Destroy(gameObject, LifeTime);
    }

    void Update()
    {
        movingDistance = Time.deltaTime * speed;
        // if (movedDistance + movingDistance > maxDistance){ movingDistance = maxDistance - movedDistance; }
        // else { movedDistance += movingDistance; }
        CheckBetweenNextPosition();
        transform.position += 
			transform.forward * movingDistance;
        //rb.AddForce(transform.forward * speed * Time.deltaTime);
        //rb.MovePosition(transform.position + speed * Time.deltaTime * transform.forward);
        //transform.Translate( Vector3.forward * Time.deltaTime * speed);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    ContactPoint contact = collision.contacts[0];
    //}
    void CheckBetweenNextPosition()
    {
        if(!damageDone)
        {
            RaycastHit hit;

            Vector3 p = transform.position ;
            if (Physics.SphereCast(p, bulletRadius, transform.forward, out hit, movingDistance, 1 << 8 | 1 << 12))
            {
                switch(hit.collider.gameObject.layer)
                {
                    case 8:
                        ArmorInteract tank = hit.collider.gameObject.GetComponent<ArmorInteract>();
                        if (shootFromWhichFaction != tank.faction)
                        {
                            transform.position += transform.forward * hit.distance;
                            tank.DamageDeal(ammoType,damage, armorPiercing, transform, shooter, shootFromWhichFaction);
                            OnDamageDone();
                        }
                        break;
                    default:
                        transform.position += transform.forward * hit.distance;
                        OnDamageDone();
                        break;
                }
            }
        }        
    }
    //void OnDrawGizmos() 
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, 3f);        
    //}
    void OnDamageDone()
    {
        damageDone = true;
        speed = -100f; 
        rb.mass = 10000;
        rb.useGravity = true;
        Destroy(bulletModel);
        movingDistance = Time.deltaTime * speed;
        //cc.enabled = false;
        //Instantiate(Explosion, contact.point, Quaternion.identity);
    }
}
