using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class ShieldTowerRangeOutlineScript : MonoBehaviour
{
    public TowerScript parentController;
    public ParticleSystem particle;
    public ParticleSystem.ShapeModule shape;

    // Start is called before the first frame update
    void Start()
    {
        parentController = GetComponentInParent<TowerScript>();
        particle = GetComponent<ParticleSystem>();
        shape = particle.GetComponent<ParticleSystem.ShapeModule>();
        shape.radius = parentController.towerData.Range[parentController.Level - 1] / 2;
    }

    // Update is called once per frame
    void Update()
    {
        particle.enableEmission = !parentController.isDowned;
    }
}
