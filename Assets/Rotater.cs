using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(Camera.main.transform.position,);
        
        //transform.forward = Camera.main.transform.forward;


        //find the vector pointing from our position to the target
        Vector3 _direction = (Camera.main.transform.position - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        //transform.rotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.LookRotation(new Vector3(_direction.x, 90, _direction.z));

        //rotate us over time according to speed until we are in the required rotation
        //transform.rotation = _lookRotation
    }
}
