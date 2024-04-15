using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muzzleflashLightLerp : MonoBehaviour
{
    public Light muzzle_light;
    public Transform flash;
    public Transform muzzlepoint;
    private void Start()
    {
        muzzle_light = GetComponent<Light>();
        flash = transform.Find("MuzzleFlame");
        muzzlepoint = GameObject.Find("CameraHolder").transform.Find("BulletPoint").GetComponent<Transform>();
    }

    private void Update()
    {
        muzzle_light.intensity -= Time.deltaTime * 2;
        flash.position = muzzlepoint.position;
    }
}
