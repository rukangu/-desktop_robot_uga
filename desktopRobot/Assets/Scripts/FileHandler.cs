using Newtonsoft.Json;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;

public class FileHandler:MonoBehaviour
{
    public int numberOfConstraints;
    public GameObject pointDisplayPrefab;
    public GameObject actionDisplayPrefab, WayPointPrefab;
    int actionCount;//, pointCount;
    private void Start()
    {
        actionCount = 0;
        //pointCount = 0;
    }
    public string ConvertToJSON(Trajectory T)
    {
        TrajectoryStorageJSON trajectoryStorageJSON = new TrajectoryStorageJSON();
        int i = 0;
        foreach (TrajectoryAction action in T.actions)
        {
            Point p = new Point();
            p.x = T.points[i].position.x;
            p.y = T.points[i].position.y;
            p.z = T.points[i].position.z;
            p.qw = T.points[i].rotation.w;
            p.qx = T.points[i].rotation.x;
            p.qy = T.points[i].rotation.y;
            p.qz = T.points[i].rotation.z;
            p.action = (int)action;
            p.timeDelta = T.timeDeltas[i];
            p.speed = T.speed;
            p.baseAngle = T.pose[i][0];
            p.shoulderAngle = T.pose[i][1];
            p.elbowAngle = T.pose[i][2];
            p.wrist1Angle = T.pose[i][3];
            p.wrist2Angle = T.pose[i][4];
            p.wrist3Angle = T.pose[i][5];
            p.handAngle = T.pose[i][6];

            trajectoryStorageJSON.points.Add(p);
            i++;
        }
        string json = JsonConvert.SerializeObject(trajectoryStorageJSON);
        return json;
    }
    public void LoadFromJSON(string s, Transform TCPTarget, ref Trajectory trajectory, ref List<GameObject>displayObjects, ref int pointCount)
    {
        // converts json string into a trajectory
        TrajectoryStorageJSON data = JsonConvert.DeserializeObject<TrajectoryStorageJSON>(s);
        float speed = 0 ;
        float[] angles = new float[numberOfConstraints];

            foreach (Point p in data.points)
        {
            speed = p.speed;
            trajectory.timeDeltas.Add(p.timeDelta);
            Vector3 pos = new Vector3(p.x, p.y, p.z);
            Quaternion rot = new Quaternion();
            rot.x = p.qx; rot.y = p.qy;rot.z = p.qz; rot.w = p.qw;
            // angles
            angles[0] = p.baseAngle; angles[1] = p.shoulderAngle; angles[2] = p.elbowAngle; angles[3] = p.wrist1Angle; angles[4] = p.wrist2Angle;
            angles[5] = p.wrist3Angle; angles[6] = p.handAngle;

            if ( (TrajectoryAction) p.action == TrajectoryAction.GripperClose)
            {
                actionCount++;
                trajectory.actions.Add(TrajectoryAction.GripperClose);
                var b = Instantiate(actionDisplayPrefab, Vector3.zero, Quaternion.identity);
                b.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "close gripper";
                displayObjects.Add(b);
                trajectory.points.Add(transform);// placeholder

                //add placeholder for pose in action
                trajectory.pose.Add(angles);

            }
            else if((TrajectoryAction)p.action == TrajectoryAction.GripperOpen)
            {
                actionCount++;
                trajectory.actions.Add(TrajectoryAction.GripperOpen);
                var b = Instantiate(actionDisplayPrefab, Vector3.zero, Quaternion.identity);
                b.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "open gripper";
                displayObjects.Add(b);
                trajectory.points.Add(transform);// placeholder. doesn't matter what's in it

                //add placeholder for pose in action
                trajectory.pose.Add(angles);
            }
            else if(((TrajectoryAction)p.action == TrajectoryAction.MoveTCP))
            {
                pointCount++;
                trajectory.actions.Add(TrajectoryAction.MoveTCP);
                var b = Instantiate(pointDisplayPrefab, Vector3.zero, Quaternion.identity);
                b.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "point " + pointCount.ToString();
                displayObjects.Add(b);

                //create the waypoint sphere and place it at the current TCP location
                GameObject o = Instantiate(WayPointPrefab, pos, rot);
                Transform t = o.transform;
                WayPoint wayPoint = b.gameObject.GetComponent<WayPoint>();
                wayPoint.marker = o;
                wayPoint.TCPTarget = TCPTarget;
                wayPoint.name = "point " + pointCount.ToString();
                trajectory.points.Add(t);
                trajectory.pose.Add(angles);

            }
            else
            {
                Debug.LogError("why do you have a weird action");
            }
        }
        trajectory.speed = speed;
    }
    public bool WriteFile(string path, string fileName, string data, FileMode mode)
    {
        //Write some text to the test.txt file
        bool retValue = false;
        string dataPath = path;
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        dataPath = dataPath + "/" + fileName;
        try
        {
            System.IO.File.WriteAllText(dataPath, data);
            retValue = true;
        }
        catch (System.Exception ex)
        {
            string ErrorMessages = "File Write Error\n" + ex.Message;
            retValue = false;
            Debug.LogError(ErrorMessages);
        }
        return retValue;
    }

    public string ReadFile(string path)
    {
       // path = path + "/" + fileName;
        //string path = Application.persistentDataPath + "/test.txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string txt = reader.ReadToEnd();
        reader.Close();
        return txt;
    }
}