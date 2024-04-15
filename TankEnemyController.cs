using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankEnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public EnemyController mainController;
    public Transform TargetPoint;
    public Transform player;
    public Transform Dir;
    public Transform SentryDir;
    public bool Died;

    [Header("Stat")]
    public float Range;
    public float Speed;
    public float SpeedFloat;
    public float TurnSpeed;
    public float attackSpeed;
    public float reloadSpeed;
    public float lastAttackedTime;
    public float attackInterval;
    public float StoppingDistance;

    public List<GameObject> SmokeObjects = new List<GameObject>();

    [Header("Sentrys")]
    public Transform Sentry;
    public float SentryRotationSpeed;

    [Header("Tracks")]
    public List<Transform> TrackObjects = new List<Transform>();
    public float TrackRotationSpeed;

    [Header("Projectiles(Rockets)")]
    public GameObject AttackProjectile;
    public List<GameObject> ProjectileList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Sentry = transform.Find("EnemyGraphic").transform.Find("SentryParent").GetComponent<Transform>();
        mainController = GetComponent<EnemyController>();

        player = GameObject.Find("Player").GetComponent<Transform>();
        Dir = transform.Find("Dir").GetComponent<Transform>();
        SentryDir = transform.Find("SentryDir").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();

        agent.speed = Speed;
        agent.angularSpeed = TurnSpeed;

        Died = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainController.isDied && !Died)
        {
            StopAllCoroutines();
            foreach (GameObject rocket in ProjectileList)
            {
                rocket.SetActive(false);
            }

            foreach(GameObject smoke in SmokeObjects)
            {
                smoke.SetActive(false);
            }
            Died = true;
        }

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

        Dir.LookAt(TargetPoint);
        SentryDir.LookAt(TargetPoint);

        Quaternion dirvec = Dir.rotation;
        dirvec.x = 0;
        dirvec.z = 0;

        Quaternion sentryvec = SentryDir.rotation;
        sentryvec.x = 0;
        sentryvec.z = 0;

        Sentry.rotation = Quaternion.Lerp(Sentry.rotation, sentryvec, SentryRotationSpeed * Time.deltaTime);
        if (agent.remainingDistance > StoppingDistance)
        {
            SpeedFloat = Mathf.Lerp(SpeedFloat, Speed, Time.deltaTime * agent.acceleration);
            //transform.rotation = Quaternion.Lerp(transform.rotation, dirvec, TurnSpeed * Time.deltaTime);
            foreach (Transform t in TrackObjects)
            {
                t.Rotate(TrackRotationSpeed * Time.deltaTime, 0, 0);
            }
        }
        else
        {
            SpeedFloat = Mathf.Lerp(SpeedFloat, 0, Time.deltaTime * agent.acceleration);
        }

        agent.speed = SpeedFloat;

        lastAttackedTime += Time.deltaTime;
        
        if(lastAttackedTime > attackSpeed && Vector3.Distance(TargetPoint.position, transform.position) < Range)
        {
            lastAttackedTime = 0;
            StartCoroutine(Fire());
        }
    }

    IEnumerator Fire()
    {
        foreach(GameObject rocket in ProjectileList)
        {
            GameObject prj = Instantiate(AttackProjectile, rocket.transform.position, Dir.rotation);
            prj.GetComponent<RocketMissileControllerEnemy>().targetPoint = TargetPoint.position;

            rocket.SetActive(false);
            yield return new WaitForSeconds(attackInterval);
        }

        yield return new WaitForSeconds(reloadSpeed);

        foreach (GameObject rocket in ProjectileList)
        {
            rocket.SetActive(true);
        }

        StopAllCoroutines();
    }
}