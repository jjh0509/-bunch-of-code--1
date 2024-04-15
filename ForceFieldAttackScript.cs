using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldAttackScript : MonoBehaviour
{
    public float Damage;

    public float MaxDistance;
    public float LifeTime;
    public float DisappearTime;
    public float currentLifeTime;
    public float DiffusionSpeed;

    public float StunTime;
    public float StunSpeedPercent;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        currentLifeTime = 0;
    }

    private void Update()
    {
        currentLifeTime += Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentLifeTime > LifeTime)
        {
            Destroy(gameObject);
        }

        if (currentLifeTime > DisappearTime)
        {
            transform.localScale = Vector3.Slerp(transform.localScale, new Vector3(MaxDistance / 2, 0, MaxDistance / 2), DiffusionSpeed * 1.25f);
            transform.Translate(0, -1, 0);
        }
        else
            transform.localScale = Vector3.Slerp(transform.localScale, new Vector3(MaxDistance, MaxDistance, MaxDistance), DiffusionSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(Damage, false, false);
            other.GetComponent<EnemyController>().Stun(StunTime, StunSpeedPercent);
        }
    }
}
