using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//public enum GripState { Fixed = 0, Opening = -1, Closing = 1 };
public class pincer : MonoBehaviour
{
    public Slider Slider;
    public float tolerance, sliderGripperGap;
    public GameObject lFinger;
    public GameObject rFinger;
    pincerFinger fingerAController, fingerBController;
    public float grip;
    public float gripSpeed = 3.0f;
    public GripState gripState = GripState.Fixed;

    //float gripper_gap_tolerance; // what's the gap when it's closed? Don't apply force after this is reached
    //public Transform A, B, C, D;
    //public Transform oppA, oppB, oppC, oppD; // opposite side of the gripper
    //public Transform edge_target;
    //public Transform center_target;
    //public float proportional_coefficient, rotational_coefficient;
    //public Toggle closeGripper;
    //ArticulationBody rb;
    //Vector3 edge_positionHolder, center_positionHolder; // local offset from target
    //Quaternion edge_rotationHolder, center_rotationHolder; // rotational offset from target
    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<ArticulationBody>();
        //rb.maxAngularVelocity = Mathf.Infinity;
        ////get position & rotation offset in target's coordinate system
        //edge_positionHolder = edge_target.worldToLocalMatrix.MultiplyPoint(transform.position);
        //edge_rotationHolder = Quaternion.Inverse(edge_target.rotation) * transform.rotation;

        //center_positionHolder = center_target.worldToLocalMatrix.MultiplyPoint(center_target.transform.position);
        //center_rotationHolder = Quaternion.Inverse(center_target.rotation) * transform.rotation;
        fingerAController = lFinger.GetComponent<pincerFinger>();
        fingerBController = rFinger.GetComponent<pincerFinger>();
    }

    // Update is called once per frame
    void Update()
    {
        sliderGripperGap = Slider.value - grip;
        if (Mathf.Abs(sliderGripperGap) > tolerance)
        {
            if (sliderGripperGap < grip)
            {
                gripState = GripState.Closing;
            }
            else
            {
                gripState = GripState.Opening;
            }
        }
        else
        {
            gripState = GripState.Fixed;
        }
        //Vector3 deltaPosition = target.position - transform.position;
        //Quaternion deltaRotation = target.rotation * Quaternion.Inverse(transform.rotation);
        //rb.AddForce(deltaPosition * proportional_coefficient);
    }
    private void FixedUpdate()
    {
        ////move towards center of gripper
        //if (closeGripper.isOn)
        //{
        //    Debug.Log("gripper closing");
        //    //matchPositionAndRotation(center_positionHolder, center_rotationHolder, center_target, true);
        //    rb.AddForceAtPosition((oppA.position - A.position) * proportional_coefficient, A.position);
        //    rb.AddForceAtPosition((oppB.position - B.position) * proportional_coefficient, B.position);
        //    rb.AddForceAtPosition((oppC.position - C.position) * proportional_coefficient, C.position);
        //    rb.AddForceAtPosition((oppD.position - D.position) * proportional_coefficient, D.position);

        //}
        //else
        ////move towards ends of gripper
        //{
        //    // matchPositionAndRotation(edge_positionHolder, edge_rotationHolder, edge_target, false);
        //    matchPositionAndRotation(edge_target.position, edge_rotationHolder, edge_target, false);
        //}
        UpdateGrip();
        UpdateFingersForGrip();

    }

    void UpdateFingersForGrip()
    {
        fingerAController.UpdateGrip(grip);
        fingerBController.UpdateGrip(grip);
    }

    void UpdateGrip()
    {
        if(gripState != GripState.Fixed)
        {
            float gripChange = (float)gripState * gripSpeed * Time.fixedDeltaTime;
            float gripGoal = CurrentGrip() + gripChange;
            grip = Mathf.Clamp01(gripGoal);
        }
    }

    public float CurrentGrip()
    {
        // TODO - we can't really assume the fingers agree, need to think about that
        float meanGrip = (fingerAController.CurrentGrip() + fingerBController.CurrentGrip()) / 2.0f;
        return meanGrip;
    }
    //void matchPositionAndRotation(Vector3 posHolder, Quaternion rotHolder, Transform target, bool force)
    //{
    //    Vector3 desiredPos = target.localToWorldMatrix.MultiplyPoint(posHolder);
    //    Vector3 currentPos = transform.position;

    //    Quaternion desiredRot = target.rotation * rotHolder;
    //    Quaternion currentRot = transform.rotation;


    //    Quaternion offsetRot = desiredRot * Quaternion.Inverse(currentRot);
    //    float angle; Vector3 axis;
    //    offsetRot.ToAngleAxis(out angle, out axis);
    //    Vector3 rotationDiff = angle * Mathf.Deg2Rad * axis;
    //    if (force)
    //    {
    //        rb.AddForce((desiredPos - currentPos) / Time.fixedDeltaTime);
    //        rb.AddTorque(rotationDiff / Time.fixedDeltaTime);

    //    }
    //    else
    //    {
    //        rb.velocity = (desiredPos - currentPos) / Time.fixedDeltaTime;
    //        rb.angularVelocity = rotationDiff / Time.fixedDeltaTime;
    //    }

    //}
}
