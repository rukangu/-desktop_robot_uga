using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    float deltaX = 0;
    float deltaY = 0;

    public float sensitivity;
    public float scrollSensitivity;
    public float verticalMoveSensitivity;
    public float horizontalMoveSensitivity;
    public float shiftScaling;
    //
    float low_sensitivity, orig_sensitivity;
    float low_scrollSensitivity, orig_scrollSensitivity;
    float low_verticalMoveSensitivity, orig_verticalMoveSensitivity;
    float low_horizontalMoveSensitivity, orig_horizontalMoveSensitivity;

    Vector2 mousePos = Vector2.zero;

    public GameObject cameraTarget, player;
    // Start is called before the first frame update
    void Start()
    {
        orig_sensitivity = sensitivity;
        orig_scrollSensitivity = scrollSensitivity;
        orig_verticalMoveSensitivity = verticalMoveSensitivity;
        orig_horizontalMoveSensitivity = horizontalMoveSensitivity;

        low_sensitivity = sensitivity / shiftScaling;
        low_scrollSensitivity = scrollSensitivity / shiftScaling;
        low_verticalMoveSensitivity = verticalMoveSensitivity / shiftScaling;
        low_horizontalMoveSensitivity = horizontalMoveSensitivity / shiftScaling;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            sensitivity = low_sensitivity;
            scrollSensitivity = low_scrollSensitivity;
            verticalMoveSensitivity = low_verticalMoveSensitivity;
            horizontalMoveSensitivity = low_horizontalMoveSensitivity;
        }
        else
        {
            sensitivity = orig_sensitivity;
            scrollSensitivity = orig_scrollSensitivity;
            verticalMoveSensitivity = orig_verticalMoveSensitivity;
            horizontalMoveSensitivity = orig_horizontalMoveSensitivity;
        }

        #region ORBIT CAM


        float deltaX = Input.mousePosition.x - mousePos.x;
        float deltaY = Input.mousePosition.y - mousePos.y;

        if (Input.GetMouseButtonDown(1))
        {
            deltaX = 0;
            deltaY = 0;
        }
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(cameraTarget.transform.position, Vector3.up, deltaX * sensitivity);
            transform.RotateAround(cameraTarget.transform.position, transform.right, -deltaY * sensitivity);
        }

        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            //{
            //    transform.Translate(0, scrollDelta * scrollSensitivity, 0, Space.World);
            //}
            //else
            //{
            Vector3 pos = player.transform.localPosition;
            pos.z += scrollDelta * scrollSensitivity;
            player.transform.localPosition = pos;

        }

        if (Input.GetMouseButton(2))
        {
            transform.Translate(0, -deltaY * verticalMoveSensitivity, 0, Space.World);
            transform.Translate(-deltaX * horizontalMoveSensitivity, 0, 0, Space.Self);
        }

        mousePos.x = Input.mousePosition.x;
        mousePos.y = Input.mousePosition.y;

        #endregion
    }
}
