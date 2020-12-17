using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseConstructor : MonoBehaviour
{
    public List<StatusObject> statuses = new List<StatusObject>();
    public List<ActionObject> actions = new List<ActionObject>();
    public List<ItemObject> items = new List<ItemObject>();

    //prefabs
    public GameObject daggerModel;
    public GameObject swordModel;

    //icons
    public Sprite daggerIcon;
    public Sprite swordIcon;

    private void Awake()
    {
        BuildStatusDatabase();
        BuildActionDatabase();
        BuildItemDatabase();
    }

    //get status object
    public StatusObject GetStatusObject(string statusName)
    {
        return statuses.Find(status => status.statusName == statusName);
    }

    //get action object
    public ActionObject GetActionObject(string actionName)
    {
        return actions.Find(action => action.actionName == actionName);
    }

    public ItemObject GetItemObject(string itemName)
    {
        return items.Find(item => item.name == itemName);
    }

    void BuildItemDatabase()
    {
        items = new List<ItemObject>()
        {
            new ItemObject(
                "dagger",
                daggerIcon,
                daggerModel,
                GetActionObject("daggerMelee"),
                GetActionObject("daggerThrow")),

            new ItemObject(
                "sword",
                swordIcon,
                swordModel,
                GetActionObject("swordMelee"),
                GetActionObject("swordThrow"))
        };
    }

    void BuildStatusDatabase()
    {
        statuses = new List<StatusObject>() {

            //--WEAPON--//
            
            //daggerCast
            new StatusObject("daggerCast", "cast", 20, 0, 0, 0),
            
            //daggerHit 
            new StatusObject("daggerHit", "instance", 50, 0, 0, 0),

            //swordCast
            new StatusObject("swordCast", "cast", 40, 0, 0, 0),

            //swordHit
            new StatusObject("swordHit", "instance", 100, 0, 0, 0),

            //wandCast
            new StatusObject("wandCast", "cast", 80, 0, 0, 0),

            //--EFFECT--//

            //genericStun
            new StatusObject("stun", "duration", 0, 0, 0, 1)
        }; 
    }

    void BuildActionDatabase()
    {
        actions = new List<ActionObject>() {

            //daggerMelee
            new ActionObject(
                "dagger",
                "daggerMelee", //ActionName
                "Melee", //ActionType
                0.5f, //ActionSize
                0, //PointRange
                0, //ProjSpeed
                100, //Force
                0.3f, //Windup
                0.05f, //ActTime
                0.4f, //WindDown
                
                //daggerCastStatuses
                new List<StatusObject>(){
                    GetStatusObject("daggerCast")
                },

                //daggerGiveStatuses
                new List<StatusObject>(){
                    GetStatusObject("daggerHit")
                }

            ),

            //daggerThrow
            new ActionObject(
                "dagger",
                "daggerThrow", //ActionName
                "Throw", //ActionType
                0, //ActionSize
                0, //PointRange
                20, //ProjSpeed
                100, //Force
                0.3f, //Windup
                0.05f, //ActTime
                0.4f, //WindDown
                
                //daggerCastStatuses
                new List<StatusObject>(){
                    GetStatusObject("daggerCast")
                },

                //daggerGiveStatuses
                new List<StatusObject>(){
                    GetStatusObject("daggerHit"),
                    GetStatusObject("stun")
                }

            ),

            //swordMelee
            new ActionObject(
                "sword",
                "swordMelee", //ActionName
                "Melee", //ActionType
                1f, //ActionSize
                0, //PointRange
                0, //ProjSpeed
                150, //Force
                0.45f, //Windup
                0.05f, //ActTime
                0.4f, //WindDown
                
                //daggerCastStatuses
                new List<StatusObject>(){
                    GetStatusObject("swordCast")
                },

                //daggerGiveStatuses
                new List<StatusObject>(){
                    GetStatusObject("swordHit")
                }

            ),

            //swordThrow
            new ActionObject(
                "sword",
                "swordThrow", //ActionName
                "Throw", //ActionType
                0, //ActionSize
                0, //PointRange
                18, //ProjSpeed
                150, //Force
                0.45f, //Windup
                0.05f, //ActTime
                0.4f, //WindDown
                
                //daggerCastStatuses
                new List<StatusObject>(){
                    GetStatusObject("swordCast")
                },

                //daggerGiveStatuses
                new List<StatusObject>(){
                    GetStatusObject("swordHit"),
                    GetStatusObject("stun")
                }

            ),
        };
    }
}
