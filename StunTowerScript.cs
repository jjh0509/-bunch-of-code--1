using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTowerScript : MonoBehaviour
{
    public TowerScript mainController;
    public float SpinSpeed;

    [Header("AttackVars")]
    public bool Charging;
    public float LastAttackedTime;
    public float ChargeTime;
    public float DefaultSpinSpeed;
    public float ChargeSpinSpeed;
    public AudioClip ChargeSFX;
    public AudioClip ExplosionSFX;
    public ParticleSystem ChargeParticle;
    public GameObject ExplosionPrefab;

    public AudioSource sfx;

    [Header("Objects")]
    public Transform SpinningObject;

    // Start is called before the first frame update
    void Start()
    {
        sfx = GetComponent<AudioSource>();
        mainController = GetComponent<TowerScript>();

        LastAttackedTime = 0;
        SpinSpeed = DefaultSpinSpeed;

        ChargeParticle.Stop();

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

        LastAttackedTime += Time.deltaTime;

        if (LastAttackedTime > mainController.towerData.fireRate[mainController.Level])
        {
            LastAttackedTime = 0;
            StartCoroutine(StartPulse());
        }
    }

    private void FixedUpdate()
    {
        if (mainController.isDowned)
            return;

        if (Charging)
        {
            SpinSpeed = Mathf.Lerp(SpinSpeed, ChargeSpinSpeed, 0.1f);
        }
        else
        {
            SpinSpeed = Mathf.Lerp(SpinSpeed, DefaultSpinSpeed, 0.1f);
        }

        SpinningObject.Rotate(0, 0, SpinSpeed);
    }

    public IEnumerator StartPulse()
    {
        sfx.PlayOneShot(ChargeSFX);
        Charging = true;
        yield return new WaitForSeconds(ChargeTime);
        PulseBoom();
        StopAllCoroutines();
    }

    public void PulseBoom()
    {
        Charging = false;
        sfx.PlayOneShot(ExplosionSFX);
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
    }
}
