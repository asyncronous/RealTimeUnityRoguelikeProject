using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class VignetteLerper : MonoBehaviour
{
    private Volume v;
    private Vignette vg;
    public bool timeStopped;
    public bool lerping;

    // Start is called before the first frame update
    void Start()
    {
        v = GetComponent<Volume>();
        v.profile.TryGet(out vg);
        lerping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(lerping == true)
        {
            if (timeStopped == true)
            {
                StartCoroutine(ApplyVignette(0.35f, 0.1f));
            }
            else
            {

                StartCoroutine(ApplyVignette(0, 0.1f));
            }
        }
    }

    IEnumerator ApplyVignette(float endValue, float duration)
    {
        float time = 0;
        //lerping = false;

        float startValue = vg.intensity.value;

        while (time < duration)
        {
            float value = Mathf.SmoothStep(startValue, endValue, time / duration);
            vg.intensity.value = value;
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        vg.intensity.value = endValue;
        lerping = false;
    }
}
