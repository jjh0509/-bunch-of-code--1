using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] Transform cameraPosition;
    [SerializeField] PlayerController_CharacterController playerScript;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController_CharacterController>();
        cameraPosition = GameObject.Find("Player").transform.Find("CameraPosition").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.isRiding) 
        {
            transform.parent = playerScript.SelectedTower.GetComponent<TowerScript>().DirectControlCamPoint;
            transform.localPosition = Vector3.Slerp(transform.localPosition , new Vector3(0, 0, 0), Time.deltaTime * 10);
        }

        else
        {
            transform.parent = null;
            transform.position = Vector3.Slerp(transform.position ,cameraPosition.position, Time.deltaTime * 10);
        }
    }
}
