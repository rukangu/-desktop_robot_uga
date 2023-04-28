using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerMovement : MonoBehaviour
{
    public TMP_Text debugText1, debugText2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotateHorizontal = Input.GetAxis("Mouse X");
        float rotateVertical = Input.GetAxis("Mouse Y");
        //Debug.Log("mouseX: " + rotateHorizontal);
        //Debug.Log("mouseY: " + rotateVertical);
        debugText1.text = rotateHorizontal.ToString();
        debugText2.text = rotateVertical.ToString();

    }
}
