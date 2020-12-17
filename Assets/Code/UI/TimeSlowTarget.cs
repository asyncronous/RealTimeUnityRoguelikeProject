using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlowTarget : MonoBehaviour
{
    public string castState;
    private float timer;
    public TimeStopper timeStopper;
    public ActionController actionController;
    public EntityController entityController;
    public UIInventory uiInven;
    string input;

    public Image uiElement;
    
    // Start is called before the first frame update
    void Start()
    {
        castState = "Idle";
        uiElement = transform.GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //time is slow
        if (Time.timeScale == 0.001f && castState == "Idle")
        {
            input = null;
            transform.position = entityController.projLookTar;
            uiElement.color = Color.white;

            if (Input.GetButton("Fire1"))
            {
                input = "left";
            }
            else if (Input.GetButton("Fire2"))
            {
                input = "right";
            }

            if (input != null
                && actionController.actionInProgress == false
                && actionController.currItem != null
                && actionController.disableAttack == false
                && MouseInputUIBlocker.BlockedByUI == false
                && Inventory.waitingToSwap == false)
            {
                transform.position = entityController.projLookTar;
                timeStopper.casting = true;
                timeStopper.timeSlowOverride = true;
                entityController.lookOverride = true;
                uiElement.color = Color.red;
                castState = "RotatingToTarget";
            }
        }
        //time is fast
        else if (Time.timeScale == 1)
        {
            if (castState == "RotatingToTarget")
            {
                //make sure time doesn't revert
                timeStopper.timeSlowOverride = false;

                //lock entdeslooktar looktarget
                entityController.entDesLookTar = new Vector3(transform.position.x, 0.85f, transform.position.z);

                //calc diff between target and current rotation
                float currentRot = entityController.entRigBody.rotation.eulerAngles.y;
                float targetRot = entityController.pubAngle.eulerAngles.y;
                float ans = Mathf.Abs(currentRot - targetRot);

                if (ans < 25)
                {

                    //begin correct attack
                    if (input == "left")
                    {
                        actionController.overrideAttack = "left";

                    }
                    else if (input == "right")
                    {
                        actionController.overrideAttack = "right";
                    }

                    castState = "Casting";
                    timer = Time.time;
                    //uiElement.color = Color.white;
                }
            }

            else if (castState == "Casting")
            {
                if (actionController.actionInProgress == false && actionController.overrideAttack == null && Time.time - timer > 0.25f)
                {
                    timeStopper.timeSlowOverride = false;
                    entityController.lookOverride = false;
                    timeStopper.casting = false;
                    castState = "Idle";
                    uiElement.color = Color.clear;
                }
            }
            else
            {
                transform.position = entityController.projLookTar;
                uiElement.color = new Color(1,1,1, 0.5f);
            }
        }

        else
        {
            if(uiElement.color != Color.red)
            {
                transform.position = entityController.projLookTar;
                uiElement.color = new Color(1, 1, 1, 0.5f);
            }  
        }
    }
}
