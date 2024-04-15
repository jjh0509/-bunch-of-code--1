using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingScoutController : MonoBehaviour
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

    public NavMeshAgent agent;

    public EnemyController mainController;

    public Transform player;
    public Transform Dir;
    public Transform Graphic;
    public Vector3 BobbingOffsetVector;
    public float BobOffsetChangeTime;
    private float timer;
    public float BobFloat;
    public float BobSpeed;
    public List<Transform> MuzzleProjectilePoints = new List<Transform>();
    public List<Transform> MuzzleGraphicPoint = new List<Transform>();
    public List<Vector3> MuzzleGraphicPoint_Origin = new List<Vector3>();
    public int lastAttackedMuzzle;
    public float Recoil;
    public float RecoilRecover;
    public Transform TargetPoint;

    public Vector3 AttackPointOffset;

    // Start is called before the first frame update
    void Start()
    {
        lastAttackedTime = 0;
        timer = 0;
        lastAttackedMuzzle = 0;
        player = GameObject.Find("Player").GetComponent<Transform>();
        Dir = transform.Find("Dir").GetComponent<Transform>();
        Graphic = transform.Find("EnemyGraphic").GetComponent<Transform>();
        mainController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();

        if(MuzzleGraphicPoint.Count > 0)
        {
            foreach(Transform v in MuzzleGraphicPoint)
            {
                MuzzleGraphicPoint_Origin.Add(v.localPosition);
            }
        }

        agent.speed = Speed;
        agent.angularSpeed = TurnSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainController.SpawnDone || mainController.isDied)
            return;

        if (TargetPoint == null)
        {
            TargetPoint = mainController.FindClosestTower().GetComponent<Transform>();
            return;
        }

        if (Vector3.Distance(player.position, transform.position) < mainController.PlayerAttackDistance && agent.remainingDistance > mainController.AttackRange + 2 && TargetPoint != player && !player.GetComponent<PlayerController_CharacterController>().isRiding)
        {
            TargetPoint = player;
        }

        if (Vector3.Distance(player.position, transform.position) > mainController.PlayerDismissDistance && TargetPoint == player)
        {
            TargetPoint = mainController.FindClosestTower().GetComponent<Transform>();
        }


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


        if (TargetPoint == null)
            return;

        if (TargetPoint.gameObject.CompareTag("Tower"))
        {
            if (mainController.UseAgentStoppingDistance)
            {
                agent.stoppingDistance = TargetPoint.GetComponent<TowerBoundScript>().EnemyCollisionRadius;
            }
            if (TargetPoint.GetComponent<TowerScript>().isDowned)
                TargetPoint = null;
        }

        agent.SetDestination(TargetPoint.position);

        if (MuzzleGraphicPoint.Count > 0)
        {
            for (int i = 0; i < MuzzleGraphicPoint_Origin.Count; i++)
            {
                MuzzleGraphicPoint[i].localPosition = Vector3.Lerp(MuzzleGraphicPoint[i].localPosition, MuzzleGraphicPoint_Origin[i], Time.deltaTime * RecoilRecover);
            }
        }

        Dir.LookAt(TargetPoint);

        agent.destination = TargetPoint.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Dir.rotation, TurnSpeed * Time.deltaTime * 10);

        timer += Time.deltaTime;
        if(timer > BobOffsetChangeTime)
        {
            BobbingOffsetVector = new Vector3(Random.Range(-BobFloat, BobFloat), Random.Range(-BobFloat, BobFloat), Random.Range(-BobFloat, BobFloat));
            timer = 0;
        }
        Graphic.localPosition = Vector3.Slerp(Graphic.localPosition, BobbingOffsetVector, Time.deltaTime * BobSpeed);

        if(Vector3.Distance(TargetPoint.position,transform.position) < Range)
        {
            CurrentSpeed = 0;
            AttackStarted = true;
        }
        else
        {
            CurrentSpeed = Speed;
            AttackStarted = false;
        }

        Speedfloat = Mathf.Lerp(Speedfloat, CurrentSpeed, Time.deltaTime * 3);
        agent.speed = Speedfloat;

        lastAttackedTime += Time.deltaTime;
        if (AttackStarted)
        {
            if(lastAttackedTime > attackSpeed)
            {
                CannonAttack(lastAttackedMuzzle);
                lastAttackedMuzzle++;
                if (lastAttackedMuzzle >= MuzzleProjectilePoints.Count)
                    lastAttackedMuzzle = 0;

                lastAttackedTime = 0;
            }
        }
    }

    public void CannonAttack(int attackMuzzle)
    {
        GameObject prj = Instantiate(AttackProjectile, MuzzleProjectilePoints[attackMuzzle].position, transform.rotation);
        prj.GetComponent<EnemyProjectileScript>().dmg = Damage;
        prj.transform.LookAt(TargetPoint.position + AttackPointOffset);
        if (MuzzleGraphicPoint.Count > 0)
        {
            MuzzleGraphicPoint[attackMuzzle].Translate(MuzzleGraphicPoint[attackMuzzle].right * Recoil);
        }
    }
}
