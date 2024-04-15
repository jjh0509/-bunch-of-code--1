using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingRocket_Enemy : MonoBehaviour
{
    public Transform Target;
    public Transform Dir;
    public float Damage;
    public float Speed;
    public float HomingSpeed;
    public float HomingLifeTime;

    public Rigidbody rb;

    public float MaxLifeTime;
    public float LifeTime;

    public GameObject ExplodeEffect;
    // Start is called before the first frame update
    void Start()
    {
        LifeTime = 0;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Dir = transform.Find("Dir").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime += Time.deltaTime;

        if (LifeTime > MaxLifeTime)
            Explode();

        Dir.LookAt(Target);
        HomingSpeed = Mathf.Lerp(HomingSpeed, 0, Time.deltaTime / HomingLifeTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Dir.rotation, Time.deltaTime * HomingSpeed);

        rb.velocity = transform.forward * Speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        GameObject explosion = Instantiate(ExplodeEffect, transform.position, Quaternion.identity);
        Explosion_Particle_Damage_Radius_BothSide explosionDmg;
        if(explosion.TryGetComponent<Explosion_Particle_Damage_Radius_BothSide>(out explosionDmg))
        {
            explosionDmg = explosion.GetComponent<Explosion_Particle_Damage_Radius_BothSide>();
            explosionDmg.dmg = Damage;
        }

        Destroy(gameObject);
    }
}
