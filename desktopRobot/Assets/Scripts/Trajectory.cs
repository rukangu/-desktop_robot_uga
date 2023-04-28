using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGXUnity;

public class Trajectory:MonoBehaviour
{
    public List<Transform> points;
    //public int dof;
    public List<float[]> pose; // holds all the degrees of freedom for the robot
    public List<float> timeDeltas;
    public List<TrajectoryAction> actions;
    public float speed;
    // Start is called before the first frame update
    void Awake()
    {
        points = new List<Transform>();
        timeDeltas = new List<float>();
        actions = new List<TrajectoryAction>();
        pose = new List<float[]>();
    }

    public void UndoLastPoint(int lastIdx)
    {
        points.RemoveAt(lastIdx);
        actions.RemoveAt(lastIdx);
        timeDeltas.RemoveAt(lastIdx);
        pose.RemoveAt(lastIdx);
    }
}
