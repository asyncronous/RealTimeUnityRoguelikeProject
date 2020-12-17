//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIClicker : MonoBehaviour
{
    public GraphicRaycaster m_Raycaster;
    public PointerEventData m_PointerEventData;
    public EventSystem m_EventSystem;

    public GameObject lastSelected = null;
    public bool currSelectedNullSprite;
    public Inventory playerInv;

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        //m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = EventSystem.current;
    }

    void Update()
    {
        //Check if the left Mouse button is clicked
        if (Input.GetButton("Fire1"))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(EventSystem.current);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.transform.GetComponent<Button>());

                //lastSelected = result.gameObject;
            }

        }

        if (EventSystem.current != null)
        {
            
            
            if(EventSystem.current.currentSelectedGameObject != null)
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }

            if (currSelectedNullSprite == true)
            {
                lastSelected = null;
                EventSystem.current.SetSelectedGameObject(null);
                currSelectedNullSprite = false;
            }


        }
    }
}