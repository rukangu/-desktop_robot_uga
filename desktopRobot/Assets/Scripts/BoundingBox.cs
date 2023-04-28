using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    public Transform TCPTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 pos = TCPTarget.position;
        pos.x = Mathf.Clamp(pos.x, transform.position.x - transform.localScale.x / 2f, transform.position.x + transform.localScale.x / 2);
        pos.y = Mathf.Clamp(pos.y, transform.position.y - transform.localScale.y / 2f, transform.position.y + transform.localScale.y / 2);
        pos.z = Mathf.Clamp(pos.z, transform.position.z - transform.localScale.z / 2f, transform.position.z + transform.localScale.z / 2);
        TCPTarget.position = pos;
    }
}
