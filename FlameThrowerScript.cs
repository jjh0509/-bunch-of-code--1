using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerScript : MonoBehaviour
{
    public TowerScript mainController;
    public ParticleCollisionScript dmgParticle;
    public ParticleSystem[] FlameParticles;

    // Start is called before the first frame update
    void Start()
    {
        mainController = GetComponent<TowerScript>();

        mainController.Target = transform.Find("NoTargetFound").GetComponent<Transform>();

        mainController.Sentry = transform.Find("Sentry").GetComponent<Transform>();
        mainController.Muzzle = mainController.Sentry.Find("Muzzle").GetComponent<Transform>();
        mainController.TargetDir_Object = transform.Find("TargetDirs").GetComponent<Transform>();
        mainController.TargetDir_Muzzle = mainController.Sentry.Find("TargetDirs_Muzzle").GetComponent<Transform>();
        dmgParticle = GetComponentInChildren<ParticleCollisionScript>();

        foreach (ParticleSystem p in FlameParticles)
        {
            p.loop = true;
            p.Stop();
        }

        mainController.UpgradeEffectParent = transform.Find("UpgradeEffect").gameObject;
        mainController.PillarsParent = mainController.UpgradeEffectParent.transform.Find("Pillars").gameObject;
        mainController.Roofs = mainController.PillarsParent.transform.Find("Roofs").gameObject;
        mainController.Floors = mainController.UpgradeEffectParent.transform.Find("Floors").gameObject;
        mainController.DoneEffect = mainController.UpgradeEffectParent.transform.Find("Done").gameObject;
        mainController.RangeCylinder = transform.Find("RangeCylinder").gameObject;

        mainController.HP = mainController.towerData.HP[mainController.Level - 1];
        mainController.lastattackedtime = mainController.towerData.fireRate[mainController.Level - 1];

        mainController.findnewtargetdelay = 0;

        mainController.PillarsParent.transform.localScale = new Vector3(1, 0, 1);
        mainController.Floors.transform.localScale = new Vector3(0, 1, 0);

        mainController.DoneEffect.transform.localPosition = new Vector3(0, -4, 0);
        mainController.DoneEffect.SetActive(false);
        mainController.UpgradeEffectParent.SetActive(false);

        mainController.ChildrenColliders = GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainController.isDowned)
            return;

        dmgParticle.isPlayerPrj = mainController.isRiding;
        if (mainController.isRiding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                foreach (ParticleSystem p in FlameParticles)
                {
                    p.Play();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                
                foreach (ParticleSystem p in FlameParticles)
                {
                    p.Stop();
                }
            }

            mainController.Sentry.transform.rotation = Quaternion.Slerp(mainController.Sentry.rotation, mainController.playerScript.orientation.rotation, mainController.TurnSpeed * 3);
            mainController.Muzzle.transform.localRotation = Quaternion.Slerp(mainController.Muzzle.localRotation, mainController.playerScript.mouseRotInputs, mainController.TurnSpeed * 3);

            if (Input.GetKeyDown(mainController.Gdata.Ride) && mainController.playerScript.behaviourCooldown > 1)
            {
                mainController.isRiding = false;
                mainController.playerScript.UnRide();
                mainController.playerScript.behaviourCooldown = 0;
                mainController.RangeCylinder.SetActive(false);
                if (!(mainController.MinRangeCylinder == null))
                    mainController.MinRangeCylinder.SetActive(false);
            }

            return;
        }

        if (mainController.Target.name == "NoTargetFound")
        {
            foreach (ParticleSystem p in FlameParticles)
            {
                p.Stop();
            }

            mainController.findnewtargetdelay += Time.deltaTime;
            if (mainController.findnewtargetdelay > 0.5f)
            {
                mainController.findnewtargetdelay = 0;
                mainController.Target = mainController.FindClosestEnemy().GetComponent<Transform>();
                return;
            }
        }

        if (mainController.Target == null || mainController.Target.name == "NoTargetFound")
            return;

        if (mainController.Target.GetComponent<EnemyController>().isDied)
        {
            mainController.Target = transform.Find("NoTargetFound").GetComponent<Transform>();
            return;
        }
        if (!FlameParticles[0].isPlaying)
        {
            foreach (ParticleSystem p in FlameParticles)
            {
                p.Play();
            }
        }
        Vector3 targetPoint = new Vector3(mainController.Target.position.x, mainController.TargetDir_Object.position.y, mainController.Target.position.z);

        mainController.TargetDir_Object.LookAt(targetPoint);

        mainController.Sentry.transform.rotation = Quaternion.Slerp(mainController.Sentry.rotation, mainController.TargetDir_Object.rotation, mainController.TurnSpeed);

        Vector3 targetHead = new Vector3(mainController.Target.position.x, mainController.Target.GetComponent<EnemyController>().TowerAttackPoint.position.y, mainController.Target.position.z);
        mainController.TargetDir_Muzzle.LookAt(targetHead);
        mainController.Muzzle.transform.rotation = Quaternion.Slerp(mainController.Muzzle.rotation, mainController.TargetDir_Muzzle.rotation, mainController.TurnSpeed);
    }
}
