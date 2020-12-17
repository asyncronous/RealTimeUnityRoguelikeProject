using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TimeStopper : MonoBehaviour
{
    private const float DEFAULT_FIXED_DELTA_TIME = 0.02f;
    private const float SLOW_DOWN_RATE = 0.65f;
    private const float SPEED_UP_RATE = 1.5f;

    private Coroutine _timeChange;
    private float _targetTimeScale;

    //TimeSlow
    private bool timeSlowInput;
    private float timeSlowCheck;

    public VignetteLerper vignetteLerper;

    public bool casting;
    public bool timeSlowOverride;

    // Update is called once per frame
    void Update()
    {
        timeSlowInput = Input.GetButtonDown("TimeSlow");

        if (Time.timeScale == 1)
        {
            if ((timeSlowInput == true && casting == false && Time.unscaledTime > timeSlowCheck + 0.1f) || Inventory.waitingToSwap == true)
            {
                SlowDownTime();
                timeSlowCheck = Time.unscaledTime;

                vignetteLerper.lerping = true;
                vignetteLerper.timeStopped = true;
                vignetteLerper.lerping = true;
            }
        }
        else if (Time.timeScale == 0.001f && Inventory.waitingToSwap == false)
        {
            if ((timeSlowInput == true || timeSlowOverride == true) && Time.unscaledTime > timeSlowCheck + 0.1f)
            {
                SpeedUpTime();
                timeSlowCheck = Time.unscaledTime;

                vignetteLerper.lerping = true;
                vignetteLerper.timeStopped = false;
                vignetteLerper.lerping = true;
            }
        }
    }

    private void SlowDownTime()
    {
        _targetTimeScale = 0.001f;
        if (_timeChange != null)
        {
            StopCoroutine(_timeChange);
        }
        _timeChange = StartCoroutine(MakeFixedTimeAgreeWithTimeScale());
    }

    private void SpeedUpTime()
    {
        _targetTimeScale = 1f;
        if (_timeChange != null)
        {
            StopCoroutine(_timeChange);
        }
        _timeChange = StartCoroutine(MakeFixedTimeAgreeWithTimeScale());
    }

    private IEnumerator MakeFixedTimeAgreeWithTimeScale()
    {
        while (Time.timeScale > _targetTimeScale && !Mathf.Approximately(Time.timeScale, _targetTimeScale))
        {
            Time.timeScale = Mathf.Max(_targetTimeScale, Time.timeScale * SLOW_DOWN_RATE);
            //Time.fixedDeltaTime = DEFAULT_FIXED_DELTA_TIME * Time.timeScale;
            //Debug.Log($"Reducing TimeScale Time to {Time.timeScale} to match TimeScale of {Time.timeScale} at time {Time.unscaledTime}");
            yield return null;
        }

        while (Time.timeScale < _targetTimeScale && !Mathf.Approximately(Time.timeScale, _targetTimeScale))
        {
            Time.timeScale = Mathf.Min(_targetTimeScale, Time.timeScale * SPEED_UP_RATE);
            //Time.fixedDeltaTime = DEFAULT_FIXED_DELTA_TIME * Time.timeScale;
            //Debug.Log($"Increasing TimeScale to {Time.timeScale} to match TimeScale of {Time.timeScale} at time {Time.unscaledTime}");
            yield return null;
        }
    }
}

