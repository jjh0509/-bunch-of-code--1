using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearDropScript : MonoBehaviour
{
    public float DelayTime;
    public float ExplosionDelayTime;

    public float SpawnHeight;
    public float DropForce;

    public GameObject CollisionEffect;
    public GameObject ExplosionPrefab;
    public Vector3 EffectOffset;

    public ParticleSystem trailParticle;

    public Collider trigger;

    public Transform HeadOfSpear;

    public Vector3 CollisionPos;
    public Quaternion rot;

    public Rigidbody rb;
    public AudioSource sfx;
    public AudioClip GroundHitSound;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        sfx = GetComponent<AudioSource>();
        trigger = GetComponent<Collider>();
        HeadOfSpear = transform.Find("Head").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trailParticle = GetComponentInChildren<ParticleSystem>();
        trigger.enabled = false;
        rb.useGravity = false;
        transform.localScale = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Translate(transform.up * SpawnHeight);

        trailParticle.Stop();

        rot = Quaternion.Euler(180 + Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForFixedUpdate();

            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1.25f, 1), 0.1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.1f);
        }

        yield return new WaitForSeconds(DelayTime);

        trailParticle.Play();
        trigger.enabled = true;
        rb.AddForce(transform.up * DropForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        CollisionPos = other.transform.position;
        CollisionPos.x = HeadOfSpear.position.x;
        CollisionPos.z = HeadOfSpear.position.z;
        trailParticle.Stop();
        rb.isKinematic = true;
        Instantiate(CollisionEffect, CollisionPos + EffectOffset, Quaternion.identity);
        sfx.PlayOneShot(GroundHitSound);
        StartCoroutine(CollisionCoroutine());
    }


    private IEnumerator CollisionCoroutine()
    {
        yield return new WaitForSeconds(ExplosionDelayTime);

        Instantiate(ExplosionPrefab, CollisionPos + EffectOffset, Quaternion.Euler(rot.eulerAngles.x - 180, -rot.eulerAngles.y, -rot.eulerAngles.z));

        Destroy(gameObject);
    }
}
