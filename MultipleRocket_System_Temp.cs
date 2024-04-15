using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleRocket_System_Temp : MonoBehaviour
{
    public Transform target; // 대상 타겟
    public GameObject projectilePrefab; // 발사체 프리팹
    public Transform firePoint; // 발사 지점
    public float fireInterval = 2f; // 발사 간격
    public float projectileSpeed = 10f; // 발사체 속도
    public float Fire_Spread = 10f; //탄퍼짐

    public bool firing;

    public GameObject explosionPrefab; //폭발 이펙트 프리펩

    private float fireTimer = 0f; // 발사 타이머

    private void Start()
    {
        firing = false;
    }

    void Update()
    {
        if (target == null)
        {
            // 타겟이 없으면 발사하지 않음
            return;
        }

        // 발사 타이머 업데이트
        if(!firing)
            fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            // 발사 간격이 지났으면 발사
            fireTimer = 0f;
            IEnumerator fire = Fire_Multiple(8, 0.1f);
            StartCoroutine(fire);
        }
    }

    void Fire()
    {
        // 발사체 프리팹을 발사 지점에서 생성
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<Goksa>().velocity = projectileSpeed;
        Vector3 spreadpos = new Vector3(target.position.x - Random.RandomRange(-Fire_Spread, Fire_Spread), target.position.y, target.position.z - Random.RandomRange(-Fire_Spread, Fire_Spread));
        Vector3 direction = (spreadpos - firePoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        projectile.GetComponent<Goksa>().target = spreadpos;
        projectile.GetComponent<Goksa>().Particle_Explode = explosionPrefab;
    }

    IEnumerator Fire_Multiple(int num, float delay)
    {
        firing = true;
        for (int i = 0; i < num; i++)
        {
            Fire();
            yield return new WaitForSeconds(delay);
        }
        firing = false;
    }
}
