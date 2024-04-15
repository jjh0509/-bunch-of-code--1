using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionScript : MonoBehaviour
{
    public bool isPlayerPrj;

    [Header("Stats")]
    public float Damage;
    public bool isArmorPiercing;
    public bool isExplosive;
    public bool isLooping;
    public bool playOnAwake;

    [Header("Particles")]
    public ParticleSystem particle;
    public ParticleSystem[] particle_childs;
    public GameObject Collision_Explosion;
    public GameObject Collision_Particle;
    public float particleDestroytime;
    public List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        particle_childs = GetComponentsInChildren<ParticleSystem>();

        collisionEvents = new List<ParticleCollisionEvent>();

        if (particle_childs.Length > 0)
        {
            foreach (ParticleSystem p in particle_childs)
            {
                p.loop = isLooping;
                if(playOnAwake)
                    p.Play();
            }
        }

        particle = GetComponent<ParticleSystem>();
    }
    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = 0;
        if (collisionEvents != null)
            numCollisionEvents = particle.GetCollisionEvents(other, collisionEvents);
        int i = 0;
        if (numCollisionEvents < 1)
            return;
        /*
        if (other.CompareTag("EnemyGraphic"))
        {
            other.GetComponentInParent<EnemyController>().TakeDamage(Damage, isArmorPiercing, false);
        }
        */
        while (i < numCollisionEvents)
        {
            Vector3 pos = collisionEvents[i].intersection;
            if (isExplosive)
            {
                GameObject explosion = Instantiate(Collision_Explosion, pos, Quaternion.identity);
                explosion.GetComponentInChildren<Explosion_Particle_Damage_Radius_BothSide>().dmg = Damage;
                explosion.GetComponentInChildren<Explosion_Particle_Damage_Radius_BothSide>().isPlayerProjectile = isPlayerPrj;
            }
            if(!(Collision_Particle == null))
            {
                GameObject particle = Instantiate(Collision_Particle, pos, Quaternion.identity);
                if(particleDestroytime > 0)
                    Destroy(particle, particleDestroytime);
            }
            i++;
        }
    }

        /*
        Debug.Log("AAAAAWERWEJHWEIQER");
        if (other.CompareTag("Enemy") && !isExplosive)
        {
            EnemyController controller = other.GetComponent<EnemyController>();
            controller.TakeDamage(Damage);
        }

        if (isExplosive)
        {
            Instantiate(Collision_Effect, , Quaternion.identity);
        }
        */
    /*private void OnCollisionEnter(Collision collision)
    {
        if (isExplosive)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Instantiate(Collision_Explosion, contact.point, Quaternion.identity);
            }
        }
    }*/
}
