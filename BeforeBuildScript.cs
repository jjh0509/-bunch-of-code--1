using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeBuildScript : MonoBehaviour
{
    public TowerData data;

    public GameObject UpgradeEffectParent;
    public GameObject PillarsParent;
    public GameObject Roofs;
    public GameObject Floors;
    public GameObject DoneEffect;
    public GameObject NextLevelTower;

    public GameObject Graphics;

    public GameObject BuildingTower;

    public float Buildtime;
    public bool TurnPillars;
    IEnumerator Start()
    {
        UpgradeEffectParent = transform.Find("UpgradeEffect").gameObject;
        PillarsParent = UpgradeEffectParent.transform.Find("Pillars").gameObject;
        Roofs = PillarsParent.transform.Find("Roofs").gameObject;
        Floors = UpgradeEffectParent.transform.Find("Floors").gameObject;
        DoneEffect = UpgradeEffectParent.transform.Find("Done").gameObject;

        Graphics = transform.Find("Graphics").gameObject;
        Graphics.SetActive(true);

        PillarsParent.transform.localScale = new Vector3(0, 0, 0);
        Floors.transform.localScale = new Vector3(0, 1, 0);
        DoneEffect.transform.localPosition = new Vector3(0, -4, 0);
        DoneEffect.SetActive(false);
        UpgradeEffectParent.SetActive(true);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            Floors.transform.localScale = Vector3.Slerp(Floors.transform.localScale, new Vector3(1, 1, 1), 0.05f);
        }
        yield return new WaitForSeconds(0.2f);

        PillarsParent.transform.localScale = new Vector3(1, 0, 1);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            PillarsParent.transform.localScale = Vector3.Slerp(PillarsParent.transform.localScale, new Vector3(1, 1, 1), 0.05f);
        }
        yield return new WaitForSeconds(0.2f);
        TurnPillars = true;

        yield return new WaitForSeconds(Buildtime);


        //UpgradeDoneNow
        DoneEffect.SetActive(true);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            DoneEffect.transform.localPosition = Vector3.Lerp(DoneEffect.transform.localPosition, new Vector3(0, 4, 0), 0.05f);
        }
        yield return new WaitForSeconds(0.2f);

        BuildDone();
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            PillarsParent.transform.localScale = Vector3.Slerp(PillarsParent.transform.localScale, new Vector3(1, 0, 1), 0.05f);
            DoneEffect.transform.localPosition = Vector3.Lerp(DoneEffect.transform.localPosition, new Vector3(0, -4, 0), 0.05f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            Floors.transform.localScale = Vector3.Slerp(Floors.transform.localScale, new Vector3(0, 1, 0), 0.05f);
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void BuildDone()
    {
        Graphics.SetActive(false);
        GameObject builtTower = Instantiate(BuildingTower, transform.position, transform.rotation);
        builtTower.GetComponent<TowerScript>().StartActiveDelay = 2;
    }
    private void Update()
    {
        if (TurnPillars)
        {
            PillarsParent.transform.Rotate(0, 10 * Time.deltaTime, 0);
        }
    }
}
