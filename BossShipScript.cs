using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShipScript : MonoBehaviour
{
    public bool isActived;

    [Header("BossObj")]
    public GameObject BossObject;
    public Animator BossAnimator;
    public GameObject BossSpawnEffect;
    public GameObject BossTeleportEffect;
    public Transform BossSpawnPoint;

    [Header("Weaponarys")]
    public Transform[] CannonHeads;
    public GameObject CannonFireEffect;
    public GameObject[] CannonSpawningEnemyPrefabs;
    public float FireForce;
    public Transform[] MissileHeads;
    public GameObject MissilePrj;

    [Header("SFX")]
    public AudioSource sfx;
    public AudioClip CannonFireSound;

    [Header("Stats")]
    public float BehaviourRate;
    public float DelayTimer;

    [Header("Others")]
    public GameObject FighterPrefab;
    public Transform FighterSpawnPoint;
    public ParticleSystem[] UnderSmokes;

    void Start()
    {
        sfx = GetComponent<AudioSource>();
        isActived = false;

        BossObject = transform.Find("Boss").gameObject;
        BossAnimator = BossObject.GetComponentInChildren<Animator>();

        BossAnimator.SetBool("SIT", true);
        BossAnimator.SetBool("WALKING", false);

        DelayTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActived)
            return;

        DelayTimer += Time.deltaTime;

        if (DelayTimer > BehaviourRate)
        {
            DOSOMETHING();
        }
    }

    public void DOSOMETHING()
    {
        DelayTimer = 0;
        int A = Random.Range(0, 101);

        if(A <= 5)
        {
            StartCoroutine(SpawnFighters());
        }
        else if(A > 5 && A <= 100)
        {
            StartCoroutine(MissileBarrage());
        }
    }

    public IEnumerator SpawnFighters()
    {
        GameObject f = Instantiate(FighterPrefab, FighterSpawnPoint.position, Quaternion.identity);
        Vector3 lerpos = f.transform.position + f.transform.right * 220;
        for(int i = 0; i < 350; i++)
        {
            //Y Pos must be 140.5
            f.transform.position = Vector3.Slerp(f.transform.position, lerpos, 0.01f);
            yield return new WaitForFixedUpdate();
            if(i > 60)
            {
                lerpos.y = 140.5f;
            }
        }
        yield return new WaitForSeconds(1);
        f.GetComponent<EnemyController>().agent.enabled = true;
        f.GetComponent<EnemyController>().SpawnDone = true;
    }

    public void SpawnPhase()
    {
        StartCoroutine(SpawningAttack());
    }

    public IEnumerator SpawningAttack()
    {
        foreach(Transform t in CannonHeads)
        {
            for (int i = 0; i < 2; i++) 
            {
                Instantiate(CannonFireEffect, t.position, t.rotation);
                GameObject prj = Instantiate(CannonSpawningEnemyPrefabs[Random.Range(0, CannonSpawningEnemyPrefabs.Length)], t.position, t.rotation);
                prj.transform.Rotate(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
                prj.GetComponent<Rigidbody>().AddForce(FireForce * prj.transform.right, ForceMode.Impulse);
            }
        }
        //sfx.PlayOneShot(CannonFireSound);
        yield return new WaitForSeconds(3f);
        Vector3 Rot = transform.rotation.eulerAngles;
        Vector3 Rot2 = Rot + (transform.right * 30);

        Vector3 Pos = transform.position;
        Debug.Log(Pos);
        Pos += transform.forward * 120;
        Pos.y = -45.7f;
        Debug.Log(Pos);
        for(int i = 0; i < 350; i++)
        {
            yield return new WaitForFixedUpdate();

            transform.position = Vector3.Lerp(transform.position, Pos, 0.003f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Rot2), 0.003f);
        }
        foreach(ParticleSystem p in UnderSmokes)
        {
            p.Play();
        }

        for (int i = 0; i < 500; i++)
        {
            yield return new WaitForFixedUpdate();

            transform.position = Vector3.Lerp(transform.position, Pos, 0.01f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Rot), 0.005f);
        }

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForFixedUpdate();

            transform.position = Vector3.Lerp(transform.position, Pos, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Rot), 0.1f);
        }
        foreach (ParticleSystem p in UnderSmokes)
        {
            p.Stop();
        }
        BossAnimator.SetTrigger("SPAWN");
        yield return new WaitForSeconds(0.1f);
        BossAnimator.SetBool("SIT", false);
        yield return new WaitForSeconds(6.5f);

        Instantiate(BossSpawnEffect, BossSpawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        Instantiate(BossTeleportEffect, BossSpawnPoint.position, Quaternion.identity);
        BossAnimator.SetTrigger("TELE");
        BossObject.transform.position = BossSpawnPoint.position;

        yield return new WaitForSeconds(6);
        BossObject.GetComponent<BossMoveScript>().SpawnDone();
        BossObject.transform.parent = null;

        GameObject.Find("Canvas").GetComponent<UIScript>().BossWaveStarted(BossObject);

        isActived = true;
        StopAllCoroutines();
    }

    public IEnumerator MissileBarrage()
    {
        foreach(Transform t in MissileHeads)
        {
            yield return new WaitForSeconds(0.03f);
            Instantiate(MissilePrj, t.position, t.rotation);
        }
    }
}
