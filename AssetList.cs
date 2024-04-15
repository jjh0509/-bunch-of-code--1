using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetList : MonoBehaviour
{
    public List<GameObject> Tower_Prefabs = new List<GameObject>();
    public List<GameObject> Pre_Build_Guide_Prefabs = new List<GameObject>();

    public Material Bound_Material;
    public Material MainBase_Energy_Radius_Material;
    public Material GunGlowMaterial;
    public Material GunMagazineMaterial;

    public Material BuildGuidePrefab_Material;

    public GameObject DamageTextPrefab;
}
