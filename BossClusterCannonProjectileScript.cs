using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossClusterCannonProjectileScript : MonoBehaviour
{
    public ParticleSystem[] particles;
    public TrailRenderer trail;
    public MeshRenderer rend;

    public Transform Destination;
    public Vector3 TargetPoint;
    public float ExplodeHeight;
    public float ExplosionOffset;
    public float Speed;

    public bool Exploded;
    public GameObject ExplosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Exploded = false;
        particles = GetComponentsInChildren<ParticleSystem>();
        trail = GetComponentInChildren<TrailRenderer>();
        rend = GetComponent<MeshRenderer>();

        Destination = transform.Find("Destination").GetComponent<Transform>();

        ExplodeHeight = Vector3.Distance(transform.position, TargetPoint) / 4;
        TargetPoint.y = ExplodeHeight;

        Destination.position = TargetPoint;
        Destination.parent = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Exploded)
        {
            transform.LookAt(Destination);
            transform.Translate(transform.forward * Speed);
        }

        if(Vector3.Distance(transform.position, TargetPoint) < ExplosionOffset)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (Exploded)
            return;
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        foreach (ParticleSystem p in particles)
        {
            p.Stop();
        }
        trail.emitting = false;
        rend.enabled = false;
        Exploded = true;
        Destroy(Destination.gameObject);
        Destroy(gameObject, 4);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Destination1"))
            Explode();
    }
}
