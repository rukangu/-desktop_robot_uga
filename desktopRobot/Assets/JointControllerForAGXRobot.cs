using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AGXUnity;
using RuntimeGizmos;
using System.Threading.Tasks;
using TMPro;
//using MathNet.Numerics.LinearAlgebra;
public class JointControllerForAGXRobot : MonoBehaviour
{
    public bool IK;
    // public TMP_Text debugText;
    public PracticeSceneUI UI; //for some shared control variables
    //public Transform gizmoHighlightTarget;
    //public TransformGizmo gizmo;
    //public Slider baseSlider, shoulder, elbow, wrist1, wrist2, wrist3;
    public Constraint baseConstraint, shoulderConstraint, elbowConstraint, wrist1Constraint, wrist2Constraint, wrist3Constraint, handConstraint, finger1Constraint, finger2Constraint, targetConstraint;
    Slider[] jointArray;
    public Constraint[] constraintArray;
    public Constraint[] fingerConstraints;
    // public float[] deltas = new float[6];
    //public Toggle closeGripper, useIKbutton;

    public Transform TCPFollowTarget;
    public bool orderedToStop = false;
    [Tooltip("Tolerance for moving the tcp target during trajectory execution")]
    public float tolerance, rotationTolerance;
    public float RobotSpeed;
    public float gripperSpeed;
    [Tooltip("Delay after the end of each robot move")]
    public int millisecondsDelay;
    [Tooltip("max rotation delta in degrees")]
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //jointArray = new Slider[6] { baseSlider, shoulder, elbow, wrist1, wrist2, wrist3 };
        constraintArray = new Constraint[7] { baseConstraint, shoulderConstraint, elbowConstraint, wrist1Constraint, wrist2Constraint, wrist3Constraint, handConstraint }; // The arrangment matters. This is the order in which we store them in the Trajectory. It's the bottom up link arrangement anyway
        fingerConstraints = new Constraint[2] { finger1Constraint, finger2Constraint };

        // match the slider values to the current joint positions

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //close
            //foreach (Constraint c in fingerConstraints)
            //{

