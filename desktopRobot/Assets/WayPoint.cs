using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class WayPoint : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
    public GameObject marker;    
    public Button myButton;
    Vector3 initialScale;
    public bool permanentSelection;
    public Transform TCPTarget;
    public string myName;
    public PracticeSceneUI UI;
    public float finger1Pos;
    public float finger2Pos;
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        initialScale = marker.transform.localScale;
        MakeInvisible();
        permanentSelection = false;
        myButton.onClick.AddListener(MoveTCPToMe);

        // assign UI if we're loaded from saved trajectory and none is assigned
        if(UI == null)
        {
            UI = FindObjectOfType<PracticeSceneUI>();
        }

    }

    public void MakeVisible()
    {
        marker.transform.localScale = initialScale;
        
    }
    public void MakeInvisible()
    {
        if (!permanentSelection)
        {
            //marker.transform.localScale = Vector3.zero;
        }
    }
    // When highlighted with mouse.
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Do something.
       // Debug.Log("<color=red>event:</color> completed mouse highlight.");
        MakeVisible();
    }
    // When selected.
    public void OnSelect(BaseEventData eventData)
    {
        // Do something.
        // Debug.Log("<color=red>Event:</color> Completed selection.");
        //permanentSelection = true;
        //MakeVisible();

        // make robot move to me
    }


    public void OnDestroy()
    {
        GameObject.Destroy(marker);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MakeInvisible();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        permanentSelection = false;
        //MakeInvisible();
    }
    public void MoveTCPToMe()
    {
        TCPTarget.position = marker.transform.position;
        TCPTarget.rotation = marker.transform.rotation;
        UI.HandleMoveRobotToPosition(this);
        UI.ShowGizmo();
    }
}
