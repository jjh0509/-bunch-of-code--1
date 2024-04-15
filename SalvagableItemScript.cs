using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalvagableItemScript : MonoBehaviour
{
    public int ScrapGives;
    public int AdvancedResourceGives;
    public float EnergyGives;

    public float DismissTime;

    private void Start()
    {
        Destroy(gameObject, DismissTime);
    }
}