            //}
            Grip(true);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //open
            Grip(false);
        }
    }

    //called by UI
    public void SliderValueChanged()
    {
        //Debug.Log("sliders changed");
        // find the deltas

        for (int i = 0; i < jointArray.Length; i++)
        {
            constraintArray[i].GetController<LockController>().Position = jointArray[i].value;
        }
    }
    //called by UI
    //public void Grip(bool open)
    //{
    //    int input;

    //    if (open)
    //    {
    //        input = 1;
    //    }
    //    else
    //    {
    //        input = -1;

    //    }
    //    foreach (Constraint c in fingerConstraints)
    //    {
    //        var lockController = c.GetController<LockController>();
    //        lockController.Enable = true;
    //        var range = c.GetController<RangeController>().Range;
    //        var curr_pos = c.GetCurrentAngle();
    //        lockController.Position = Mathf.Clamp(curr_pos + input * Time.fixedDeltaTime * 0.07f, range.Min, range.Max);
    //    }
    //}
    public void Grip(bool open)
    {
        //int input;
        float targetPosition;
        if (!open)
        {
            targetPosition = 0;

        }
        else
        {
            targetPosition = .02f;
        }
        float minR = 0;
        float maxR = .02f;
        //try to close them together
        var c1 = fingerConstraints[0].GetController<LockController>();
        var c2 = fingerConstraints[1].GetController<LockController>();
        var p1 = fingerConstraints[0].GetCurrentAngle();
        var p2 = fingerConstraints[1].GetCurrentAngle();

        var d1 = p1 - targetPosition;
        var d2 = p2 - targetPosition;

        if (Mathf.Abs(d1) > Mathf.Abs(d2))
        {
            float t1 = Mathf.Clamp(p1 - d1 * Time.deltaTime * gripperSpeed, minR, maxR);
            c1.Position = t1;
        }
        else
        {
            float t2 = Mathf.Clamp(p2 - d2 * Time.deltaTime * gripperSpeed, minR, maxR);

            c2.Position = t2;
        }
    }
    public void SetFingerPositions(float finger1pos,float finger2Pos)
    {
        finger1Constraint.GetController<LockController>().Position = finger1pos;
        finger2Constraint.GetController<LockController>().Position = finger2Pos;
    }
    //public void SwitchedControlMode()
    //{
    //    bool useIK = useIKbutton.isOn;
    //    if (useIK)
    //    {
    //        Debug.Log("using IK");
    //        gizmo.HighlightTarget(gizmoHighlightTarget);
    //    }
    //    else
    //    {
    //        gizmo.ClearTargets();
    //        Debug.Log("NOT using IK");
    //    }
    //}
    public async Task StopTrajectoryExecution()
    {
        orderedToStop = true;
        await Task.Delay(millisecondsDelay);
    }
    public async Task ExecuteTrajectory(Trajectory T)
    {
        //Debug.Log("executing trajectory");
        //first of all, make sure that we stop all previous motion
        await StopTrajectoryExecution();
        int i = 0;
        orderedToStop = false;
        foreach (TrajectoryAction action in T.actions)
        {
            //Debug.Log(i.ToString());
            if (orderedToStop)
            {
                break;
            }
            Transform target = T.points[i];
            float dT = T.timeDeltas[i];

            if (action == TrajectoryAction.MoveTCP)
            {
                if (UI.IK)
                {
                    await RunRobotMove(target, RobotSpeed);
                }
                else
                {
                    await RobotJointMove(T.pose[i], RobotSpeed);
                }

            }
            else
            {
                await RunRobotAction(action, UI.GripTime);
            }
            i++;
        }
        await Task.Yield();
    }
    //public void StopTrajectoryExecution()
    //{
    //    orderedToStop = true;
    //}
    public async Task RunRobotAction(TrajectoryAction action, float time)
    {
        float endTime = Time.time + time;
        while (Time.time < endTime)
        {
            if (orderedToStop)
            {
                break;
            }
            if (action == TrajectoryAction.GripperOpen)
            {
                //Debug.Log("opening gripper ");
                Grip(false);
            }
            else if (action == TrajectoryAction.GripperClose)
            {
                //Debug.Log("closing gripper ");
                Grip(true);
            }

            await Task.Yield();
        }
        //Debug.Log("Delaying " + (millisecondsDelay / 1000).ToString() + " seconds");
        await Task.Delay(millisecondsDelay);
        //Debug.Log("done gripper action");

    }
    //public async Task RunRobotMove(Transform point, float speed)
    //{

    //    float t;
    //    Quaternion initRotation = TCPFollowTarget.rotation;
    //    float initAngleDiff = Quaternion.Angle(point.rotation, TCPFollowTarget.rotation);
    //    float distance = (point.position - TCPFollowTarget.position).magnitude;
    //    Debug.Log("moving robot");
    //    Vector3 gap = point.position - TCPFollowTarget.position;
    //    while ((point.position - TCPFollowTarget.position).magnitude > tolerance || 
    //        Quaternion.Angle(TCPFollowTarget.rotation,point.rotation) > rotationTolerance)
    //    {
    //        //debugText.text = (point.position - TCPFollowTarget.position).magnitude.ToString();
    //        //debugText.text = Quaternion.Angle(TCPFollowTarget.rotation, point.rotation).ToString();
    //        //gap = point.position - TCPFollowTarget.position;
    //        if (orderedToStop)
    //        {
    //            Debug.Log("Task forced to quit");
    //            break;
    //        }

    //        TCPFollowTarget.position = Vector3.Lerp(TCPFollowTarget.position, point.position, Time.deltaTime * speed);
    //        //TCPFollowTarget.rotation = Quaternion.Slerp(TCPFollowTarget.rotation, point.rotation, rotationSpeed * Time.deltaTime);

    //        //constant velocity
    //        //want t to vary from 0 - 1 over the course of the rotation

    //        float currentAngleDiff = Quaternion.Angle(point.rotation, TCPFollowTarget.rotation);
    //        t = (initAngleDiff - currentAngleDiff) / initAngleDiff;
    //        Debug.Log("t: " + t.ToString());
    //        //TCPFollowTarget.rotation = Quaternion.Slerp(TCPFollowTarget.rotation, point.rotation, t);
    //        TCPFollowTarget.rotation = Quaternion.RotateTowards(TCPFollowTarget.rotation, point.rotation, rotationSpeed );

    //        await Task.Yield();
    //    }
    //    //TCPFollowTarget.position = point.position;
    //    //TCPFollowTarget.rotation = point.rotation;
    //    Debug.Log("Delaying " + (millisecondsDelay / 1000).ToString() + " seconds");
    //    await Task.Delay(millisecondsDelay);
    //    Debug.Log("done");
    //}
    public async Task RunRobotMove(Transform point, float speed)
    {


        float t;
        Quaternion initRotation = TCPFollowTarget.rotation;
        float initAngleDiff = Quaternion.Angle(point.rotation, TCPFollowTarget.rotation);
        float distance = (point.position - TCPFollowTarget.position).magnitude;
        //Debug.Log("moving robot");
        Vector3 gap = point.position - TCPFollowTarget.position;

        while ((point.position - TCPFollowTarget.position).magnitude > tolerance ||
        Quaternion.Angle(TCPFollowTarget.rotation, point.rotation) > rotationTolerance)
        {
            //debugText.text = (point.position - TCPFollowTarget.position).magnitude.ToString();
            //debugText.text = Quaternion.Angle(TCPFollowTarget.rotation, point.rotation).ToString();
            //gap = point.position - TCPFollowTarget.position;
            if (orderedToStop)
            {
                Debug.Log("Task forced to quit");
                break;
            }

            TCPFollowTarget.position = Vector3.Lerp(TCPFollowTarget.position, point.position, Time.deltaTime * speed);
            //TCPFollowTarget.rotation = Quaternion.Slerp(TCPFollowTarget.rotation, point.rotation, rotationSpeed * Time.deltaTime);

            //constant velocity
            //want t to vary from 0 - 1 over the course of the rotation

            float currentAngleDiff = Quaternion.Angle(point.rotation, TCPFollowTarget.rotation);
            t = (initAngleDiff - currentAngleDiff) / initAngleDiff;
            //Debug.Log("t: " + t.ToString());
            //TCPFollowTarget.rotation = Quaternion.Slerp(TCPFollowTarget.rotation, point.rotation, t);
            TCPFollowTarget.rotation = Quaternion.RotateTowards(TCPFollowTarget.rotation, point.rotation, rotationSpeed);

            await Task.Yield();
        }
        //TCPFollowTarget.position = point.position;
        //TCPFollowTarget.rotation = point.rotation;
        //Debug.Log("Delaying " + (millisecondsDelay / 1000).ToString() + " seconds");
        await Task.Delay(millisecondsDelay);
        //Debug.Log("done");
    }
    public async Task RobotJointMove(float[] angles, float speed)
    {    // do it the angle way

        // if any of the angles hasn't achieved the target within tolerance
        float b, s, e, w1, w2, w3, h;
       // float curr_b, curr_s, curr_e, curr_w1, curr_w2, curr_w3, curr_h;

        b = 0; s = 0; e = 0; w1 = 0; w2 = 0; w3 = 0; h = 0;
        //curr_b = 0; curr_s = 0; curr_e = 0; curr_w1 = 0; curr_w2 = 0; curr_w3 = 0; curr_h = 0;
        helperForJointPositions(ref b, ref s, ref e, ref w1, ref w2, ref w3, ref h);
        while (angles[0] - b > tolerance || angles[1] - s > tolerance || angles[2] - e > tolerance || angles[3] -  w1 > tolerance || angles[4] - w2 > tolerance || angles[5] - w3 > tolerance || angles[6] - h > tolerance)
        {
            baseConstraint.GetController<LockController>().Position = Mathf.Lerp(b, angles[0], Time.deltaTime * speed);
            shoulderConstraint.GetController<LockController>().Position = Mathf.Lerp(s, angles[1], Time.deltaTime * speed);
            elbowConstraint.GetController<LockController>().Position = Mathf.Lerp(e, angles[2], Time.deltaTime * speed);
            wrist1Constraint.GetController<LockController>().Position = Mathf.Lerp(w1, angles[3], Time.deltaTime * speed);
            wrist2Constraint.GetController<LockController>().Position = Mathf.Lerp(w2, angles[4], Time.deltaTime * speed);
            wrist3Constraint.GetController<LockController>().Position = Mathf.Lerp(w3, angles[5], Time.deltaTime * speed);
            handConstraint.GetController<LockController>().Position = Mathf.Lerp(h, angles[6], Time.deltaTime * speed);
            await Task.Yield();
        }
    }

    // helper function to get the current joint angles
    void helperForJointPositions(ref float b, ref float s, ref float e, ref float w1, ref float w2, ref float w3, ref float h)
    {
        b = baseConstraint.GetCurrentAngle();
        s = shoulderConstraint.GetCurrentAngle();
        e = elbowConstraint.GetCurrentAngle();
        w1 = wrist1Constraint.GetCurrentAngle();
        w2 = wrist2Constraint.GetCurrentAngle();
        w3 = wrist3Constraint.GetCurrentAngle();
        h = handConstraint.GetCurrentAngle();
    }
    private void OnDestroy()
    {
        orderedToStop = true;
    }
    private void OnApplicationQuit()
    {
        orderedToStop = true;
    }
}
