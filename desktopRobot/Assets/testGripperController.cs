using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testGripperController : MonoBehaviour
{
    public GameObject leftFinger, rightFinger;
    testFingerController fingerA, fingerB;
    public GripState gripState = GripState.Fixed;
    public Slider gripperSlider;
    float maxGap, currentGap, minGap,gripTolerance;
    // Start is called before the first frame update
    void Start()
    {
        maxGap = Vector3.Distance(leftFinger.transform.position, rightFinger.transform.position); // initially open
        fingerA = leftFinger.GetComponent<testFingerController>();
        fingerB = rightFinger.GetComponent<testFingerController>();
    }

    // Update is called once per frame
    void Update()
    {
        currentGap = Vector3.Distance(leftFinger.transform.position, rightFinger.transform.position);
        //if (gripState != GripState.Fixed)
        //{
        //     // move the gripper (open or close it)

        //}
        float deviation = currentGap / maxGap - gripperSlider.value;

        if (Mathf.Abs(deviation) > gripTolerance)
        {
            if(deviation < 0)
            {
                gripState = GripState.Opening;
            } else if(deviation > 0)
            {
                gripState = GripState.Closing;
            }
        }
        else
        {
            gripState = GripState.Fixed;
        }

        
        if(gripState == GripState.Closing)
        {
            //closing

        }
        else if(gripState == GripState.Opening)
        {
            //opening
        }




    }
}
