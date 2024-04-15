using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyProjectileScript : MonoBehaviour
{
    Rigidbody rb;

    public float Damage;
    public float BlastPower;
    public float Spread;
    public bool zoomed;
    public GameObject ExplodeObject;
    public bool playerProjectile;

    public float ExplosionForce;
    public float ExplosionRadius;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.Rotate(new Vector3(Random.Range(-Spread * 50, Spread * 50), Random.Range(-Spread * 50, Spread * 50), 0));
        rb.AddForce(transform.forward * BlastPower, ForceMode.Impulse);

        if(zoomed)
            rb.AddForce(transform.up * (BlastPower / 12), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player") || other.transform.CompareTag("Boundary") || other.transform.CompareTag("Projectile"))
            return;
        GameObject boom = Instantiate(ExplodeObject, transform.position, transform.rotation);
        boom.GetComponentInChildren<Explosion_Particle_Damage_Radius_BothSide>().isPlayerProjectile = playerProjectile;
        boom.GetComponentInChildren<Explosion_Particle_Damage_Radius_BothSide>().dmg = Damage;
        Destroy(gameObject);
    }
}
