using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationScript : MonoBehaviour
{
    public PlayerController_CharacterController playerScript;
    public EnemyController controller;
    public GameObject Effect_AttackRadius1;


    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController_CharacterController>();
        controller = GetComponentInParent<EnemyController>();
    }

    public void SpawnAnimationEnded()
    {
        controller.SpawnAnimationPlaying = false;
    }

    public void MotionDelayTimeSet(float time)
    {
        controller.MotionDelay(time);
    }

    public void MeleeAttack_CollisionOn(int AttackNum)
    {
        controller.MeleeColliders[AttackNum - 1].enabled = true;
    }

    public void MeleeAttack_CollisionOff(int AttackNum)
    {
        controller.MeleeColliders[AttackNum - 1].enabled = false;
    }

    public void MeleeAttackHitEffectOn()
    {
        controller.MeleeAttackHitEffect();
    }

    public void KnightMeleeAttack_MagicCircle()
    {
        controller.KnightMeleeAttackParticle();
    }

    public void TrailEffectsOn()
    {
        controller.TrailEffectOn();
    }

    public void TrailEffectsOff()
    {
        controller.TrailEffectOff();
    }
}
