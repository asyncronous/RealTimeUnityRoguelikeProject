using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCamAspect : MonoBehaviour
{
    public Camera cam;
    
    // Update is called once per frame
    void Update()
    {
        cam.aspect = Camera.main.aspect;
    }
}
