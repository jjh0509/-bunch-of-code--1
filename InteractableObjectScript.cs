using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public class InteractableObjectScript : MonoBehaviour
{
    [Header("Outlines")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float outlineScaleFactor;
    [SerializeField] private Color outlineColor;
    private Renderer outlineRenderer;

    [Header("Time Setting")]
    public float InteractTime;

    [Header("Resources Setting")]
    public int ProvidingScraps;
    public int ProvidingAdvancedResources;
    public float ProvidingEnergies;
    public float ProvidingWeaponEnergies;
    public List<GameObject> DroppingObjects = new List<GameObject>();

    [Header("External Components")]
    public PlayerController_CharacterController playerScript;

    [Header("Others")]
    public string ObjectName;
    public float ExplosionForce;
    public bool AlreadyInteracted;
    public bool Selected;
    public bool HasAnimation;
    public Animator anim;

    public Vector3 originScale;

    void Start()
    {
        originScale = transform.localScale;
        transform.localScale = new Vector3(1, 1, 1);

        AlreadyInteracted = false;
        outlineRenderer = CreateOutline(outlineMaterial, outlineScaleFactor, outlineColor);

        playerScript = GameObject.Find("Player").GetComponent<PlayerController_CharacterController>();

        if (HasAnimation)
        {
            anim = GetComponent<Animator>();
        }

        transform.localScale = originScale;
    }

    Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
    {
        GameObject outlineObject = Instantiate(this.gameObject, transform.position, transform.rotation, transform);
        Renderer rend = outlineObject.GetComponent<Renderer>();

        rend.material = outlineMat;
        rend.material.SetColor("_OutlineColor", color);
        rend.material.SetFloat("_Scale", scaleFactor);
        rend.shadowCastingMode = ShadowCastingMode.Off;

        outlineObject.GetComponent<InteractableObjectScript>().enabled = false;
        outlineObject.GetComponent<Collider>().enabled = false;


        rend.enabled = false;

        return rend;
    }

    // Update is called once per frame
    void Update()
    {
        //outlineRenderer.enabled = Selected;
    }

    public void UnSelectObject()
    {
        Selected = false;
        outlineRenderer.enabled = false;
        //Destroy(outlineRenderer);
    }

    public void SelectObject()
    {
        Selected = true;
        outlineRenderer.enabled = true;
        //outlineRenderer = CreateOutline(outlineMaterial, outlineScaleFactor, outlineColor);
    }

    public void Interact()
    {
        if (AlreadyInteracted)
            return;

        AlreadyInteracted = true;

        playerScript.Scraps += ProvidingScraps;
        playerScript.AdvancedResources += ProvidingAdvancedResources;
        playerScript.BaseEnergies += ProvidingEnergies;
        playerScript.Energy += ProvidingWeaponEnergies;

        if(DroppingObjects.Count > 0)
        {
            List<Rigidbody> objrb = new List<Rigidbody>();
            foreach(GameObject obj in DroppingObjects)
            {
                GameObject drops = Instantiate(obj, transform.position, Quaternion.identity);
                drops.transform.Rotate(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
                objrb.Add(drops.GetComponent<Rigidbody>());
            }

            foreach(Rigidbody rb in objrb)
            {
                rb.AddExplosionForce(ExplosionForce, transform.position, 4);
            }
        }

        if (HasAnimation)
        {
            anim.SetTrigger("Triggered");
        }
    }
}
