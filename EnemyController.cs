using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public bool WaveSpawnedEntity;
    public Transform player;
    public Rigidbody rb;
    public Transform TargetPoint;

    public Transform lastHitDirection;

    [Header("Stat")]
    public float speed = 4f;
    public float rotationSpeed = 5.0f;
    public float HP = 10f;
    public float AttackDamage;
    public float DamageReduceAmount;
    public float DamageReduceAmount_Extra;
    public float DetectRange;
    public float AttackRange;
    public float AttackSpeed;
    public float ReactSpeed;
    public float ShieldStrength;
    public float PlayerAttackDistance;
    public float PlayerDismissDistance;

    [Header("Spawn")]
    public bool UseOtherScriptForSpawn;
    public Transform GroundCollisionPoint;
    public float Spawn_PreDelayTime;
    public bool HasSpawnAnimation;
    public float SpawnDelayTime;
    public bool SpawnAnimationPlaying;
    public GameObject SpawnEffectPrefab;
    public float SpawnEffectLifeTime;
    public GameObject SummonUpEffectPrefab;
    public float SummonUpEffectLifeTime;
    public float SpawnPositionDistance;
    public bool SpawnDone;

    [Header("Info")]
    public bool useOtherController; //If True, this script will be used only for HP calculation
    public bool UseAgentStoppingDistance;
    public int AttackType; //1 : Default Ranged(Sentry), 2 : Melee, 3 : Self-Explosive
    public float maxAngle;
    public int NumofAttackTypes;
    public bool hasShield;
    public bool ShieldOn;
    public GameObject attackProjectile;
    public Transform ProjectilePoint;
    public GameObject DeathEffect;
    public Vector3 SentryOffset_Pos;
    public Vector3 SentryOffset_Rot;
    public Vector3 MuzzleOffset_Pos;
    public Vector3 MuzzleOffset_Rot;
    public float SentryHeight;
    public bool isProjectileLookAtTarget;

    private float spawndelaydeltatimer;

    [Header("DroppingScraps")]
    public List<GameObject> Scrap_Prefabs = new List<GameObject>();
    public int numOfDrops;
    public float DestroyTime; //Only Non-Salvagable Scraps Only, Salvagable Scraps Does not destroy when this time.

    [Header("Others")]
    public List<ParticleSystem> children_Particles = new List<ParticleSystem>();
    public Transform TowerAttackPoint;
    public GameObject HPBar;
    public List<Collider> MeleeColliders = new List<Collider>();

    public float splashdamageinvtime;

    public float direction;
    public NavMeshAgent agent;
    public float current_HP;

    public Animator anim;
    public bool isDied;

    public List<GameObject> SentryObjects = new List<GameObject>();
    public List<Vector3> SentryObjects_StartPos = new List<Vector3>();
    public List<Quaternion> SentryObjects_StartRot = new List<Quaternion>();
    public List<GameObject> MuzzleObjects = new List<GameObject>();
    public GameObject SentryParent;
    public Transform SentryDir;
    public Transform DirObj;
    public GameObject EnemyGraphic;

    public GameObject MagicCircleParticle;

    public float Movefloat;

    public bool AttackStarted;

    public float XDirOffset;

    public float lastAttackedTime;
    public float lastHitTime;

    public float MotionDelayTime;

    [Header("Otherthers")]
    public GameObject MagicCircleEffectPrefab;
    public GameObject MeleeAttackHitEffectPrefab;
    public Transform MeleeAttackHitPoint;
    public bool HasTrailEffects;
    public List<TrailRenderer> TrailEffects;
    public float TimeTillExplode;
    public GameObject TimeTillExplode_Effect;
    public Vector3 attackPointOffset;

    public Collider MinCollider;

    public float StunTimer;
    public float StunResistance; //Percent
    public float StunSpeed;

    [Header("SFX")]
    public AudioSource sfx;
    public AudioClip AttackSfx;

    public bool imBoss;

    void Start()
    {
        sfx = GetComponent<AudioSource>();
        SpawnDone = false;
        spawndelaydeltatimer = 0;
        MotionDelayTime = 0;
        lastAttackedTime = 0;
        AttackStarted = false;
        Movefloat = 1;
        splashdamageinvtime = 0;
        isDied = false;
        current_HP = HP;

        GroundCollisionPoint = transform.Find("Feet").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        Vector3 originPos = transform.position;
        Vector3 originFeet = GroundCollisionPoint.position;

        agent.updateRotation = true;

        if (!UseOtherScriptForSpawn)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - SpawnPositionDistance, transform.position.z);
            StartCoroutine(SpawnCoroutine(originPos, originFeet));
        }

        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        lastHitDirection = transform.Find("LastHitDirection").GetComponent<Transform>();
        rb.isKinematic = true;
        TowerAttackPoint = transform.Find("TowerAttackPoint").GetComponent<Transform>();
        if (!imBoss)
        {
            HPBar = transform.Find("HPBarParent").transform.Find("HPBar").gameObject;

            HPBar.SetActive(true);
        }

        MinCollider = transform.Find("EnemyMinCollider").GetComponent<Collider>();
        MinCollider.enabled = true;

        if (imBoss)
            return;

        Rigidbody[] childrenrbs = transform.Find("EnemyGraphic").GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rbc in childrenrbs)
        {
            rbc.isKinematic = true;
        }

        if (transform.Find("EnemyGraphic").TryGetComponent<Animator>(out anim))
        {
            anim = transform.Find("EnemyGraphic").GetComponent<Animator>();
            anim.enabled = true;
        }



        if (useOtherController)
            return;

        agent.speed = speed;
        agent.acceleration = speed;
        agent.angularSpeed = rotationSpeed;
        agent.stoppingDistance = AttackRange;

        if(MeleeColliders.Count > 0)
        {
            for(int i = 0; i < MeleeColliders.Count; i++)
            {
                MeleeColliders[i].enabled = false;
            }
        }

        if (HasTrailEffects)
        {
            TrailEffects = new List<TrailRenderer>(transform.GetComponentsInChildren<TrailRenderer>());

            foreach (TrailRenderer trail in TrailEffects)
            {
                trail.enabled = false;
            }
        }

        if(!(TimeTillExplode_Effect == null))
        {
            TimeTillExplode_Effect.SetActive(false);
        }

        if (AttackType == 1)
        {
            foreach (GameObject current in SentryObjects)
            {
                SentryObjects_StartPos.Add(current.transform.localPosition);
                SentryObjects_StartRot.Add(current.transform.localRotation);
            }
        }
        else if (AttackType == 2)
        {
            DirObj = transform.Find("DirObj").GetComponent<Transform>();
        }
    }

    IEnumerator SpawnCoroutine(Vector3 origin, Vector3 origin_feet)
    {
        if (!(SpawnEffectPrefab == null))
        {
            GameObject effect = Instantiate(SpawnEffectPrefab, origin_feet, Quaternion.identity);
            Destroy(effect, SpawnEffectLifeTime);
        }

        yield return new WaitForSeconds(Spawn_PreDelayTime);

        if (HasSpawnAnimation)
        {
            SpawnAnimationPlaying = true;
            anim.SetTrigger("Spawn");
        }

        if(!(SummonUpEffectPrefab == null))
        {
            GameObject s_effect = Instantiate(SummonUpEffectPrefab, origin_feet, Quaternion.identity);
            Destroy(s_effect, SummonUpEffectLifeTime);
        }

        for (int i = 0; i < 100; i++)
        {
            transform.position = Vector3.Slerp(transform.position, origin, 0.1f);
            yield return new WaitForSeconds(0.01f);
        }

        agent.enabled = true;
        SpawnDone = true;
    }

    
    void Update()
    {
        if (!HasSpawnAnimation && spawndelaydeltatimer < SpawnDelayTime)
            spawndelaydeltatimer += Time.deltaTime;

        StunTimer -= Time.deltaTime;
        splashdamageinvtime += Time.deltaTime;
        lastAttackedTime += Time.deltaTime;
        lastHitTime += Time.deltaTime;
        MotionDelayTime -= Time.deltaTime;
        if (isDied || MotionDelayTime > 0 || SpawnAnimationPlaying || spawndelaydeltatimer < SpawnDelayTime || !SpawnDone || useOtherController)
            return;

        if (ShieldOn)
            DamageReduceAmount_Extra = ShieldStrength;
        else
            DamageReduceAmount_Extra = 0;

        if (TargetPoint == player)
        {
            if (player.GetComponent<PlayerController_CharacterController>().isRiding)
            {
                TargetPoint = null;
            }
        }

        if (TargetPoint == null)
        {
            TargetPoint = FindClosestTower().GetComponent<Transform>();
            return;
        }

        if (Vector3.Distance(player.position, transform.position) < PlayerAttackDistance && agent.remainingDistance > AttackRange+2 && TargetPoint!=player && !player.GetComponent<PlayerController_CharacterController>().isRiding)
        {
            TargetPoint = player;
        }

        if(Vector3.Distance(player.position, transform.position) > PlayerDismissDistance && TargetPoint==player)
        {
            TargetPoint = FindClosestTower().GetComponent<Transform>();
        }

        if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("Failed To Find Path");
            TargetPoint = null;
            return;
        }
        if (TargetPoint.gameObject.CompareTag("Tower"))
        {
            if (UseAgentStoppingDistance)
            {
                agent.stoppingDistance = TargetPoint.GetComponent<TowerBoundScript>().EnemyCollisionRadius;
            }
            if (TargetPoint.GetComponent<TowerScript>().isDowned)
                TargetPoint = TargetPoint = FindClosestTower().GetComponent<Transform>();
        }

        agent.SetDestination(TargetPoint.position);

        if (StunTimer > 0)
        {
            agent.speed = StunSpeed;
            return;
        }

        if (AttackType == 1)
        {
            if (agent.remainingDistance <= AttackRange)
            {
                anim.SetBool("IsMoving", false);
                Movefloat -= Time.deltaTime;
                agent.speed = Mathf.Lerp(agent.speed, 0, Time.deltaTime * 5);
            }
            else
            {
                anim.SetBool("IsMoving", true);
                Movefloat += Time.deltaTime;
                agent.speed = Mathf.Lerp(agent.speed, speed, Time.deltaTime * 5);
            }
        }
        else if(AttackType==2 || AttackType==3)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                anim.SetBool("IsMoving", false);
                Movefloat -= Time.deltaTime;
            }
            else
            {
                anim.SetBool("IsMoving", true);
                Movefloat += Time.deltaTime;
            }
        }
        
        Movefloat = Mathf.Clamp(Movefloat, 0, ReactSpeed);
        if (hasShield)
        {
            lastAttackedTime +=Time.deltaTime;
            if (lastHitTime > 30)
            {
                anim.SetBool("BeingAttacked", false);
                ShieldOn = false;
            }
        }

        if (Movefloat > 0.1f)
        {
            if (AttackStarted)
            {
                AttackStarted = false;
                if (AttackType == 1)
                {
                    for (int i = 0; i < SentryObjects.Count; i++)
                    {
                        SentryObjects[i].transform.parent = EnemyGraphic.transform;
                        SentryObjects[i].transform.localPosition = SentryObjects_StartPos[i];
                        SentryObjects[i].transform.localRotation = SentryObjects_StartRot[i];
                    }
                    if(MuzzleObjects.Count > 0)
                    {
                        for (int i = 0; i < MuzzleObjects.Count; i++)
                        {
                            MuzzleObjects[i].transform.parent = EnemyGraphic.transform;
                        }
                    }
                    anim.enabled = true;

                    SentryParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    SentryParent.transform.localPosition = new Vector3(0, SentryHeight, 1);
                }
                
            }
            
        }
        else
        {
            if (!AttackStarted) {
                AttackStarted = true;
                if (AttackType == 1)
                {
                    for (int i = 0; i < SentryObjects.Count; i++)
                    {
                        SentryObjects[i].transform.parent = SentryParent.transform;
                        SentryObjects[i].transform.localPosition = SentryObjects_StartPos[i] - SentryParent.transform.localPosition;
                        SentryObjects[i].transform.localRotation = SentryObjects_StartRot[i];
                    }

                    if (MuzzleObjects.Count > 0)
                    {
                        for (int i = 0; i < MuzzleObjects.Count; i++)
                        {
                            MuzzleObjects[i].transform.parent = SentryParent.transform.Find("MuzzlePivot").transform;
                        }
                    }
                    anim.enabled = false;
                    SentryDir.transform.localRotation = Quaternion.Euler(SentryOffset_Rot);
                    SentryParent.transform.localPosition = new Vector3(0, SentryHeight, 1);
                }
            }
        }

        if (AttackStarted)
        {
            if (AttackType == 1)
            {
                if(MuzzleObjects.Count <= 0)
                {
                    SentryDir.LookAt(TargetPoint.position + attackPointOffset);
                    Quaternion Dir = SentryDir.localRotation;
                    Dir.x = Mathf.Clamp(Dir.x, -maxAngle, maxAngle);
                    Dir.x += XDirOffset;
                    SentryParent.transform.localRotation = Quaternion.Slerp(SentryParent.transform.localRotation, Dir, Time.deltaTime * 20);
                }
                else
                {
                    SentryDir.LookAt(TargetPoint.position + attackPointOffset);
                    Quaternion Dir = SentryDir.localRotation;
                    Dir.x = 0;
                    Dir.z = 0;
                    SentryParent.transform.localRotation = Quaternion.Slerp(SentryParent.transform.localRotation, Dir, Time.deltaTime * 20);

                    Quaternion Dir_Muzzle = SentryDir.localRotation;
                    Dir_Muzzle.y = 0;
                    Dir_Muzzle.z = 0;
                    Dir_Muzzle.x =
                    Dir_Muzzle.x = Mathf.Clamp(Dir_Muzzle.x, -maxAngle, maxAngle);

                    Transform muzzlepivot = SentryParent.transform.Find("MuzzlePivot").GetComponent<Transform>();
                    muzzlepivot.localRotation = Quaternion.Slerp(muzzlepivot.localRotation, Dir_Muzzle, Time.deltaTime * 20);
                }
                
                if (lastAttackedTime > AttackSpeed)
                {
                    GameObject prj = Instantiate(attackProjectile, ProjectilePoint.position, ProjectilePoint.rotation);
                    if (isProjectileLookAtTarget)
                        prj.transform.LookAt(TargetPoint.position + attackPointOffset);
                    Destroy(prj, 2);
                    lastAttackedTime = 0;
                }
            }
            else if (AttackType == 2)
            {
                if (lastAttackedTime > AttackSpeed)
                {
                    int attacknum = Random.Range(1, NumofAttackTypes + 1);
                    anim.SetTrigger("Attack" + attacknum);
                    lastAttackedTime = 0;
                }
                DirObj.LookAt(TargetPoint.position + attackPointOffset);
                Quaternion Dir = DirObj.rotation;
                Dir.x = 0;
                Dir.z = 0;
                agent.updateRotation = false;
                transform.rotation = Quaternion.Slerp(transform.rotation, Dir, Time.deltaTime * 20);
            }
            if (AttackType == 3)
            {
                SelfExplode();
            }
        }
        else
        {
            agent.updateRotation = true;
            if (AttackType == 1)
            {
                Vector3 lookAtPos = TargetPoint.position - transform.position;
                lookAtPos.y = 0;
                Quaternion rot = Quaternion.LookRotation(lookAtPos);
                //transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                SentryParent.transform.localRotation = Quaternion.Slerp(SentryParent.transform.localRotation, SentryDir.localRotation, Time.deltaTime * 20);
            }
        }
    }

    /*private void FixedUpdate()
    {
        // 방향 계산
        Vector3 direction = (player.position - transform.position).normalized;

        // Y축 회전값만 사용
        float yRotation = Quaternion.LookRotation(direction).eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, yRotation, 0);

        // 회전값 적용
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 이동
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
    }
    */
    public void BossDead()
    {

    }
    private void Dead()
    {
        StopAllCoroutines();
        if (imBoss)
        {
            BossDead();
            return;
        }
        if (isDied)
            return;

        MinCollider.enabled = false;
        if (!(TimeTillExplode_Effect == null))
        {
            TimeTillExplode_Effect.SetActive(false);
        }
        if(!(anim==null))
            anim.enabled = false;
        agent.enabled = false;
        isDied = true;
        rb.isKinematic = false;

        HPBar.SetActive(false);

        Rigidbody[] childrenrbs = transform.Find("EnemyGraphic").GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rbc in childrenrbs)
        {
            rbc.isKinematic = false;
            rbc.AddExplosionForce(8, transform.position, 8, 4, ForceMode.Impulse);
        }

        for (int i = 0; i < numOfDrops - Random.Range(-1, 2); i++)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            GameObject scrap = Instantiate(Scrap_Prefabs[Random.Range(0, Scrap_Prefabs.Count)], pos, Quaternion.identity);
            scrap.transform.Rotate(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
            scrap.transform.Translate(scrap.transform.forward * 1.5f);
            scrap.GetComponent<Rigidbody>().AddExplosionForce(8, transform.position, 8, 4, ForceMode.Impulse);
        }
        /*
        if(lastHitDirection.name == "Muzzle")
            rb.AddForce(lastHitDirection.forward * 16, ForceMode.Impulse);
        else
            rb.AddForce(lastHitDirection.forward * -8, ForceMode.Impulse);
        rb.AddForce(Vector3.up, ForceMode.Impulse);
        */
        int newLayer = LayerMask.NameToLayer("EnemyDead");
        Transform[] childrentrs = transform.Find("EnemyGraphic").GetComponentsInChildren<Transform>();
        foreach (Transform ctr in childrentrs)
        {
            ctr.gameObject.layer = newLayer;
        }

        if(children_Particles.Count > 0)
        {
            foreach(ParticleSystem p in children_Particles)
            {
                p.Stop();
            }
        }

        if(WaveSpawnedEntity)
            GameObject.Find("DataManager").GetComponent<GameManagement>().NumOfEnemies -= 1;

        gameObject.layer = newLayer;
        Instantiate(DeathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, DestroyTime);
    }

    public void TakeDamage(float amount, bool DamageReduceIgnore, bool isCritical)
    {
        GameObject DText = Instantiate(GameObject.Find("DataManager").GetComponent<AssetList>().DamageTextPrefab, transform.position, transform.rotation);
        //splashdamageinvtime = 0;
        if (DamageReduceIgnore)
        {
            current_HP -= amount;
            DText.GetComponent<DamageTextScript>().Damage = amount;
        }
        else
        {
            current_HP -= amount - (amount * ((DamageReduceAmount + DamageReduceAmount_Extra) * 0.01f));
            DText.GetComponent<DamageTextScript>().Damage = amount - (amount * ((DamageReduceAmount + DamageReduceAmount_Extra) * 0.01f));
        }
        DText.GetComponent<DamageTextScript>().isCritical = isCritical;
        lastHitTime = 0;
        if (hasShield)
        {
            anim.SetBool("BeingAttacked", true);
            ShieldOn = true;
        }

        if (current_HP <= 0)
        {
            Dead();
        }
    }

    public GameObject FindClosestTower()
    {
        GameObject[] AllTowers = GameObject.FindGameObjectsWithTag("Tower");

        GameObject closestTower = null;
        float closestDistance = Mathf.Infinity;
        int numoftower = 0;
        foreach (GameObject tower in AllTowers)
        {
            if(!tower.GetComponent<TowerScript>().isDowned)
            {
                numoftower++;
            }
        }
        if (numoftower < 1)
            return GameObject.Find("MainBase").gameObject;

        foreach (GameObject tower in AllTowers)
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
        }
        if (Vector3.Distance(transform.position, GameObject.Find("MainBase").transform.position) < closestDistance)
            closestTower = GameObject.Find("MainBase").gameObject;
        return closestTower;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectRange);
    }

    public void TrailEffectOn()
    {
        foreach (TrailRenderer trail in TrailEffects)
        {
            trail.enabled = true;
        }
    }

    public void TrailEffectOff()
    {
        foreach (TrailRenderer trail in TrailEffects)
        {
            trail.enabled = false;
        }
    }

    public void GotExploded(Transform BoomPoint)
    {
        splashdamageinvtime = 0;
        lastHitDirection.LookAt(BoomPoint);
    }

    public void ShieldOnNow()
    {
        if (hasShield)
        {
            lastHitTime = 0;
            anim.SetBool("BeingAttacked", true);
            ShieldOn = true;
        }
    }

    public void MotionDelay(float time)
    {
        MotionDelayTime = time;
    }

    public void MeleeAttackHitEffect()
    {
        GameObject particle = Instantiate(MeleeAttackHitEffectPrefab, MeleeAttackHitPoint.position, Quaternion.identity);
        Destroy(particle, 2);
    }

    public void KnightMeleeAttackParticle()
    {
        GameObject particle = Instantiate(MagicCircleEffectPrefab,MeleeAttackHitPoint.position, Quaternion.identity);
        Destroy(particle, 3);
    }

    public void SelfExplode()
    {
        MotionDelayTime = 60;

        StartCoroutine(SelfExplodeCoroutine());
    }

    IEnumerator SelfExplodeCoroutine()
    {
        if (!(TimeTillExplode_Effect == null))
        {
            TimeTillExplode_Effect.SetActive(true);
        }
        yield return new WaitForSeconds(TimeTillExplode);
        GameObject particle = Instantiate(attackProjectile, transform.position, Quaternion.identity);
        Destroy(particle, 4f);
        current_HP = 0;
        if (!(TimeTillExplode_Effect == null))
        {
            TimeTillExplode_Effect.SetActive(false);
        }
        Dead();

        Rigidbody[] childrenrbs = transform.Find("EnemyGraphic").GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rbc in childrenrbs)
        {
            rbc.AddExplosionForce(24, transform.position, 10, 8, ForceMode.Impulse);
        }
    }

    public void Stun(float Time, float Speed)
    {
        StunTimer = Time - (Time * 0.01f * StunResistance);

        StunSpeed = speed * (100 - (Speed * 0.01f * (100 - StunResistance))) * 0.01f;
        StunSpeed = Mathf.Clamp(StunSpeed, 0, speed);
    }
}
