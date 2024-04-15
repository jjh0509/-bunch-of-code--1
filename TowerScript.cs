using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{
    public LayerMask IgnoreLayer;
    public DataManager Gdata;

    [Header("Targets")]
    public Transform TargetDir_Object;
    public Transform TargetDir_Muzzle;
    public Transform TargetDir_Muzzle2;
    public Transform Target;

    [Header("Parts")]
    public Transform Sentry;
    public Transform Muzzle;
    public Transform RecoilPivot;
    public Transform ProjectilePoint;
    public Transform BaseBox;

    [Header("Parts_Extra")]
    public Transform Muzzle2;
    public Transform RecoilPivot2;
    public Transform ProjectilePoint2;
    public Transform Spinner;

    [Header("Info")]
    public int AttackType; //1 : Default, 2 : Dual Wield, 3 : Artillery, 4 : Gattling, 5 : Fixed_Indirect, 6 : Fixed_Direct, 7 : Not-Attackable(Only for Shield)
    public bool isDowned;

    [Header("Stat")]
    public float HP;
    public float HPRegenPerSecond;
    public float TurnSpeed;
    public TowerData towerData;
    public int Level;
    public float StartActiveDelay;
    [SerializeField] private float StartActiveDelay_Deltatime;
    public float MinDistance;

    [Header("UpgradeEffects")]
    public GameObject UpgradeEffectParent;
    public GameObject PillarsParent;
    public GameObject Roofs;
    public GameObject Floors;
    public GameObject DoneEffect;
    public GameObject NextLevelTower;

    [Header("RideComponents")]
    public Transform RidePoint;
    public Transform UnRidePoint;
    public Transform DirectControlCamPoint;
    public PlayerController_CharacterController playerScript;
    public float RideCamMinAngle;
    public float RideCamMaxAngle;

    [Header("ChargeComponents")]
    public bool HasCharge;
    public float ChargingTime;
    public GameObject ChargeEffect;
    public AudioClip ChargeSFX;
    public GameObject AttackEffect;

    public GameObject RangeCylinder;
    public GameObject MinRangeCylinder;

    public float lastattackedtime;

    public GameObject AttackLineEffect;
    public bool isUpgrading;

    [HideInInspector] public bool TurnPillars;
    [HideInInspector] public float findnewtargetdelay;

    public bool Spin;
    public float SpinSpeed;

    public int lastAttackedMuzzle; //Var for Dual Wield Towers;

    private GameManagement gameManager;

    public bool isRiding;

    public Material ShieldMaterial;
    public Transform ShieldTransform;

    public float shieldTransparency;
    public Collider shieldCollider;

    public float DownTimer;
    public GameObject DownedParticles;
    public bool isFixed;

    public GameObject SelectedEffect;

    [Header("SFX")]
    public AudioSource _audio;
    public float maxVolume;
    public AudioClip AttackSFX;
    public float maxSoundDistance;
    public float volumeOffset;


    private float lastdamagedtime;
    private float deltatimer1;

    public Collider[] ChildrenColliders;

    public bool UseOtherController;
    public bool ChangeTargetOnHit;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        deltatimer1 = 0;
        lastdamagedtime = 0;
        isDowned = false;
        isRiding = false;
        StartActiveDelay_Deltatime = 0;
        isUpgrading = false;
        TurnPillars = false;
        Spin = false;
        SpinSpeed = 0;
        Gdata = GameObject.Find("DataManager").GetComponent<DataManager>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController_CharacterController>();
        gameManager = GameObject.Find("DataManager").GetComponent<GameManagement>();
        _audio = GetComponent<AudioSource>();

        if (AttackType == 7)
        {
            UpgradeEffectParent = transform.Find("UpgradeEffect").gameObject;
            PillarsParent = UpgradeEffectParent.transform.Find("Pillars").gameObject;
            Roofs = PillarsParent.transform.Find("Roofs").gameObject;
            Floors = UpgradeEffectParent.transform.Find("Floors").gameObject;
            DoneEffect = UpgradeEffectParent.transform.Find("Done").gameObject;

            PillarsParent.transform.localScale = new Vector3(1, 0, 1);
            Floors.transform.localScale = new Vector3(0, 1, 0);

            DoneEffect.transform.localPosition = new Vector3(0, -4, 0);
            DoneEffect.SetActive(false);
            UpgradeEffectParent.SetActive(false);

            HP = towerData.HP[Level - 1];
        }


        SelectedEffect = transform.Find("SelectedEffect").gameObject;
        SelectedEffect.SetActive(false);

        DownedParticles = transform.Find("DownedParticles").gameObject;
        DownedParticles.SetActive(false);

        ShieldTransform = transform.Find("ShieldGraphic").GetComponent<Transform>();
        ShieldMaterial = transform.Find("ShieldGraphic").GetComponent<Renderer>().material;
        shieldCollider = transform.Find("ShieldGraphic").GetComponent<Collider>();
        ShieldMaterial.color = new Color(1, 1, 1, 0);

        RangeCylinder = transform.Find("RangeCylinder").gameObject;
        RangeCylinder.transform.localScale = new Vector3(towerData.Range[Level - 1] * 2, 0.3f, towerData.Range[Level - 1] * 2);
        RangeCylinder.SetActive(false);

        if (AttackType == 7)
        {
            ShieldTransform.localScale = new Vector3(towerData.Range[Level - 1], towerData.Range[Level - 1], towerData.Range[Level - 1]);
            return;
        }


        if (UseOtherController)
            return;

        FindComponents();



        if (MinDistance > 0)
        {
            MinRangeCylinder = transform.Find("MinRangeCylinder").gameObject;
            MinRangeCylinder.SetActive(false);
        }
        HP = towerData.HP[Level - 1];
        lastattackedtime = towerData.fireRate[Level - 1];

        findnewtargetdelay = 0;

        PillarsParent.transform.localScale = new Vector3(1, 0, 1);
        Floors.transform.localScale = new Vector3(0, 1, 0);

        DoneEffect.transform.localPosition = new Vector3(0, -4, 0);
        DoneEffect.SetActive(false);
        UpgradeEffectParent.SetActive(false);

        ChildrenColliders = GetComponentsInChildren<Collider>();
    }

    void FindComponents()
    {
        if (HasCharge)
        {
            ChargeEffect = GetComponentInChildren<TowerChargeScript>().gameObject;
            ChargeEffect.SetActive(false);
        }

        if (!isFixed)
        {
            if (AttackType == 1)
            {
                Target = transform.Find("NoTargetFound").GetComponent<Transform>();

                Sentry = transform.Find("Sentry").GetComponent<Transform>();
                Muzzle = Sentry.Find("Muzzle").GetComponent<Transform>();
                RecoilPivot = Muzzle.Find("RecoilPivot").GetComponent<Transform>();
                ProjectilePoint = RecoilPivot.Find("ProjectilePoint").GetComponent<Transform>();
                TargetDir_Object = transform.Find("TargetDirs").GetComponent<Transform>();
                TargetDir_Muzzle = Sentry.Find("TargetDirs_Muzzle").GetComponent<Transform>();
                BaseBox = transform.Find("BaseBox").GetComponent<Transform>();

                UpgradeEffectParent = transform.Find("UpgradeEffect").gameObject;
                PillarsParent = UpgradeEffectParent.transform.Find("Pillars").gameObject;
                Roofs = PillarsParent.transform.Find("Roofs").gameObject;
                Floors = UpgradeEffectParent.transform.Find("Floors").gameObject;
                DoneEffect = UpgradeEffectParent.transform.Find("Done").gameObject;

                RangeCylinder = transform.Find("RangeCylinder").gameObject;
            }

            if (AttackType == 2)
            {
                lastAttackedMuzzle = 0;
                Target = transform.Find("NoTargetFound").GetComponent<Transform>();

                Sentry = transform.Find("Sentry").GetComponent<Transform>();
                Muzzle = Sentry.Find("Muzzle1").GetComponent<Transform>();
                Muzzle2 = Sentry.Find("Muzzle2").GetComponent<Transform>();
                RecoilPivot = Muzzle.Find("RecoilPivot1").GetComponent<Transform>();
                RecoilPivot2 = Muzzle2.Find("RecoilPivot2").GetComponent<Transform>();
                ProjectilePoint = RecoilPivot.Find("ProjectilePoint1").GetComponent<Transform>();
                ProjectilePoint2 = RecoilPivot2.Find("ProjectilePoint2").GetComponent<Transform>();
                TargetDir_Object = transform.Find("TargetDirs").GetComponent<Transform>();
                TargetDir_Muzzle = Sentry.Find("TargetDirs_Muzzle1").GetComponent<Transform>();
                TargetDir_Muzzle2 = Sentry.Find("TargetDirs_Muzzle2").GetComponent<Transform>();
                BaseBox = transform.Find("BaseBox").GetComponent<Transform>();

                UpgradeEffectParent = transform.Find("UpgradeEffect").gameObject;
                PillarsParent = UpgradeEffectParent.transform.Find("Pillars").gameObject;
                Roofs = PillarsParent.transform.Find("Roofs").gameObject;
                Floors = UpgradeEffectParent.transform.Find("Floors").gameObject;
                DoneEffect = UpgradeEffectParent.transform.Find("Done").gameObject;
            }

            if (AttackType == 3)
            {
                Target = transform.Find("NoTargetFound").GetComponent<Transform>();

                Sentry = transform.Find("Sentry").GetComponent<Transform>();
                Muzzle = Sentry.Find("Muzzle").GetComponent<Transform>();
                RecoilPivot = Muzzle.Find("RecoilPivot").GetComponent<Transform>();
                ProjectilePoint = RecoilPivot.Find("ProjectilePoint").GetComponent<Transform>();
                TargetDir_Object = transform.Find("TargetDirs").GetComponent<Transform>();
                TargetDir_Muzzle = Sentry.Find("TargetDirs_Muzzle").GetComponent<Transform>();
                BaseBox = transform.Find("BaseBox").GetComponent<Transform>();

                UpgradeEffectParent = transform.Find("UpgradeEffect").gameObject;
                PillarsParent = UpgradeEffectParent.transform.Find("Pillars").gameObject;
                Roofs = PillarsParent.transform.Find("Roofs").gameObject;
                Floors = UpgradeEffectParent.transform.Find("Floors").gameObject;
                DoneEffect = UpgradeEffectParent.transform.Find("Done").gameObject;
            }

            if (AttackType == 4)
            {
                Target = transform.Find("NoTargetFound").GetComponent<Transform>();

                Sentry = transform.Find("Sentry").GetComponent<Transform>();
                Muzzle = Sentry.Find("Muzzle").GetComponent<Transform>();
                Spinner = Muzzle.Find("Spinner").GetComponent<Transform>();
                ProjectilePoint = Spinner.Find("ProjectilePoint").GetComponent<Transform>();

                TargetDir_Object = transform.Find("TargetDirs").GetComponent<Transform>();
                TargetDir_Muzzle = Sentry.Find("TargetDirs_Muzzle").GetComponent<Transform>();
                BaseBox = transform.Find("BaseBox").GetComponent<Transform>();

                UpgradeEffectParent = transform.Find("UpgradeEffect").gameObject;
                PillarsParent = UpgradeEffectParent.transform.Find("Pillars").gameObject;
                Roofs = PillarsParent.transform.Find("Roofs").gameObject;
                Floors = UpgradeEffectParent.transform.Find("Floors").gameObject;
                DoneEffect = UpgradeEffectParent.transform.Find("Done").gameObject;
            }
        }
        else
        {
            ProjectilePoint = transform.Find("TowerGraphic").transform.Find("ProjectilePoint").GetComponent<Transform>();
            Target = transform.Find("NoTargetFound").GetComponent<Transform>();
            UpgradeEffectParent = transform.Find("UpgradeEffect").gameObject;
            PillarsParent = UpgradeEffectParent.transform.Find("Pillars").gameObject;
            Roofs = PillarsParent.transform.Find("Roofs").gameObject;
            Floors = UpgradeEffectParent.transform.Find("Floors").gameObject;
            DoneEffect = UpgradeEffectParent.transform.Find("Done").gameObject;
            BaseBox = transform.Find("BaseBox").GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        shieldTransparency = Mathf.Clamp01(shieldTransparency);
        ShieldMaterial.color = new Color(1, 1, 1, shieldTransparency);

        if (isDowned)
        {
            shieldTransparency = Mathf.Lerp(shieldTransparency, 0.3f, Time.deltaTime);
            DownTimer -= Time.deltaTime;
            if (DownTimer < 0)
            {
                Recovered();
            }
            return;
        }
        else
        {
            shieldTransparency = Mathf.Lerp(shieldTransparency, 0, Time.deltaTime * 3);
        }

        if (UseOtherController)
            return;

        lastattackedtime += Time.deltaTime;

        if (StartActiveDelay_Deltatime < StartActiveDelay)
        {
            StartActiveDelay_Deltatime += Time.deltaTime;
            return;
        }
        if (TurnPillars)
            PillarsParent.transform.Rotate(0, 10 * Time.deltaTime, 0);

        if (isUpgrading || !gameManager.isWave || AttackType==7)
            return;

        lastdamagedtime += Time.deltaTime;
        if (lastdamagedtime > 30)
        {
            deltatimer1 += Time.deltaTime;
            if(deltatimer1 > 1)
            {
                deltatimer1 = 0;
                HP += HPRegenPerSecond;
            }
        }
        HP = Mathf.Clamp(HP, 0, towerData.HP[Level - 1]);

        if (Target == null)
        {
            Target = transform.Find("NoTargetFound").GetComponent<Transform>();
        }

        if (isRiding)
        {
            Target = transform.Find("NoTargetFound").GetComponent<Transform>();
            if (!isFixed)
            {
                if (AttackType == 1)
                {
                    Sentry.transform.rotation = Quaternion.Slerp(Sentry.rotation, playerScript.orientation.rotation, TurnSpeed * 3);
                    Muzzle.transform.localRotation = Quaternion.Slerp(Muzzle.localRotation, playerScript.mouseRotInputs, TurnSpeed * 3);

                    if (Input.GetMouseButton(0))
                    {
                        if (HasCharge)
                        {
                            if (lastattackedtime > towerData.fireRate[Level - 1])
                            {
                                AttackType1_DirectControl_HasCharge();
                            }
                        }
                        else
                        {
                            if (lastattackedtime > towerData.fireRate[Level - 1])
                            {
                                AttackType1_DirectControl();
                            }
                        }
                    }
                    RecoilPivot.localPosition = Vector3.Slerp(RecoilPivot.localPosition, new Vector3(0, 0, 0), towerData.RecoilRecoverSpeed);
                }

                if (AttackType == 2)
                {
                    Sentry.transform.rotation = Quaternion.Slerp(Sentry.rotation, playerScript.orientation.rotation, TurnSpeed * 3);
                    Muzzle.transform.localRotation = Quaternion.Slerp(Muzzle.localRotation, playerScript.mouseRotInputs, TurnSpeed * 3);
                    Muzzle2.transform.localRotation = Quaternion.Slerp(Muzzle2.localRotation, playerScript.mouseRotInputs, TurnSpeed * 3);
                    DirectControlCamPoint.localRotation = Quaternion.Slerp(Muzzle2.localRotation, playerScript.mouseRotInputs, TurnSpeed * 3);

                    if (Input.GetMouseButton(0))
                    {
                        if (lastattackedtime > towerData.fireRate[Level - 1])
                        {
                            AttackType2_DirectControl();
                        }
                    }
                    RecoilPivot.localPosition = Vector3.Slerp(RecoilPivot.localPosition, new Vector3(0, 0, 0), towerData.RecoilRecoverSpeed);
                }

                if (AttackType == 3)
                {
                    Vector3 rot = Vector3.zero;
                    rot += playerScript.orientation.rotation.eulerAngles;
                    rot += playerScript.mouseRotInputs.eulerAngles;
                    DirectControlCamPoint.rotation = Quaternion.Slerp(DirectControlCamPoint.rotation, Quaternion.Euler(rot), TurnSpeed * 3);
                    Sentry.transform.rotation = Quaternion.Slerp(Sentry.rotation, playerScript.orientation.rotation, TurnSpeed * 3);

                    if (Input.GetMouseButton(0))
                    {
                        if (lastattackedtime > towerData.fireRate[Level - 1])
                        {
                            AttackType3_DirectControl();
                        }
                    }
                }

                if (AttackType == 4)
                {
                    Sentry.transform.rotation = Quaternion.Slerp(Sentry.rotation, playerScript.orientation.rotation, TurnSpeed * 3);
                    Muzzle.transform.localRotation = Quaternion.Slerp(Muzzle.localRotation, playerScript.mouseRotInputs, TurnSpeed * 3);

                    Spinner.Rotate(0, 0, SpinSpeed * Time.deltaTime * 200);
                    SpinSpeed = Mathf.Clamp(SpinSpeed, 0, 4);
                    if (Physics.Raycast(ProjectilePoint.position, ProjectilePoint.forward, out RaycastHit hitInfo, towerData.Range[Level - 1], ~IgnoreLayer))
                    {
                        if (Input.GetMouseButton(0))
                            SpinSpeed += Time.deltaTime * 3;
                        else
                            SpinSpeed -= Time.deltaTime * 3;

                        if (SpinSpeed >= 3.9f && lastattackedtime > towerData.fireRate[Level-1])
                        {
                            AttackType4_DirectControl();
                        }
                    }
                    else
                        SpinSpeed -= Time.deltaTime * 3;

                }
            }
            else
            {
                Vector3 rot = Vector3.zero;
                rot += playerScript.orientation.rotation.eulerAngles;
                rot += playerScript.mouseRotInputs.eulerAngles;
                ProjectilePoint.rotation = Quaternion.Slerp(ProjectilePoint.rotation, Quaternion.Euler(rot), TurnSpeed * 3);
                if (Input.GetMouseButton(0))
                {
                    if (lastattackedtime > towerData.fireRate[Level - 1])
                    {
                        AttackType5_DirectControl();
                    }
                }
            }

            if (Input.GetKeyDown(Gdata.Ride) && playerScript.behaviourCooldown > 1)
            {
                isRiding = false;
                playerScript.UnRide();
                playerScript.behaviourCooldown = 0;
                RangeCylinder.SetActive(false);
                if (!(MinRangeCylinder == null))
                    MinRangeCylinder.SetActive(false);
            }

            return;
        }

        if (Target.name == "NoTargetFound")
        {
            if (AttackType == 4)
            {
                Spinner.Rotate(0, 0, SpinSpeed);
                if (Spin)
                    SpinSpeed = Mathf.Lerp(SpinSpeed, 4, 0.01f);
                else
                    SpinSpeed = Mathf.Lerp(SpinSpeed, 0, 0.01f);
            }

            findnewtargetdelay += Time.deltaTime;
            if (findnewtargetdelay > 0.5f)
            {
                findnewtargetdelay = 0;
                Target = FindClosestEnemy().GetComponent<Transform>();
                return;
            }
        }
        else
        {
            //Vector3 targetPoint_muzzle = new Vector3(TargetDir_Muzzle.localPosition.x, Target.position.y, TargetDir_Muzzle.localPosition.z);

            //TargetDir_Muzzle.LookAt(Target.position); *



            //Muzzle.localRotation = new Quaternion(Mathf.Clamp(Muzzle.localRotation.x, -25, 25), Muzzle.rotation.y, Muzzle.rotation.z,1);
            //Debug.Log();

            //Quaternion Clamped = new Quaternion(Mathf.Clamp(TargetDir_Muzzle.rotation.x, -25, 25), 0, 0,1);
            //Debug.Log(Clamped);

            //Muzzle.transform.localRotation = Quaternion.Slerp(Muzzle.localRotation, TargetDir_Muzzle.rotation, TurnSpeed); *

            //Vector3 targetlinepos = new Vector3(0,
            //Mathf.Abs(Target.position.y) - Mathf.Abs(ProjectilePoint.position.y), Mathf.Abs(Target.position.z - ProjectilePoint.position.z));


            //Vector3 targetlinepos = new Vector3(0,
            //Target.position.y - ProjectilePoint.position.y, Vector3.Distance(ProjectilePoint.position, Target.position));

            if(Vector3.Distance(transform.position, Target.position) < MinDistance)
            {
                Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                return;
            }

            if (!isFixed)
            {
                if (AttackType == 1)
                {
                    Vector3 targetPoint = new Vector3(Target.position.x, TargetDir_Object.position.y, Target.position.z);

                    TargetDir_Object.LookAt(targetPoint);

                    Sentry.transform.rotation = Quaternion.Slerp(Sentry.rotation, TargetDir_Object.rotation, TurnSpeed);

                    Vector3 targetHead = new Vector3(Target.position.x, Target.GetComponent<EnemyController>().TowerAttackPoint.position.y, Target.position.z);
                    TargetDir_Muzzle.LookAt(targetHead);
                    Muzzle.transform.rotation = Quaternion.Slerp(Muzzle.rotation, TargetDir_Muzzle.rotation, TurnSpeed);
                    if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) < towerData.Range[Level - 1])
                    {
                        if (HasCharge)
                            AttackType1_HasCharge();
                        else
                            AttackType1();
                        lastattackedtime = Random.Range(-towerData.fireRate[Level - 1] * 0.02f, towerData.fireRate[Level - 1] * 0.02f);
                    }
                    else if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) > towerData.Range[Level - 1])
                    {
                        Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                    }

                    if (Target.gameObject.CompareTag("Enemy"))
                    {
                        if (Target.GetComponent<EnemyController>().isDied)
                        {
                            Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                        }
                    }

                    RecoilPivot.localPosition = Vector3.Slerp(RecoilPivot.localPosition, new Vector3(0, 0, 0), towerData.RecoilRecoverSpeed);
                }

                else if (AttackType == 2)
                {
                    Vector3 targetPoint = new Vector3(Target.position.x, TargetDir_Object.position.y, Target.position.z);

                    TargetDir_Object.LookAt(targetPoint);

                    Sentry.transform.rotation = Quaternion.Slerp(Sentry.rotation, TargetDir_Object.rotation, TurnSpeed);

                    Vector3 targetHead = new Vector3(Target.position.x, Target.GetComponent<EnemyController>().TowerAttackPoint.position.y, Target.position.z);
                    TargetDir_Muzzle.LookAt(targetHead);
                    Muzzle.transform.rotation = Quaternion.Slerp(Muzzle.rotation, TargetDir_Muzzle.rotation, TurnSpeed);

                    TargetDir_Muzzle2.LookAt(targetHead);
                    Muzzle2.transform.rotation = Quaternion.Slerp(Muzzle2.rotation, TargetDir_Muzzle2.rotation, TurnSpeed);
                    if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) < towerData.Range[Level - 1])
                    {
                        AttackType2();
                        lastattackedtime = Random.Range(-towerData.fireRate[Level - 1] * 0.02f, towerData.fireRate[Level - 1] * 0.02f);
                    }
                    else if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) > towerData.Range[Level - 1])
                    {
                        Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                    }

                    if (Target.gameObject.CompareTag("Enemy"))
                    {
                        if (Target.GetComponent<EnemyController>().isDied)
                        {
                            Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                        }
                    }

                    RecoilPivot.localPosition = Vector3.Slerp(RecoilPivot.localPosition, new Vector3(0, 0, 0), towerData.RecoilRecoverSpeed);
                    RecoilPivot2.localPosition = Vector3.Slerp(RecoilPivot2.localPosition, new Vector3(0, 0, 0), towerData.RecoilRecoverSpeed);
                }

                else if (AttackType == 3)
                {
                    Vector3 targetPoint = new Vector3(Target.position.x, TargetDir_Object.position.y, Target.position.z);

                    TargetDir_Object.LookAt(targetPoint);

                    Sentry.transform.rotation = Quaternion.Slerp(Sentry.rotation, TargetDir_Object.rotation, TurnSpeed);
                    if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) < towerData.Range[Level - 1])
                    {
                        AttackType3();
                        lastattackedtime = Random.Range(-towerData.fireRate[Level - 1] * 0.02f, towerData.fireRate[Level - 1] * 0.02f);
                    }
                    else if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) > towerData.Range[Level - 1])
                    {
                        Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                    }

                    if (Target.gameObject.CompareTag("Enemy"))
                    {
                        if (Target.GetComponent<EnemyController>().isDied)
                        {
                            Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                        }
                    }

                    RecoilPivot.localPosition = Vector3.Slerp(RecoilPivot.localPosition, new Vector3(0, 0, 0), towerData.RecoilRecoverSpeed);
                }

                else if (AttackType == 4)
                {
                    Vector3 targetPoint = new Vector3(Target.position.x, TargetDir_Object.position.y, Target.position.z);

                    TargetDir_Object.LookAt(targetPoint);

                    Sentry.transform.rotation = Quaternion.Slerp(Sentry.rotation, TargetDir_Object.rotation, TurnSpeed);

                    Vector3 targetHead = new Vector3(Target.position.x, Target.GetComponent<EnemyController>().TowerAttackPoint.position.y, Target.position.z);
                    TargetDir_Muzzle.LookAt(targetHead);
                    Muzzle.transform.rotation = Quaternion.Slerp(Muzzle.rotation, TargetDir_Muzzle.rotation, TurnSpeed);
                    if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) < towerData.Range[Level - 1] && SpinSpeed >= 3.9f)
                    {
                        AttackType4();
                        lastattackedtime = Random.Range(-towerData.fireRate[Level-1] * 0.02f, towerData.fireRate[Level - 1] * 0.02f);
                    }
                    else if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) > towerData.Range[Level - 1])
                    {
                        Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                    }

                    if (Target.gameObject.CompareTag("Enemy"))
                    {
                        Spin = true;
                        if (Target.GetComponent<EnemyController>().isDied)
                        {
                            Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                            Spin = false;
                        }
                    }
                    else
                        Spin = false;

                    Spinner.Rotate(0, 0, SpinSpeed * Time.deltaTime * 200);
                    if (Spin)
                        SpinSpeed += Time.deltaTime * 3;
                    else
                        SpinSpeed -= Time.deltaTime * 3;
                    SpinSpeed = Mathf.Clamp(SpinSpeed, 0, 4);
                }
            }
            else
            {
                ProjectilePoint.LookAt(Target);
                if (AttackType == 5)
                {
                    if (Target.gameObject.CompareTag("Enemy"))
                    {
                        if (Target.GetComponent<EnemyController>().isDied)
                        {
                            Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                            return;
                        }
                    }
                    if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) < towerData.Range[Level - 1])
                    {
                        AttackType5();
                        lastattackedtime = Random.Range(-towerData.fireRate[Level - 1] * 0.02f, towerData.fireRate[Level - 1] * 0.02f);
                    }
                    else if (lastattackedtime > towerData.fireRate[Level - 1] && Vector3.Distance(transform.position, Target.position) > towerData.Range[Level - 1])
                    {
                        Target = transform.Find("NoTargetFound").GetComponent<Transform>();
                    }
                }
            }
        }
    }

    void AttackType1()
    {
        if (Target.gameObject.CompareTag("Enemy"))
        {
            Target.GetComponent<EnemyController>().TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
            Target.GetComponent<EnemyController>().lastHitDirection = Muzzle;
        }

        Vector3 targetlinepos;

        targetlinepos.z = Vector3.Distance(ProjectilePoint.position, Target.position);
        targetlinepos.x = 0;
        targetlinepos.y = 0;

        GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
        LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;
        LineEffect.transform.parent = ProjectilePoint;

        LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
        Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
        linerenderer.SetPositions(Linepos);

        if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
            _audio.volume = 0;
        else
            _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
        _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
        _audio.PlayOneShot(AttackSFX);
        if (!(AttackEffect == null))
        {
            GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
            Destroy(effect, 3);
        }
        RecoilPivot.localPosition = new Vector3(0, 0, -towerData.Recoil);
    }
    void AttackType1_HasCharge()
    {
        StartCoroutine(AttackType1_ChargeCoroutine());
    }

    IEnumerator AttackType1_ChargeCoroutine()
    {
        lastattackedtime = 0;
        if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
            _audio.volume = 0;
        else
            _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
        _audio.PlayOneShot(ChargeSFX);
        ChargeEffect.SetActive(true);

        yield return new WaitForSeconds(ChargingTime);

        ChargeEffect.SetActive(false);

        if(!(Target == null) || Target.name== "NoTargetFound")
        {
            StopAllCoroutines();
        }

        if (Target.GetComponent<EnemyController>().isDied)
        {
            FindClosestEnemy();
        }

        if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
            _audio.volume = 0;
        else
            _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
        _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
        _audio.PlayOneShot(AttackSFX);
        if (!(AttackEffect == null))
        {
            GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
            Destroy(effect, 3);
        }

        if (Target.gameObject.CompareTag("Enemy"))
        {
            Target.GetComponent<EnemyController>().TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
            Target.GetComponent<EnemyController>().lastHitDirection = Muzzle;
        }
        Vector3 targetlinepos;

        targetlinepos.z = Vector3.Distance(ProjectilePoint.position, Target.position);
        targetlinepos.x = 0;
        targetlinepos.y = 0;

        GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
        LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;
        LineEffect.transform.parent = ProjectilePoint;

        LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
        Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
        linerenderer.SetPositions(Linepos);

        RecoilPivot.localPosition = new Vector3(0, 0, -towerData.Recoil);

        StopAllCoroutines();
    }

    void AttackType1_DirectControl()
    {
        if (Physics.Raycast(ProjectilePoint.position, ProjectilePoint.forward, out RaycastHit hitInfo, towerData.Range[Level - 1], ~IgnoreLayer))
        {
            Vector3 point = hitInfo.point;
            if (hitInfo.transform.CompareTag("Enemy"))
            {
                EnemyController controller = hitInfo.transform.GetComponent<EnemyController>();
                if (Random.Range(0, 101) <= gameManager.CriticalChance)
                {

                    controller.TakeDamage(towerData.Damage[Level - 1] * 2, towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                }
                else
                {
                    controller.TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                }
            }

            if (hitInfo.transform.CompareTag("EnemyGraphic"))
            {
                EnemyController controller = hitInfo.transform.GetComponentInParent<EnemyController>();
                if (Random.Range(0, 101) <= gameManager.CriticalChance)
                {
                    controller.TakeDamage(towerData.Damage[Level - 1] * 2 , towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                }
                else
                {
                    controller.TakeDamage(towerData.Damage[Level - 1] , towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                }

            }

            Vector3 targetlinepos;

            targetlinepos.z = Vector3.Distance(ProjectilePoint.position, point);
            targetlinepos.x = 0;
            targetlinepos.y = 0;

            GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
            LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;

            LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
            Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
            linerenderer.SetPositions(Linepos);

            RecoilPivot.localPosition = new Vector3(0, 0, -towerData.Recoil * 2);

            if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
                _audio.volume = 0;
            else
                _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
            _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
            _audio.PlayOneShot(AttackSFX);
            if (!(AttackEffect == null))
            {
                GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
                Destroy(effect, 3);
            }
            lastattackedtime = 0;
        }
    }

    void AttackType1_DirectControl_HasCharge()
    {
        if (Physics.Raycast(ProjectilePoint.position, ProjectilePoint.forward, out RaycastHit hitInfo, towerData.Range[Level - 1], ~IgnoreLayer))
        {
            StartCoroutine(AttackType1_DirectControl_ChargeCoroutine());
        }
    }

    IEnumerator AttackType1_DirectControl_ChargeCoroutine()
    {
        lastattackedtime = 0;
        if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
            _audio.volume = 0;
        else
            _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
        _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
        _audio.PlayOneShot(ChargeSFX);

        ChargeEffect.SetActive(true);

        yield return new WaitForSeconds(ChargingTime);

        ChargeEffect.SetActive(false);

        if (Physics.Raycast(ProjectilePoint.position, ProjectilePoint.forward, out RaycastHit hitInfo, towerData.Range[Level - 1], ~IgnoreLayer) && isRiding)
        {
            if (hitInfo.transform.CompareTag("Enemy"))
            {
                EnemyController controller = hitInfo.transform.GetComponent<EnemyController>();
                if (Random.Range(0, 101) <= gameManager.CriticalChance)
                {

                    controller.TakeDamage(towerData.Damage[Level - 1] * 2 , towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                }
                else
                {
                    controller.TakeDamage(towerData.Damage[Level - 1] , towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                }
            }

            if (hitInfo.transform.CompareTag("EnemyGraphic"))
            {
                EnemyController controller = hitInfo.transform.GetComponentInParent<EnemyController>();
                if (Random.Range(0, 101) <= gameManager.CriticalChance)
                {
                    controller.TakeDamage(towerData.Damage[Level - 1] * 2, towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                }
                else
                {
                    controller.TakeDamage(towerData.Damage[Level - 1] , towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                }

            }

            if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
                _audio.volume = 0;
            else
                _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
            _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
            _audio.PlayOneShot(AttackSFX);
            if (!(AttackEffect == null))
            {
                GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
                Destroy(effect, 3);
            }

            Vector3 targetlinepos;
            targetlinepos.z = Vector3.Distance(ProjectilePoint.position, hitInfo.point);
            targetlinepos.x = 0;
            targetlinepos.y = 0;

            GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
            LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;

            LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
            Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
            linerenderer.SetPositions(Linepos);

            RecoilPivot.localPosition = new Vector3(0, 0, -towerData.Recoil * 2);
        }
        StopAllCoroutines();
    }

    void AttackType2()
    {
        if (Target.gameObject.CompareTag("Enemy"))
        {
            Target.GetComponent<EnemyController>().TakeDamage(towerData.Damage[Level - 1] , towerData.ArmorPiercing, false);
            Target.GetComponent<EnemyController>().lastHitDirection = Muzzle;
        }

        Vector3 targetlinepos;

        if (lastAttackedMuzzle == 0)
        {
            targetlinepos.z = Vector3.Distance(ProjectilePoint.position, Target.position);
            targetlinepos.x = 0;
            targetlinepos.y = 0;

            GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
            LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;
            LineEffect.transform.parent = ProjectilePoint;

            LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
            Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
            linerenderer.SetPositions(Linepos);

            RecoilPivot.localPosition = new Vector3(0, 0, -towerData.Recoil);
            lastAttackedMuzzle = 1;
        }
        else if (lastAttackedMuzzle == 1)
        {
            targetlinepos.z = Vector3.Distance(ProjectilePoint2.position, Target.position);
            targetlinepos.x = 0;
            targetlinepos.y = 0;

            GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint2.position, ProjectilePoint2.rotation);
            LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;
            LineEffect.transform.parent = ProjectilePoint2;

            LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
            Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
            linerenderer.SetPositions(Linepos);

            RecoilPivot2.localPosition = new Vector3(0, 0, -towerData.Recoil);

            if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
                _audio.volume = 0;
            else
                _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
            _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
            _audio.PlayOneShot(AttackSFX);
            if (!(AttackEffect == null))
            {
                GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
                Destroy(effect, 3);
            }
            lastAttackedMuzzle = 0;
        }
    }

    void AttackType2_DirectControl()
    {
        if (Physics.Raycast(DirectControlCamPoint.position, DirectControlCamPoint.forward, out RaycastHit hitInfo, towerData.Range[Level - 1], ~IgnoreLayer))
        {
            if (lastAttackedMuzzle == 0)
            {
                Vector3 point = hitInfo.point;
                if (hitInfo.transform.CompareTag("Enemy"))
                {
                    EnemyController controller = hitInfo.transform.GetComponent<EnemyController>();
                    if (Random.Range(0, 101) <= gameManager.CriticalChance)
                    {

                        controller.TakeDamage(towerData.Damage[Level - 1] * 2, towerData.ArmorPiercing, false);
                        controller.lastHitDirection = Muzzle;

                        GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                    }
                    else
                    {
                        controller.TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
                        controller.lastHitDirection = Muzzle;

                        GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                    }
                }

                if (hitInfo.transform.CompareTag("EnemyGraphic"))
                {
                    EnemyController controller = hitInfo.transform.GetComponentInParent<EnemyController>();
                    if (Random.Range(0, 101) <= gameManager.CriticalChance)
                    {
                        controller.TakeDamage(towerData.Damage[Level - 1] * 2, towerData.ArmorPiercing, false);
                        controller.lastHitDirection = Muzzle;

                        GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                    }
                    else
                    {
                        controller.TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
                        controller.lastHitDirection = Muzzle;

                        GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                    }

                }
                Vector3 targetlinepos;

                targetlinepos.z = Vector3.Distance(ProjectilePoint.position, point);
                targetlinepos.x = 0;
                targetlinepos.y = 0;

                GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
                LineEffect.transform.LookAt(point);
                LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;

                LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
                Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
                linerenderer.SetPositions(Linepos);

                RecoilPivot.localPosition = new Vector3(0, 0, -towerData.Recoil * 2);
                lastattackedtime = 0;
                lastAttackedMuzzle = 1;
            }
            else if (lastAttackedMuzzle == 1)
            {
                Vector3 point = hitInfo.point;
                if (hitInfo.transform.CompareTag("Enemy"))
                {
                    EnemyController controller = hitInfo.transform.GetComponent<EnemyController>();
                    if (Random.Range(0, 101) <= gameManager.CriticalChance)
                    {

                        controller.TakeDamage(towerData.Damage[Level - 1] * 2, towerData.ArmorPiercing, false);
                        controller.lastHitDirection = Muzzle;

                        GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                    }
                    else
                    {
                        controller.TakeDamage(towerData.Damage[Level - 1] , towerData.ArmorPiercing, false);
                        controller.lastHitDirection = Muzzle;

                        GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                    }
                }

                if (hitInfo.transform.CompareTag("EnemyGraphic"))
                {
                    EnemyController controller = hitInfo.transform.GetComponentInParent<EnemyController>();
                    if (Random.Range(0, 101) <= gameManager.CriticalChance)
                    {
                        controller.TakeDamage(towerData.Damage[Level - 1] * 2, towerData.ArmorPiercing, false);
                        controller.lastHitDirection = Muzzle;

                        GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                    }
                    else
                    {
                        controller.TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
                        controller.lastHitDirection = Muzzle;

                        GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                    }

                }
                Vector3 targetlinepos;

                targetlinepos.z = Vector3.Distance(ProjectilePoint.position, point);
                targetlinepos.x = 0;
                targetlinepos.y = 0;

                GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint2.position, ProjectilePoint2.rotation);
                LineEffect.transform.LookAt(point);
                LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;
                LineEffect.transform.parent = ProjectilePoint2;

                LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
                Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
                linerenderer.SetPositions(Linepos);

                RecoilPivot2.localPosition = new Vector3(0, 0, -towerData.Recoil * 2);
                lastattackedtime = 0;
                lastAttackedMuzzle = 0;
            }
            if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
                _audio.volume = 0;
            else
                _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
            _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
            _audio.PlayOneShot(AttackSFX);
            if (!(AttackEffect == null))
            {
                GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
                Destroy(effect, 3);
            }
        }
    }

    void AttackType3()
    {
        StartCoroutine(AttackType3Coroutine());
    }

    IEnumerator AttackType3Coroutine()
    {
        GameObject Projectile = Instantiate(towerData.Projectile[Level - 1], Muzzle.position, ProjectilePoint.rotation);
        Goksa Projectile_Curve = Projectile.GetComponent<Goksa>();
        //Debug.Log(Vector3.Distance(transform.position, Target.position));
        Vector3 Target_Offseted = Target.position + (Target.forward *  (Target.GetComponent<EnemyController>().speed / towerData.Projectile[Level-1].GetComponent<Goksa>().velocity * Vector3.Distance(transform.position,Target.position)));
        Target_Offseted.x += Random.Range(-towerData.Spread[Level-1], towerData.Spread[Level - 1]);
        Target_Offseted.z += Random.Range(-towerData.Spread[Level - 1], towerData.Spread[Level - 1]);
        Projectile_Curve.target = Target_Offseted;

        Vector3 direction = Target_Offseted - Projectile_Curve.transform.position;
        float distance = direction.magnitude;
        Vector3 muz_dirY = Vector3.up * Mathf.Sqrt((Projectile_Curve.fire_angle * -1) * Physics.gravity.y * distance / Projectile_Curve.velocity);

        Projectile.GetComponentInChildren<MeshRenderer>().enabled = false;
        Projectile.GetComponentInChildren<TrailRenderer>().enabled = false;

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.02f);
            Muzzle.localRotation = Quaternion.Slerp(Muzzle.localRotation,Quaternion.Euler(-muz_dirY.y,0,0),0.1f);
        }
        Projectile.transform.position = ProjectilePoint.position;
        Projectile_Curve.damage = towerData.Damage[Level - 1];
        Projectile_Curve.Fire = true;
        Projectile.GetComponentInChildren<MeshRenderer>().enabled = true;
        Projectile.GetComponentInChildren<TrailRenderer>().enabled = true;

        RecoilPivot.localPosition = new Vector3(0, 0, -towerData.Recoil);

        if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
            _audio.volume = 0;
        else
            _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
        _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
        _audio.PlayOneShot(AttackSFX);
        if (!(AttackEffect == null))
        {
            GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
            Destroy(effect, 3);
        }
        StopAllCoroutines();
    }

    void AttackType3_DirectControl()
    {
        StartCoroutine(AttackType3Coroutine_DirectControl());     
    }

    IEnumerator AttackType3Coroutine_DirectControl()
    {
        if (Physics.Raycast(DirectControlCamPoint.position, DirectControlCamPoint.forward, out RaycastHit hitInfo, 1000, ~IgnoreLayer))
        {
            if (Vector3.Distance(transform.position, hitInfo.point) < towerData.Range[Level - 1] && Vector3.Distance(transform.position, hitInfo.point) > MinDistance)
            {
                lastattackedtime = 0;
                GameObject Projectile = Instantiate(towerData.Projectile[Level - 1], Muzzle.position, ProjectilePoint.rotation);
                Goksa Projectile_Curve = Projectile.GetComponent<Goksa>();

                Projectile_Curve.target = hitInfo.point;
                Projectile_Curve.playerProjectile = true;

                Vector3 Target_Offseted = hitInfo.point;
                Target_Offseted.x += Random.Range(-towerData.Spread[Level - 1], towerData.Spread[Level - 1]);
                Target_Offseted.z += Random.Range(-towerData.Spread[Level - 1], towerData.Spread[Level - 1]);
                Projectile_Curve.target = Target_Offseted;

                Vector3 direction = Target_Offseted - Projectile_Curve.transform.position;
                float distance = direction.magnitude;
                Vector3 muz_dirY = Vector3.up * Mathf.Sqrt((Projectile_Curve.fire_angle * -1) * Physics.gravity.y * distance / Projectile_Curve.velocity);

                Projectile.GetComponentInChildren<MeshRenderer>().enabled = false;
                Projectile.GetComponentInChildren<TrailRenderer>().enabled = false;

                for (int i = 0; i < 20; i++)
                {
                    yield return new WaitForSeconds(0.02f);
                    Muzzle.localRotation = Quaternion.Slerp(Muzzle.localRotation, Quaternion.Euler(-muz_dirY.y, 0, 0), 0.1f);
                }
                Projectile.transform.position = ProjectilePoint.position;
                Projectile_Curve.damage = towerData.Damage[Level - 1];
                Projectile_Curve.Fire = true;
                Projectile.GetComponentInChildren<MeshRenderer>().enabled = true;
                Projectile.GetComponentInChildren<TrailRenderer>().enabled = true;

                if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
                    _audio.volume = 0;
                else
                    _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
                _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
                _audio.PlayOneShot(AttackSFX);
                if (!(AttackEffect == null))
                {
                    GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
                    Destroy(effect, 3);
                }
            }
        }
        StopAllCoroutines();
    }

    void AttackType4()
    {
        if (Target.gameObject.CompareTag("Enemy"))
        {
            Target.GetComponent<EnemyController>().TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
            Target.GetComponent<EnemyController>().lastHitDirection = Muzzle;
        }

        Vector3 targetlinepos;

        targetlinepos.z = Vector3.Distance(ProjectilePoint.position, Target.position);
        targetlinepos.x = 0;
        targetlinepos.y = 0;

        GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
        LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;
        //LineEffect.transform.parent = ProjectilePoint;

        LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
        Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
        linerenderer.SetPositions(Linepos);

        if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
            _audio.volume = 0;
        else
            _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
        _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
        _audio.PlayOneShot(AttackSFX);
        if (!(AttackEffect == null))
        {
            GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
            Destroy(effect, 3);
        }
    }

    void AttackType4_DirectControl()
    {
        if (Physics.Raycast(ProjectilePoint.position, ProjectilePoint.forward, out RaycastHit hitInfo, towerData.Range[Level - 1], ~IgnoreLayer))
        {
            Vector3 point = hitInfo.point;
            if (hitInfo.transform.CompareTag("Enemy"))
            {
                EnemyController controller = hitInfo.transform.GetComponent<EnemyController>();
                if (Random.Range(0, 101) <= gameManager.CriticalChance)
                {
                    
                    controller.TakeDamage(towerData.Damage[Level - 1] * 2, towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                }
                else
                {
                    controller.TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                }
            }

            if (hitInfo.transform.CompareTag("EnemyGraphic"))
            {
                EnemyController controller = hitInfo.transform.GetComponentInParent<EnemyController>();
                if (Random.Range(0, 101) <= gameManager.CriticalChance)
                {
                    controller.TakeDamage(towerData.Damage[Level - 1] * 2, towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                }
                else
                {
                    controller.TakeDamage(towerData.Damage[Level - 1], towerData.ArmorPiercing, false);
                    controller.lastHitDirection = Muzzle;

                    GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                }

            }
            Vector3 targetlinepos;

            targetlinepos.z = Vector3.Distance(ProjectilePoint.position, point);
            targetlinepos.x = 0;
            targetlinepos.y = 0;

            GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
            LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;

            LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
            Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
            linerenderer.SetPositions(Linepos);

            lastattackedtime = 0;

            if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
                _audio.volume = 0;
            else
                _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
            _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
            _audio.PlayOneShot(AttackSFX);
            if (!(AttackEffect == null))
            {
                GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
                Destroy(effect, 3);
            }
        }
    }

    void AttackType5()
    {
        Vector3 targetlinepos;

        targetlinepos.z = Vector3.Distance(ProjectilePoint.position, Target.position);
        targetlinepos.x = 0;
        targetlinepos.y = 0;

        GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
        LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;
        LineEffect.transform.parent = ProjectilePoint;

        LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
        Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
        linerenderer.SetPositions(Linepos);

        Vector3 endEffectPos = Target.position;
        endEffectPos.y = transform.position.y;

        GameObject eff = Instantiate(towerData.Projectile[Level-1], endEffectPos, Quaternion.identity);

        if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
            _audio.volume = 0;
        else
            _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
        _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
        _audio.PlayOneShot(AttackSFX);
        if (!(AttackEffect == null))
        {
            GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
            Destroy(effect, 3);
        }
    }

    public void AttackType5_DirectControl()
    {
        Debug.Log("RHWEIQHWER");
        if (Physics.Raycast(ProjectilePoint.position, ProjectilePoint.forward, out RaycastHit hitInfo, towerData.Range[Level - 1], ~IgnoreLayer))
        {
            Vector3 point = hitInfo.point;
            Vector3 targetlinepos;

            targetlinepos.z = Vector3.Distance(ProjectilePoint.position, point);
            targetlinepos.x = 0;
            targetlinepos.y = 0;

            GameObject LineEffect = Instantiate(AttackLineEffect, ProjectilePoint.position, ProjectilePoint.rotation);
            LineEffect.GetComponent<LineAttackEffect>().length = targetlinepos.z;

            LineRenderer linerenderer = LineEffect.GetComponent<LineRenderer>();
            Vector3[] Linepos = { new Vector3(0, 0, 0), targetlinepos };
            linerenderer.SetPositions(Linepos);

            GameObject eff = Instantiate(towerData.Projectile[Level - 1], point, Quaternion.identity);
            lastattackedtime = 0;

            if (maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset <= 0.2f)
                _audio.volume = 0;
            else
                _audio.volume = maxSoundDistance / Vector3.Distance(transform.position, playerScript.transform.position) + volumeOffset;
            _audio.volume = Mathf.Clamp(_audio.volume, 0, maxVolume);
            _audio.PlayOneShot(AttackSFX);
            if (!(AttackEffect == null))
            {
                GameObject effect = Instantiate(AttackEffect, ProjectilePoint.position, Quaternion.identity);
                Destroy(effect, 3);
            }
        }
    }

    public GameObject FindClosestEnemy()
    {
        //Debug.Log("FINDING");
        Collider[] colliders = Physics.OverlapSphere(transform.position, towerData.Range[Level-1]);

        List<Collider> colliders_without_mindistance = new List<Collider>();

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        int numofenemy = 0;
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy")) {
                if (Vector3.Distance(collider.transform.position, transform.position) > MinDistance)
                {
                    numofenemy++;
                    colliders_without_mindistance.Add(collider);
                }
            }
        }

        if (numofenemy < 1)
                return transform.Find("NoTargetFound").gameObject;

        foreach (var collider in colliders_without_mindistance)
        {
            if (collider.CompareTag("Enemy"))
            {

                if (!collider.transform.GetComponent<EnemyController>().isDied)
                {
                    GameObject currentEnemy = collider.gameObject;
                    float distance = Vector3.Distance(transform.position, currentEnemy.transform.position);

                    if (distance < closestDistance)
                    {
                        closestEnemy = currentEnemy;
                        closestDistance = distance;
                    }
                }
            }
        }
        return closestEnemy;
    }

    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, towerData.Range[Level-1]);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, MinDistance);
    }

    public void Upgrade()
    {
        if (Level >= 5)
            return;
        isUpgrading = true;
        StartCoroutine(UpgradeEffectOn());
    }

    public void UpgradeDone()
    {
        BaseBox.gameObject.SetActive(false);
        if (!(Sentry == null))
            Sentry.gameObject.SetActive(false);

        GameObject newTower = Instantiate(NextLevelTower, transform.position, transform.rotation);
        if (!isFixed)
        {
            newTower.transform.Find("Sentry").transform.rotation = Sentry.rotation;
        }
        newTower.GetComponent<TowerScript>().StartActiveDelay = 2;
    }

    IEnumerator UpgradeEffectOn()
    {
        PillarsParent.transform.localScale = new Vector3(0, 0, 0);
        Floors.transform.localScale = new Vector3(0, 1, 0);
        DoneEffect.transform.localPosition = new Vector3(0, -4, 0);
        DoneEffect.SetActive(false);
        UpgradeEffectParent.SetActive(true);
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            Floors.transform.localScale = Vector3.Slerp(Floors.transform.localScale, new Vector3(1, 1, 1), 0.05f);
        }
        yield return new WaitForSeconds(0.2f);

        PillarsParent.transform.localScale = new Vector3(1, 0, 1);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            PillarsParent.transform.localScale = Vector3.Slerp(PillarsParent.transform.localScale, new Vector3(1, 1, 1), 0.05f);
        }
        yield return new WaitForSeconds(0.2f);
        TurnPillars = true;

        yield return new WaitForSeconds(towerData.UpgradeTime[Level]);


        //UpgradeDoneNow
        DoneEffect.SetActive(true);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            DoneEffect.transform.localPosition = Vector3.Lerp(DoneEffect.transform.localPosition, new Vector3(0, 4, 0), 0.05f);
        }
        yield return new WaitForSeconds(0.2f);

        MeshRenderer[] childrenrenderers;
        childrenrenderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in childrenrenderers)
        {
            if(!mr.CompareTag("UpgradeEffects"))
                mr.enabled = false;
        }
        UpgradeDone();
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            PillarsParent.transform.localScale = Vector3.Slerp(PillarsParent.transform.localScale, new Vector3(1, 0, 1), 0.05f);
            DoneEffect.transform.localPosition = Vector3.Lerp(DoneEffect.transform.localPosition, new Vector3(0, -4, 0), 0.05f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            Floors.transform.localScale = Vector3.Slerp(Floors.transform.localScale, new Vector3(0, 1, 0), 0.05f);
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    public void TakeDamage(float amount)
    {
        if(ChangeTargetOnHit)
            Target = FindClosestEnemy().GetComponent<Transform>();
        HP -= amount;
        if(HP <= 0)
        {
            Downed();
        }
        shieldTransparency = 0.05f;
        lastdamagedtime = 0;

        GameObject DText = Instantiate(GameObject.Find("DataManager").GetComponent<AssetList>().DamageTextPrefab, transform.position, transform.rotation);
        DText.GetComponent<DamageTextScript>().Damage = amount;
        DText.GetComponent<DamageTextScript>().isCritical = false;
    }

    private void Downed()
    {
        if (isDowned)
            return;
        isDowned = true;
        DownTimer = gameManager.TowerDownAutoRepairTime;
        shieldCollider.enabled = false;
        DownedParticles.SetActive(true);

        foreach (Collider col in ChildrenColliders)
        {
            if (col.CompareTag("TowerSelect"))
                continue;
            col.enabled = false;
        }

        if (isRiding)
        {
            playerScript.UnRide();
        }
    }

    public void Recovered()
    {
        DownTimer = -1;
        isDowned = false;
        HP = towerData.HP[Level - 1];
        shieldCollider.enabled = true;
        DownedParticles.SetActive(false);

        foreach (Collider col in ChildrenColliders)
        {
            col.enabled = true;
        }
    }
}
