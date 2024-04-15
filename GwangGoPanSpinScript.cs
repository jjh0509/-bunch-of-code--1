using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GwangGoPanSpinScript : MonoBehaviour
{
    public Vector3 SpinRotPerSec;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(SpinRotPerSec * Time.deltaTime);
    }
}
