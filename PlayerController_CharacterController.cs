using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerController_CharacterController : MonoBehaviour
{
    public bool isLobby;

    public GameObject[] SpawningObjects;

    public Quaternion mouseRotInputs;
    public LayerMask IgnoreLayer;
    public LayerMask Layer_Selection;
    public LayerMask IgnoreLayer_Build;

    public static Action shootInput;
    public static Action reloadInput;

    public DataManager Gdata;
    public GameManagement Gmanager;

    [Header("Components")]
    public Transform BaseTransform;
    public CharacterController characterController;
    public GunScript gunScript;
    public Transform orientation;
    private Vector3 grav;
    public NavMeshObstacle navMeshObstacle;
    public GameObject SelectedTower;
    public UIScript UI;
    public GameObject visionEffect;
    public VolumeProfile visionEffectProfile;
    public ParticleSystem BoostTrail;

    [Header("MoveStat")]
    public float playerWalkSpeed = 7f;
    public float playerRunSpeed = 12f;
    public float playerBoosterSpeed = 36f;
    public float playerZoomSpeed = 4f;
    public float jumpHeight = 5f;
    public float defaultJumpHeight = 5f;
    public float boostJumpHeight = 10f;
    public float playerGravity = 5.8f;
    private float playerSpeed;

    [Header("Stat")]
    public float MaxHP;
    public float HP;
    public float MaxShield;
    public float Shield;
    public float MaxEnergy;
    public float Energy;
    public float TPSpeedMultipier;
    public float TPCost;
    public float TeleportCooldown;
    public float lastTeleportedTime;

    [Header("GroundCheck")]
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Slope Handle")]
    public float maxSlopeAngle;
    public RaycastHit slopeHit;

    [Header("Resources")]
    public int Scraps;
    public int AdvancedResources;
    public float BaseEnergies;

    [Header("Audio")]
    public new AudioSource audio;
    public List<AudioClip> takeHitSoundEffects = new List<AudioClip>();
    public AudioClip ShieldBreakSoundEffet;

    public bool canJump;
    public bool isGrounded;
    public bool isRunning;
    public bool isBoosting;
    public bool isMoving;

    private float lastjumpedtime;

    public int Mode; //1 : Default, 2 : Build, 3 : BaseInteract
    public int Selected_Tower;

    private AssetList assets;

    public GameObject Build_Guide_Object;

    public Transform MainBase_Transform;

    public Transform Camera_Transform;

    public int number_of_tower_types;

    public bool isRiding;

    public float behaviourCooldown;

    public float visionEffectIntensity;

    public float ShieldRechargeTimer;

    public float LastDamagedTime;

    public float InvincibleTime;

    public GameObject LastSelectedInteractionObject;
    public float RummageTimer;

    public bool isTestMode;

    // Start is called before the first frame update
    void Start()
    {
        if (isLobby)
            return;

        TPSpeedMultipier = 1;
        lastTeleportedTime = 0;
        RummageTimer = 0;
        Scraps = 0;
        AdvancedResources = 0;
        BaseEnergies = 0;
        LastDamagedTime = 0;
        isRiding = false;
        lastjumpedtime = 0;
        playerSpeed = playerWalkSpeed;
        isRunning = false;
        isMoving = false;
        isGrounded = false;
        canJump = true;
        if(!isTestMode)
            BaseTransform = GameObject.Find("MainBase").GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
        maxSlopeAngle = characterController.slopeLimit;
        Mode = 1;
        Selected_Tower = 1;
        audio = GetComponent<AudioSource>();
        assets = GameObject.Find("DataManager").GetComponent<AssetList>();
        Gdata = GameObject.Find("DataManager").GetComponent<DataManager>();
        Gmanager = GameObject.Find("DataManager").GetComponent<GameManagement>();
        Gmanager.GameStart();
        Build_Guide_Object = transform.Find("Build_Guides").gameObject;
        Camera_Transform = GameObject.Find("CameraHolder").GetComponent<Transform>();
        if(!isTestMode)
            MainBase_Transform = GameObject.Find("MainBase").GetComponent<Transform>();
        gunScript = GameObject.Find("CameraHolder").transform.Find("Parent_GunHolder").transform.Find("GunHolder").transform.Find("Gun").GetComponent<GunScript>();
        assets.MainBase_Energy_Radius_Material.color = new Color(0.41f, 0.81f, 1, 0);
        UI = GameObject.Find("Canvas").GetComponent<UIScript>();
        visionEffect = GameObject.Find("Global Volume").gameObject;
        BoostTrail = Camera_Transform.GetComponentInChildren<ParticleSystem>();
        SelectedTower = null;

        UnityEngine.Rendering.VolumeProfile volumeProfile = visionEffect.GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));


        UnityEngine.Rendering.Universal.Vignette vignette;
        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        vignette.intensity.Override(0.9f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLobby)
            return;


        lastTeleportedTime += Time.deltaTime;
        InvincibleTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.M))
        {
            Energy = MaxEnergy;
            BaseEnergies += 100;
            Scraps += 100;
            AdvancedResources += 100;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector3 pos = new Vector3(transform.position.x, 80, transform.position.z);
            Instantiate(SpawningObjects[UnityEngine.Random.Range(0, SpawningObjects.Length)], pos, Quaternion.identity);
        }

        visionEffectToDefault();
        behaviourCooldown += Time.deltaTime;
        ShieldRechargeTimer -= Time.deltaTime;
        LastDamagedTime += Time.deltaTime;

        Energy = Mathf.Clamp(Energy, 0, MaxEnergy);

        if(ShieldRechargeTimer > 0 && LastDamagedTime > 8)
        {
            Shield += Time.deltaTime * 5;
            Shield = Mathf.Clamp(Shield, 0, MaxShield);
        }

        if (Mode == 3)
        {
            if (Input.GetKeyDown(Gdata.Interact))
                Mode = 1;

            return;
        }        

        if (isRiding)
            return;

        if (Mode == 1)
            Build_Guide_Object.SetActive(false);

        if (Mode == 2)
        {
            BuildMode();
            Build_Guide_Object.SetActive(true);
        }

        orientation = transform.Find("Orientation").GetComponent<Transform>();
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1.5f, 0), groundDistance, groundMask);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        isMoving = !(x == 0 && z == 0);

        lastjumpedtime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSlope() && canJump && lastjumpedtime > 1)
        {
            grav.y = Mathf.Sqrt(jumpHeight * 2f * playerGravity);
            lastjumpedtime = 0;
        }

        grav.y -= playerGravity * Time.deltaTime;

        if (isGrounded && lastjumpedtime > 1)
        {
            grav.y = 0;
        }

        characterController.Move(grav * Time.deltaTime);

        if (Input.GetMouseButton(0))
        {
            shootInput?.Invoke();
        }
        if (Input.GetKeyDown(Gdata.Reload))
        {
            reloadInput?.Invoke();
        }

        if (Input.GetKey(Gdata.Run))
        {
            isRunning = true;
            if (Gdata.BoostToggleMode)
            {
                if (Input.GetKeyDown(Gdata.Boost))
                    isBoosting = !isBoosting;
            }
            else
            {
                if (Input.GetKeyDown(Gdata.Boost))
                    isBoosting = true;
            }
        }
        else
        {
            isRunning = false;
        }

        if(!Gdata.BoostToggleMode)
            if (Input.GetKeyUp(Gdata.Boost))
                isBoosting = false;

        if (!isMoving)
            isBoosting = false;

        if (gunScript.isZoomed)
        {
            playerSpeed = playerZoomSpeed;
        }

        if (Input.GetKeyDown(Gdata.Build))
        {
            if (Mode == 1)
            {
                Build_Guide_Object.SetActive(true);
                for (int i = 0; i < number_of_tower_types; i++)
                {
                    Build_Guide_Object.transform.Find((i + 1).ToString()).gameObject.SetActive(false);
                }
                Build_Guide_Object.transform.Find(Selected_Tower.ToString()).gameObject.SetActive(true);
                Mode = 2;
                assets.MainBase_Energy_Radius_Material.color = new Color(0.41f, 0.81f, 1, 0.5f);
            }
            else if (Mode == 2)
            {
                Mode = 1;
                Build_Guide_Object.SetActive(false);
                assets.MainBase_Energy_Radius_Material.color = new Color(0.41f, 0.81f, 1, 0);
                assets.Bound_Material.color = new Color(1, 0, 0, 0);
            }
        }

        if (isBoosting)
        {
            UnityEngine.Rendering.VolumeProfile volumeProfile = visionEffect.GetComponent<UnityEngine.Rendering.Volume>()?.profile;
            if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));


            UnityEngine.Rendering.Universal.Vignette vignette;
            if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));


            vignette.color.value = Color.Lerp(vignette.color.value , new Color(0.5f, 0.91f, 1f), Time.deltaTime * 8);
            visionEffectIntensity = Mathf.Lerp(visionEffectIntensity, 0.9f, Time.deltaTime * 8);

            if (isRunning)
                Energy -= Time.deltaTime;
            else
                Energy -= Time.deltaTime / 2;

            if (Energy <= 0.1f)
            {
                isBoosting = false;
                Energy = 0;
            }
        }
        else
        {

        }


        if (isRunning)
        {
            if (Input.GetKeyDown(Gdata.Teleport) && lastTeleportedTime > TeleportCooldown && Energy >= TPCost)
                StartCoroutine(TeleportDash());

            if (isBoosting && !gunScript.isZoomed)
            {
                playerSpeed = Mathf.Lerp(playerSpeed, playerBoosterSpeed, Time.deltaTime * 10);
                jumpHeight = Mathf.Lerp(jumpHeight, boostJumpHeight, Time.deltaTime * 10);
                BoostTrail.enableEmission = true;
            }
            else
            {
                playerSpeed = Mathf.Lerp(playerSpeed, playerRunSpeed, Time.deltaTime * 10);
                jumpHeight = Mathf.Lerp(jumpHeight, defaultJumpHeight, Time.deltaTime * 10);
                BoostTrail.enableEmission = false;
            }
        }
        else
        {
            if (isBoosting && !gunScript.isZoomed)
            {
                playerSpeed = Mathf.Lerp(playerSpeed, playerWalkSpeed * 1.5f, Time.deltaTime * 10);
                jumpHeight = Mathf.Lerp(jumpHeight, boostJumpHeight * 0.75f, Time.deltaTime * 10);
                BoostTrail.enableEmission = false;
            }
            else
            {
                playerSpeed = Mathf.Lerp(playerSpeed, playerWalkSpeed, Time.deltaTime * 10);
                jumpHeight = Mathf.Lerp(jumpHeight, defaultJumpHeight, Time.deltaTime * 10);
                BoostTrail.enableEmission = false;
            }
        }
        if (gunScript.isZoomed && isBoosting)
        {
            playerSpeed = Mathf.Lerp(playerSpeed, playerZoomSpeed * 2, Time.deltaTime * 10);
            jumpHeight = Mathf.Lerp(jumpHeight, defaultJumpHeight, Time.deltaTime * 10);
            BoostTrail.enableEmission = false;
        }

        Vector3 move = x * orientation.transform.right + z * orientation.transform.forward;
        characterController.Move(move * Time.deltaTime * playerSpeed * TPSpeedMultipier);


        if (Input.GetKeyDown(Gdata.Interact))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera_Transform.position, Camera_Transform.transform.forward, out hit, 10, Layer_Selection))
            {
                if (hit.transform.CompareTag("TowerSelect"))
                {
                    TowerScript towerData = hit.transform.GetComponentInParent<TowerScript>();
                    SelectedTower = towerData.gameObject;
                    UI.ShowTowerStatus(towerData.towerData.Name, towerData.HP, towerData.towerData.HP[towerData.Level - 1], towerData.towerData.Damage[towerData.Level - 1], towerData.towerData.fireRate[towerData.Level - 1], towerData.towerData.Range[towerData.Level - 1], towerData.Level, towerData.isDowned);
                    towerData.RangeCylinder.SetActive(true);
                    if (!(towerData.MinRangeCylinder == null))
                        towerData.MinRangeCylinder.SetActive(true);

                    towerData.SelectedEffect.SetActive(true);
                }
                else
                {
                    if (!(SelectedTower == null))
                    {
                        SelectedTower.GetComponent<TowerScript>().RangeCylinder.SetActive(false);
                        SelectedTower.GetComponent<TowerScript>().SelectedEffect.SetActive(false);
                        SelectedTower = null;

                    }
                    UI.HideTowerStatus();
                }

                if (hit.transform.CompareTag("BaseSelect"))
                {
                    MainBaseScript baseScript = hit.transform.GetComponentInParent<MainBaseScript>();
                    Mode = 3;
                }

                if (!(LastSelectedInteractionObject == null))
                {
                    LastSelectedInteractionObject.GetComponent<InteractableObjectScript>().UnSelectObject();
                    LastSelectedInteractionObject = null;
                }

                if (hit.transform.CompareTag("InteractableObject"))
                {

                    LastSelectedInteractionObject = hit.transform.gameObject;
                    LastSelectedInteractionObject.GetComponent<InteractableObjectScript>().SelectObject();
                }

                if (hit.transform.CompareTag("TrapCard"))
                {
                    hit.transform.GetComponent<TrapCardObject>().YouJustActivatedMyTrapCard();
                }
            }
            else
            {
                if (!(SelectedTower == null))
                {
                    SelectedTower.GetComponent<TowerScript>().RangeCylinder.SetActive(false);
                    SelectedTower.GetComponent<TowerScript>().SelectedEffect.SetActive(false);
                    if(!(SelectedTower.GetComponent<TowerScript>().MinRangeCylinder == null))
                    {
                        SelectedTower.GetComponent<TowerScript>().MinRangeCylinder.SetActive(false);
                    }
                    SelectedTower = null;
                }
                UI.HideTowerStatus();

                if (!(LastSelectedInteractionObject == null))
                {
                    LastSelectedInteractionObject.GetComponent<InteractableObjectScript>().UnSelectObject();
                    LastSelectedInteractionObject = null;
                }
            }
        }

        if(!(LastSelectedInteractionObject == null))
        {
            if (Input.GetKey(Gdata.Rummage))
            {
                if (Vector3.Distance(LastSelectedInteractionObject.transform.position, transform.position) < 10)
                {
                    InteractableObjectScript IOScript = LastSelectedInteractionObject.GetComponent<InteractableObjectScript>();
                    RummageTimer += Time.deltaTime;
                    UI.ShowObjectInteractionUI(IOScript.ObjectName, RummageTimer / IOScript.InteractTime);

                    if (RummageTimer > IOScript.InteractTime)
                    {
                        IOScript.Interact();
                        IOScript.UnSelectObject();
                        UI.HideObjectInteractionUI();
                        LastSelectedInteractionObject = null;
                        RummageTimer = 0;
                    }
                }
            }

            if (LastSelectedInteractionObject == null)
                return;

            if (Vector3.Distance(LastSelectedInteractionObject.transform.position, transform.position) > 10)
            {
                InteractableObjectScript IOScript = LastSelectedInteractionObject.GetComponent<InteractableObjectScript>();
                RummageTimer = 0;
                IOScript.UnSelectObject();
                UI.HideObjectInteractionUI();
                LastSelectedInteractionObject = null;
            }

            if (Input.GetKeyUp(Gdata.Rummage))
            {
                RummageTimer = 0;
                UI.HideObjectInteractionUI();
            }
        }


        if (Input.GetKeyDown(Gdata.Upgrade))
        {
            if (!(SelectedTower == null))
            {
                TowerScript script = SelectedTower.GetComponent<TowerScript>();
                if (!script.isDowned && script.Target.name== "NoTargetFound" && script.Level < 5)
                {
                    if (script.towerData.EnergyCosts[script.Level] <= BaseEnergies &&
                    script.towerData.ScrapCosts[script.Level] <= Scraps &&
                    script.towerData.AdvancedResourcesCosts[script.Level] <= AdvancedResources)
                    {
                        Upgrade_SelectedTower();
                        UI.HideTowerStatus();
                        script.RangeCylinder.SetActive(false);
                        script.SelectedEffect.SetActive(false);
                    }
                    else
                    {
                        UI.NOTENOUGHRESOURCES();
                    }
                }
            }
        }

        if (Input.GetKeyDown(Gdata.Ride) && behaviourCooldown > 1 && !gunScript.gunData.reloading && !(SelectedTower == null))
        {
            TowerScript TData = SelectedTower.GetComponent<TowerScript>();
            if (!TData.isDowned && !TData.isUpgrading && Vector3.Distance(transform.position, SelectedTower.transform.position) < 20 && TData.towerData.Ridable)
                Ride();

            if (TData.isDowned && !TData.isUpgrading && Vector3.Distance(transform.position, SelectedTower.transform.position) < 20)
            {
                if (TData.towerData.EnergyCosts[TData.Level - 1] * 0.5f <= BaseEnergies)
                {
                    TData.Recovered();
                    BaseEnergies -= TData.towerData.EnergyCosts[TData.Level - 1] * 0.5f;
                    UI.ShowTowerStatus(TData.towerData.Name, TData.HP, TData.towerData.HP[TData.Level - 1], TData.towerData.Damage[TData.Level - 1], TData.towerData.fireRate[TData.Level - 1], TData.towerData.Range[TData.Level - 1], TData.Level, TData.isDowned);
                }
            }
        }
    }

    public void Ride()
    {
        behaviourCooldown = 0;
        isRiding = true;
        TowerScript towerController = SelectedTower.GetComponent<TowerScript>();
        transform.position = towerController.RidePoint.position;
        towerController.isRiding = true;
        UI.CrossHairModeChange(2);
        UI.HideTowerStatus();
        towerController.SelectedEffect.SetActive(false);
    }

    public void UnRide()
    {
        behaviourCooldown = 0;
        isRiding = false;
        TowerScript towerController = SelectedTower.GetComponent<TowerScript>();
        transform.position = towerController.UnRidePoint.position;
        UI.CrossHairModeChange(1);
        UI.HideTowerStatus();

        Camera_Transform.localScale = new Vector3(1, 1, 1);
    }

    bool isSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, transform.localScale.y * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    

    public void BuildMode()
    {
        if (Input.GetKeyDown(Gdata.NextTower))
        {
            Selected_Tower++;
            if (Selected_Tower > number_of_tower_types)
                Selected_Tower = 1;

            for (int i = 0; i < number_of_tower_types; i++)
            {
                Build_Guide_Object.transform.Find((i + 1).ToString()).gameObject.SetActive(false);
            }
            Build_Guide_Object.transform.Find(Selected_Tower.ToString()).gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(Gdata.PreviousTower))
        {
            Selected_Tower--;
            if (Selected_Tower < 1)
                Selected_Tower = number_of_tower_types;

            for (int i = 0; i < number_of_tower_types; i++)
            {
                Build_Guide_Object.transform.Find((i + 1).ToString()).gameObject.SetActive(false);
            }
            Build_Guide_Object.transform.Find(Selected_Tower.ToString()).gameObject.SetActive(true);
        }


        RaycastHit hit;
        if (Physics.Raycast(Camera_Transform.position, Camera_Transform.transform.forward, out hit, 26,~IgnoreLayer_Build))
        {
            if (Vector3.Distance(transform.position, hit.point) > 6)
            {
                Debug.Log(hit.transform.name);
                Build_Guide_Object.SetActive(true);
                Build_Guide_Object.transform.position = hit.point;

                if (Input.GetKey(Gdata.BuildGuide_TurnRight))
                {
                    Build_Guide_Object.transform.Rotate(0, 0.5f, 0);
                }
                if (Input.GetKey(Gdata.BuildGuide_TurnLeft))
                {
                    Build_Guide_Object.transform.Rotate(0, -0.5f, 0);
                }

                if (!hit.transform.CompareTag("Boundary") && Vector3.Distance(MainBase_Transform.position, hit.point) < MainBase_Transform.Find("Energy_Radius").transform.localScale.x / 2)
                {
                    assets.BuildGuidePrefab_Material.color = new Color(0.28f, 0.8f, 1, 0.5f);
                    assets.Bound_Material.color = new Color(1, 0, 0, 0);
                    if (Input.GetKeyDown(Gdata.Interact) && assets.Pre_Build_Guide_Prefabs[Selected_Tower - 1].GetComponent<BeforeBuildScript>().data.EnergyCosts[0] <= BaseEnergies &&
                        assets.Pre_Build_Guide_Prefabs[Selected_Tower - 1].GetComponent<BeforeBuildScript>().data.ScrapCosts[0] <= Scraps &&
                        assets.Pre_Build_Guide_Prefabs[Selected_Tower - 1].GetComponent<BeforeBuildScript>().data.AdvancedResourcesCosts[0] <= AdvancedResources &&
                        assets.Pre_Build_Guide_Prefabs[Selected_Tower - 1].GetComponent<BeforeBuildScript>().data.Researched)
                    {
                        GameObject newTower = Instantiate(assets.Pre_Build_Guide_Prefabs[Selected_Tower - 1], Build_Guide_Object.transform.position, Build_Guide_Object.transform.rotation);
                        Mode = 1;
                        Build_Guide_Object.SetActive(false);
                        assets.MainBase_Energy_Radius_Material.color = new Color(0.41f, 0.81f, 1, 0);

                        BaseEnergies -= assets.Pre_Build_Guide_Prefabs[Selected_Tower - 1].GetComponent<BeforeBuildScript>().data.EnergyCosts[0];
                        Scraps -= assets.Pre_Build_Guide_Prefabs[Selected_Tower - 1].GetComponent<BeforeBuildScript>().data.ScrapCosts[0];
                        AdvancedResources -= assets.Pre_Build_Guide_Prefabs[Selected_Tower - 1].GetComponent<BeforeBuildScript>().data.AdvancedResourcesCosts[0];
                    }
                }
                else
                {
                    assets.Bound_Material.color = new Color(1, 0, 0, 0.15f);
                    assets.BuildGuidePrefab_Material.color = new Color(1, 0, 0, 0.5f);
                }
            }
            else
            {
                //Build_Guide_Object.SetActive(false);
                assets.BuildGuidePrefab_Material.color = new Color(1, 0, 0, 0.5f);
            }
        }
        else
        {
            //Build_Guide_Object.SetActive(false);
            assets.BuildGuidePrefab_Material.color = new Color(1, 0, 0, 0.5f);
        }
    }

    public void Upgrade_SelectedTower()
    {
        TowerScript script = SelectedTower.GetComponent<TowerScript>();
        if (!script.isUpgrading)
        {
            script.Upgrade();
            BaseEnergies -= script.towerData.EnergyCosts[script.Level];
            Scraps -= script.towerData.ScrapCosts[script.Level];
            AdvancedResources -= script.towerData.AdvancedResourcesCosts[script.Level];
        }
    }

    void visionEffectToDefault()
    {
        if(!isBoosting)
            visionEffectIntensity = Mathf.Lerp(visionEffectIntensity, 0, Time.deltaTime * 2);

        UnityEngine.Rendering.VolumeProfile volumeProfile = visionEffect.GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));


        UnityEngine.Rendering.Universal.Vignette vignette;
        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
        vignette.intensity.Override(visionEffectIntensity);
    }

    public void TakeDamage(float amount)
    {
        if (InvincibleTime > 0)
            return;

        InvincibleTime = 1;

        audio.PlayOneShot(takeHitSoundEffects[UnityEngine.Random.Range(0, takeHitSoundEffects.Count)]);
        LastDamagedTime = 0;
        UnityEngine.Rendering.VolumeProfile volumeProfile = visionEffect.GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));


        UnityEngine.Rendering.Universal.Vignette vignette;
        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        if (Shield > 0)
        {
            vignette.color.value = new Color(0, 0.9f, 0.9f);
            Shield -= amount;
            if(Shield <= 0)
            {
                Shield = 0;
                Debug.Log("SHIELD BROKEN!");
                audio.PlayOneShot(ShieldBreakSoundEffet);
            }
            visionEffectIntensity = 0.8f;
        }
        else
        {
            vignette.color.value = new Color(1, 0, 0);
            HP -= amount;
            visionEffectIntensity = 0.5f;
        }
        UI.ShakeUI(4);


        if (HP <= 0)
            Dead();
    }

    IEnumerator TeleportDash()
    {
        float power = 1;
        while (true)
        {
            if (Input.GetKeyUp(Gdata.Teleport))
                break;

            if(Input.GetKeyDown(Gdata.BackToBase))
            {
                transform.position = BaseTransform.transform.position + BaseTransform.transform.forward * 12;
                lastTeleportedTime = 0;
                Energy -= TPCost;
                StopAllCoroutines();
            }

            yield return new WaitForFixedUpdate();
            power += 1f;

            UI.BlurOn();
            Debug.Log("Teleport Power : "+power);

            power = Mathf.Clamp(power, 0, 100);
        }

        lastTeleportedTime = 0;
        Energy -= TPCost;
        TPSpeedMultipier = 10 * power;
        yield return new WaitForSeconds(0.1f);
        TPSpeedMultipier = 1;
    }

    public void Dead()
    {
        UI.Died();
    }
}
