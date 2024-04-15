using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnSceneScript : MonoBehaviour
{
    public Transform PlayerBase;

    public ParticleSystem[] portalParticles;
    public GameObject Blinder;
    public float BlinderOnTime;
    public float DelayToSpawnShip;
    [SerializeField] private float timer;
    [SerializeField] private bool BlinderActived;

    [Header("Ship")]
    public GameObject Ship;
    public Vector3 ShipSpawnPoint;
    public Vector3 ShipEndPoint;
    public Vector3 ShipRotation;

    // Start is called before the first frame update
    void Start()
    {
        PlayerBase = GameObject.Find("MainBase").GetComponent<Transform>();
        transform.LookAt(PlayerBase);

        timer = 0;

        Ship.transform.localPosition = ShipSpawnPoint;
        ShipRotation = transform.rotation.eulerAngles;
        Ship.SetActive(false);

        BlinderActived = false;
        Blinder.SetActive(false);

        foreach(ParticleSystem p in portalParticles)
        {
            p.loop = true;
            p.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(BlinderOnTime < timer && !BlinderActived)
        {
            Blinder.SetActive(true);
            MoveShip();
            BlinderActived = true;
        }
    }

    public void MoveShip()
    {
        StartCoroutine(MoveShipCoroutine());
    }

    IEnumerator MoveShipCoroutine()
    {
        ShipRotation.x = 0;
        ShipRotation.z = 0;
        yield return new WaitForSeconds(DelayToSpawnShip);

        Ship.SetActive(true);

        for (int i = 0; i < 400; i++)
        {
            yield return new WaitForFixedUpdate();
            Ship.transform.localPosition = Vector3.Lerp(Ship.transform.localPosition, ShipEndPoint, 0.01f);

            Ship.transform.rotation = Quaternion.Lerp(Ship.transform.rotation, Quaternion.Euler(ShipRotation), 0.01f);

            if (i == 340)
            {
                foreach (ParticleSystem p in portalParticles)
                {
                    p.Stop();
                }
                Blinder.SetActive(false);
            }
        }
        /*
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForFixedUpdate();
            Ship.transform.localPosition = Vector3.Slerp(Ship.transform.localPosition, ShipEndPoint, 0.05f);
            ShipRotation.x = Mathf.Lerp(ShipRotation.x, 0 , 0.1f);
            ShipRotation.z = Mathf.Lerp(ShipRotation.z, 0, 0.1f);
            Ship.transform.localRotation = Quaternion.Euler(ShipRotation);
        }
        */
        Ship.transform.localPosition = ShipEndPoint;
        Ship.transform.SetParent(null);

        ShipRotation = Ship.transform.rotation.eulerAngles;
        ShipRotation.x = 0;
        ShipRotation.z = 0;
        Ship.transform.rotation = Quaternion.Euler(ShipRotation);

        yield return new WaitForSeconds(2);

        Ship.GetComponent<BossShipScript>().SpawnPhase();
        Destroy(gameObject,2);
    }
}
