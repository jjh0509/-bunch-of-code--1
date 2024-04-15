using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmallFighterScript : MonoBehaviour
{
    [Header("Stat")]
    public float Range;
    public float Speed;
    public float SpeedFloat;
    public float TurnSpeed;
    public float attackSpeed;
    public float lastAttackedTime;
    public int lastAttackedMuzzle;
    public float StoppingDistance;

    [Header("Sentrys")]
    public Transform Sentry1;
    public Transform Sentry2;
    public Transform SentryDir1;
    public Transform SentryDir2;
    public Transform ProjectilePoint1;
    public Transform ProjectilePoint2;

    [Header("Muzzles")]
    public Transform Muzzle1;
    public Transform Muzzle2;
    public Transform MuzzleDir1;
    public Transform MuzzleDir2;

    [Header("MainGun")]
    public Transform MainGunPivot;
    public Transform MainGunProjectilePoint;
    public float MainGunAttackSpeed;
    public float lastAttackedTime_MainGun;
    public GameObject MainGun_Projectile;

    [Header("Projectiles")]
    public GameObject AttackProjectile;

    [Header("Others")]
    public Transform player;
    public Transform Dir;
    public Transform Graphic;
    public NavMeshAgent agent;
    public EnemyController mainController;
    public Transform TargetPoint;
    public bool Died;

    // Start is called before the first frame update
    void Start()
    {
        lastAttackedMuzzle = 1;
        lastAttackedTime = 0;
        player = GameObject.Find("Player").GetComponent<Transform>();
        Dir = transform.Find("Dir").GetComponent<Transform>();
        Graphic = transform.Find("EnemyGraphic").GetComponent<Transform>();
        mainController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = Speed;
        agent.stoppingDistance = StoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainController.SpawnDone || mainController.isDied)
            return;

        if (TargetPoint == null)
            TargetPoint = mainController.FindClosestTower().GetComponent<Transform>();

        if (Vector3.Distance(player.position, transform.position) < mainController.PlayerAttackDistance && agent.remainingDistance > mainController.AttackRange + 2 && TargetPoint != player && !player.GetComponent<PlayerController_CharacterController>().isRiding)
        {
            TargetPoint = player;
        }

        if (Vector3.Distance(player.position, transform.position) > mainController.PlayerDismissDistance && TargetPoint == player)
        {
            TargetPoint = mainController.FindClosestTower().GetComponent<Transform>();
        }

        agent.destination = TargetPoint.position;
        if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            TargetPoint = null;
        }

        if (TargetPoint == player)
        {
            if (player.GetComponent<PlayerController_CharacterController>().isRiding)
            {
                TargetPoint = null;
            }
        }

        if (TargetPoint.gameObject.CompareTag("Tower"))
        {
            if (mainController.UseAgentStoppingDistance)
            {
                agent.stoppingDistance = TargetPoint.GetComponent<TowerBoundScript>().EnemyCollisionRadius;
            }
            if (TargetPoint.GetComponent<TowerScript>().isDowned)
                TargetPoint = null;
        }

        if (TargetPoint == null)
            return;

        SentryDir1.LookAt(TargetPoint);
        SentryDir2.LookAt(TargetPoint);

        Vector3 dir1 = SentryDir1.localEulerAngles;
        dir1.x = 90;
        //dir1.y = 0;
        Vector3 dir2 = SentryDir2.localEulerAngles;
        dir2.x = -90;
        //dir2.y = 0;

        Sentry1.localRotation = Quaternion.Euler(dir1);
        Sentry2.localRotation = Quaternion.Euler(dir2);

        MuzzleDir1.LookAt(TargetPoint);
        MuzzleDir2.LookAt(TargetPoint);

        Vector3 muzdir1 = MuzzleDir1.localEulerAngles;
        muzdir1.y = 0;
        muzdir1.z = 0;
        muzdir1.x += 60;
        Vector3 muzdir2 = -MuzzleDir2.localEulerAngles;
        muzdir2.y = 0;
        muzdir2.z = 0;
        muzdir2.x += 60;

        Muzzle1.localRotation = Quaternion.Euler(muzdir1);
        Muzzle2.localRotation = Quaternion.Euler(muzdir2);

        if (Vector3.Distance(transform.position, TargetPoint.position) < Range)
        {
            //Attack Seq
            lastAttackedTime += Time.deltaTime;
            if (lastAttackedTime > attackSpeed)
            {
                Attack();
            }

            //MainGunAttack Seq
            MainGunPivot.LookAt(TargetPoint);

            lastAttackedTime_MainGun += Time.deltaTime;
            if (lastAttackedTime_MainGun > MainGunAttackSpeed)
            {
                MainGunAttack();
            }
        }
    }
    public void Attack()
    {
        lastAttackedTime = 0;

        if(lastAttackedMuzzle == 1)
        {
            lastAttackedMuzzle = 2;

            GameObject prj = Instantiate(AttackProjectile, ProjectilePoint1.position, Quaternion.identity);
            prj.transform.LookAt(TargetPoint);
        }

        else if (lastAttackedMuzzle == 2)
        {
            lastAttackedMuzzle = 1;

            GameObject prj = Instantiate(AttackProjectile, ProjectilePoint2.position, Quaternion.identity);
            prj.transform.LookAt(TargetPoint);
        }
    }

    public void MainGunAttack()
    {
        lastAttackedTime_MainGun = 0;

        GameObject prj = Instantiate(MainGun_Projectile, MainGunProjectilePoint.position, Quaternion.identity);
        prj.transform.LookAt(TargetPoint);
    }
}
