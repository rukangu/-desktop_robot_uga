using System.Collections.Generic;

[System.Serializable]
public class Point
{
    public float x;
    public float y;
    public float z;
    public float qw;
    public float qx;
    public float qy;
    public float qz;
    public int action;
    public float timeDelta;
    public float speed;
    public float baseAngle, shoulderAngle, elbowAngle, wrist1Angle, wrist2Angle, wrist3Angle, handAngle;

}

[System.Serializable]
public class TrajectoryStorageJSON
{
    public List<Point> points;
    public TrajectoryStorageJSON()
    {
        points = new List<Point>();
    }
}