using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EnemyHPBar : MonoBehaviour
{
    public Transform Fill;
    public Transform TransformPivot;
    public EnemyController parent;
    public Transform player;
    public TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        TransformPivot = transform.parent;
        parent = GetComponentInParent<EnemyController>();
        Fill = transform.Find("Bar").transform.Find("Pivot").GetComponent<Transform>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        text = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        TransformPivot.LookAt(player);

        if(parent.current_HP > 0 && parent.HP != 0)
            Fill.localScale = new Vector3(parent.current_HP / parent.HP, 1, 1);
        text.text = parent.HP + "       /       " + parent.current_HP;
    }
}
