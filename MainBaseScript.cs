using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBaseScript : MonoBehaviour
{
    public Transform[] StartPoints;
    public GameObject StartPointsParent;
    [Header("Stats")]
    public float HP;
    public float MaxHP;

    public List<float> Float_GeneratePerSecondLevel = new List<float>();
    public List<int> Cost_GeneratePerSecond = new List<int>();
    public List<int> Cost_Repair = new List<int>();
    public int CurrentCost_Repair;

    public int NumOfUpgradedGen;
    public int NumOfRepair;

    public int AdReSynthCost_Scraps;
    public float AdReSynthCost_Energies;
    public float AdReSynthCost_Time;

    public List<int> Cost_AdRe_UpgradeWeaponEnergy = new List<int>();
    public List<int> Cost_AdRe_UpgradeShield = new List<int>();
    public List<float> Float_WeaponEnergyPerLevel = new List<float>();
    public List<float> Float_MaxShieldPerLevel = new List<float>();

    public int NumOfUpgradedWeaponEnergy;
    public int NumOfUpgradedShield;

    public float EnergyIncomePerSecond;
    private float deltatimer;
    PlayerController_CharacterController playerScript;
    GameManagement Gmanager;

    public bool Ended;
    // Start is called before the first frame update
    void Start()
    {
        StartPointsParent = GameObject.Find("StartPositions").gameObject;
        StartPoints = StartPointsParent.GetComponentsInChildren<Transform>();

        transform.position = StartPoints[Random.Range(0, StartPoints.Length)].position;

        playerScript = GameObject.Find("Player").GetComponent<PlayerController_CharacterController>();
        Gmanager = GameObject.Find("DataManager").GetComponent<GameManagement>();
        Gmanager.FindPlayerBaseTransform();
        deltatimer = 0;

        playerScript.transform.position = transform.position + transform.forward * 12;

        NumOfUpgradedGen = 0;
        NumOfRepair = 0;
        NumOfUpgradedWeaponEnergy = 0;
        NumOfUpgradedShield = 0;
        EnergyIncomePerSecond = Float_GeneratePerSecondLevel[NumOfUpgradedGen];
        CurrentCost_Repair = Cost_Repair[NumOfRepair];

        Ended = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Ended)
            return;

        deltatimer += Time.deltaTime;

        if(deltatimer > 1)
        {
            deltatimer = 0;
            playerScript.BaseEnergies += EnergyIncomePerSecond;
            playerScript.BaseEnergies = Mathf.Clamp(playerScript.BaseEnergies, 0, 10000);
        }

        if (HP <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        Debug.Log("GAME OVER");
        Ended = true;
    }

    public void TakeDamage(float amount)
    {
        HP -= amount;
    }

    public void UpgradeGen()
    {
        if(NumOfUpgradedGen >= Cost_GeneratePerSecond.Count)
        {
            Debug.Log("Already At Max Level of That :/");
            return;
        }
        if(playerScript.AdvancedResources >= Cost_GeneratePerSecond[NumOfUpgradedGen])
        {
            playerScript.AdvancedResources -= Cost_GeneratePerSecond[NumOfUpgradedGen];
            EnergyIncomePerSecond = Float_GeneratePerSecondLevel[NumOfUpgradedGen];
            NumOfUpgradedGen++;
        }
        else
        {
            Debug.Log("Not Enough Resources");
        }
    }

    public void UpgradePlayerWeaponEnergy()
    {
        if (NumOfUpgradedWeaponEnergy >= Cost_AdRe_UpgradeWeaponEnergy.Count)
        {
            Debug.Log("Already At Max Level of That :/");
            return;
        }
        if (playerScript.AdvancedResources >= Cost_AdRe_UpgradeWeaponEnergy[NumOfUpgradedWeaponEnergy])
        {
            playerScript.AdvancedResources -= Cost_AdRe_UpgradeWeaponEnergy[NumOfUpgradedWeaponEnergy];
            playerScript.MaxEnergy = Float_WeaponEnergyPerLevel[NumOfUpgradedWeaponEnergy];
            playerScript.Energy = playerScript.MaxEnergy;
            NumOfUpgradedWeaponEnergy++;
        }
        else
        {
            Debug.Log("Not Enough Resources");
        }
    }

    public void UpgradePlayerShield()
    {
        if (NumOfUpgradedShield >= Cost_AdRe_UpgradeShield.Count)
        {
            Debug.Log("Already At Max Level of That :/");
            return;
        }
        if (playerScript.AdvancedResources >= Cost_AdRe_UpgradeShield[NumOfUpgradedShield])
        {
            playerScript.AdvancedResources -= Cost_AdRe_UpgradeShield[NumOfUpgradedShield];
            playerScript.MaxShield = Float_MaxShieldPerLevel[NumOfUpgradedShield];
            playerScript.Shield = playerScript.MaxShield;
            NumOfUpgradedShield++;
        }
        else
        {
            Debug.Log("Not Enough Resources");
        }
    }

    public void RechargePlayerWeaponEnergy()
    {
        if(playerScript.BaseEnergies >= Mathf.Abs(playerScript.Energy - playerScript.MaxEnergy))
        {
            playerScript.BaseEnergies -= Mathf.Abs(playerScript.Energy - playerScript.MaxEnergy);
            playerScript.Energy = playerScript.MaxEnergy;
        }
        else
        {
            Debug.Log("Not Enough Resources");
        }
    }

    public void RechargePlayerShield()
    {
        if (playerScript.BaseEnergies >= Mathf.Abs(playerScript.Shield - playerScript.MaxShield))
        {
            playerScript.BaseEnergies -= Mathf.Abs(playerScript.Shield - playerScript.MaxShield);
            playerScript.Shield = playerScript.MaxShield;
        }
        else
        {
            Debug.Log("Not Enough Resources");
        }
    }

    public void Repair()
    {
        if (playerScript.AdvancedResources >= Cost_Repair[NumOfRepair])
        {
            playerScript.AdvancedResources -= Cost_Repair[NumOfRepair];
            NumOfRepair++;
            HP = MaxHP;
        }
        else
        {
            Debug.Log("Not Enough Resources");
        }

        if (NumOfRepair >= Cost_Repair.Count)
            NumOfRepair = Cost_Repair.Count - 1;
    }
}
