using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class CustomOnHitShaker : MonoBehaviour
{
    public Shaker MyShaker;
    public ShakePreset ShakePreset;
    public bool shake;
    
    // Update is called once per frame
    void Update()
    {
        if (shake == true)
        {
            shake = false;
            MyShaker.Shake(ShakePreset);
        }
    }
}
