using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by" + other.transform.name);
        robotSensor rs = other.transform.GetComponent<robotSensor>();
        if(rs != null)
        {
            var cubeRenderer = this.transform.gameObject.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", Color.green);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided with" + collision.transform.name);
    }
}
