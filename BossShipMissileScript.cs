using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShipMissileScript : MonoBehaviour
{
    public Vector3 TargetPoint;
    public float TargetPointSpread;
    public Transform Dir;
    public float Damage;
    public float Speed;
    public float ImpulseSpeed;
    public float HomingSpeed;

    public GameObject ExplosionRadiusPreEffect;
    public GameObject ExplosionPrefab;

    public Rigidbody rb;

    public float DirChangeAfterTime;
    public float MaxLifeTime;
    public float LifeTime;
    public float DetectionStartTime;

    public Collider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
        LifeTime = 0;
        rb = GetComponent<Rigidbody>();
        Dir = transform.Find("Dir").GetComponent<Transform>();
        TargetPoint = GameObject.Find("Player").GetComponent<Transform>().position;

        TargetPoint.x += Random.Range(-TargetPointSpread, TargetPointSpread);
        TargetPoint.z += Random.Range(-TargetPointSpread, TargetPointSpread);
        TargetPoint.y = 0.1f;

        rb.AddForce(transform.forward * ImpulseSpeed, ForceMode.Impulse);

        Instantiate(ExplosionRadiusPreEffect, TargetPoint, Quaternion.identity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (DirChangeAfterTime < LifeTime)
        {
            rb.useGravity = false;
            Dir.LookAt(TargetPoint);

            transform.rotation = Quaternion.Lerp(transform.rotation, Dir.rotation, HomingSpeed);
            rb.velocity = Vector3.Slerp(rb.velocity, transform.forward * Speed, HomingSpeed);
        }
        else
        {
            rb.AddForce(Vector3.up * 200,ForceMode.Force);
        }
    }

    private void Update()
    {
        LifeTime += Time.deltaTime;

        if(DetectionStartTime < LifeTime)
        {
            col.enabled = true;
        }

        if (LifeTime > MaxLifeTime)
            Explosion();
    }

    private void Explosion()
    {
        GameObject explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponentInChildren<Explosion_Particle_Damage_Radius_BothSide>().dmg = Damage;
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explosion();
    }
}
