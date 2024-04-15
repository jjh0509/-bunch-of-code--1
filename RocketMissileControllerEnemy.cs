using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketMissileControllerEnemy : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject ExplosionPrefab;
    public Vector3 targetPoint;

    [Header("Stat")]
    public float Speed;
    public float Spread;
    public float Damage;
    public float MaxLifeTime;
    private float lifetime;

    public float DetectStartTime;
    private float timerdetect;

    public Vector3 StartRotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        timerdetect = 0;
        lifetime = 0;

        transform.LookAt(targetPoint);
        transform.Rotate(Random.Range(-Spread,Spread), 0,Random.Range(-Spread,Spread));

        rb = GetComponent<Rigidbody>();

        transform.Rotate(StartRotationOffset);

        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        timerdetect += Time.deltaTime;
        lifetime += Time.deltaTime;
        rb.velocity = -transform.up * Speed;
        if (lifetime > MaxLifeTime)
            Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        if (timerdetect < DetectStartTime)
            return;

        GameObject explode = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);

        Explosion_Particle_Damage_Radius_BothSide epd;
        if (explode.TryGetComponent<Explosion_Particle_Damage_Radius_BothSide>(out epd))
        {
            epd = explode.GetComponent<Explosion_Particle_Damage_Radius_BothSide>();
            epd.dmg = Damage;
        }
        Destroy(gameObject);
    }
}
