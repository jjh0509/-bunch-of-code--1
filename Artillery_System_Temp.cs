using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery_System_Temp : MonoBehaviour
{
    public Transform target; // ��� Ÿ��
    public GameObject projectilePrefab; // �߻�ü ������
    public Transform firePoint; // �߻� ����
    public float fireInterval = 2f; // �߻� ����
    public float projectileSpeed = 10f; // �߻�ü �ӵ�
    public float Fire_Spread = 10f; //ź����

    public GameObject explosionPrefab; //���� ����Ʈ ������

    private float fireTimer = 0f; // �߻� Ÿ�̸�

    void Update()
    {
        if (target == null)
        {
            // Ÿ���� ������ �߻����� ����
            return;
        }

        // �߻� Ÿ�̸� ������Ʈ
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            // �߻� ������ �������� �߻�
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        // �߻�ü �������� �߻� �������� ����
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<Goksa>().velocity = projectileSpeed;
        Vector3 spreadpos = new Vector3(target.position.x - Random.RandomRange(-Fire_Spread, Fire_Spread), target.position.y, target.position.z - Random.RandomRange(-Fire_Spread, Fire_Spread));
        Vector3 direction = (spreadpos - firePoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        projectile.GetComponent<Goksa>().target = spreadpos;
        projectile.GetComponent<Goksa>().Particle_Explode = explosionPrefab;
    }
}
