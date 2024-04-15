using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMoveForRecording : MonoBehaviour
{
    public CharacterController cc;

    [Header("Movement Stat")]
    public float Speed;
    public float RotateSpeed;
    public float sensX;
    public float sensY;
    public float multiplier;

    private float mouseX;
    private float mouseY;

    private float xRotation;
    private float yRotation;


    private void Start()
    {
        cc = GetComponent<CharacterController>();

        mouseX = 0;
        mouseY = 0;
        xRotation = 0;
        yRotation = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void FixedUpdate()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        if (Input.GetKey(KeyCode.W))
        {
            cc.Move(transform.forward * Speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            cc.Move(transform.forward * -Speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            cc.Move(transform.right * -Speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            cc.Move(transform.right * Speed);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, RotateSpeed, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -RotateSpeed, 0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                sensX += 1f;
                sensY += 1f;
            }
            else
                Speed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                sensX -= 1f;
                sensY -= 1f;
            }
            else
                Speed -= 0.1f;
        }
    }
}
