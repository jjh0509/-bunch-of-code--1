using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaSlerpMoveScript : MonoBehaviour
{
    public TowerScript mainController;

    [Header("Moving Stats")]
    public float MoveDelay;
    public float Speed;
    public Vector3 movePoint1;
    public Vector3 movePoint2;
    public float CurrentSpeed;
    public float CurrentDelayTime;
    public bool DirUp;
    // Start is called before the first frame update
    void Start()
    {
        CurrentSpeed = Speed;
        CurrentDelayTime = 0;
        mainController = GetComponentInParent<TowerScript>();
    }

    private void Update()
    {
        CurrentDelayTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if(CurrentDelayTime > MoveDelay)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, Speed, 0.01f);
        }
        else
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, 0.01f);
            return;
        }

        if (DirUp)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, movePoint2, CurrentSpeed);

            if(Vector3.Distance(transform.localPosition, movePoint2) < 0.1f)
            {
                DirUp = !DirUp;
                CurrentDelayTime = 0;
            }
        }
        else
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, movePoint1, CurrentSpeed);

            if (Vector3.Distance(transform.localPosition, movePoint1) < 0.1f)
            {
                DirUp = !DirUp;
                CurrentDelayTime = 0;
            }
        }
    }
}
