using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrtrController : MonoBehaviour
{
    public EntityController Body;
    public AraachModelController Model;
    public BoxCollider GiveBox;
    public GiveBoxMelee GiveBoxScript;
    public EntityInventory entInv;
    public Rigidbody entRigBody;

    //actions
    public bool actionInProgress;
    public bool dashed;
    public bool thrown;
    public float actionTime;
    private float windUpTime;
    private float actTime;
    private float windDownTime;

    private Vector3 intendedMove;
    public bool leftClickInput;
    public bool rightClickInput;

    //public Transform LeftAttackRotation;
    //public Transform RightAttackRotation;
    //public Transform LeftArmAttackRotation;
    //public Transform RightArmAttackRotation;
    private ActionObject currentAction;
    private string actionType;
    private float meleeXsize;
    private float meleeZsize;
    public ItemObject currItem;
    private GameObject newGameObject;

    public bool disableAttack;
    public string overrideAttack;

    // Update is called once per frame
    void Update()
    {
        intendedMove = Body.intendedMove;
        leftClickInput = Body.leftClickInput;
        rightClickInput = Body.rightClickInput;

        //GetActionStats();
        if (Body.isRagdoll == false && MouseInputUIBlocker.BlockedByUI == false && disableAttack == false)
        {
            ActionMachine();
        }
    }

    void GetActionStats(string input)
    {
        if (entInv.entityItems.Count == 0 || currItem == null)
        {
            currItem = null;
            return;
        }

        //currItem = entInv.entityItems[0];

        if (input == "leftClick")
        {
            currentAction = currItem.primAction;
            //used to determine whether activate melee givebox or throw projectile or spawn object at position etc

            actionType = currItem.primAction.actionType;
            meleeXsize = currItem.primAction.actionSize;
            meleeZsize = currItem.primAction.actionSize;
            windUpTime = currItem.primAction.windUp;
            actTime = currItem.primAction.actTime;
            windDownTime = currItem.primAction.windDown;

        }

        else if (input == "rightClick")
        {
            currentAction = currItem.secAction;

            actionType = currItem.secAction.actionType;
            meleeXsize = currItem.secAction.actionSize;
            meleeZsize = currItem.secAction.actionSize;
            windUpTime = currItem.secAction.windUp;
            actTime = currItem.secAction.actTime;
            windDownTime = currItem.secAction.windDown;
        }

        //else if (input == "dash")
        //{

        //}


        //Get action size

        //Get action type : Dash, Kick, Melee, Throw, Proj, Consume, PointCast
        //Get action size

        //Get action speed
        //Get action Range (Mostly just for PointCast, will just throw at speed with proj)
        //Get action Force
        //Get WindUp
        //windUpTime = 0.3f;
        //Get ActionTime
        //actTime = 0.05f;
        //Get WindDown
        //windDownTime = 0.4f;
        //Get Caster Statuses
        //Get RecieverStatuses

        //Get Secondary Effect Object
        //Trigger? : True False
        //CastDelay
        //Duration
        //Radius
        //Force
        //Caster Statuses
        //Reciever Statuses
        //Spawns?
        //Spawn Object
    }

    void ActionMachine()
    {
        //set each frame for model controller in Main Controller
        Model.windUpTime = windUpTime;
        Model.actTime = actTime;
        Model.windDownTime = windDownTime;
        Model.actionInProgress = actionInProgress;
        Model.actionTime = actionTime;

        //Model.rightArmRotation = transform.rotation;

        //leftAction state machine
        if ((actionInProgress == false && leftClickInput == true && Time.timeScale == 1) || (overrideAttack == "left" && actionInProgress == false)) //and action for item is allowed and not on cooldown
        {
            //leftaction
            actionInProgress = true;
            actionTime = Time.time;
            //Model.rightArmTarget = Model.righthandHeightTarget;
            dashed = false;
            GetActionStats("leftClick");
            thrown = false;

            if (currItem == null || disableAttack == true)
            {
                actionInProgress = false;
                disableAttack = false;
            }
        }
        else if ((actionInProgress == false && rightClickInput == true && Time.timeScale == 1) || (overrideAttack == "right" && actionInProgress == false)) //and action for item is allowed and not on cooldown
        {
            //rightaction
            actionInProgress = true;
            actionTime = Time.time;
            //Model.rightArmTarget = Model.righthandHeightTarget;
            dashed = false;
            GetActionStats("rightClick");
            thrown = false;

            if (currItem == null || disableAttack == true)
            {
                actionInProgress = false;
            }
        }

        if (actionInProgress == true)
        {
            if (Time.time - actionTime < windUpTime)
            {
                // move arm and torso to wind up angle
                //Model.torsoTargetTime = windUpTime;
                //Model.torsoTarget = LeftAttackRotation.position;
                //Model.rightArmTarget = LeftArmAttackRotation.position;
                //Model.rightArmRotation = LeftArmAttackRotation.rotation;
                Body.inputLocked = true;
            }
            else if (Time.time - actionTime > windUpTime && Time.time - actionTime < actTime + windUpTime)
            {
                Body.inputLocked = false;

                /////////////
                //--MELEE--//
                /////////////

                if (actionType == "Melee")
                {
                    //if dash 
                    if (dashed == false)
                    {
                        dashed = true;
                        entRigBody.AddForce(intendedMove * 200, ForceMode.Impulse);
                    }

                    // set attack collider to active/throw item, move to wind down angle over actTime - windUpTime
                    GiveBoxScript.currentAction = currentAction;
                    GiveBox.size = new Vector3(meleeXsize, 1, meleeZsize);
                    GiveBox.center = new Vector3(0, 0, meleeZsize / 2);
                    GiveBox.enabled = true;
                }

                /////////////   ////////////
                //--THROW--//   //--PROJ--//
                /////////////   ////////////

                else if (actionType == "Throw")
                {
                    if (thrown == false)
                    {
                        thrown = true;
                        newGameObject = Instantiate(currItem.itemModel);


                        //set initial position
                        newGameObject.transform.position = transform.position + transform.forward * 0.5f;
                        //set initial rotation
                        //newGameObject.transform.rotation = transform.rotation;


                        Vector3 indicatorPos = transform.position;
                        Vector3 targetPos = Body.projLookTar;
                        Vector3 distance = targetPos - indicatorPos;
                        Quaternion rot = Quaternion.LookRotation(distance.normalized);
                        newGameObject.transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z);

                        //set velocity using item projectile speed
                        newGameObject.GetComponent<Rigidbody>().velocity = newGameObject.transform.forward * currentAction.projSpeed;
                        //set action stats
                        newGameObject.GetComponentInChildren<GiveBoxProj>().currentAction = currentAction;
                        //set item info in projectile
                        newGameObject.GetComponentInChildren<GiveBoxProj>().thisItem = currItem;

                        //remove from inventory
                        //entInv.entityItems.RemoveAt(entInv.currentSlot);
                        entInv.entityItems[entInv.currentSlot] = null;
                        entInv.currentItem = null;
                    }

                    //if dash 
                    if (dashed == false)
                    {
                        dashed = true;
                        entRigBody.AddForce(intendedMove * 200, ForceMode.Impulse);
                    }

                }

                //Model.torsoTargetTime = actTime;
                //Model.torsoTarget = RightAttackRotation.position;
                //Model.rightArmTarget = RightArmAttackRotation.position;
                //Model.rightArmRotation = RightArmAttackRotation.rotation;
            }
            else if (Time.time - actionTime > actTime + windUpTime && Time.time - actionTime < actTime + windUpTime + windDownTime)
            {
                //Model.torsoTargetTime = windDownTime;
                //Model.torsoTarget = Model.DummyForward.position;
                //Model.rightArmTarget = Model.righthandHeightTarget;
                //Model.rightArmRotation = RightArmAttackRotation.rotation;
                GiveBox.enabled = false;
                if (Body.sprinting == true)
                {
                    Body.inputLocked = true;
                }
                else
                {
                    Body.inputLocked = false;
                }
            }

            else if (Time.time - actionTime > actTime + windUpTime)
            {
                //Model.torsoTargetTime = windDownTime;
                //Model.torsoTarget = Model.DummyForward.position;
                //Model.rightArmTarget = Model.righthandHeightTarget;
                //Model.rightArmRotation = transform.rotation;
                actionInProgress = false;
                GiveBox.enabled = false;
                Body.inputLocked = false;
                overrideAttack = null;
            }
        }
    }
}
