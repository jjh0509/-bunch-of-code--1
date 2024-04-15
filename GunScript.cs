using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public bool isLobby;

    [Header("References")]
    public DataManager Gdata;
    public GunData gunData;
    [SerializeField] Transform muzzle;
    [SerializeField] Transform flashPoint;
    [SerializeField] Animator anim;
    [SerializeField] GunSway2 gunSway;
    [SerializeField] Light flashLight;
    [SerializeField] float flIntensity;
    public GameManagement gameManager;
    
    public LayerMask IgnoreLayer;

    public bool isGrounded;

    float timeSinceLastShot;

    [SerializeField] Transform camHolder;
    [SerializeField] PlayerController_CharacterController playerController;
    [SerializeField] Transform gunHolder;
    [SerializeField] Transform parent;
    [SerializeField] GameObject prj_effect;
    [SerializeField] GameObject gunGraphics;

    public int SelectedWeapon;
    public int NumOfWeapons;

    private int a;

    [SerializeField] Camera cam;
    public float Zoom_FOV;

    public float runswayspeed;
    public float zoomSpeed;
    public float SpreadMultiplier;
    public float JumpSpreadMultiplier;

    public bool isZoomed;
    private bool isMoving;

    public bool alreadyShooted;

    public DataManager datamanager;
    public AssetList assets;
    private void Start()
    {
        if (isLobby)
            return;

        flIntensity = 0;
        Zoom_FOV = 60;
        a = 1;
        isZoomed = false;
        alreadyShooted = false;
        SpreadMultiplier = 1;
        JumpSpreadMultiplier = 1;
        isMoving = false;

        timeSinceLastShot = 0;
        PlayerController_CharacterController.shootInput += Shoot;
        playerController = GameObject.Find("Player").GetComponent<PlayerController_CharacterController>();
        gunHolder = GameObject.Find("CameraHolder").transform.Find("Parent_GunHolder").transform.Find("GunHolder").GetComponent<Transform>();
        parent = GameObject.Find("CameraHolder").transform.Find("Parent_GunHolder").GetComponent<Transform>();
        camHolder = GameObject.Find("CameraHolder").GetComponent<Transform>();
        cam = GameObject.Find("CameraHolder").transform.Find("Main Camera").GetComponent<Camera>();
        gunSway = GetComponent<GunSway2>();
        gunGraphics = transform.Find("GunGraphics").gameObject;
        gameManager = GameObject.Find("DataManager").GetComponent<GameManagement>();
        Gdata = GameObject.Find("DataManager").GetComponent<DataManager>();

        ChangeWeapon(SelectedWeapon);

        FindWeaponPoints();

        gunData.currentAmmo = gunData.magSize;
        datamanager = GameObject.Find("DataManager").GetComponent<DataManager>();
        gunSway = GetComponent<GunSway2>();

        assets = GameObject.Find("DataManager").GetComponent<AssetList>();
        assets.GunGlowMaterial.SetColor("_BaseColor", gunData.GlowColor);
        assets.GunGlowMaterial.SetColor("_EmissionColor", gunData.GlowColor * Mathf.LinearToGammaSpace(25f));

        assets.GunMagazineMaterial.SetColor("_BaseColor", gunData.MagazineColor);
        assets.GunMagazineMaterial.SetColor("_EmissionColor", gunData.MagazineColor * Mathf.LinearToGammaSpace(100f));

        gunGraphics.SetActive(true);
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f) && playerController.Mode == 1;

    public void StartReload()
    {
        if (!gunData.reloading && playerController.Energy >= gunData.reloadCostEnergy)
        {
            StartCoroutine(Reload());
        }

        if (playerController.Energy < gunData.reloadCostEnergy * (gunData.magSize - gunData.currentAmmo))
            GameObject.Find("Canvas").GetComponent<UIScript>().NOTENOUGHENERGY();
    }

    void FindWeaponPoints()
    {
        flashPoint = anim.gameObject.transform.Find("FlashPoint").GetComponent<Transform>();
    }

    private IEnumerator Reload()
    {
        if (SelectedWeapon == 3)
            anim.enabled = true;
        anim.SetTrigger("Reload");

        gunData.reloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        

        if (SelectedWeapon == 3)
            anim.enabled = false;

        if (playerController.Energy >= gunData.reloadCostEnergy * (gunData.magSize - gunData.currentAmmo)) 
        {
            playerController.Energy -= gunData.reloadCostEnergy * (gunData.magSize - gunData.currentAmmo);
            gunData.currentAmmo = gunData.magSize;
        }
        else
        {
            int i = 0;
            while (true)
            {
                i++;
                if (i * gunData.reloadCostEnergy >= playerController.Energy)
                    break;
            }
            gunData.currentAmmo += i;
            playerController.Energy -= i * gunData.reloadCostEnergy;
        }


        assets.GunMagazineMaterial.SetColor("_EmissionColor", gunData.MagazineColor * Mathf.LinearToGammaSpace(100f));
        gunData.reloading = false;
        StopAllCoroutines();
    }

    public void Shoot()
    {
        if(gunData.currentAmmo > 0 && !playerController.isRunning && playerController.Mode == 1)
        {
            if (CanShoot())
            {
                if (!gunData.CanAuto && alreadyShooted)
                    return;
                if (gunData.Projectile == null)
                {

                    if (Physics.Raycast(muzzle.position, -transform.right + new Vector3(Random.Range(-gunData.Spread * SpreadMultiplier * JumpSpreadMultiplier, gunData.Spread * SpreadMultiplier * JumpSpreadMultiplier), Random.Range(-gunData.Spread * SpreadMultiplier * JumpSpreadMultiplier, gunData.Spread * SpreadMultiplier * JumpSpreadMultiplier), 0), out RaycastHit hitInfo, gunData.maxDistance, ~IgnoreLayer))
                    {
                        GameObject effect = Instantiate(prj_effect, hitInfo.point, Quaternion.identity);
                        /*
                        if (hitInfo.transform.CompareTag("Enemy"))
                        {
                            hitInfo.transform.GetComponent<EnemyController>().lastHitDirection.forward = -muzzle.forward;
                            hitInfo.transform.GetComponent<EnemyController>().TakeDamage(gunData.Damage);
                            effect.transform.parent = hitInfo.transform;
                        }
                        */
                        if (hitInfo.transform.CompareTag("EnemyGraphic"))
                        {
                            EnemyController controller = hitInfo.transform.GetComponentInParent<EnemyController>();
                            if (Random.Range(0, 101) <= gameManager.CriticalChance)
                            {
                                controller.TakeDamage(gunData.Damage * 2, gunData.ArmorPiercing, true);
                                controller.lastHitDirection.forward = -muzzle.forward;

                                GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(true);
                            }
                            else
                            {
                                controller.TakeDamage(gunData.Damage, gunData.ArmorPiercing, false);
                                controller.lastHitDirection.forward = -muzzle.forward;

                                GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(false);
                            }
                            Transform Parent = controller.transform.GetComponent<Transform>();
                            effect.transform.parent = Parent;
                        }
                        if (hitInfo.transform.CompareTag("EnemyShield") || hitInfo.transform.CompareTag("EnemyWeapon"))
                        {
                            EnemyController controller = hitInfo.transform.GetComponentInParent<EnemyController>();
                            Transform Parent = controller.transform.GetComponent<Transform>();
                            effect.transform.parent = Parent;
                        }
                        if (hitInfo.transform.CompareTag("TestHitObject"))
                        {
                            GameObject.Find("Canvas").GetComponent<UIScript>().DamageTaken(Random.Range(0f,10f) > 8f);
                        }

                        Destroy(effect, 1f);
                    }
                }
                else
                {
                    GameObject prj = Instantiate(gunData.Projectile, flashPoint.position, flashPoint.rotation);
                    prj.GetComponent<RigidbodyProjectileScript>().Spread = gunData.Spread * SpreadMultiplier;
                    prj.GetComponent<RigidbodyProjectileScript>().Damage = gunData.Damage;
                    prj.GetComponent<RigidbodyProjectileScript>().playerProjectile = true;
                    if (SelectedWeapon == 3)
                    {
                        if (isZoomed)
                            prj.GetComponent<RigidbodyProjectileScript>().zoomed = true;
                        anim.gameObject.transform.Find("Cylinder001").GetComponent<GrenadeLauncherMagazineScript>().ShotUpdateMaterial();
                    }
                }
                playerController.audio.PlayOneShot(gunData.FireSound);

                gunData.currentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();
                alreadyShooted = true;
                Recoil_Camera();
            }
        }
    }

    private void Recoil_Camera()
    {
        playerController.orientation.Rotate(camHolder.up * Random.Range(gunData.Recoil / 2, gunData.Recoil * 2), Random.Range(gunData.Recoil / 2, gunData.Recoil * 2),0);
    }

    IEnumerator Recoil(float Strength)
    {
        gunHolder.localPosition = new Vector3(gunHolder.localPosition.x, gunHolder.localPosition.y, gunHolder.localPosition.z - Strength);
        for (int i = 0; i < 25; i++)
        {
            gunHolder.localPosition = new Vector3(gunHolder.localPosition.x, gunHolder.localPosition.y, gunHolder.localPosition.z + (Strength/25));
            yield return new WaitForSeconds(0.001f);
        }
    }
    private void Update()
    {
        if (isLobby)
            return;

        if (playerController.isRiding)
        {
            gunGraphics.SetActive(false);
            return;
        }
        else
        {
            gunGraphics.SetActive(true);
        }

        Debug.DrawRay(muzzle.position, -transform.right * gunData.maxDistance, Color.red);
        if (Input.GetKey(Gdata.Run) && gunData.reloading)
        {
            StopAllCoroutines();
            gunData.reloading = false;
            if (SelectedWeapon == 3)
            {
                gunData.currentAmmo = 0;
                anim.gameObject.transform.Find("Cylinder001").GetComponent<GrenadeLauncherMagazineScript>().StopReload();
            }
        }
        anim.SetBool("IsRunning", playerController.isRunning);
        isGrounded = playerController.isGrounded;
        timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(muzzle.position, muzzle.forward);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            isMoving = true;
        else
            isMoving = false;

        if (isMoving && !isZoomed)
        {
            if(playerController.isRunning)
                SpreadMultiplier = 2.5f;
            else
                SpreadMultiplier = 1.5f;
        }
        else if (!isMoving && !isZoomed)
            SpreadMultiplier = 1f;
        else if (isZoomed)
            SpreadMultiplier = 0f;

        if (playerController.isGrounded)
            JumpSpreadMultiplier = 1;
        else
            JumpSpreadMultiplier = 2;

        GameObject.Find("Canvas").GetComponent<UIScript>().Update_Crosshair_Spread(gunData.Spread * SpreadMultiplier * JumpSpreadMultiplier);

        /*
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StopAllCoroutines();
            cam.fieldOfView = 60;
            gunData.reloading = false;
        }
        if (Input.GetKey(KeyCode.LeftShift) && playerController.isMoving)
        {
            cam.fieldOfView += Time.deltaTime * 50;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 60, 70);
            gunHolder.localRotation = Quaternion.Euler(10, 90, -40);
            gunHolder.localPosition = new Vector3(0.4f, 0, 0.6f);
        }
        else if (!Input.GetKey(KeyCode.LeftShift) || !playerController.isMoving)
        {
            cam.fieldOfView -= Time.deltaTime * 50;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 60, 70);
            gunHolder.localPosition = new Vector3(0, 0, 1);
            gunHolder.localRotation = Quaternion.Euler(0, 90, 0);
        }

        */

        if (playerController.Mode == 1)
        {
            assets.GunGlowMaterial.SetColor("_EmissionColor", gunData.GlowColor * Mathf.LinearToGammaSpace(25f));
            if (Input.GetKeyDown(Gdata.Reload))
            {
                StartReload();
            }

            if (datamanager.ZoomToggleMode)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    isZoomed = !isZoomed;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(1))
                {
                    isZoomed = true;
                }

                if (Input.GetMouseButtonUp(1))
                {
                    isZoomed = false;
                }
            }

            if (gunData.reloading)
                isZoomed = false;

            if (playerController.isRunning && isMoving)
                Running();
            else
                Not_Running();

            if (isZoomed)
                Zoom_In();
            else
                Zoom_Out();

            GunSwayBobChange();

            if (Input.GetMouseButtonUp(0))
            {
                alreadyShooted = false;
            }
        }
        else if (playerController.Mode == 2)
        {
            assets.GunGlowMaterial.SetColor("_EmissionColor", new Color(2.92f, 1.3f, 0.29f) * Mathf.LinearToGammaSpace(25f));
        }

        
        //Debug for WeaponChange
        if (Input.GetKeyDown(KeyCode.Keypad0) && !gunData.reloading && datamanager.isDebugMode)
        {
            a++;
            if (a > NumOfWeapons)
                a = 1;
            ChangeWeapon(a);
            gunData.currentAmmo = gunData.magSize;
        }
        
        if(isZoomed)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Zoom_FOV, Time.deltaTime * 5);

        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hit, 1000, ~IgnoreLayer))
        {
            flIntensity = Vector3.Distance(transform.position, hit.point) * 1.25f;
            flashLight.intensity = Mathf.Lerp(flashLight.intensity, flIntensity, Time.deltaTime * 5);
        }

        if (Input.GetKeyDown(datamanager.FlashlightToggle))
        {
            flashLight.enabled = !flashLight.enabled;
        }
    }

    public void OnGunShot()
    {
        StartCoroutine(Recoil(gunData.Recoil));
        GameObject flash = Instantiate(gunData.weaponFlash, flashPoint.position, camHolder.rotation);
        flash.transform.parent = flashPoint;

        assets.GunMagazineMaterial.SetColor("_EmissionColor", gunData.MagazineColor * Mathf.LinearToGammaSpace((float)(gunData.currentAmmo / (float)gunData.magSize) * 100));
        Debug.Log((float)(gunData.currentAmmo / (float)gunData.magSize));
        //flash.transform.rotation = camHolder.rotation;
        Destroy(flash, 1);

        GameObject.Find("Player").GetComponent<PlayerCamera_Temp>().Recoil(Random.Range(-gunData.maxRecoil / 3, gunData.maxRecoil / 3), Random.Range(gunData.maxRecoil / 2, gunData.maxRecoil));
    }

    public void ChangeWeapon(int num)
    {
        
        SelectedWeapon = num;
        if(SelectedWeapon == 2 || SelectedWeapon == 3 || SelectedWeapon == 4)
            anim = transform.Find("GunGraphics").transform.Find("GunGraphic_" + SelectedWeapon).transform.Find("Pivot").GetComponent<Animator>();
        else
            anim = transform.Find("GunGraphics").transform.Find("GunGraphic_" + SelectedWeapon).GetComponent<Animator>();
        gunData = anim.gameObject.GetComponent<GunDataInGunGraphic>().Data;

        flashLight = anim.transform.GetComponentInChildren<Light>();

        for (int i = 0; i < NumOfWeapons; i++)
        {
            transform.Find("GunGraphics").transform.Find("GunGraphic_" + (i + 1)).gameObject.SetActive(false);
        }
        transform.Find("GunGraphics").transform.Find("GunGraphic_" + SelectedWeapon).gameObject.SetActive(true);
        if (SelectedWeapon == 1)
        {
            anim.gameObject.transform.Find("Box001").transform.localPosition = new Vector3(0, 61, 11.5f);
            anim.gameObject.transform.Find("Box001").transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        else if (SelectedWeapon == 2)
        {
            anim.gameObject.transform.Find("Box001").transform.localPosition = new Vector3(0, 0.3f, 0);
            anim.gameObject.transform.Find("Box001").transform.localRotation = Quaternion.Euler(-90, 0, 0);
            anim.gameObject.transform.Find("Box086").transform.localPosition = new Vector3(0, 0.3f, 0);
            anim.gameObject.transform.Find("Box086").transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        else if (SelectedWeapon == 3)
        {
            anim.gameObject.transform.Find("Box001").transform.localPosition = new Vector3(0, 0, 0);
            anim.gameObject.transform.Find("Box001").transform.localRotation = Quaternion.Euler(-90, 0, 0);
            anim.gameObject.transform.Find("Cylinder001").transform.localPosition = new Vector3(0, 0, 0);
            anim.gameObject.transform.Find("Cylinder001").transform.localRotation = Quaternion.Euler(0, -90, 90);
            anim.enabled = false;
        }
        else if (SelectedWeapon == 3)
        {
            anim.gameObject.transform.Find("Box003").transform.localPosition = new Vector3(0, 0, 0);
            anim.gameObject.transform.Find("Box003").transform.localRotation = Quaternion.Euler(-90, 0, 0);
            anim.gameObject.transform.Find("Cylinder004").transform.localPosition = new Vector3(0, 0, 0);
            anim.gameObject.transform.Find("Cylinder004").transform.localRotation = Quaternion.Euler(-90, 0, 0);
            anim.gameObject.transform.Find("GeoSphere001").transform.localPosition = new Vector3(0, 0, 0);
            anim.gameObject.transform.Find("GeoSphere001").transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        FindWeaponPoints();
    }

    private void Running()
    {
        isZoomed = false;

        if (gunData.Type == 1)
        {
            parent.localPosition = Vector3.Slerp(parent.localPosition, new Vector3(0.1f, -0.8f, 0.25f), Time.deltaTime * 20);
            parent.localRotation = Quaternion.Slerp(parent.localRotation, Quaternion.Euler(-60, 15, -15), Time.deltaTime * 20);
        }
        else if(gunData.Type == 2)
        {
            parent.localPosition = Vector3.Slerp(parent.localPosition, new Vector3(0.8f, -0.25f, 0.35f), Time.deltaTime * 20);
            parent.localRotation = Quaternion.Slerp(parent.localRotation, Quaternion.Euler(-7.5f, -90, 0), Time.deltaTime * 20);
        }
    }

    private void Not_Running()
    {
        parent.localPosition = Vector3.Slerp(parent.localPosition, new Vector3(0, 0, 0), 0.1f);
        parent.localRotation = Quaternion.Slerp(parent.localRotation, Quaternion.Euler(0, 0, 0), 0.1f * Time.deltaTime * 200);
    }

    private void Zoom_In()
    {
        if (SelectedWeapon == 1)
        {
            gunHolder.transform.localPosition = Vector3.Lerp(gunHolder.transform.localPosition, new Vector3(0, -0.14f, 0.44f), zoomSpeed * Time.deltaTime * 200);
            Zoom_FOV = Mathf.Lerp(Zoom_FOV, 60, zoomSpeed * Time.deltaTime * 200);
        }
        else if (SelectedWeapon == 2)
        {
            gunHolder.transform.localPosition = Vector3.Lerp(gunHolder.transform.localPosition, new Vector3(0, -0.182f, 0.22f), zoomSpeed * Time.deltaTime * 200);
            Zoom_FOV = Mathf.Lerp(Zoom_FOV, 60, zoomSpeed * Time.deltaTime * 200);
        }
        else if (SelectedWeapon == 3)
        {
            gunHolder.transform.localPosition = Vector3.Lerp(gunHolder.transform.localPosition, new Vector3(-0.01f, -0.261f, -0.1f), zoomSpeed * Time.deltaTime * 200);
            Zoom_FOV = Mathf.Lerp(Zoom_FOV, 18, zoomSpeed * Time.deltaTime * 200);
            anim.gameObject.transform.Find("Box001").transform.Find("ScopeLightPivot").gameObject.SetActive(true);
        }
        else if(SelectedWeapon == 4)
        {
            gunHolder.transform.localPosition = Vector3.Lerp(gunHolder.transform.localPosition, new Vector3(0, -0.15f, 0.04f), zoomSpeed * Time.deltaTime * 200);
            Zoom_FOV = Mathf.Lerp(Zoom_FOV, 30, zoomSpeed * Time.deltaTime * 200);
        }
    }

    private void Zoom_Out()
    {
        gunHolder.transform.localPosition = Vector3.Lerp(gunHolder.transform.localPosition, new Vector3(0.5f, -0.3f, 0.68f), zoomSpeed * Time.deltaTime * 200);
        Zoom_FOV = Mathf.Lerp(Zoom_FOV, 60, zoomSpeed * Time.deltaTime * 200);
        if(SelectedWeapon == 3)
            anim.gameObject.transform.Find("Box001").transform.Find("ScopeLightPivot").gameObject.SetActive(false);

    }

    private void GunSwayBobChange()
    {
        if (isZoomed)
        {
            transform.GetComponent<GunSway2>().bobExaggeration = 1;
            if (SelectedWeapon == 1)
            {
                gunSway.multiplier.y = 0;
                gunSway.bobLimit.x = 0.01f;
                gunSway.bobLimit.y = 0.005f;
            }
            else if (SelectedWeapon == 2)
            {
                gunSway.multiplier.y = 0;
                gunSway.bobLimit.x = 0.004f;
                gunSway.bobLimit.y = 0.001f;
            }
            else if(SelectedWeapon == 3)
            {
                gunSway.multiplier.y = 0;
                gunSway.bobLimit.x = 0.002f;
                gunSway.bobLimit.y = 0.001f;
            }
            else if(SelectedWeapon == 4)
            {
                gunSway.multiplier.y = 0;
                gunSway.bobLimit.x = 0.004f;
                gunSway.bobLimit.y = 0.001f;
            }
        }
        else
        {
            if (playerController.isRunning)
            {
                gunSway.multiplier.y = 2;
                gunSway.bobLimit.x = 0.02f;
                gunSway.bobLimit.y = 0.02f;
                gunSway.bobExaggeration = 15;
            }
            else
            {
                gunSway.multiplier.y = 2;
                gunSway.bobLimit.x = 0.02f;
                gunSway.bobLimit.y = 0.02f;
                gunSway.bobExaggeration = 5;
            }
        }
    }
}
