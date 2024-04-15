using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttackScript : MonoBehaviour
{
    public EnemyController parent;

    private void Start()
    {
        parent = GetComponentInParent<EnemyController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController_CharacterController>().TakeDamage(parent.AttackDamage);
        }
        if (other.CompareTag("Tower"))
        {
            other.GetComponent<TowerScript>().TakeDamage(parent.AttackDamage);
        }
        if (other.CompareTag("MainBase"))
        {
            other.GetComponent<MainBaseScript>().TakeDamage(parent.AttackDamage);
        }
        if (other.CompareTag("MainBaseGraphic"))
        {
            other.GetComponentInParent<MainBaseScript>().TakeDamage(parent.AttackDamage);
        }
    }
}
