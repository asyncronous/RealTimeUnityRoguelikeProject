using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Transactions;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public bool sneaking;
    public float sneakCheck;
    public bool sneakEnable;
    public bool sprinting;
    public float sprintCheck;
    public bool sprintEnable;
    public bool walking;
    public bool slowed;

    public bool inputLocked;

    public float verticalInput;
    public float horizontalInput;
    Vector3 moveInput;
    Vector3 entVelocity;
    Vector3 entDirection;
    public float entSpeed;
    public float maxSpeed;
    private float coeffDragValue;
    private float backwardMulti;
    private float alignmentSpeed;
    private float alignmentDamping;
    private float maxVel;
    public Rigidbody entRigBody;
    private float accelCoeff;

    public float baseMaxSpeed;
    public bool entIsPlayer;
    public bool isRagdoll;

    public EntityStats entityStats;
    public GameObject entRagdoll;
    private Collider[] entModelColliders;
    private Rigidbody[] entModelRigidB;
    private RigidbodyConstraints entConstraints;
    private float totalEntMass;
    private Vector3 entPos;
    public Vector3 entDesLookTar;
    public Vector3 projLookTar;
    public bool transitioned;
    public bool forced;

    //modelbones
    public GameObject entModel;

    //ragdoll
    private Transform[] RagdollChildren;
    private Transform[] ModelChildren;
    public List<Vector3> RagdollPos;
    public List<Quaternion> RagdollRot;

    private bool ragdollEnable;
    //private bool ragdoll;
    private float ragdollCheck;

    public bool sprintInput;
    public bool sneakInput;
    public bool ragdollInput;
    public bool leftClickInput;
    public bool rightClickInput;

    public Quaternion pubAngle;
    public Vector3 intendedMove;

    //physicsCollider
    public CapsuleCollider CollisionCollider;
    private float ColRadius;
    private float ColHeight;
    public CapsuleCollider ReceiveBox;
    public BoxCollider GiveBox;

    //damageTest
    public float damageToApply;

    //ModelControl
    public Transform DummyForward;

    //
    public Camera mainCam;
    public CustomOnHitShaker camShaker;
    public string alertLevel;
    public string traversalType;
    public bool deadSeen;

    public float lastOffset;
    public float sneakHeight;
    public float sprintHeight;
    public float walkHeight;
    public float raycastTargetDistance;
    public float raycastDistance;
    public bool lookOverride;

    // Start is called before the first frame update
    void Start()
    {
        entModelColliders = entRagdoll.GetComponentsInChildren<Collider>();
        entModelRigidB = entRagdoll.GetComponentsInChildren<Rigidbody>();
        foreach (Collider x in entModelColliders)
        {
            x.enabled = false;
        }
        totalEntMass = 0;
        foreach (Rigidbody x in entModelRigidB)
        {
            x.useGravity = false;
            totalEntMass += x.mass;
        }
        entRigBody = transform.GetComponent<Rigidbody>();
        entConstraints = entRigBody.constraints;
        accelCoeff = 0.01f;
        maxSpeed = baseMaxSpeed;

        coeffDragValue = -5;
        backwardMulti = -0.75f;

        alignmentSpeed = 15f;
        alignmentDamping = 90f;
        maxVel = 500;
        entRigBody.maxAngularVelocity = maxVel;

        totalEntMass = entRigBody.mass;

        RagdollChildren = entRagdoll.transform.GetComponentsInChildren<Transform>();
        ModelChildren = entModel.transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in RagdollChildren)
        {
            RagdollPos.Add(child.localPosition);
            RagdollRot.Add(child.localRotation);
        }

        ColRadius = CollisionCollider.radius;
        ColHeight = CollisionCollider.height;
    }

    void Update()
    {
        if (entIsPlayer == true)
        {
            verticalInput = Input.GetAxisRaw("Vertical");
            horizontalInput = Input.GetAxisRaw("Horizontal");
            sprintInput = Input.GetButton("Sprint");
            sneakInput = Input.GetButton("Sneak");
            ragdollInput = Input.GetButton("Ragdoll");
            leftClickInput = Input.GetButton("Fire1");
            rightClickInput = Input.GetButton("Fire2");

            //lookDirection
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Ray playerCameraRay = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Vector3 looktar = new Vector3();

            if (Physics.Raycast(playerCameraRay, out hit, 50, LayerMask.GetMask("WallLayer")))
            {
                looktar = hit.point;
            }

            if (lookOverride == false)
            {
                entDesLookTar = new Vector3(looktar.x, 0.85f, looktar.z);
                projLookTar = new Vector3(looktar.x, looktar.y + 0.85f, looktar.z);
            }

            //entDesLookTar = new Vector3(looktar.x, 0.85f, looktar.z);
        }

        //inputs should be checked in update, physics in fixedUpdate
        RagdollButton();
    }

    void HeightChanger()
    {
        lastOffset = 0;
        raycastTargetDistance = 0;
        Vector3 heightTarget;

        //heights for different stance
        if (walking == true)
        {
            //walk height
            lastOffset = 0.225f;
            //raycastTargetDistance = 0.725f;
            raycastTargetDistance = walkHeight;
        }
        else if (sprinting == true)
        {
            //sprint height
            lastOffset = 0.15f;
            //raycastTargetDistance = 0.65f;
            raycastTargetDistance = sprintHeight;
        }
        else if (sneaking == true)
        {
            //sneak height
            lastOffset = 0.1f;
            //raycastTargetDistance = 0.6f;
            raycastTargetDistance = sneakHeight;
        }

        RaycastHit hit;
        raycastDistance = 0;
        Vector3 raycastPos = new Vector3(0, 0, 0);

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 2, LayerMask.GetMask("WallLayer")))
        {
            raycastDistance = hit.distance;
            raycastPos = hit.point;
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }

        if (raycastDistance < raycastTargetDistance)
        {
            heightTarget = new Vector3(transform.position.x, raycastTargetDistance + raycastPos.y, transform.position.z);


            Vector3 direction = (heightTarget - transform.position);
            entRigBody.AddForce(15000 * direction - 900 * new Vector3(0, entRigBody.velocity.y, 0));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ragdoll/damage test
        if (damageToApply != 0)
        {
            isRagdoll = true;
            damageToApply = 0;
        }

        if (isRagdoll == false)
        {
            //for first frame
            if (transitioned == false)
            {
                //reset ragdoll
                DisableRagdoll();
                entRagdoll.SetActive(false);

                int x = 0;

                foreach (Transform child in ModelChildren)
                {
                    RagdollPos[x] = child.localPosition;
                    RagdollRot[x] = child.localRotation;
                    x++;
                }

                x = 0;
                entRagdoll.transform.localPosition = entModel.transform.localPosition;
                foreach (Transform child in RagdollChildren)
                {
                    child.localPosition = RagdollPos[x];
                    child.localRotation = RagdollRot[x];
                    x++;
                }

                entRigBody.constraints = RigidbodyConstraints.FreezeAll;
                entRigBody.angularVelocity = new Vector3(0, 0, 0);
                transform.rotation = Quaternion.Euler(0, pubAngle.eulerAngles.y, 0);
                //transform.rotation = pubAngle;
                transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);

                CollisionCollider.radius = ColRadius;
                CollisionCollider.height = ColHeight;

                transitioned = true;
                forced = false;
            }

            //move and look physics
            MovementUpdatePlayer();
            LookController();
            HeightChanger();

            //set constraints and weight to work correctly with movemenet physics
            entRigBody.constraints = entConstraints;
            entModel.SetActive(true);
            entRigBody.mass = totalEntMass;
        }
        else
        {
            //set constraints to none for correct ragdoll behaviour
            entRigBody.constraints = RigidbodyConstraints.None;

            //correct weight of parent rigidbody, disable main collider, disable model and enable ragdoll
            entRigBody.mass = 10;
            //CollisionCollider.enabled = false;
            CollisionCollider.radius = 0f;
            CollisionCollider.height = 0;
            entModel.SetActive(false);
            entRagdoll.SetActive(true);

            //for first frame
            if (forced == false)
            {
                int x = 0;

                foreach (Transform child in ModelChildren)
                {
                    RagdollPos[x] = child.localPosition;
                    RagdollRot[x] = child.localRotation;
                    x++;
                }

                x = 0;
                entRagdoll.transform.localPosition = entModel.transform.localPosition;
                foreach (Transform child in RagdollChildren)
                {
                    child.localPosition = RagdollPos[x];
                    child.localRotation = RagdollRot[x];
                    x++;
                }

                EnableRagdoll();

                entRigBody.AddForce(entRigBody.velocity * 10, ForceMode.Impulse);
                forced = true;
                transitioned = false;
            }
        }
    }

    void DisableRagdoll()
    {
        foreach (Collider x in entModelColliders)
        {
            x.enabled = false;
        }
        foreach (Rigidbody x in entModelRigidB)
        {
            x.useGravity = false;
            x.detectCollisions = false;
        }

        entRigBody.constraints = entConstraints;
    }
    void EnableRagdoll()
    {
        foreach (Collider x in entModelColliders)
        {
            x.enabled = true;
        }

        entRagdoll.transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        foreach (Rigidbody x in entModelRigidB)
        {
            x.useGravity = true;
            x.isKinematic = false;
            x.detectCollisions = true;
            x.velocity = new Vector3(0, 0, 0);
            x.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void RagdollButton()
    {
        //ragdoll state machine Debugger
        if (ragdollInput == false)
        {
            ragdollEnable = true;
        }
        if (isRagdoll == false)
        {
            if (ragdollInput == true && Time.time > ragdollCheck + 0.2f)
            {
                isRagdoll = true;
                ragdollCheck = Time.time;
                ragdollEnable = false;
            }
        }
        else if (isRagdoll == true && ragdollEnable == true)
        {
            if (ragdollInput == true && Time.time > ragdollCheck + 0.2f)
            {
                isRagdoll = false;
                ragdollCheck = Time.time;
                ragdollEnable = false;
            }
        }
    }
    void MovementUpdatePlayer()
    {        
        //sprint state machine
        if (sprintInput == false)
        {
            sneakEnable = true;
            sprintEnable = true;
        }
        if (sprinting == false && sprintEnable == true)
        {
            if (sprintInput == true && Time.time > sprintCheck + 0.2f)
            {
                sprinting = true;
                sneaking = false;
                sprintCheck = Time.time;
                sneakCheck = Time.time;
                sneakEnable = false;
                sprintEnable = false;
            }
        }
        else if (sprinting == true && sprintEnable == true)
        {
            if (sprintInput == true && Time.time > sprintCheck + 0.2f)
            {
                sprinting = false;
                sprintCheck = Time.time;
                sneakEnable = false;
                sprintEnable = false;
            }
        }

        //sneak state machine
        if (sneakInput == false)
        {
            sprintEnable = true;
            sneakEnable = true;
        }

        if (sneaking == false && sneakEnable == true)
        {
            if (sneakInput == true && Time.time > sneakCheck + 0.2f)
            {
                sneaking = true;
                sprinting = false;
                sneakCheck = Time.time;
                sprintCheck = Time.time;
                sneakEnable = false;
                sprintEnable = false;
            }
        }
        else if (sneaking == true && sneakEnable == true)
        {
            if (sneakInput == true && Time.time > sneakCheck + 0.2f)
            {
                sneaking = false;
                sneakCheck = Time.time;
                sneakEnable = false;
                sprintEnable = false;
            }
        }

        //walking state machine
        if (sneaking == false && sprinting == false)
        {
            walking = true;
        }
        else if (sneaking == true || sprinting == true)
        {
            walking = false;
        }

        //check if slowed
        if (slowed == true)
        {
            sprinting = false;
            sneaking = false;
            walking = false;
        }

        //maxSpeed
        if (sprinting == true)
        {
            maxSpeed = baseMaxSpeed;
            coeffDragValue = -5;
            accelCoeff = 0.01f;
            backwardMulti = -3f;
        }
        if (walking == true)
        {
            maxSpeed = baseMaxSpeed / 2f;
            coeffDragValue = -6.5f;
            accelCoeff = 0.00001f;
            backwardMulti = -0.30f;
        }
        if (sneaking == true || slowed == true)
        {
            maxSpeed = baseMaxSpeed / 3f;
            coeffDragValue = -7f;
            accelCoeff = 0.00001f;
            backwardMulti = -0.1f;
        }

        intendedMove = new Vector3(horizontalInput, 0, verticalInput);

        //input locker
        if (inputLocked == false)
        {
            moveInput = new Vector3(horizontalInput, 0, verticalInput);
        }
        else
        {
            moveInput = new Vector3(0, 0, 0);
        }

        entVelocity = entRigBody.velocity;
        entDirection = entVelocity.normalized;
        entSpeed = entVelocity.magnitude;

        //next accel
        float entDsrAccel = ((-maxSpeed * Mathf.Pow(accelCoeff, Time.fixedDeltaTime + (Mathf.Log(-((entSpeed - maxSpeed) / maxSpeed)) / Mathf.Log(accelCoeff))) + maxSpeed) - entSpeed) / Time.fixedDeltaTime;

        //drag calc
        Vector3 entDragx = new Vector3(entSpeed * Mathf.Cos(Vector3.Angle(entDirection, new Vector3(1, 0, 0)) * Mathf.Deg2Rad), 0, 0);
        Vector3 entDragz = new Vector3(0, 0, entSpeed * Mathf.Cos(Vector3.Angle(entDirection, new Vector3(0, 0, 1)) * Mathf.Deg2Rad));

        //add speed
        if (entSpeed < maxSpeed)
        {
            entRigBody.AddForce(moveInput.normalized * (totalEntMass * entDsrAccel));
        }

        //if sneaking or not moving in that axis, add horizontal drag
        if (horizontalInput == 0 || entSpeed > maxSpeed)
        {
            entRigBody.AddForce(coeffDragValue * entDragx * (1 / (Time.fixedDeltaTime / Time.timeScale)));
        }
        //if sneaking, or not moving in that axis, add vertical drag
        if (verticalInput == 0 || entSpeed > maxSpeed)
        {
            entRigBody.AddForce(coeffDragValue * entDragz * (1 / (Time.fixedDeltaTime / Time.timeScale)));
        }

        //if attempting to move in opposite direction to current movement direction, or change direction while sprinting, apply drag
        if (Vector3.Dot(moveInput, entDirection) < 0 || (Vector3.Dot(moveInput.normalized, entDirection) < 0.95) || entSpeed > maxSpeed)
        {

            entRigBody.AddForce(coeffDragValue * entDragz * (1 / (Time.fixedDeltaTime / Time.timeScale)));
            entRigBody.AddForce(coeffDragValue * entDragx * (1 / (Time.fixedDeltaTime / Time.timeScale)));
        }

        //if walking in the opposite direction you're facing, walk slower
        if (Vector3.Dot(transform.forward, entDirection) < 0.9 )
        {
            entRigBody.AddForce(-(Vector3.Dot(transform.forward, entDirection) - 1) * backwardMulti * entDragz * (1 / (Time.fixedDeltaTime / Time.timeScale)));
            entRigBody.AddForce(-(Vector3.Dot(transform.forward, entDirection) - 1) * backwardMulti * entDragx * (1 / (Time.fixedDeltaTime / Time.timeScale)));
        }
    }
    void LookController()
    {
        entPos = entRigBody.position;

        Quaternion cPR = entRigBody.rotation;
        Vector3 cPD = (transform.forward);
        Vector3 dPD = (entDesLookTar - entPos);

        //rotation required
        Quaternion targetRotation = Quaternion.LookRotation(dPD, cPD); ; // The target rotation can be replaced with whatever rotation you want to align to
        Quaternion deltaRotation = Quaternion.Inverse(cPR) * targetRotation;
        Vector3 deltaAngles = GetRelativeAngles(deltaRotation.eulerAngles);

        //new Vector3(0, deltaAngles.y, 0);
        Vector3 worldDeltaAngles = transform.TransformDirection(new Vector3(0, deltaAngles.y, 0));

        // alignmentSpeed controls how fast you rotate the body towards the target rotation
        // alignmentDamping prevents overshooting the target rotation
        // Values used: alignmentSpeed = 0.025, alignmentDamping = 0.2
        entRigBody.AddTorque(alignmentSpeed * worldDeltaAngles - alignmentDamping * entRigBody.angularVelocity);
        pubAngle = targetRotation;
    }
    Vector3 GetRelativeAngles(Vector3 angles)
    {
        Vector3 relativeAngles = angles;
        if (relativeAngles.x > 180f)
            relativeAngles.x -= 360f;
        if (relativeAngles.y > 180f)
            relativeAngles.y -= 360f;
        if (relativeAngles.z > 180f)
            relativeAngles.z -= 360f;
        return relativeAngles;
    }
}
