using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusObject
{
    public string statusName;
    public string statusType; //instance, overTime, 
    public int debuffValue;
    public int buffValue;
    public float applyTime;
    public int duration;

    public StatusObject(
        string _statusName, 
        string _statusType, 
        int _debuffValue,
        int _buffValue,
        float _applyTime,
        int _duration)
    {
        statusName = _statusName;
        statusType = _statusType; //instance, overTime, 
        debuffValue = _debuffValue;
        buffValue = _buffValue;
        applyTime = _applyTime;
        duration = _duration;
    }

    public StatusObject(StatusObject status)
    {
        statusName = status.statusName;
        statusType = status.statusType; //instance, overTime, 
        debuffValue = status.debuffValue;
        buffValue = status.buffValue;
        applyTime = status.applyTime;
        duration = status.duration;
    }
}
