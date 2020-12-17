using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour, IDropHandler, IDragHandler, IEndDragHandler
{
    public Inventory playerInv;
    public EventSystem uiEvent;
    public List<ItemObject> entityItems;
    public List<GameObject> slots;
    public Sprite nullSprite;
    public UIClicker selectScript;

    public GameObject currentSelect;
    private int firstItemSlot;
    private int secondItemSlot;

    public GameObject InventoryParent;
    public ActionController actionController;

    public bool dragging;
    public float dragTime;

    // Start is called before the first frame update
    void Start()
    {
        entityItems = playerInv.entityItems;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 10; i++)
        {
            if (i < entityItems.Count)
            {
                ItemObject item = entityItems[i];

                if (item == null)
                {
                    slots[i].GetComponent<Image>().sprite = nullSprite;
                    //slots[i].GetComponent<Button>().interactable = false;

                    if (dragging == false && Inventory.waitingToSwap == false)
                    {
                        slots[i].GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        slots[i].GetComponent<Button>().interactable = true;
                    }

                }
                else
                {
                    slots[i].GetComponent<Image>().sprite = item.itemIcon;
                    slots[i].GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                slots[i].GetComponent<Image>().sprite = nullSprite;
            }
        }

        for (int i = 0; i < 10; i++)
        {
            if (i < entityItems.Count)
            {
                int keypress = KeySelect();

                //determine if key is being pressed to choose item, if not then do last pressed
                if (keypress != -1 && keypress != -2)
                {
                    EventSystem.current.SetSelectedGameObject(slots[keypress]);
                    playerInv.currentItem = entityItems[keypress];
                    playerInv.currentSlot = keypress;
                    slots[keypress].transform.Find("Selector").gameObject.SetActive(true);
                }
                else if (slots[i] == selectScript.lastSelected)
                {
                    playerInv.currentItem = entityItems[i];
                    playerInv.currentSlot = i;
                    slots[i].transform.Find("Selector").gameObject.SetActive(true);
                }
                else
                {
                    slots[i].transform.Find("Selector").gameObject.SetActive(false);
                }
            }
            else
            {
                slots[i].GetComponent<Image>().sprite = nullSprite;
            }
        }

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == nullSprite)
            {
                selectScript.currSelectedNullSprite = true;
                playerInv.currentItem = null;
            }
            else
            {
                selectScript.currSelectedNullSprite = false;
            }
        }
    }

    public static int KeySelect()
    {
        if (Input.GetButton("OneKey"))
        {
            return 0;
        }
        else if (Input.GetButton("TwoKey"))
        {
            return 1;
        }
        else if (Input.GetButton("ThreeKey"))
        {
            return 2;
        }
        else if (Input.GetButton("FourKey"))
        {
            return 3;
        }
        else if (Input.GetButton("FiveKey"))
        {
            return 4;
        }
        else if (Input.GetButton("SixKey"))
        {
            return 5;
        }
        else if (Input.GetButton("SevenKey"))
        {
            return 6;
        }
        else if (Input.GetButton("EightKey"))
        {
            return 7;
        }
        else if (Input.GetButton("NineKey"))
        {
            return 8;
        }
        else if (Input.GetButton("TenKey"))
        {
            return 9;
        }
        else if (Input.GetButton("Escape"))
        {
            return -1;
        }
        else
        {
            return -2;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {        
        if(eventData.pointerEnter != null)
        {
            //drap and drop, swap selected item for highlighted item
            for (int i = 0; i < 10; i++)
            {
                if (i < entityItems.Count)
                {
                    if (slots[i] == eventData.lastPress)
                    {
                        firstItemSlot = i;

                        if (entityItems[i] == null)
                        {
                            return;
                        }

                    }
                    if (slots[i] == eventData.pointerEnter)
                    {
                        secondItemSlot = i;
                    }
                }
            }

            ItemObject first = entityItems[firstItemSlot];
            ItemObject second = entityItems[secondItemSlot];

            entityItems[firstItemSlot] = second;
            entityItems[secondItemSlot] = first;
            EventSystem.current.SetSelectedGameObject(slots[secondItemSlot]);
        }

        actionController.disableAttack = false;
        dragging = false;
        dragTime = Time.time;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        dragging = true;

        actionController.disableAttack = true;

        for (int i = 0; i < 10; i++)
        {
            if (i < entityItems.Count)
            {
                if (slots[i] == eventData.lastPress)
                {
                    firstItemSlot = i;

                    if (entityItems[i] == null)
                    {
                        actionController.disableAttack = false;
                        dragging = false;
                        dragTime = Time.time;
                        return;
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        
        //drap and drop, swap selected item for highlighted item
        for (int i = 0; i < 10; i++)
        {
            if (i < entityItems.Count)
            {
                if (slots[i] == data.lastPress)
                {
                    firstItemSlot = i;

                    if (entityItems[i] == null)
                    {
                        actionController.disableAttack = false;
                        dragging = false;
                        dragTime = Time.time;
                        return;
                    }
                }
            }
        }

        if (InventoryParent == data.pointerEnter && Inventory.waitingToSwap == false)
        {
            var newGameObject = Instantiate(entityItems[firstItemSlot].itemModel);

            //
            newGameObject.GetComponentInChildren<GiveBoxProj>().giveCollider.enabled = false;

            //set initial position
            newGameObject.transform.position = playerInv.transform.position + playerInv.transform.forward * 0.25f;
            //set initial rotation
            newGameObject.transform.rotation = playerInv.transform.rotation;
            //set velocity using item projectile speed
            newGameObject.GetComponent<Rigidbody>().velocity = playerInv.transform.forward * 2;
            //set action stats
            newGameObject.GetComponentInChildren<GiveBoxProj>().currentAction = entityItems[firstItemSlot].secAction;
            //set item info in projectile
            newGameObject.GetComponentInChildren<GiveBoxProj>().thisItem = entityItems[firstItemSlot];

            //remove from inventory
            //entInv.entityItems.RemoveAt(entInv.currentSlot);
            entityItems[firstItemSlot] = null;
        }

        actionController.disableAttack = false;
        dragging = false;
        dragTime = Time.time;
    }
}
