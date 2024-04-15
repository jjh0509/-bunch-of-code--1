using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCamera_Temp : MonoBehaviour
{
    public bool isLobby;
    public float sensX;
    public float sensY;

    [SerializeField] private Transform cam;
    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerController_CharacterController playerScript;
    [SerializeField] private GunScript gunScript;
    [SerializeField] private Camera maincam;
    [SerializeField] private ParticleSystem boostParticle;

    float mouseX;
    float mouseY;


    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    private void Start()
    {
        if (isLobby) return;

        playerScript = GameObject.Find("Player").GetComponent<PlayerController_CharacterController>();
        gunScript = GameObject.Find("CameraHolder").GetComponentInChildren<GunScript>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GameObject.Find("CameraHolder").GetComponent<Transform>();
        orientation = GameObject.Find("Player").transform.Find("Orientation").GetComponent<Transform>();
        maincam = cam.Find("Main Camera").GetComponent<Camera>();
        boostParticle = maincam.transform.Find("BoostParticle").GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (isLobby) return;

        if (playerScript.Mode == 3)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MyInput();
        if (playerScript.isRiding)
        {
            cam.localRotation = Quaternion.Euler(0, 0, 0);
            playerScript.mouseRotInputs = Quaternion.Euler(xRotation, 0, 0);

            if (Input.GetKey(KeyCode.C))
                maincam.fieldOfView = Mathf.Lerp(maincam.fieldOfView, 20, Time.deltaTime * 4);
            else
                maincam.fieldOfView = Mathf.Lerp(maincam.fieldOfView, 60, Time.deltaTime * 4);
        }
        else
        {
            cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            boostParticle.gameObject.SetActive(playerScript.isBoosting && playerScript.isRunning);

            if (playerScript.isBoosting && !gunScript.isZoomed)
            {
                maincam.fieldOfView = Mathf.Lerp(maincam.fieldOfView, 80, Time.deltaTime * 4);
            }
            else if(!playerScript.isBoosting && !gunScript.isZoomed)
            {
                maincam.fieldOfView = Mathf.Lerp(maincam.fieldOfView, 60, Time.deltaTime * 4);
            }
        }
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        if(playerScript.isRiding)
            xRotation = Mathf.Clamp(xRotation, playerScript.SelectedTower.GetComponent<TowerScript>().RideCamMinAngle, playerScript.SelectedTower.GetComponent<TowerScript>().RideCamMaxAngle);
        else
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    public void Recoil(float x, float y)
    {
        xRotation -= y;
        yRotation += x;
    }
}