using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherMagazineScript : MonoBehaviour
{
    public List<Material> lasermaterials = new List<Material>();
    public GunScript parentGunScript;
    public float rotateDeg;
    // Start is called before the first frame update
    void Start()
    {
        rotateDeg = 90;
        parentGunScript = GameObject.Find("CameraHolder").transform.Find("Parent_GunHolder").transform.Find("GunHolder").transform.Find("Gun").GetComponent<GunScript>();

        for (int i = 0; i < parentGunScript.gunData.magSize; i++)
        {
            lasermaterials[i].DisableKeyword("_EmissionColor");
        }

        
    }
    // Update is called once per frame
    void Update()
    {
        //if(!parentGunScript.gunData.reloading)
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, -90, rotateDeg), 0.2f);
    }

    public void ShotUpdateMaterial()
    {
        StopAllCoroutines();
        rotateDeg += 30;
        lasermaterials[parentGunScript.gunData.magSize - parentGunScript.gunData.currentAmmo].SetColor("_EmissionColor", new Color(0.395f, 4.93f, 4.15f) * Mathf.LinearToGammaSpace(0));
    }

    IEnumerator StopedReloadingAndDisableAnimator()
    {
        yield return new WaitForSeconds(0.2f);
        Animator anim = GetComponentInParent<Animator>();
        anim.gameObject.transform.Find("Box001").transform.localPosition = new Vector3(0, 0, 0);
        anim.gameObject.transform.Find("Box001").transform.localRotation = Quaternion.Euler(-90, 0, 0);
        anim.gameObject.transform.Find("Cylinder001").transform.localPosition = new Vector3(0, 0, 0);
        anim.gameObject.transform.Find("Cylinder001").transform.localRotation = Quaternion.Euler(0, -90, 90);
        rotateDeg = 90;

        anim.ResetTrigger("Reload");
        anim.enabled = false;

        for (int i = 0; i < parentGunScript.gunData.magSize; i++)
        {
            lasermaterials[i].SetColor("_EmissionColor", new Color(0.395f, 4.93f, 4.15f) * Mathf.LinearToGammaSpace(0));
        }
        StopAllCoroutines();
    }
    public void NeonOff()
    {
        for (int i = 0; i < parentGunScript.gunData.magSize; i++)
        {
            lasermaterials[i].SetColor("_EmissionColor", new Color(0.395f, 4.93f, 4.15f) * Mathf.LinearToGammaSpace(0));
        }
    }

    public void Reload()
    {
        StartCoroutine(ReNeon());
    }

    IEnumerator ReNeon()
    {
        NeonOff();

        for(int i = 0; i < parentGunScript.gunData.magSize; i++)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(TurnOnCurrentNeon(i));
        }
        yield return new WaitForSeconds(2f);
        rotateDeg = 90;
        transform.localRotation = Quaternion.Euler(0, -90, 90);
        StopAllCoroutines();
    }

    IEnumerator TurnOnCurrentNeon(int num)
    {
        float i = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            i+=0.1f;
            lasermaterials[num].SetColor("_EmissionColor", new Color(0.395f, 4.93f, 4.15f) * Mathf.LinearToGammaSpace(i));
            if (i >= 6)
                break;
        }

        for(int j = 0; j < 1; j++)
        {
            yield return new WaitForSeconds(0.01f);
            i-=0.2f;
            lasermaterials[num].SetColor("_EmissionColor", new Color(0.395f, 4.93f, 4.15f) * Mathf.LinearToGammaSpace(i));
        }
    }

    public void StopReload()
    {
        StopAllCoroutines();
        StartCoroutine(StopedReloadingAndDisableAnimator());
        NeonOff();

        rotateDeg = 90;
        transform.localRotation = Quaternion.Euler(0, -90, 90);
    }
}
