using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextScript : MonoBehaviour
{
    public TextMeshPro text;
    public Rigidbody rb;
    public Transform player;

    public float Damage;
    public bool isCritical;
    public float Force;
    public float LifeTime;
    public float ExtraGravity;
    private float deltaTimer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        text = transform.Find("Text").GetComponent<TextMeshPro>();

        deltaTimer = 0;

        transform.Rotate(Random.Range(-20, 20), Random.Range(-10, 10), 0);
        rb.AddForce(transform.up * Force,ForceMode.Impulse);

        transform.LookAt(player);

        text.text = Damage.ToString();
        if (isCritical)
            text.color = new Color(1, 0, 0);
        else
            text.color = new Color(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), Time.deltaTime * 1.5f);

        rb.AddForce(Vector3.down * ExtraGravity);
        deltaTimer += Time.deltaTime;
        if (deltaTimer > LifeTime)
            Destroy(gameObject);
    }    
}
