using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RobotJointSliders : MonoBehaviour
{
    public Slider baseSlider, shoulder, elbow, wrist1, wrist2, wrist3;
    public Toggle updatePositions;
    public Robot robot;
    public float initialSliderValue; //basically, make the slider relative rather than absolute, which was causing weird behavior at the start of joint updates
    public float[] prevSliderValues;// = new float[6];
    public float[] deltas = new float[6];
    ArticulationBody[] ABArray;
    Slider[] SliderArray;
    // Start is called before the first frame update
    void Start()
    {
        ABArray  = new ArticulationBody[] { robot.baseJoint, robot.shoulderJoint, robot.elbowJoint, robot.wrist1Joint, robot.wrist2Joint, robot.wrist3Joint};
        SliderArray = new Slider[6] { baseSlider, shoulder, elbow, wrist1, wrist2, wrist3};
        prevSliderValues = new float[6] { baseSlider.value, shoulder.value, elbow.value, wrist1.value, wrist2.value, wrist3.value};
    }

    // Update is called once per frame
    void Update()
    {
        //if (updatePositions.isOn)

        {
            //robot.baseJoint.SetJointPositions(new List<float> { baseSlider.value, shoulder.value, elbow.value, wrist1.value, wrist2.value, wrist3.value});
            //robot.baseJoint.jointPosition = new ArticulationReducedSpace(baseSlider.value - initialSliderValue);
            //setABPosition(robot.baseJoint, baseSlider.value);
            //setABPosition(robot.shoulderJoint, shoulder.value);
            //setABPosition(robot.elbowJoint, elbow.value);
            //setABPosition(robot.wrist1Joint, wrist1.value);
            //setABPosition(robot.wrist2Joint, wrist2.value);
            //setABPosition(robot.wrist3Joint, wrist3.value);


            //robot.shoulderJoint.jointPosition = new ArticulationReducedSpace(shoulder.value - initialSliderValue);
            //robot.elbowJoint.jointPosition = new ArticulationReducedSpace(elbow.value - initialSliderValue);
            //robot.wrist1Joint.jointPosition = new ArticulationReducedSpace(wrist1.value - initialSliderValue);
            //robot.wrist2Joint.jointPosition = new ArticulationReducedSpace(wrist2.value - initialSliderValue);
            //robot.wrist3Joint.jointPosition = new ArticulationReducedSpace(wrist3.value - initialSliderValue);

        }        

    }
    public void setABPosition(ArticulationBody ab, float position)
    {
        var drive = ab.xDrive;
        drive.target = position*Mathf.Rad2Deg;
        ab.xDrive = drive;
    }
    public void sliderValueChanged() {
        Debug.Log("sliders changed");
        // find the deltas

        for (int i = 0; i < prevSliderValues.Length; i++)
        {
            deltas[i] = SliderArray[i].value - prevSliderValues[i];
            prevSliderValues[i] = SliderArray[i].value;
        }
        UpdateABPositions(deltas);
    }
    void UpdateABPositions(float[] deltas)
    {
        int i = 0;
        foreach (ArticulationBody ab in ABArray)
        {
            float val = ab.xDrive.target + deltas[i];
            setABPosition(ab, deltas[i]);
            Debug.Log("setting " + ab.ToString() + " target to " + val.ToString() + "previous value was: " + ab.xDrive.target.ToString());
            i++;
        }
    }
}
