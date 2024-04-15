using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawningProjectileScript : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject SpawningEnemy;
    public GameObject CollisionEffect;
    public MeshRenderer mrenderer;
    public float FirePreDelay;
    private float spawnedTime;
    public float ContactHeightOffset;
    public float Speed;
    public float Spread;
    public float TimeToDestroy;
    public bool Collided;
    public Collider col;
    public float CollisionStartTime;

    public Quaternion lookdir;

    public ParticleSystem[] particles;

    public Vector3 collisionPoint;

    public float ExtraGravity;

    public bool DontUseThis;

    public bool isWaveSpawning;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
        Collided = false;
        particles = GetComponentsInChildren<ParticleSystem>();
        mrenderer = transform.Find("Graphic").GetComponent<MeshRenderer>();

        spawnedTime = 0;
        rb = GetComponent<Rigidbody>();

        if (!DontUseThis)
        {
            rb.useGravity = false;

            transform.Rotate(Random.Range(-Spread, Spread), Random.Range(-Spread, Spread), Random.Range(-Spread, Spread));

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit))
            {
                collisionPoint = hit.point;
            }

            mrenderer.enabled = false;



            yield return new WaitForSeconds(FirePreDelay - 0.5f);

            foreach (ParticleSystem p in particles)
            {
                if (p.transform.CompareTag("IgnorePlayParticle1"))
                    continue;
                p.Play();
            }

            yield return new WaitForSeconds(0.5f);

            rb.velocity = (transform.up * -Speed);
            lookdir = transform.rotation;
            mrenderer.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        spawnedTime += Time.deltaTime;
        if (spawnedTime > CollisionStartTime && !col.enabled)
            col.enabled = true;

        if (DontUseThis)
        {
            if(transform.position.y < 0 && !Collided)
            {
                DoSpawn();
            }
            return;
        }
        if (FirePreDelay < spawnedTime && transform.position.y < collisionPoint.y && !Collided)
        {
            DoSpawn();
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * ExtraGravity, ForceMode.Force);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(!Collided)
            DoSpawn();
    }

    void DoSpawn()
    {
        if (DontUseThis)
        {
            Vector3 vec = transform.position;
            vec.y = 0;
            transform.position = vec;
        }
        rb.isKinematic = true;
        transform.rotation = lookdir;
        GameObject spawned = Instantiate(SpawningEnemy, new Vector3(transform.position.x, collisionPoint.y + ContactHeightOffset, transform.position.z), Quaternion.identity);
        spawned.GetComponent<EnemyController>().WaveSpawnedEntity = isWaveSpawning;
        mrenderer.enabled = false;
        foreach (ParticleSystem p in particles)
        {
            p.Stop();
        }
        Collided = true;
        Destroy(gameObject, TimeToDestroy);
    }
}
