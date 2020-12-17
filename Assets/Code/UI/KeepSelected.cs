using UnityEngine;
using UnityEngine.EventSystems;

public class KeepSelected : MonoBehaviour
{
    private EventSystem eventSystem;
    public GameObject lastSelected = null;
    public bool currSelectedNullSprite;
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        if (EventSystem.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }

            if (currSelectedNullSprite == true)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                //EventSystem.current.SetSelectedGameObject(EventSystem.current.GetComponent<StandaloneInputModule>().)
            }
        }
    }
}