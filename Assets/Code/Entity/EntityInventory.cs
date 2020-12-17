using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.EventSystems;

public class EntityInventory : MonoBehaviour
{
    public List<Transform> closeItems;
    public List<ItemObject> entityItems = new List<ItemObject>();
    public ItemObject currentItem;
    public int currentSlot;
    public DatabaseConstructor database;
    public bool interactClosest;
    public CrtrController actionContr;
    private ItemObject objectStashed;

    public UIInventory uiInv;

    public static bool waitingToSwap;
    //public WeildedItemSwapper weildedItemSwapper;

    private void Start()
    {
        GiveItem("dagger");
        //GiveItem("dagger");
        //GiveItem("dagger");
        //GiveItem("sword");
        //GiveItem("sword");

        //GiveItem("sword");
        //GiveItem("sword");
        //GiveItem("sword");
        //GiveItem("sword");
        //GiveItem("sword");

        //SpawnItem("sword");

        currentItem = entityItems[0];
        objectStashed = null;
    }

    private void Update()
    {
        interactClosest = false;


        if (Input.GetButton("Interact"))
        {
            interactClosest = true;
        }

        //CheckInteractables();

        if (currentItem != null)
        {
            actionContr.currItem = currentItem;
            //actionContr.currentSlot = currentSlot;
        }

        actionContr.currItem = currentItem;

        //transform.Find("WeildedParent").position = actionContr.Model.RightHandBone.transform.position;
        ////transform.Find("WeildedParent").rotation = actionContr.Model.RightHandBone.transform.rotation;

        //if (currentItem != null)
        //{
        //    if (weildedItemSwapper.weildedItemModel == null)
        //    {
        //        weildedItemSwapper.weildedItemModel = currentItem.itemModel;

        //        var newGameObject = Instantiate(currentItem.itemModel);

        //        newGameObject.GetComponentInChildren<GiveBoxProj>().giveCollider.enabled = false;

        //        newGameObject.GetComponent<BoxCollider>().enabled = false;

        //        //set initial position
        //        newGameObject.transform.position = weildedItemSwapper.transform.position;
        //        //set initial rotation
        //        newGameObject.transform.rotation = weildedItemSwapper.transform.rotation;
        //        //Destroy Rigidbody
        //        Destroy(newGameObject.transform.GetComponent<Rigidbody>());
        //        //set action stats
        //        Destroy(newGameObject.GetComponentInChildren<GiveBoxProj>().gameObject);

        //        newGameObject.transform.parent = weildedItemSwapper.transform;

        //        //set initial position
        //        newGameObject.transform.localPosition = new Vector3(0, 0, 0.12f);
        //    }

        //    else if (currentItem.itemModel.name != weildedItemSwapper.weildedItemModel.name)
        //    {
        //        weildedItemSwapper.weildedItemModel = currentItem.itemModel;
        //        foreach (Transform child in weildedItemSwapper.transform)
        //        {
        //            Destroy(child.gameObject);
        //        }

        //        var newGameObject = Instantiate(currentItem.itemModel);

        //        newGameObject.GetComponentInChildren<GiveBoxProj>().giveCollider.enabled = false;

        //        newGameObject.GetComponent<BoxCollider>().enabled = false;

        //        //set initial position
        //        newGameObject.transform.position = weildedItemSwapper.transform.position;
        //        //set initial rotation
        //        newGameObject.transform.rotation = weildedItemSwapper.transform.rotation;
        //        //Destroy Rigidbody
        //        Destroy(newGameObject.transform.GetComponent<Rigidbody>());
        //        //set action stats
        //        Destroy(newGameObject.GetComponentInChildren<GiveBoxProj>().gameObject);

        //        newGameObject.transform.parent = weildedItemSwapper.transform;

        //        //set initial position
        //        newGameObject.transform.localPosition = new Vector3(0, 0, 0.12f);
        //    }
        //}

        //else
        //{
        //    if (weildedItemSwapper.weildedItemModel != null)
        //    {
        //        foreach (Transform child in weildedItemSwapper.transform)
        //        {
        //            Destroy(child.gameObject);
        //        }

        //        weildedItemSwapper.weildedItemModel = null;
        //    }
        //}
    }

    public void GiveItem(string itemName)
    {
        ItemObject itemToAdd = database.GetItemObject(itemName);
        entityItems.Add(itemToAdd);
    }

    public void SpawnItem(string itemName)
    {
        ItemObject itemToAdd = database.GetItemObject(itemName);
        var newGameObject = Instantiate(itemToAdd.itemModel);

        newGameObject.GetComponentInChildren<GiveBoxProj>().giveCollider.enabled = false;

        //set initial position
        newGameObject.transform.position = transform.position + transform.forward * 0.25f;
        //set initial rotation
        newGameObject.transform.rotation = transform.rotation;
        //set velocity using item projectile speed
        newGameObject.GetComponent<Rigidbody>().velocity = transform.forward * 2;
        //set action stats
        newGameObject.GetComponentInChildren<GiveBoxProj>().currentAction = itemToAdd.secAction;
        //set item info in projectile
        newGameObject.GetComponentInChildren<GiveBoxProj>().thisItem = itemToAdd;
    }

