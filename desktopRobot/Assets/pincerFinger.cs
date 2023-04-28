using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pincerFinger : MonoBehaviour
{
    public float closedX;
    Vector3 openPosition;
    ArticulationBody ab;
    // Start is called before the first frame update
    void Start()
    {
        openPosition = transform.localPosition;
        ab = GetComponent<ArticulationBody>();
        SetLimits();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetLimits()
    {
        float openXTarget = XDriveTarget(0.0f);
        float closedXTarget = XDriveTarget(1.0f);
        float min = Mathf.Min(openXTarget, closedXTarget);
        float max = Mathf.Max(openXTarget, closedXTarget);

        var drive = ab.zDrive;
        drive.lowerLimit = min;
        drive.upperLimit = max;
        ab.zDrive = drive;
    }

    public float CurrentGrip()
    {
        float grip = Mathf.InverseLerp(openPosition.x, closedX, transform.localPosition.x);
        return grip;
    }

    public Vector3 GetOpenPosition()
    {
        return openPosition;
    }

    public void UpdateGrip(float grip)
    {
        float targetX = XDriveTarget(grip);
        var drive = ab.zDrive;
        drive.target = targetX;
        ab.zDrive = drive;
    }
    public void ForceOpen(Transform t)
    {
        t.localPosition = openPosition;
        UpdateGrip(0.0f);
    }
    float XDriveTarget(float grip)
    {
        float xPosition = Mathf.Lerp(openPosition.x, closedX, grip);
        float targetX = (xPosition - openPosition.x) * transform.parent.localScale.x;
        return targetX;
    }
}
