using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemObject
{
    public string name;
    public GameObject itemModel;
    public Sprite itemIcon;
    public ActionObject primAction;
    public ActionObject secAction;

    public ItemObject(
        string _name,
        Sprite _itemIcon,
        GameObject _itemModel,
        ActionObject _primAction,
        ActionObject _secAction)
    {
        name = _name;
        itemIcon = _itemIcon;
        itemModel = _itemModel;
        primAction = _primAction;
        secAction = _secAction;
    }
}

public class ActionObject
{
    public string itemName;
    public string actionName;
    public string actionType;
    public float actionSize;
    public float pointRange;
    public float projSpeed;
    public float force;
    public float windUp;
    public float actTime;
    public float windDown;
    public List<StatusObject> castStatuses;
    public List<StatusObject> giveStatuses;

    public ActionObject(
        string _itemName,
        string _actionName,
        string _actionType,
        float _actionSize,
        float _pointRange,
        float _projSpeed,
        float _force,
        float _windUp,
        float _actTime,
        float _windDown,
        List<StatusObject> _castStatuses,
        List<StatusObject> _giveStatuses)
    {
        itemName = _itemName;
        actionName = _actionName;
        actionType = _actionType;
        actionSize = _actionSize;
        pointRange = _pointRange;
        projSpeed = _projSpeed;
        force = _force;
        windUp = _windUp;
        actTime = _actTime;
        windDown = _windDown;
        castStatuses = _castStatuses;
        giveStatuses = _giveStatuses;
    }

}