    void CheckInteractables()
    {
        if (objectStashed == null)
        {
            Collider[] closeObjects = Physics.OverlapSphere(transform.position, 3);
            closeItems.Clear();

            foreach (var closeObject in closeObjects)
            {
                if (closeObject.tag == "Object")
                {
                    closeItems.Add(closeObject.transform);
                }
            }

            //if interacting
            if (interactClosest == true)
            {
                ItemObject closestItem = null;
                Transform closestTransform = null;
                float maxDistance = 0.5f;

                if (closeItems.Count > 0)
                {
                    foreach (Transform trans in closeItems)
                    {
                        Vector3 pos = trans.root.position;
                        Vector3 adjPos = new Vector3(pos.x, transform.position.y, pos.z);
                        float distance = Vector3.Distance(adjPos, transform.position);

                        if (distance < maxDistance)
                        {
                            maxDistance = distance;
                            closestItem = trans.GetComponentInChildren<GiveBoxProj>().thisItem;
                            closestTransform = trans;
                        }
                    }
                }

                //add to first null
                if (closestItem != null)
                {
                    bool freeslot = false;

                    for (int i = 0; i < 10; i++)
                    {
                        if (entityItems[i] == null)
                        {
                            entityItems[i] = closestItem;
                            freeslot = true;
                            break;
                        }
                    }

                    if (freeslot == false)
                    {
                        waitingToSwap = true;
                        objectStashed = closestItem;
                        EventSystem.current.SetSelectedGameObject(null);
                        uiInv.selectScript.lastSelected = null;
                    }

                    Destroy(closestTransform.gameObject);
                }
            }
        }
        else
        {
            int keypress = UIInventory.KeySelect();

            for (int i = 0; i < 10; i++)
            {
                if (i < entityItems.Count)
                {
                    //determine if cancel swap
                    if (keypress == -1)
                    {
                        var newGameObject = Instantiate(objectStashed.itemModel);

                        newGameObject.GetComponentInChildren<GiveBoxProj>().giveCollider.enabled = false;

                        //set initial position
                        newGameObject.transform.position = transform.position + transform.forward * 0.25f;
                        //set initial rotation
                        newGameObject.transform.rotation = transform.rotation;
                        //set velocity using item projectile speed
                        newGameObject.GetComponent<Rigidbody>().velocity = transform.forward * 2;
                        //set action stats
                        newGameObject.GetComponentInChildren<GiveBoxProj>().currentAction = objectStashed.secAction;
                        //set item info in projectile
                        newGameObject.GetComponentInChildren<GiveBoxProj>().thisItem = objectStashed;

                        waitingToSwap = false;
                        objectStashed = null;
                        break;
                    }
                    //else if valid key swap
                    else if (keypress != -2)
                    {
                        if (entityItems[keypress] != null)
                        {
                            var newGameObject = Instantiate(entityItems[keypress].itemModel);

                            newGameObject.GetComponentInChildren<GiveBoxProj>().giveCollider.enabled = false;

                            //set initial position
                            newGameObject.transform.position = transform.position + transform.forward * 0.25f;
                            //set initial rotation
                            newGameObject.transform.rotation = transform.rotation;
                            //set velocity using item projectile speed
                            newGameObject.GetComponent<Rigidbody>().velocity = transform.forward * 2;
                            //set action stats
                            newGameObject.GetComponentInChildren<GiveBoxProj>().currentAction = entityItems[keypress].secAction;
                            //set item info in projectile
                            newGameObject.GetComponentInChildren<GiveBoxProj>().thisItem = entityItems[keypress];
                        }

                        //remove from inventory
                        //entInv.entityItems.RemoveAt(entInv.currentSlot);
                        entityItems[keypress] = objectStashed;
                        waitingToSwap = false;
                        objectStashed = null;
                        break;
                    }
                    else if (uiInv.slots[i] == uiInv.selectScript.lastSelected)
                    {
                        if (entityItems[i] != null)
                        {
                            var newGameObject = Instantiate(entityItems[i].itemModel);

                            newGameObject.GetComponentInChildren<GiveBoxProj>().giveCollider.enabled = false;

                            //set initial position
                            newGameObject.transform.position = transform.position + transform.forward * 0.25f;
                            //set initial rotation
                            newGameObject.transform.rotation = transform.rotation;
                            //set velocity using item projectile speed
                            newGameObject.GetComponent<Rigidbody>().velocity = transform.forward * 2;
                            //set action stats
                            newGameObject.GetComponentInChildren<GiveBoxProj>().currentAction = entityItems[i].secAction;
                            //set item info in projectile
                            newGameObject.GetComponentInChildren<GiveBoxProj>().thisItem = entityItems[i];
                        }

                        //remove from inventory
                        //entInv.entityItems.RemoveAt(entInv.currentSlot);
                        entityItems[i] = objectStashed;
                        waitingToSwap = false;
                        objectStashed = null;
                        break;
                    }
                }
            }
        }
    }
}
