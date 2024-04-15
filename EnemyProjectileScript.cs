using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    public Rigidbody rb;
    public float dmg;
    public GameObject ExplodeParticle;
    public bool isExplosive;
    public bool is90DegreeOffseted;
    public bool isReversed;
    private int reverseMultiplier;

    public float timeToDestroyWhenDetect;
    public Collider _collider;

    public float FireForce;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = true;

        reverseMultiplier = 1;
        if (isReversed)
            reverseMultiplier = -1;
        rb = GetComponent<Rigidbody>();
        if(is90DegreeOffseted)
            rb.AddForce(transform.up * FireForce * reverseMultiplier);
        else
            rb.AddForce(transform.forward * FireForce * reverseMultiplier);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && !isExplosive)
        {
            collision.transform.GetComponent<PlayerController_CharacterController>().TakeDamage(dmg);
        }

        if(collision.transform.CompareTag("Tower") && !isExplosive)
        {
            collision.transform.GetComponent<TowerScript>().TakeDamage(dmg);
        }

        if(collision.transform.CompareTag("TowerShield") && !isExplosive)
        {
            collision.transform.GetComponentInParent<TowerScript>().TakeDamage(dmg);
        }

        if (collision.transform.CompareTag("MainBase"))
        {
            collision.transform.GetComponent<MainBaseScript>().TakeDamage(dmg);
        }

        if (collision.transform.CompareTag("MainBaseGraphic"))
        {
            collision.transform.GetComponentInParent<MainBaseScript>().TakeDamage(dmg);
        }

        if (!(ExplodeParticle == null))
        {
            GameObject Particle = Instantiate(ExplodeParticle, collision.contacts[0].point, Quaternion.identity);
            Destroy(Particle, timeToDestroyWhenDetect);
        }
        _collider.enabled = false; 
        rb.isKinematic = true;
        Destroy(gameObject, timeToDestroyWhenDetect);
    }
}
