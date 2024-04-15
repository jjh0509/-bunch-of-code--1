using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Script : MonoBehaviour
{
    public Vector3 TargetPoint;

    [Header("Stat")]
    public float damage;
    public float speed;


    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
