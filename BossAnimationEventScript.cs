using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationEventScript : MonoBehaviour
{
    public BossMoveScript parentScript;
    // Start is called before the first frame update
    void Start()
    {
        parentScript = GetComponentInParent<BossMoveScript>();
    }

    public void AnimStart()
    {
        parentScript.AnimationPlaying = true;
    }

    public void AnimEnd()
    {
        parentScript.AnimationPlaying = false;
    }

    public void HammerPreEffectOn()
    {
        parentScript.HammerPreEffect();
    }

    public void HammerEffectOn()
    {
        parentScript.HammerEffect();
    }

    public void SpearDrop()
    {
        parentScript.StartCoroutine(parentScript.SpearAttackCoroutine());
    }

    public void SwordTrailOn()
    {
        parentScript.SwordTrailOn();
    }
    public void SwordTrailOff()
    {
        parentScript.SwordTrailOff();
    }

    public void SwordParticleOn()
    {
        parentScript.SwordParticleOn();
    }
    public void SwordParticleOff()
    {
        parentScript.SwordParticleOff();
    }
    public void SwordAttackDetectionOn()
    {
        parentScript.SwordAttackDetectionOn();
    }

    public void SpearTrailOn()
    {
        parentScript.SpearTrailOn();
    }
    public void SpearTrailOff()
    {
        parentScript.SpearTrailOff();
    }

    public void AxeAttackIceEffectOn()
    {
        parentScript.AxeAttack_IceEffectOn();
    }

    public void AxeAttackFireEffectOn()
    {
        parentScript.AxeAttack_FireEffectOn();
    }

    public void ShieldTrailOn()
    {
        parentScript.ShieldTrailOn();
    }

    public void ShieldTrailOff()
    {
        parentScript.ShieldTrailOff();
    }

    public void ShieldOn()
    {
        parentScript.ShieldOn();
    }

    public void HandTrailOn()
    {
        parentScript.HandTrailOn();
    }

    public void HandTrailOff()
    {
        parentScript.HandTrailOff();
    }

    public void ScytheSpawn()
    {
        parentScript.ScytheSpawn();
    }
    public void CannonFire()
    {
        parentScript.CannonFire();
    }
}
