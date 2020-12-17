using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AraachModelController : MonoBehaviour
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

    //modeltargets
    public Transform rightForeTarget;
    public Transform leftForeTarget;
    public Transform rightBackTarget;
    public Transform leftBackTarget;


    //proceduralMovementTargets and Vectors
    private Vector3 heightTarget;
    //private float armOffset;

    private Vector3 rightForeHeightTarget;
    private Vector3 leftForeHeightTarget;
    private Vector3 rightBackHeightTarget;
    private Vector3 leftBackHeightTarget;

    private Vector3 rightForeVector;
    private Vector3 leftForeVector;
    private Vector3 rightBackVector;
    private Vector3 leftBackVector;

    private Vector3 rightHandVectorFinal;
    private Vector3 leftHandVectorFinal;

    public Transform rightForeBone;
    public Transform leftForeBone;
    public Transform rightBackBone;
    public Transform leftBackBone;
    //public Transform LeftLookRotation;
    //public Transform RightLookRotation;
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
        if (isRagdoll == false)
        {
            ModelControl();
        }
    }

    public void ModelControl()
    {
        //heights for different stance
        if (walking == true)
        {
            rightForeHeightTarget = new Vector3(0, -0.1f, 0) + rightForeBone.position;
            leftForeHeightTarget = new Vector3(0, -0.1f, 0) + leftForeBone.position;
            rightBackHeightTarget = new Vector3(0, -0.1f, 0) + rightBackBone.position;
            leftBackHeightTarget = new Vector3(0, -0.1f, 0) + leftBackBone.position;
        }
        else if (sprinting == true)
        {

            rightForeHeightTarget = new Vector3(0, -0.1f, 0) + rightForeBone.position;
            leftForeHeightTarget = new Vector3(0, -0.1f, 0) + leftForeBone.position;
            rightBackHeightTarget = new Vector3(0, -0.1f, 0) + rightBackBone.position;
            leftBackHeightTarget = new Vector3(0, -0.1f, 0) + leftBackBone.position;
        }
        else if (sneaking == true)
        {
            rightForeHeightTarget = new Vector3(0, -0.1f, 0) + rightForeBone.position;
            leftForeHeightTarget = new Vector3(0, -0.1f, 0) + leftForeBone.position;
            rightBackHeightTarget = new Vector3(0, -0.1f, 0) + rightBackBone.position;
            leftBackHeightTarget = new Vector3(0, -0.1f, 0) + leftBackBone.position;
        }

        if (entSpeed < 0.1f)
        {
            idle = true;
            movementOffset = 0.025f;
            lerpTime = 0.025f;
        }

        if (entSpeed < 0.5f)
        {
            movementOffset = 0.25f;
            lerpTime = 0.025f;
        }

        //more than 0.1f x or z axis
        if (entSpeed > 0.5f && (entVelocity.x > 0.5f || entVelocity.x < -0.5f || entVelocity.z > 0.5f || entVelocity.z < -0.5f))
        {
            //forward and backwards
            if (Vector3.Dot(transform.forward, entDirection) > 0.75 || Vector3.Dot(transform.forward, entDirection) < -0.75)
            {
                if (walking == true)
                {
                    float offset = 0.35f;

                    lerpTime = 0.01f;
                    movementOffset = offset / 2;
                    rightForeHeightTarget += entRigBody.velocity.normalized * offset;
                    leftForeHeightTarget += entRigBody.velocity.normalized * offset;
                    rightBackHeightTarget += entRigBody.velocity.normalized * offset;
                    leftBackHeightTarget += entRigBody.velocity.normalized * offset;
                }

                if (sprinting == true)
                {
                    float offset = 0.35f;

                    lerpTime = 0.01f;
                    movementOffset = offset / 2.5f;
                    rightForeHeightTarget += entRigBody.velocity.normalized * offset;
                    leftForeHeightTarget += entRigBody.velocity.normalized * offset;
                    rightBackHeightTarget += entRigBody.velocity.normalized * offset;
                    leftBackHeightTarget += entRigBody.velocity.normalized * offset;
                }

                if (sneaking == true)
                {
                    float offset = 0.35f;

                    lerpTime = 0.01f;
                    movementOffset = offset / 2;
                    rightForeHeightTarget += entRigBody.velocity.normalized * offset;
                    leftForeHeightTarget += entRigBody.velocity.normalized * offset;
                    rightBackHeightTarget += entRigBody.velocity.normalized * offset;
                    leftBackHeightTarget += entRigBody.velocity.normalized * offset;
                }
            }

            //strafing
            if (Vector3.Dot(transform.forward, entDirection) < 0.75 && Vector3.Dot(transform.forward, entDirection) > -0.75)
            {
                if (walking == true)
                {
                    float offset = 0.325f * 0.8f;

                    lerpTime = 0.01f;
                    movementOffset = offset / 2;
                    rightForeHeightTarget += entRigBody.velocity.normalized * offset;
                    leftForeHeightTarget += entRigBody.velocity.normalized * offset;
                    rightBackHeightTarget += entRigBody.velocity.normalized * offset;
                    leftBackHeightTarget += entRigBody.velocity.normalized * offset;
                }

                if (sprinting == true)
                {
                    float offset = 0.4f;

                    lerpTime = 0.01f;
                    movementOffset = offset / 2;
                    rightForeHeightTarget += entRigBody.velocity.normalized * offset;
                    leftForeHeightTarget += entRigBody.velocity.normalized * offset;
                    rightBackHeightTarget += entRigBody.velocity.normalized * offset;
                    leftBackHeightTarget += entRigBody.velocity.normalized * offset;
                }

                if (sneaking == true)
                {
                    float offset = 0.35f * 0.8f;

                    lerpTime = 0.01f;
                    movementOffset = offset / 2;
                    rightForeHeightTarget += entRigBody.velocity.normalized * offset;
                    leftForeHeightTarget += entRigBody.velocity.normalized * offset;
                    rightBackHeightTarget += entRigBody.velocity.normalized * offset;
                    leftBackHeightTarget += entRigBody.velocity.normalized * offset;
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
                    rightForeVector = new Vector3(
                        Mathf.SmoothStep(rightForeVector.x, rightForeHeightTarget.x, lt),
                        Mathf.SmoothStep(rightForeVector.y, rightForeHeightTarget.y, lt),
                        Mathf.SmoothStep(rightForeVector.z, rightForeHeightTarget.z, lt));
                }
                else if (Time.time - rightTime > lerpTime)
                {
                    rightForeVector = new Vector3(
                        rightForeHeightTarget.x,
                        rightForeHeightTarget.y,
                        rightForeHeightTarget.z);

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
                    leftForeVector = new Vector3(
                        Mathf.SmoothStep(leftForeVector.x, leftForeHeightTarget.x, lt),
                        Mathf.SmoothStep(leftForeVector.y, leftForeHeightTarget.y, lt),
                        Mathf.SmoothStep(leftForeVector.z, leftForeHeightTarget.z, lt));
                }
                else if (Time.time - leftTime > lerpTime)
                {
                    leftForeVector = new Vector3(
                        leftForeHeightTarget.x,
                        leftForeHeightTarget.y,
                        leftForeHeightTarget.z);

                    nextLeg = "rightBack";
                    lastMoveVector = transform.position;
                    idle = false;
                    lerping = false;
                }
            }
            else if (nextLeg == "rightBack")
            {
                if (lerping == false)
                {
                    rightTime = Time.time;
                    lerping = true;
                }

                float lt = (Time.time - rightTime) / lerpTime;

                if (Time.time - rightTime < lerpTime)
                {
                    rightBackVector = new Vector3(
                        Mathf.SmoothStep(rightBackVector.x, rightBackHeightTarget.x, lt),
                        Mathf.SmoothStep(rightBackVector.y, rightBackHeightTarget.y, lt),
                        Mathf.SmoothStep(rightBackVector.z, rightBackHeightTarget.z, lt));
                }
                else if (Time.time - rightTime > lerpTime)
                {
                    rightBackVector = new Vector3(
                        rightBackHeightTarget.x,
                        rightBackHeightTarget.y,
                        rightBackHeightTarget.z);

                    nextLeg = "leftBack";
                    lastMoveVector = transform.position;
                    idle = false;
                    lerping = false;
                }
            }
            else if (nextLeg == "leftBack")
            {
                if (lerping == false)
                {
                    leftTime = Time.time;
                    lerping = true;
                }

                float lt = (Time.time - leftTime) / lerpTime;

                if (Time.time - leftTime < lerpTime)
                {
                    leftBackVector = new Vector3(
                        Mathf.SmoothStep(leftBackVector.x, leftBackHeightTarget.x, lt),
                        Mathf.SmoothStep(leftBackVector.y, leftBackHeightTarget.y, lt),
                        Mathf.SmoothStep(leftBackVector.z, leftBackHeightTarget.z, lt));
                }
                else if (Time.time - leftTime > lerpTime)
                {
                    leftBackVector = new Vector3(
                        leftBackHeightTarget.x,
                        leftBackHeightTarget.y,
                        leftBackHeightTarget.z);

                    nextLeg = "right";
                    lastMoveVector = transform.position;
                    idle = false;
                    lerping = false;
                }
            }
        }

        rightForeTarget.position = rightForeVector;
        leftForeTarget.position = leftForeVector;
        rightBackTarget.position = rightBackVector;
        leftBackTarget.position = leftBackVector;

        //if (actionInProgress == false)
        //{
        //    if (entSpeed > 0.5f && Vector3.Dot(transform.forward, entDirection) >= 0)
        //    {
        //        if (leftTime > rightTime)
        //        {
        //            leftHandVector = lefthandHeightTarget + -transform.forward.normalized * armOffset;
        //            rightHandVector = righthandHeightTarget + transform.forward.normalized * armOffset;
        //        }
        //        else if (leftTime < rightTime)
        //        {
        //            leftHandVector = lefthandHeightTarget + transform.forward.normalized * armOffset;
        //            rightHandVector = righthandHeightTarget + -transform.forward.normalized * armOffset;
        //        }
        //    }
        //    else if (entSpeed > 0.5f && Vector3.Dot(transform.forward, entDirection) < 0)
        //    {
        //        if (leftTime > rightTime)
        //        {
        //            leftHandVector = lefthandHeightTarget + transform.forward.normalized * armOffset;
        //            rightHandVector = righthandHeightTarget + -transform.forward.normalized * armOffset;
        //        }
        //        else if (leftTime < rightTime)
        //        {
        //            leftHandVector = lefthandHeightTarget + -transform.forward.normalized * armOffset;
        //            rightHandVector = righthandHeightTarget + transform.forward.normalized * armOffset;
        //        }
        //    }
        //    else
        //    {
        //        leftHandVector = lefthandHeightTarget;
        //        rightHandVector = righthandHeightTarget;
        //    }

        //    float armLerp = lerpTime * 1.5f;
        //    float ltt = (Time.time - leftTime) / armLerp;
        //    float rtt = (Time.time - rightTime) / armLerp;

        //    if (Time.time - leftTime < armLerp)
        //    {
        //        leftHandVectorFinal = new Vector3(
        //            Mathf.SmoothStep(leftHandVectorFinal.x, leftHandVector.x, ltt),
        //            Mathf.SmoothStep(leftHandVectorFinal.y, leftHandVector.y, ltt),
        //            Mathf.SmoothStep(leftHandVectorFinal.z, leftHandVector.z, ltt));
        //    }
        //    else
        //    {
        //        leftHandVectorFinal = leftHandVector;
        //    }

        //    if (Time.time - rightTime < armLerp)
        //    {
        //        rightHandVectorFinal = new Vector3(
        //            Mathf.SmoothStep(rightHandVectorFinal.x, rightHandVector.x, rtt),
        //            Mathf.SmoothStep(rightHandVectorFinal.y, rightHandVector.y, rtt),
        //            Mathf.SmoothStep(rightHandVectorFinal.z, rightHandVector.z, rtt));
        //    }
        //    else
        //    {
        //        rightHandVectorFinal = rightHandVector;
        //    }
        //}
        //else
        //{
        //    rightHandVector = rightArmTarget + transform.forward.normalized * 0.05f;
        //    leftHandVector = lefthandHeightTarget;

        //    if (Time.time - actionTime < windUpTime)
        //    {
        //        float at = (Time.time - actionTime) / windUpTime;
        //        torsoLerpTime = at;
        //        rightHandVectorFinal = new Vector3(
        //            Mathf.SmoothStep(rightHandVectorFinal.x, rightHandVector.x, at),
        //            Mathf.SmoothStep(rightHandVectorFinal.y, rightHandVector.y, at),
        //            Mathf.SmoothStep(rightHandVectorFinal.z, rightHandVector.z, at));

        //        leftHandVectorFinal = new Vector3(
        //            Mathf.SmoothStep(leftHandVectorFinal.x, leftHandVector.x, at),
        //            Mathf.SmoothStep(leftHandVectorFinal.y, leftHandVector.y, at),
        //            Mathf.SmoothStep(leftHandVectorFinal.z, leftHandVector.z, at));
        //    }
        //    else if (Time.time - actionTime > windUpTime && Time.time - actionTime < actTime + windUpTime)
        //    {
        //        float at = (Time.time - (actionTime + windUpTime)) / (actTime);
        //        torsoLerpTime = at;
        //        rightHandVectorFinal = new Vector3(
        //            Mathf.SmoothStep(rightHandVectorFinal.x, rightHandVector.x, at),
        //            Mathf.SmoothStep(rightHandVectorFinal.y, rightHandVector.y, at),
        //            Mathf.SmoothStep(rightHandVectorFinal.z, rightHandVector.z, at));

        //        leftHandVectorFinal = new Vector3(
        //            Mathf.SmoothStep(leftHandVectorFinal.x, leftHandVector.x, at),
        //            Mathf.SmoothStep(leftHandVectorFinal.y, leftHandVector.y, at),
        //            Mathf.SmoothStep(leftHandVectorFinal.z, leftHandVector.z, at));
        //    }

        //    else if (Time.time - actionTime > windUpTime + actTime && Time.time - actionTime < actTime + windUpTime + windDownTime)
        //    {
        //        float at = (Time.time - (actionTime + windUpTime + actTime)) / (windDownTime);
        //        torsoLerpTime = at;
        //        rightHandVectorFinal = new Vector3(
        //            Mathf.SmoothStep(rightHandVectorFinal.x, rightHandVector.x, at),
        //            Mathf.SmoothStep(rightHandVectorFinal.y, rightHandVector.y, at),
        //            Mathf.SmoothStep(rightHandVectorFinal.z, rightHandVector.z, at));

        //        leftHandVectorFinal = new Vector3(
        //            Mathf.SmoothStep(leftHandVectorFinal.x, leftHandVector.x, at),
        //            Mathf.SmoothStep(leftHandVectorFinal.y, leftHandVector.y, at),
        //            Mathf.SmoothStep(leftHandVectorFinal.z, leftHandVector.z, at));
        //    }
        //    else
        //    {
        //        leftHandVectorFinal = leftHandVector;
        //        rightHandVectorFinal = rightHandVector;
        //    }
        //}

        //leftHandTarget.position = leftHandVectorFinal;
        //rightHandTarget.position = rightHandVectorFinal;
        //rightHandTarget.rotation = rightArmRotation;

        //pelvis rotation
        //Vector3 l = LeftLookRotation.position - transform.position;
        //Vector3 r = RightLookRotation.position - transform.position;
        //Vector3 la = LeftLookRotation.position - transform.position;
        //Vector3 ra = RightLookRotation.position - transform.position;

        //Vector3 tt = torsoTarget - transform.position;

        //l = new Vector3(l.x, transform.forward.y, l.z);
        //r = new Vector3(r.x, transform.forward.y, r.z);
        //la = new Vector3(la.x, transform.forward.y, la.z);
        //ra = new Vector3(ra.x, transform.forward.y, ra.z);

        //Vector3 f = new Vector3();
        //Vector3 u = new Vector3();

        ////action
        //if (actionInProgress == false)
        //{
        //    //Torso Rotation
        //    if ((leftTime > rightTime
        //    && Vector3.Dot(transform.forward, entRigBody.velocity.normalized) >= 0.02f && entRigBody.velocity.magnitude > 0.1f)
        //    || (leftTime < rightTime
        //    && Vector3.Dot(transform.forward, entRigBody.velocity.normalized) < -0.02f && entRigBody.velocity.magnitude > 0.1f)
        //    )
        //    {
        //        f = l;
        //    }
        //    else if ((leftTime < rightTime
        //        && Vector3.Dot(transform.forward, entRigBody.velocity.normalized) >= 0.02f && entRigBody.velocity.magnitude > 0.1f)
        //        || (leftTime > rightTime
        //        && Vector3.Dot(transform.forward, entRigBody.velocity.normalized) < -0.02f && entRigBody.velocity.magnitude > 0.1f)
        //    )
        //    {
        //        f = r;
        //    }
        //    else
        //    {
        //        f = lastf;
        //    }

        //    if (entSpeed < 0.1f)
        //    {
        //        f = transform.forward;
        //    }

        //    torsoLerpTime = 0.25f;
        //}
        //else
        //{
        //    f = tt;
        //    torsoLerpTime = (Time.time - actionTime) / (actTime + windDownTime + windUpTime);
        //}


        ////pelvis rotation
        //if ((leftTime > rightTime
        //    && Vector3.Dot(transform.forward, entDirection) >= 0.02f && entSpeed > 0.1f)
        //    || (leftTime < rightTime
        //    && Vector3.Dot(transform.forward, entDirection) < -0.02f && entSpeed > 0.1f)
        //    )
        //{
        //    u = ra;
        //}
        //else if ((leftTime < rightTime
        //    && Vector3.Dot(transform.forward, entDirection) >= 0.02f && entSpeed > 0.1f)
        //|| (leftTime > rightTime
        //&& Vector3.Dot(transform.forward, entDirection) < -0.02f && entSpeed > 0.1f))
        //{
        //    u = la;
        //}
        //else
        //{
        //    u = lastu;
        //}

        //if (entSpeed < 0.1f)
        //{
        //    u = transform.forward;
        //}

        //lastf = f;
        //lastu = u;

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
        //Quaternion yRotation = Quaternion.Slerp(entModel.transform.rotation, Quaternion.LookRotation(transform.forward), 0.25f);

        //torso rotation
        Quaternion mirrorQuaternion = Quaternion.Slerp(TorsoBone.rotation, Quaternion.LookRotation(transform.forward), torsoLerpTime);

        Quaternion leanRotation = Quaternion.Slerp(entModel.transform.rotation, xrot, 0.25f);

        //Vector3 yRotEulerCorrection = GetRelativeAngles(yRotation.eulerAngles);

        //entModel.transform.rotation = Quaternion.Euler(new Vector3(leanRotation.eulerAngles.x, yRotation.eulerAngles.y, 0));

        TorsoBone.rotation = Quaternion.Euler(new Vector3(leanRotation.eulerAngles.x, mirrorQuaternion.eulerAngles.y, 0));
        //HeadBone.rotation = transform.rotation;
    }
}
