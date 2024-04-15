using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMoveScript : MonoBehaviour
{
    public bool Spawned;

    [Header("Components")]
    public EnemyController mainController;
    public Animator anim;
    public AudioSource sfx;
    public NavMeshAgent agent;
    public Transform Target;
    public Transform player;

    [Header("Swords")]
    public ParticleSystem[] SwordParticles;
    public TrailRenderer[] SwordTrails;
    public GameObject SwordSlashPrj;
    public GameObject SwordDamageRadius;
    public GameObject SwordDamageRadiusPreEffect;
    public Transform SwordPrjPoint;

    [Header("Hammers")]
    public GameObject HammerDamageRadiusEffect;
    public GameObject HammerShockEffect;
    public Transform HammerHead;
    public AudioClip HammerHitGroundSfx;

    [Header("Cannons")]
    public GameObject CannonProjectile;
    public Transform CannonFirePoint;

    [Header("Scythes")]
    public TrailRenderer HandTrail;
    public GameObject[] SpawningPrefabs;
    public Transform[] SpawningPoints;
    public GameObject ScytheEffect;
    public Transform ScytheEffectPoint;

    [Header("Spears")]
    public GameObject SpearPrj;
    public GameObject SpearGroundHitEffect;
    public Transform SpearGroundHitPoint;
    public Transform SpearHead;
    public TrailRenderer[] SpearTrails;

    [Header("Axes")]
    public GameObject AxeAttack_IceEffect;
    public GameObject AxeAttack_FireEffect;
    public Transform AxeAttack_IcePoint;
    public Transform AxeAttack_FirePoint;

    [Header("Shields")]
    public TrailRenderer[] ShieldTrails;
    public ParticleSystem ShieldOrbitParticle;
    public Transform ShieldForceFieldTrs;
    public float ShieldCurrentTime;
    public bool ShieldActived;
    public Vector3 ShieldScale;

    [Header("Stats")]
    public float BehaviourRate;
    public float BehaviourRange;
    public float StopRange;
    public float PlayerDismissRange;
    public float DeltaTimer;
    public float Speed;
    public float ShieldTime;
    public bool isMoving;

    [Header("Others")]
    public bool AnimationPlaying;
    public bool IsDied;
    [SerializeField] private float RemainingDistanceToTarget;
    public float LastScytheUsedTime;
    public float RateLimitTime_Scythe;
    public float LastShieldActivedTime;
    public float RateLimitTime_Shield;
    // Start is called before the first frame update
    void Start()
    {
        LastScytheUsedTime = 0;
        LastShieldActivedTime = 0;
        IsDied = false;
        AnimationPlaying = false;
        mainController = GetComponent<EnemyController>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        ShieldCurrentTime = 0;
        DeltaTimer = 0;
        Spawned = false;
        agent = GetComponent<NavMeshAgent>();
        sfx = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();

        agent.enabled = false;

        foreach (TrailRenderer t in SwordTrails)
        {
            t.emitting = false;
        }

        foreach (TrailRenderer t in ShieldTrails)
        {
            t.emitting = false;
        }

        foreach (TrailRenderer t in SpearTrails)
        {
            t.emitting = false;
        }

        HandTrail.emitting = false;

        ShieldForceFieldTrs.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!(Target==null))
            RemainingDistanceToTarget = Vector3.Distance(transform.position, Target.position);
        LastScytheUsedTime += Time.deltaTime;
        LastShieldActivedTime += Time.deltaTime;

        if (!Spawned || IsDied)
            return;

        if(mainController.current_HP <= 0 && !IsDied)
        {
            DEAD();
        }

        if (Target == null)
        {
            Target = FindClosestTower().GetComponent<Transform>();
            if (!(Target == null))
                agent.SetDestination(Target.position);
            else
                return;
        }

        if (Target.CompareTag("Tower"))
        {
            if (Target.GetComponent<TowerScript>().isDowned)
            {
                Target = null;
                return;
            }
        }
        else if (Target.CompareTag("Player"))
        {
            if (Vector3.Distance(Target.position, transform.position) > PlayerDismissRange)
            {
                Target = null;
                return;
            }
        }


        if (AnimationPlaying)
        {
            agent.speed = 0;
            return;
        }

        if (agent.remainingDistance > StopRange)
        {
            isMoving = true;
            agent.speed = Speed;
        }
        else
        {
            isMoving = false;
            agent.speed = 0;
        }

        DeltaTimer += Time.deltaTime;
        ShieldCurrentTime -= Time.deltaTime;

        if (DeltaTimer > BehaviourRate && RemainingDistanceToTarget < BehaviourRange)
        {
            DOSOMETHING();
        }

        if (ShieldActived && ShieldCurrentTime < 0)
        {
            mainController.DamageReduceAmount = 0;
            ShieldActived = false;
            StartCoroutine(ShieldOffCoroutine());
        }

        anim.SetBool("WALKING", isMoving);
    }

    public void DEAD()
    {
        IsDied = true;
        anim.SetTrigger("DEATH");
        StartCoroutine(EndGame());
    }

    public IEnumerator EndGame()
    {
        yield return new WaitForSeconds(10);
        Debug.Log("DEATH");
    }

    IEnumerator ShieldOnCoroutine()
    {
        for (int i = 0; i < 400; i++)
        {
            yield return new WaitForFixedUpdate();

            ShieldForceFieldTrs.localScale = Vector3.Lerp(ShieldForceFieldTrs.localScale, ShieldScale, 0.04f);
        }
    }

    IEnumerator ShieldOffCoroutine()
    {
        for (int i = 0; i < 400; i++)
        {
            yield return new WaitForFixedUpdate();

            ShieldForceFieldTrs.localScale = Vector3.Lerp(ShieldForceFieldTrs.localScale, new Vector3(0.1f, 0.1f, 0.1f),0.04f);
        }
    }

    public void DOSOMETHING()
    {
        if(RemainingDistanceToTarget < 250 && RemainingDistanceToTarget > 100)
        {
            int A = Random.Range(1, 4);
            if (A == 1)
            {
                if (LastShieldActivedTime < RateLimitTime_Shield)
                    return;
                anim.SetTrigger("SHIELD");
            }
            else if (A == 2)
            {
                if (LastScytheUsedTime < RateLimitTime_Scythe)
                    return;
                anim.SetTrigger("SCYTHE");
            }
            else if (A == 3)
            {
                anim.SetTrigger("SWORD");
            }
        }
        else if (RemainingDistanceToTarget <= 100)
        {
            int A = Random.Range(1, 7);
            if (A == 1)
            {
                if (RemainingDistanceToTarget > 70)
                    return;

                anim.SetTrigger("HAMMER");
            }
            else if (A == 2)
            {
                anim.SetTrigger("SWORD");
            }
            else if (A == 3)
            {
                if (LastShieldActivedTime < RateLimitTime_Shield)
                    return;
                anim.SetTrigger("SHIELD");
            }
            else if (A == 4)
            {
                if (LastScytheUsedTime < RateLimitTime_Scythe)
                    return;
                anim.SetTrigger("SCYTHE");
            }
            else if (A == 5)
            {
                anim.SetTrigger("SPEAR");
            }
            else if (A == 6)
            {
                if (RemainingDistanceToTarget > 70)
                    return;

                anim.SetTrigger("AXE");
            }
        }
        else if(RemainingDistanceToTarget >= 250)
        {
            anim.SetTrigger("CANNON");
        }

        

        DeltaTimer = 0;
    }

    public void SpawnDone()
    {
        Spawned = true;
        agent.enabled = true;
    }

    public GameObject FindClosestTower()
    {
        GameObject[] AllTowers = GameObject.FindGameObjectsWithTag("Tower");

        GameObject closestTower = null;
        float closestDistance = Mathf.Infinity;
        int numoftower = 0;
        foreach (var tower in AllTowers)
        {
            if (!tower.GetComponent<TowerScript>().isDowned)
            {
                numoftower++;
            }
        }
        if (numoftower < 1)
            return GameObject.Find("MainBase").gameObject;
        foreach (var tower in AllTowers)
        {
            float distance = Vector3.Distance(transform.position, tower.transform.position);
            if (!tower.transform.GetComponent<TowerScript>().isDowned)
            {
                if (distance < closestDistance)
                {
                    closestTower = tower;
                    closestDistance = distance;
                }
            }
            if (distance < closestDistance)
            {
                closestTower = tower;
                closestDistance = distance;
            }
        }
        if (Vector3.Distance(transform.position, player.position) < closestDistance && Vector3.Distance(transform.position, player.position) < PlayerDismissRange)
            closestTower = player.gameObject;
        return closestTower;
    }

    public void HammerPreEffect()
    {
        Instantiate(HammerDamageRadiusEffect, HammerHead.position, Quaternion.identity);
    }

    public void HammerEffect()
    {
        Vector3 pos = HammerHead.position;
        pos.y = 0.1f;
        Instantiate(HammerShockEffect, pos, Quaternion.identity);
        sfx.PlayOneShot(HammerHitGroundSfx);
    }

    public void SwordTrailOn()
    {
        foreach(TrailRenderer t in SwordTrails)
        {
            t.emitting = true;
        }
    }
    public void SwordTrailOff()
    {
        foreach (TrailRenderer t in SwordTrails)
        {
            t.emitting = false;
        }
    }

    public void SwordParticleOn()
    {
        Instantiate(SwordDamageRadiusPreEffect, transform.position + transform.forward * 171, transform.rotation);
        foreach (ParticleSystem p in SwordParticles)
        {
            p.Play();
        }
    }
    public void SwordParticleOff()
    {
        foreach (ParticleSystem p in SwordParticles)
        {
            p.Stop();
        }
    }

    public void SwordAttackDetectionOn()
    {
        Instantiate(SwordSlashPrj, SwordPrjPoint.position, SwordPrjPoint.rotation);
        Instantiate(SwordDamageRadius, transform.position + transform.forward * 171, SwordPrjPoint.rotation);
    }

    public void SpearTrailOn()
    {
        foreach (TrailRenderer t in SpearTrails)
        {
            t.emitting = true;
        }
    }

    public void SpearTrailOff()
    {
        foreach (TrailRenderer t in SpearTrails)
        {
            t.emitting = false;
        }
    }

    public void AxeAttack_IceEffectOn()
    {
        GameObject eff = Instantiate(AxeAttack_IceEffect, AxeAttack_IcePoint.position, Quaternion.identity);
        eff.transform.parent = transform;
        eff.transform.localRotation = Quaternion.Euler(new Vector3(270, 245f, 0));
    }

    public void AxeAttack_FireEffectOn()
    {
        GameObject eff = Instantiate(AxeAttack_FireEffect, AxeAttack_FirePoint.position, Quaternion.identity);
        eff.transform.parent = transform;
        eff.transform.localRotation = Quaternion.Euler(new Vector3(0, 275f, 0));
    }

    public IEnumerator SpearAttackCoroutine()
    {
        Instantiate(SpearGroundHitEffect, SpearGroundHitPoint.position, SpearGroundHitPoint.rotation);

        for(int i = 0; i < 32; i++)
        {
            yield return new WaitForSeconds(0.012f);
            int r = Random.Range(0, 2);
            Vector3 pos = transform.position;

            if (r == 0)
                pos.x += Random.Range(40, 100);
            else
                pos.x -= Random.Range(40, 100);

            int r2 = Random.Range(0, 2);
            if (r2 == 0)
                pos.z += Random.Range(40, 100);
            else
                pos.z -= Random.Range(40, 100);

            Instantiate(SpearPrj, pos, Quaternion.identity);
        }

        //StopCoroutine(spearCoroutine);
    }

    public void ShieldTrailOn()
    {
        ShieldOrbitParticle.Play();
        foreach (TrailRenderer t in ShieldTrails)
        {
            t.emitting = true;
        }
    }

    public void ShieldTrailOff()
    {
        foreach (TrailRenderer t in ShieldTrails)
        {
            t.emitting = false;
        }
    }

    public void ShieldOn()
    {
        mainController.DamageReduceAmount = 90;
        ShieldCurrentTime = ShieldTime;
        ShieldActived = true;
        LastShieldActivedTime = 0;
        StartCoroutine(ShieldOnCoroutine());
    }

    public void HandTrailOn()
    {
        HandTrail.emitting = true;
    }

    public void HandTrailOff()
    {
        HandTrail.emitting = false;
    }

    public void ScytheSpawn()
    {
        LastShieldActivedTime = 0;
        Instantiate(ScytheEffect, ScytheEffectPoint.position, Quaternion.identity);
        foreach(Transform t in SpawningPoints)
        {
            GameObject g = Instantiate(SpawningPrefabs[Random.Range(0, SpawningPrefabs.Length)], t.position, t.rotation);
        }
    }

    public void CannonFire()
    {
        GameObject prj = Instantiate(CannonProjectile, CannonFirePoint.position, CannonFirePoint.rotation);
        prj.GetComponent<BossClusterCannonProjectileScript>().TargetPoint = new Vector3(Target.position.x, 0, Target.position.z);
    }
}
