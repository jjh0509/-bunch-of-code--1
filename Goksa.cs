using UnityEngine;

public class Goksa : MonoBehaviour
{
    public bool playerProjectile;

    public Vector3 target; // 목표 지점
    public float velocity = 10f; // 투사체의 속도

    public float fire_angle = 5f; //곡사각
    private Rigidbody rb;

    public GameObject Particle_Explode;
    public float explosion_radius = 1;
    public float damage;

    public float max_Lifetime = 100f; //라이프타임
    public bool KeepChanging;
    public bool SeeYpos;

    public bool Fire;

    private float current_lifetime;
    public bool alreadyExploded;

    private void Start()
    {
        alreadyExploded = false;
        Fire = false;
        rb = GetComponent<Rigidbody>();
        KeepChanging = true;
        SeeYpos = false;
        current_lifetime = 0;
    }


    private void Update()
    {
        if(Fire)
            current_lifetime += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        if (target == null || !Fire)
            return;
        /*
        float distance_from_target = Vector3.Distance(transform.position, target);
        if(target.y - 0.4f > transform.position.y && SeeYpos)
        {
            GameObject explode = Instantiate(Particle_Explode, transform.position, Quaternion.identity);
            explode.transform.localScale = new Vector3(explosion_radius, explosion_radius, explosion_radius);
            Destroy(gameObject);
        }
        
        if (current_lifetime > 5)
            SeeYpos = true;
        */
        if (KeepChanging)
        {
            Vector3 direction = target - transform.position;
            float distance = direction.magnitude;


            // 목표 지점까지의 시간 계산
            float time = distance / velocity;

            // 포물선 궤적 계산
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-fire_angle * Physics.gravity.y * distance / velocity);
            Vector3 velocityXZ = direction / time;

            transform.rotation = Quaternion.Euler(velocityXZ);
            // 투사체의 속도 설정
            rb.velocity = velocityXZ + velocityY;
            if (transform.position.y < target.y + 10 && SeeYpos)
            {
                KeepChanging = false;
            }
        }

        float distance_from_target = Vector3.Distance(transform.position, target);
        if (distance_from_target < 10f)
        {
            KeepChanging = false;
            rb.useGravity = true;
        }

        if (current_lifetime > max_Lifetime)
        {
            Explode();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyExploded)
            return;
        Explode();
    }

    private void Explode()
    {
        alreadyExploded = true;
        GameObject explode = Instantiate(Particle_Explode, transform.position, Quaternion.identity);
        explode.gameObject.GetComponentInChildren<Explosion_Particle_Damage_Radius_BothSide>().isPlayerProjectile = playerProjectile;
        explode.gameObject.GetComponentInChildren<Explosion_Particle_Damage_Radius_BothSide>().dmg = damage;
        explode.transform.localScale = new Vector3(explosion_radius, explosion_radius, explosion_radius);
        Destroy(gameObject);
    }
}
