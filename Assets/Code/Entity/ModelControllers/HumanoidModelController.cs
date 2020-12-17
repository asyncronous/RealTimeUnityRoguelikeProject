using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidModelController : MonoBehaviour
{
    public bool sneaking;
    public bool sprinting;
    public bool walking;
    public bool slowed;

    Vector3 entVelocity;
    Vector3 entDirection;
    public float entSpeed;
    public Rigidbody entRigBody;
    public EntityController entController;
    public bool isRagdoll;

    //modelbones
    public GameObject entModel;

    public Transform TorsoBone;
    public Transform HeadBone;
    public Transform RightHandBone;

    //modeltargets
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public Transform rightFootTarget;
    public Transform leftFootTarget;


    //proceduralMovementTargets and Vectors
    private Vector3 heightTarget;
    private float armOffset;

    public Vector3 righthandHeightTarget;
    private Vector3 rightfootHeightTarget;
    private Vector3 lefthandHeightTarget;
    private Vector3 leftfootHeightTarget;

    private Vector3 rightHandVector;
    private Vector3 leftHandVector;
    private Vector3 rightFootVector;
    private Vector3 leftFootVector;

    private Vector3 rightHandVectorFinal;
    private Vector3 leftHandVectorFinal;

    public Transform rightHipsBone;
    public Transform leftHipsBone;
    public Transform rightShoulderBone;
    public Transform leftShoulderBone;
    public Transform LeftLookRotation;
    public Transform RightLookRotation;
    public Transform DummyForward;

    public bool lerping;
    private float leftTime;
    private float rightTime;
    private float lerpTime;
    private string nextLeg = "left";
    private float movementOffset;
    private float differenceMove;
    private Vector3 lastMoveVector;
    private bool idle;

    private Vector3 lastf;
    private Vector3 lastu;

    public bool sprintInput;
    public bool sneakInput;
    public bool ragdollInput;
    public bool leftClickInput;
    public bool rightClickInput;

    //mirror action variables for modelcontroller
    public bool actionInProgress;
    public float actionTime;
    public float windUpTime;
    public float actTime;
    public float windDownTime;
    public float torsoTargetTime;
    public Vector3 torsoTarget;
    public Vector3 rightArmTarget;
    public Quaternion rightArmRotation;
    public float torsoLerpTime;
    public Quaternion pubAngle;
    public Vector3 intendedMove;

    public float lastOffset;
    public float raycastTargetDistance;
    public float raycastDistance;


    // Update is called once per frame
    void Update()
    {
        entVelocity = entRigBody.velocity;
        entDirection = entRigBody.velocity.normalized;
        entSpeed = entVelocity.magnitude;

        walking = entController.walking;
        sprinting = entController.sprinting;
        sneaking = entController.sneaking;
        isRagdoll = entController.isRagdoll;

    }

    private void FixedUpdate()
    {
        if(isRagdoll == false)
        {
            ModelControl();
        }
    }

    public void ModelControl()
    {
        //heights for different stance
        if (walking == true)
        {
            armOffset = 0.2f;

            rightfootHeightTarget = new Vector3(0, -0.555f, 0) + rightHipsBone.position;
            leftfootHeightTarget = new Vector3(0, -0.555f, 0) + leftHipsBone.position;
            righthandHeightTarget = new Vector3(0, -0.375f, 0) + rightShoulderBone.position;
            lefthandHeightTarget = new Vector3(0, -0.375f, 0) + leftShoulderBone.position;
        }
        else if (sprinting == true)
        {
            armOffset = 0.25f;

            rightfootHeightTarget = new Vector3(0, -0.475f, 0) + rightHipsBone.position;
            leftfootHeightTarget = new Vector3(0, -0.475f, 0) + leftHipsBone.position;
            righthandHeightTarget = new Vector3(0, -0.325f, 0) + rightShoulderBone.position;
            lefthandHeightTarget = new Vector3(0, -0.325f, 0) + leftShoulderBone.position;
        }
        else if (sneaking == true)
        {
            armOffset = 0.175f;

            rightfootHeightTarget = new Vector3(0, -0.415f, 0) + rightHipsBone.position;
            leftfootHeightTarget = new Vector3(0, -0.415f, 0) + leftHipsBone.position;
            righthandHeightTarget = new Vector3(0, -0.325f, 0) + rightShoulderBone.position;
            lefthandHeightTarget = new Vector3(0, -0.325f, 0) + leftShoulderBone.position;
        }

        if (entSpeed < 0.1f)
        {
            idle = true;
            movementOffset = 0.025f;
            lerpTime = 0.25f;
        }

        if (entSpeed < 0.5f)
        {
            movementOffset = 0.25f;
            lerpTime = 0.25f;
        }

        //more than 0.1f x or z axis
        if (entSpeed > 0.5f && (entVelocity.x > 0.5f || entVelocity.x < -0.5f || entVelocity.z > 0.5f || entVelocity.z < -0.5f))
        {
            //forward and backwards
            if (Vector3.Dot(transform.forward, entDirection) > 0.75 || Vector3.Dot(transform.forward, entDirection) < -0.75)
            {
                if (walking == true)
                {
                    float offset = 0.325f;

                    lerpTime = 0.125f;
                    movementOffset = offset / 2;
                    rightfootHeightTarget += entRigBody.velocity.normalized * offset;
                    leftfootHeightTarget += entRigBody.velocity.normalized * offset;
                }

                if (sprinting == true)
                {
                    float offset = 0.4f;

                    lerpTime = 0.15f;
                    movementOffset = offset / 2.5f;
                    rightfootHeightTarget += entRigBody.velocity.normalized * offset;
                    leftfootHeightTarget += entRigBody.velocity.normalized * offset;
                }

                if (sneaking == true)
                {
                    float offset = 0.35f;

                    lerpTime = 0.15f;
                    movementOffset = offset / 2;
                    rightfootHeightTarget += entRigBody.velocity.normalized * offset;
                    leftfootHeightTarget += entRigBody.velocity.normalized * offset;
                }
            }

            //strafing
            if (Vector3.Dot(transform.forward, entDirection) < 0.75 && Vector3.Dot(transform.forward, entDirection) > -0.75)
            {
                if (walking == true)
                {
                    float offset = 0.325f * 0.8f;

                    lerpTime = 0.125f;
                    movementOffset = offset / 2;
                    rightfootHeightTarget += entRigBody.velocity.normalized * offset;
                    leftfootHeightTarget += entRigBody.velocity.normalized * offset;
                }

                if (sprinting == true)
                {
                    float offset = 0.4f;

                    lerpTime = 0.15f;
                    movementOffset = offset / 2;
                    rightfootHeightTarget += entRigBody.velocity.normalized * offset;
                    leftfootHeightTarget += entRigBody.velocity.normalized * offset;
                }

                if (sneaking == true)
                {
                    float offset = 0.35f * 0.8f;

                    lerpTime = 0.125f;
                    movementOffset = offset / 2;
                    rightfootHeightTarget += entRigBody.velocity.normalized * offset;
                    leftfootHeightTarget += entRigBody.velocity.normalized * offset;
                }
            }
        }

        differenceMove = (lastMoveVector - transform.position).magnitude;

        //check if ready to take next step, also add 
        if (differenceMove > movementOffset || idle == true)
        {
            if (nextLeg == "right")
            {
                if (lerping == false)
                {
                    rightTime = Time.time;
                    lerping = true;
                }

                float lt = (Time.time - rightTime) / lerpTime;

                if (Time.time - rightTime < lerpTime)
                {
                    rightFootVector = new Vector3(
                        Mathf.SmoothStep(rightFootVector.x, rightfootHeightTarget.x, lt),
                        Mathf.SmoothStep(rightFootVector.y, rightfootHeightTarget.y, lt),
                        Mathf.SmoothStep(rightFootVector.z, rightfootHeightTarget.z, lt));
                }
                else if (Time.time - rightTime > lerpTime)
                {
                    rightFootVector = new Vector3(
                        rightfootHeightTarget.x,
                        rightfootHeightTarget.y,
                        rightfootHeightTarget.z);

                    nextLeg = "left";
                    lastMoveVector = transform.position;
                    idle = false;
                    lerping = false;
                }
            }
            else if (nextLeg == "left")
            {
                if (lerping == false)
                {
                    leftTime = Time.time;
                    lerping = true;
                }

                float lt = (Time.time - leftTime) / lerpTime;

                if (Time.time - leftTime < lerpTime)
                {
                    leftFootVector = new Vector3(
                        Mathf.SmoothStep(leftFootVector.x, leftfootHeightTarget.x, lt),
                        Mathf.SmoothStep(leftFootVector.y, leftfootHeightTarget.y, lt),
                        Mathf.SmoothStep(leftFootVector.z, leftfootHeightTarget.z, lt));
                }
                else if (Time.time - leftTime > lerpTime)
                {
                    leftFootVector = new Vector3(
                        leftfootHeightTarget.x,
                        leftfootHeightTarget.y,
                        leftfootHeightTarget.z);

                    nextLeg = "right";
                    lastMoveVector = transform.position;
                    idle = false;
                    lerping = false;
                }
            }
        }

        leftFootTarget.position = leftFootVector;
        rightFootTarget.position = rightFootVector;

        if (actionInProgress == false)
        {
            if (entSpeed > 0.5f && Vector3.Dot(transform.forward, entDirection) >= 0)
            {
                if (leftTime > rightTime)
                {
                    leftHandVector = lefthandHeightTarget + -transform.forward.normalized * armOffset;
                    rightHandVector = righthandHeightTarget + transform.forward.normalized * armOffset;
                }
                else if (leftTime < rightTime)
                {
                    leftHandVector = lefthandHeightTarget + transform.forward.normalized * armOffset;
                    rightHandVector = righthandHeightTarget + -transform.forward.normalized * armOffset;
                }
            }
            else if (entSpeed > 0.5f && Vector3.Dot(transform.forward, entDirection) < 0)
            {
                if (leftTime > rightTime)
                {
                    leftHandVector = lefthandHeightTarget + transform.forward.normalized * armOffset;
                    rightHandVector = righthandHeightTarget + -transform.forward.normalized * armOffset;
                }
                else if (leftTime < rightTime)
                {
                    leftHandVector = lefthandHeightTarget + -transform.forward.normalized * armOffset;
                    rightHandVector = righthandHeightTarget + transform.forward.normalized * armOffset;
                }
            }
            else
            {
                leftHandVector = lefthandHeightTarget;
                rightHandVector = righthandHeightTarget;
            }

            float armLerp = lerpTime * 1.5f;
            float ltt = (Time.time - leftTime) / armLerp;
            float rtt = (Time.time - rightTime) / armLerp;

            if (Time.time - leftTime < armLerp)
            {
                leftHandVectorFinal = new Vector3(
                    Mathf.SmoothStep(leftHandVectorFinal.x, leftHandVector.x, ltt),
                    Mathf.SmoothStep(leftHandVectorFinal.y, leftHandVector.y, ltt),
                    Mathf.SmoothStep(leftHandVectorFinal.z, leftHandVector.z, ltt));
            }
            else
            {
                leftHandVectorFinal = leftHandVector;
            }

            if (Time.time - rightTime < armLerp)
            {
                rightHandVectorFinal = new Vector3(
                    Mathf.SmoothStep(rightHandVectorFinal.x, rightHandVector.x, rtt),
                    Mathf.SmoothStep(rightHandVectorFinal.y, rightHandVector.y, rtt),
                    Mathf.SmoothStep(rightHandVectorFinal.z, rightHandVector.z, rtt));
            }
            else
            {
                rightHandVectorFinal = rightHandVector;
            }
        }
        else
        {
            rightHandVector = rightArmTarget + transform.forward.normalized * 0.05f;
            leftHandVector = lefthandHeightTarget;

            if (Time.time - actionTime < windUpTime)
            {
                float at = (Time.time - actionTime) / windUpTime;
                torsoLerpTime = at;
                rightHandVectorFinal = new Vector3(
                    Mathf.SmoothStep(rightHandVectorFinal.x, rightHandVector.x, at),
                    Mathf.SmoothStep(rightHandVectorFinal.y, rightHandVector.y, at),
                    Mathf.SmoothStep(rightHandVectorFinal.z, rightHandVector.z, at));

                leftHandVectorFinal = new Vector3(
                    Mathf.SmoothStep(leftHandVectorFinal.x, leftHandVector.x, at),
                    Mathf.SmoothStep(leftHandVectorFinal.y, leftHandVector.y, at),
                    Mathf.SmoothStep(leftHandVectorFinal.z, leftHandVector.z, at));
            }
            else if (Time.time - actionTime > windUpTime && Time.time - actionTime < actTime + windUpTime)
            {
                float at = (Time.time - (actionTime + windUpTime)) / (actTime);
                torsoLerpTime = at;
                rightHandVectorFinal = new Vector3(
                    Mathf.SmoothStep(rightHandVectorFinal.x, rightHandVector.x, at),
                    Mathf.SmoothStep(rightHandVectorFinal.y, rightHandVector.y, at),
                    Mathf.SmoothStep(rightHandVectorFinal.z, rightHandVector.z, at));

                leftHandVectorFinal = new Vector3(
                    Mathf.SmoothStep(leftHandVectorFinal.x, leftHandVector.x, at),
                    Mathf.SmoothStep(leftHandVectorFinal.y, leftHandVector.y, at),
                    Mathf.SmoothStep(leftHandVectorFinal.z, leftHandVector.z, at));
            }

            else if (Time.time - actionTime > windUpTime + actTime && Time.time - actionTime < actTime + windUpTime + windDownTime)
            {
                float at = (Time.time - (actionTime + windUpTime + actTime)) / (windDownTime);
                torsoLerpTime = at;
                rightHandVectorFinal = new Vector3(
                    Mathf.SmoothStep(rightHandVectorFinal.x, rightHandVector.x, at),
                    Mathf.SmoothStep(rightHandVectorFinal.y, rightHandVector.y, at),
                    Mathf.SmoothStep(rightHandVectorFinal.z, rightHandVector.z, at));

                leftHandVectorFinal = new Vector3(
                    Mathf.SmoothStep(leftHandVectorFinal.x, leftHandVector.x, at),
                    Mathf.SmoothStep(leftHandVectorFinal.y, leftHandVector.y, at),
                    Mathf.SmoothStep(leftHandVectorFinal.z, leftHandVector.z, at));
            }
            else
            {
                leftHandVectorFinal = leftHandVector;
                rightHandVectorFinal = rightHandVector;
            }
        }

        leftHandTarget.position = leftHandVectorFinal;
        rightHandTarget.position = rightHandVectorFinal;
        rightHandTarget.rotation = rightArmRotation;

        //pelvis rotation
        Vector3 l = LeftLookRotation.position - transform.position;
        Vector3 r = RightLookRotation.position - transform.position;
        Vector3 la = LeftLookRotation.position - transform.position;
        Vector3 ra = RightLookRotation.position - transform.position;

        Vector3 tt = torsoTarget - transform.position;

        l = new Vector3(l.x, transform.forward.y, l.z);
        r = new Vector3(r.x, transform.forward.y, r.z);
        la = new Vector3(la.x, transform.forward.y, la.z);
        ra = new Vector3(ra.x, transform.forward.y, ra.z);

        Vector3 f = new Vector3();
        Vector3 u = new Vector3();

        //action
        if (actionInProgress == false)
        {
            //Torso Rotation
            if ((leftTime > rightTime
            && Vector3.Dot(transform.forward, entRigBody.velocity.normalized) >= 0.02f && entRigBody.velocity.magnitude > 0.1f)
            || (leftTime < rightTime
            && Vector3.Dot(transform.forward, entRigBody.velocity.normalized) < -0.02f && entRigBody.velocity.magnitude > 0.1f)
            )
            {
                f = l;
            }
            else if ((leftTime < rightTime
                && Vector3.Dot(transform.forward, entRigBody.velocity.normalized) >= 0.02f && entRigBody.velocity.magnitude > 0.1f)
                || (leftTime > rightTime
                && Vector3.Dot(transform.forward, entRigBody.velocity.normalized) < -0.02f && entRigBody.velocity.magnitude > 0.1f)
            )
            {
                f = r;
            }
            else
            {
                f = lastf;
            }

            if (entSpeed < 0.1f)
            {
                f = transform.forward;
            }

            torsoLerpTime = 0.25f;
        }
        else
        {
            f = tt;
            torsoLerpTime = (Time.time - actionTime) / (actTime + windDownTime + windUpTime);
        }


        //pelvis rotation
        if ((leftTime > rightTime
            && Vector3.Dot(transform.forward, entDirection) >= 0.02f && entSpeed > 0.1f)
            || (leftTime < rightTime
            && Vector3.Dot(transform.forward, entDirection) < -0.02f && entSpeed > 0.1f)
            )
        {
            u = ra;
        }
        else if ((leftTime < rightTime
            && Vector3.Dot(transform.forward, entDirection) >= 0.02f && entSpeed > 0.1f)
        || (leftTime > rightTime
        && Vector3.Dot(transform.forward, entDirection) < -0.02f && entSpeed > 0.1f))
        {
            u = la;
        }
        else
        {
            u = lastu;
        }

        if (entSpeed < 0.1f)
        {
            u = transform.forward;
        }

        lastf = f;
        lastu = u;

        float xrotation = 5f;

        if (sneaking == true)
        {
            xrotation = 15;

        }
        else if (sprinting == true)
        {
            xrotation = 10;
        }
        else
        {
            xrotation = 0;
        }

        Quaternion xrot = Quaternion.Euler(new Vector3(xrotation, transform.forward.y, 0));

        //pelvis rotation
        Quaternion yRotation = Quaternion.Slerp(entModel.transform.rotation, Quaternion.LookRotation(u), 0.25f);

        //torso rotation
        Quaternion mirrorQuaternion = Quaternion.Slerp(TorsoBone.rotation, Quaternion.LookRotation(f), torsoLerpTime);

        Quaternion leanRotation = Quaternion.Slerp(entModel.transform.rotation, xrot, 0.25f);

        //Vector3 yRotEulerCorrection = GetRelativeAngles(yRotation.eulerAngles);

        entModel.transform.rotation = Quaternion.Euler(new Vector3(leanRotation.eulerAngles.x, yRotation.eulerAngles.y, 0));

        TorsoBone.rotation = Quaternion.Euler(new Vector3(leanRotation.eulerAngles.x, mirrorQuaternion.eulerAngles.y, 0));
        HeadBone.rotation = transform.rotation;
    }
}
