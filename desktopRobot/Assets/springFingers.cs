using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class springFingers : MonoBehaviour
{
    public GameObject leftFinger, rightFinger;
    float distance = .1f;
    public bool close;
    public Rigidbody rbLeft, rbRight;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            Debug.Log(distance);
            distance -= Time.deltaTime * .02f;
        }

        leftFinger.transform.localPosition = new Vector3 (0,0,distance / 2);
        rightFinger.transform.localPosition = new Vector3(0, 0, -distance / 2);
        rbLeft.AddForce(leftFinger.transform.position - rbLeft.position);
        rbRight.AddForce(rightFinger.transform.position - rbRight.position);
    }
}
