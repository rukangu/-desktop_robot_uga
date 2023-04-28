using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetmovement : MonoBehaviour
{
    public float scale = 0.01f;
    [Tooltip("This is the transform on the robot that's going to follow me")]
    public Transform Tracker;
    private void Awake()
    {
        MoveToOrigin();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            //transform.Translate(scale, 0f, 0f);
            transform.position -= Camera.main.transform.right * scale * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            // transform.Translate(-scale, 0f, 0f);
            transform.position += Camera.main.transform.right * scale * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            //transform.Translate(0.0f, 0f, -scale);
            transform.position -= Camera.main.transform.forward * scale * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            //transform.Translate(0.0f, 0f, scale);
            transform.position += Camera.main.transform.forward * scale * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            //transform.Translate(0.0f, scale, 0.0f);
            transform.position += Camera.main.transform.up * scale * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            // transform.Translate(0.0f, -scale, 0.0f);
            transform.position -= Camera.main.transform.up * scale * Time.deltaTime;
        }
    }

    public void MoveToOrigin()
    {
        transform.position = Tracker.position;
        transform.rotation = Tracker.rotation;
    }
}  

