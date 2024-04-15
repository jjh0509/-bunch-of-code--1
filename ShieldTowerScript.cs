using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTowerScript : MonoBehaviour
{
    public TowerScript mainController;

    public GameObject ForceField;
    public Transform Spin;
    public Transform Spin2;
    public float SpinSpeed;
    public ParticleSystem[] CenterParticle;
    // Start is called before the first frame update
    void Start()
    {
        foreach(ParticleSystem p in CenterParticle)
        {
            p.Play();
        }

        mainController = GetComponent<TowerScript>();
        ForceField = transform.Find("ForceField").gameObject;
        Spin = transform.Find("Spin").GetComponent<Transform>();
        Spin2 = transform.Find("Spin2").GetComponent<Transform>();

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
        {
            if (CenterParticle[0].isPlaying)
            {
                ForceField.SetActive(false);
                foreach (ParticleSystem p in CenterParticle)
                {
                    p.Stop();
                }
            }
            return;
        }
        else
        {
            if (!CenterParticle[0].isPlaying)
            {
                ForceField.SetActive(true);
                foreach (ParticleSystem p in CenterParticle)
                {
                    p.Play();
                }
            }
        }

        Spin.Rotate(0, 0, SpinSpeed);
        Spin2.Rotate(0, 0, SpinSpeed * 0.5f);
    }
}
