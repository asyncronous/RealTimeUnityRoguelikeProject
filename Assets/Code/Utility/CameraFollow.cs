using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.1F;
    private Vector3 targetPosition;

    private float alignmentSpeed;
    private float alignmentDamping;

    private Rigidbody rigidBody;
    public bool isRagdoll;

    private void Start()
    {
        alignmentSpeed = 50;
        alignmentDamping = 20;
        targetPosition = target.TransformPoint(new Vector3(0, 12, 0.3f));
        transform.position = targetPosition;
        rigidBody = transform.GetComponent<Rigidbody>();
        isRagdoll = target.transform.GetComponent<EntityController>().isRagdoll;

    }

    void FixedUpdate()
    {
        isRagdoll = target.transform.GetComponent<EntityController>().isRagdoll;

        Vector3 cameraPos = transform.position;

        //Vector3 targetPos = target.TransformPoint(new Vector3(0, 12, 0.1f)) + target.GetComponent<Rigidbody>().velocity * 0.5f;
        Vector3 targetPos = new Vector3(0, 0, 0);
        if (isRagdoll == true)
        {
            
            targetPos = target.position + new Vector3(0, 12, 0) + target.GetComponent<Rigidbody>().velocity * 0.5f;
        }
        else if(isRagdoll == false)
        {
            targetPos = target.TransformPoint(new Vector3(0, 12, 0.3f)) + target.GetComponent<Rigidbody>().velocity * 0.5f;

        }

        Vector3 direction = (targetPos - cameraPos);

        rigidBody.AddForce(alignmentSpeed * direction - alignmentDamping * rigidBody.velocity);
    }
}