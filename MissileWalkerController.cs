using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MissileWalkerController : MonoBehaviour
{
    [Header("Stat")]
    public float attackSpeed;
    public bool AttackStarted;
    public float lastAttackedTime;
    public float Damage;
    public GameObject AttackProjectile;
    public float Range;
    public float Speed;
    public float TurnSpeed;
    public float CurrentSpeed;
    public float Speedfloat;
    public float MoveFloat;
    public float ReactSpeed;

    [Header("Muzzle")]
    public float MuzzleTurnSpeed;
    public float MuzzleMaxAngle;
    public float MuzzleMinAngle;
    public Transform MuzzleDir;

    public Animator anim;

    public NavMeshAgent agent;

    public EnemyController mainController;

    public Transform player;
    public Transform Dir;
    public Transform Graphic;
    public Transform SentryParent;

    public List<Transform> ProjectilePoints = new List<Transform>();
    public int lastAttackedMuzzle;
    public float Recoil;
    public float RecoilRecover;
    public Transform TargetPoint;

    public Vector3 AttackPointOffset;


    // Start is called before the first frame update
    void Start()
    {
        MoveFloat = Speed;
        AttackStarted = false;
        lastAttackedTime = 0;
        lastAttackedMuzzle = 0;
        player = GameObject.Find("Player").GetComponent<Transform>();
        Dir = transform.Find("Dir").GetComponent<Transform>();
        Graphic = transform.Find("EnemyGraphic").GetComponent<Transform>();
        SentryParent = Graphic.Find("SentryPivot").GetComponent<Transform>();
        mainController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
        anim = transform.Find("EnemyGraphic").GetComponent<Animator>();

        agent.speed = Speed;
        agent.angularSpeed = TurnSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainController.SpawnDone || mainController.isDied)
            return;

        if (TargetPoint == null)
            TargetPoint = mainController.FindClosestTower().GetComponent<Transform>();

        if (Vector3.Distance(player.position, transform.position) < mainController.PlayerAttackDistance && Vector3.Distance(transform.position,TargetPoint.position) > mainController.AttackRange + 2 && TargetPoint != player && !player.GetComponent<PlayerController_CharacterController>().isRiding)
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

        if(Vector3.Distance(transform.position, TargetPoint.position) < Range)
        {
            anim.SetBool("IsMoving", false);
            AttackStarted = true;
            MoveFloat -= Time.deltaTime;
            agent.speed = Mathf.Lerp(agent.speed, 0, Time.deltaTime * 5);
        }
        else
        {
            AttackStarted = false;
            MoveFloat += Time.deltaTime;
            agent.speed = Mathf.Lerp(agent.speed, Speed, Time.deltaTime * 5);
        }

        lastAttackedTime += Time.deltaTime;

        MoveFloat = Mathf.Clamp(MoveFloat, 0, ReactSpeed);
        if (MoveFloat < 0.1f)
        {
            Dir.LookAt(TargetPoint);
            Quaternion vec = Dir.rotation;
            vec.x = 0;
            vec.z = 0;

            transform.rotation = Quaternion.Lerp(transform.rotation, vec, TurnSpeed * Time.deltaTime);
            if(lastAttackedTime > attackSpeed)
            {
                MissileAttack();
            }
        }
        else
        {

            //anim.enabled = true;
            //agent.enabled = true;
            anim.SetBool("IsMoving", true);
        }
    }

    void MissileAttack()
    {
        GameObject prj = Instantiate(AttackProjectile, ProjectilePoints[lastAttackedMuzzle].position, ProjectilePoints[lastAttackedMuzzle].rotation);
        prj.GetComponent<HomingRocket_Enemy>().Damage = Damage;
        prj.GetComponent<HomingRocket_Enemy>().Target = TargetPoint;

        lastAttackedMuzzle++;
        if (lastAttackedMuzzle >= ProjectilePoints.Count)
            lastAttackedMuzzle = 0;

        lastAttackedTime = 0;
    }
}
