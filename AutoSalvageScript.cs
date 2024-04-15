using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSalvageScript : MonoBehaviour
{
    public PlayerController_CharacterController playerScript;
    void Start()
    {
        playerScript = GetComponentInParent<PlayerController_CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Salvagable"))
        {
            SalvagableItemScript item_info = other.GetComponent<SalvagableItemScript>();
            TakeSalvage(item_info.ScrapGives, item_info.AdvancedResourceGives, item_info.EnergyGives);
            Destroy(other);
        }
    }

    public void TakeSalvage(int scraps, int adres, float energies)
    {
        playerScript.Scraps += scraps;
        playerScript.AdvancedResources += adres;
        playerScript.BaseEnergies += energies;
    }
}
