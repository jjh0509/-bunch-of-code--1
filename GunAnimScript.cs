using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimScript : MonoBehaviour
{
    public void GrenadeLauncherReloadNeonEffect()
    {
        transform.Find("Cylinder001").GetComponent<GrenadeLauncherMagazineScript>().Reload();
    }

    public void GrenadeLauncherReloadNeonEffectOff()
    {
        transform.Find("Cylinder001").GetComponent<GrenadeLauncherMagazineScript>().NeonOff();
    }
}
