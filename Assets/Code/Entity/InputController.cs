using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public Pathfinder pathfinder;
    public EntityController entController;
    public EntityStats entStats;
    public List<Vector3> currPath;
    public bool requestingPath = false;

    float vertInput;
    float horzInput;
    private bool ragdollInput;
    private bool leftClickInput;
    private bool rightClickInput;
    private bool sprinting;
    private bool sneaking;
    private bool walking;

    List<Transform> enemiesNotVisible;
    List<Transform> enemiesVisibleIndirect;
    List<Transform> enemiesVisibleDirect;

    List<Transform> loudNoiseRadius;
    List<Transform> curiousNoiseRadius;
    List<Transform> distressedAllyRadius;
    List<Transform> deadAllyLineOfSight;

    public Vector3 moveTarget;
    public Vector3 lookTarget;
    string actionState;


    string faction;
    private int minRangeForAlert;
    private int mediumRangeForAlert;
    private int maxRangeForAlert;
    int maxAlertPoints;
    string aiStyle;
    public int alertPoints;
    public string alertLevel;
    private float lastAlertPoint;

    //start as patrolling
    //detect anything go alert
    //if alert < 100 go fight(direct seen enemy) or search(indirect) or hunt(dead body, loud noise, lose line of sight, ally fighting)
    //if in fight store token for hunt
    //if lose line of sight go hunting
    //if end of hunting go investigate
    //once run out of currpath go patrolling

    //patrolling
    //alerting
    //searching
    //fighting
    //distressed

    public Transform Test;
    public string travType;

    // Start is called before the first frame update
    void Start()
    {
        enemiesNotVisible = new List<Transform>();
        enemiesVisibleDirect = new List<Transform>();
        enemiesVisibleIndirect = new List<Transform>();

        loudNoiseRadius = new List<Transform>();
        curiousNoiseRadius = new List<Transform>();
        distressedAllyRadius = new List<Transform>();
        deadAllyLineOfSight = new List<Transform>();

        maxAlertPoints = 500;
        alertPoints = maxAlertPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if(entController.isRagdoll == false)
        {
            //get path from this position to target position

            //get from stats
            //faction = entStats.faction;
            //max speed
            //max alertPoints
            //aiStyle
            minRangeForAlert = 1;
            mediumRangeForAlert = 3;
            maxRangeForAlert = 7;

            //get enemies in radius
            enemiesInRadius(minRangeForAlert, mediumRangeForAlert, maxRangeForAlert);

            //run direct behaviour
            DirectBehaviour();
            AlertPointMachine();

            //set
            //actionState = ;
            //moveTarget = ;
            //lookTarget = ;

            //determines

            //leftClickInput
            //rightClickInput


            //apply inputs
            entController.verticalInput = vertInput;
            entController.horizontalInput = horzInput;
            entController.entDesLookTar = lookTarget;

            entController.leftClickInput = leftClickInput;
            //entController.rightClickInput = rightClickInput;

            entController.sprinting = sprinting;
            //entController.sneaking = sneaking;
            entController.walking = walking;

        }

        entController.alertLevel = alertLevel;
    }

    void DirectBehaviour()
    {
        //decrement alert points if enemies detected
        if (enemiesNotVisible.Count > 0 || enemiesVisibleDirect.Count > 0 || enemiesVisibleIndirect.Count > 0 || distressedAllyRadius.Count > 0 || deadAllyLineOfSight.Count > 0)
        {
            if(alertLevel != "hunting")
            {
                alertLevel = "alerting";
            }

            if (alertPoints < 100)
            {
                //get closest enemy transform
                Transform closestTransform = null;
                float smallestDistance = maxRangeForAlert;
                string traversalType = null;

                if (enemiesVisibleDirect.Count > 0)
                {
                    foreach (Transform trans in enemiesVisibleDirect)
                    {
                        float distance = Vector3.Distance(trans.position, transform.position);

                        if (distance < smallestDistance)
                        {
                            traversalType = "direct";
                            alertLevel = "fighting";
                            closestTransform = trans;
                        }
                    }
                }

                if (enemiesVisibleIndirect.Count > 0)
                {
                    foreach (Transform trans in enemiesVisibleIndirect)
                    {
                        float distance = Vector3.Distance(trans.position, transform.position);

                        if (distance < smallestDistance)
                        {
                            traversalType = "indirectVisible";
                            alertLevel = "fighting";
                            closestTransform = trans;
                        }
                    }
                }

                if (enemiesNotVisible.Count > 0)
                {
                    foreach (Transform trans in enemiesNotVisible)
                    {
                        float distance = Vector3.Distance(trans.position, transform.position);

                        if (distance < smallestDistance)
                        {
                            traversalType = "indirect";

                            //only if not hunting do normal search
                            if(alertLevel != "hunting")
                            {
                                alertLevel = "searching";
                            }

                            closestTransform = trans;
                            //return closestTransform;
                        }
                    }
                }

                if (distressedAllyRadius.Count > 0)
                {
                    foreach (Transform trans in distressedAllyRadius)
                    {
                        float distance = Vector3.Distance(trans.position, transform.position);

                        if (distance < smallestDistance)
                        {
                            traversalType = "indirect";

                            alertLevel = "hunting";

                            closestTransform = trans;
                            //return closestTransform;
                        }
                    }
                }

                if (deadAllyLineOfSight.Count > 0)
                {
                    foreach (Transform trans in deadAllyLineOfSight)
                    {
                        float distance = Vector3.Distance(trans.position, transform.position);

                        if (distance < smallestDistance)
                        {
                            traversalType = "indirect";

                            alertLevel = "hunting";

                            closestTransform = trans;
                            //return closestTransform;
                        }
                    }
                }

                if (traversalType == "direct")
                {
                    sprinting = true;
                    walking = false;

                    moveTarget = closestTransform.position;
                    moveToTarget(moveTarget);

                    lookTarget = closestTransform.position;

                    if (Vector3.Distance(lookTarget, transform.position) < 1f)
                    {
                        leftClickInput = true;
                    }
                    else
                    {
                        leftClickInput = false;
                    }

                    //get path incase lose line of sight
                    //request path from pathfinder
                    if (requestingPath == false)
                    {
                        pathfinder.PathfindQueue.Add(new PathRequest(transform, this, transform.position, closestTransform.position));
                        requestingPath = true;
                    }
                }
                else if (traversalType == "indirectVisible")
                {
                    sprinting = true;
                    walking = false;

                    //request path from pathfinder
                    if (requestingPath == false)
                    {
                        pathfinder.PathfindQueue.Add(new PathRequest(transform, this, transform.position, closestTransform.position));
                        requestingPath = true;
                    }

                    //if currentpath list is not empty
                    if (currPath.Count > 0)
                    {
                        //assign movetarget the first first vector in currpath
                        moveTarget = currPath.First();

                        //determine vertical/horizontal input
                        moveToTarget(moveTarget);

                        //if transform has reached within 0.2f of the movetarget, remove the move target
                        if (Vector3.Distance(moveTarget, transform.position) < 0.2f)
                        {
                            currPath.Remove(moveTarget);
                        }
                    }

                    else
                    {
                        vertInput = 0;
                        horzInput = 0;
                    }

                    //look at enemy
                    lookTarget = closestTransform.position;
                }
                else if (traversalType == "indirect")
                {
                    //only sprint if hunting indirect
                    if (alertLevel == "hunting")
                    {
                        sprinting = true;
                        walking = false;
                    }
                    else
                    {
                        sprinting = false;
                        walking = true;
                    }

                    //request path from pathfinder
                    if (requestingPath == false)
                    {
                        pathfinder.PathfindQueue.Add(new PathRequest(transform, this, transform.position, closestTransform.position));
                        requestingPath = true;
                    }

                    //if currentpath list is not empty
                    if (currPath.Count > 0)
                    {
                        //assign movetarget the first first vector in currpath
                        moveTarget = currPath.First();
                        lookTarget = moveTarget;

                        //determine vertical/horizontal input
                        moveToTarget(moveTarget);

                        //if transform has reached within 0.2f of the movetarget, remove the move target
                        if (Vector3.Distance(moveTarget, transform.position) < 0.3f)
                        {
                            currPath.Remove(moveTarget);
                        }
                    }

                    else
                    {
                        vertInput = 0;
                        horzInput = 0;

                        lookTarget = entController.DummyForward.position;
                    }
                }
                else
                {
                    //sprinting = false;
                    //walking = true;

                    //if currentpath list is not empty
                    if (currPath.Count > 0)
                    {
                        //assign movetarget the first first vector in currpath
                        moveTarget = currPath.First();
                        lookTarget = moveTarget;

                        //determine vertical/horizontal input
                        moveToTarget(moveTarget);

                        //if transform has reached within 0.2f of the movetarget, remove the move target
                        if (Vector3.Distance(moveTarget, transform.position) < 0.3f)
                        {
                            currPath.Remove(moveTarget);
                        }
                    }

                    vertInput = 0;
                    horzInput = 0;
                }

                Test = closestTransform;
                travType = traversalType;
                entController.traversalType = travType;
            }
        }

        else
        {            
            //if no enemies but currentpath list is not empty
            if (currPath.Count > 0)
            {
                //assign movetarget the first vector in currpath
                moveTarget = currPath.First();
                lookTarget = moveTarget;

                //determine vertical/horizontal input
                moveToTarget(moveTarget);

                //if transform has reached within 0.2f of the movetarget, remove the move target
                if (Vector3.Distance(moveTarget, transform.position) < 0.3f)
                {
                    currPath.Remove(moveTarget);
                }
            }

            //lost target, reached end of path
            else
            {
                sprinting = false;
                walking = true;

                vertInput = 0;
                horzInput = 0;

                //if completely idle, check for next action
                if (requestingPath == false)
                {
                    //if was hunting before lost trail
                    if (alertLevel == "hunting")
                    {
                        alertLevel = "investigating";
                    }
                    else if (alertLevel != "thinking")
                    {
                        lookTarget = entController.DummyForward.position;
                        alertLevel = "patrolling";
                    }
                }
            }
        }

        //overwrite as hunting every pass in case lose line of sight
        if(alertLevel == "fighting")
        {
            alertLevel = "hunting";
        }

        //if triggered investigating, begin investigate loop
        if(alertLevel == "investigating")
        {
            StartCoroutine(Investigate());
        }
    }

    void AlertPointMachine()
    {
        if (Time.time - lastAlertPoint > 0.01f)
        {
            if (alertLevel == "patrolling")
            {
                alertPoints += 1;
                lastAlertPoint = Time.time;

                if (alertPoints > maxAlertPoints)
                {
                    alertPoints = maxAlertPoints;
                }
            }

            else if (alertLevel == "onEdge")
            {
                alertPoints += 1;
                lastAlertPoint = Time.time;

                if (alertPoints > maxAlertPoints / 2)
                {
                    alertPoints = maxAlertPoints / 2;
                }
            }

            else
            {
                alertPoints -= 5;
                lastAlertPoint = Time.time;

                if (alertPoints < 0)
                {
                    alertPoints = 0;
                }
            }
        }
    }

    IEnumerator Investigate()
    {

        //enemy permanently on higher awareness level, halved alert points

        sprinting = false;
        walking = true;

        alertLevel = "thinking";
        int randDuration = Random.Range(5, 10);
        float time = 0;

        for (int i = 1; i <= randDuration; i++)
        {
            vertInput = 0;
            horzInput = 0;

            while (time < i)
            {
                time += Time.deltaTime;
                yield return null;
            }

            // ensure time = i
            time = i;

            //look in rand direction
            Vector2 rand = Random.insideUnitCircle * 10;
            lookTarget = new Vector3(rand.x, transform.position.y, rand.y);
        }

        //after rand duration
        Debug.Log($"Now what");
        IndiscriminantInRadius();
    }

    void IndiscriminantInRadius()
    {
        //add all enemies in radius of 20 to list
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Entity")
            {
                //check if enemy in radius
                Transform hitTrans = hitCollider.transform;
                if ((hitTrans.GetComponent<EntityStats>().faction == "bad" && entStats.faction == "good")
                || (hitTrans.GetComponent<EntityStats>().faction == "good" && entStats.faction == "bad"))
                {
                    enemiesNotVisible.Add(hitTrans);
                    DirectBehaviour();
                    Debug.Log($"Found me");
                }
            }
        }
    }

    void moveToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        if(Vector3.Distance(target, transform.position) > 0.3f)
        {
            vertInput = direction.z;
            horzInput = direction.x;
        }
        else
        {
            vertInput = 0;
            horzInput = 0;
        }
    }

    void enemiesInRadius(int minAlertRange, int medAlertRange, int maxAlertRange)
    {
        enemiesNotVisible.Clear();
        enemiesVisibleDirect.Clear();
        enemiesVisibleIndirect.Clear();

        loudNoiseRadius.Clear();
        curiousNoiseRadius.Clear();
        distressedAllyRadius.Clear();
        deadAllyLineOfSight.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxAlertRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Entity")
            {
                //check if enemy in radius
                Transform hitTrans = hitCollider.transform;
                EntityController transformController = hitTrans.GetComponent<EntityController>();
                if ((hitTrans.GetComponent<EntityStats>().faction == "bad" && entStats.faction == "good")
                || (hitTrans.GetComponent<EntityStats>().faction == "good" && entStats.faction == "bad"))
                {
                   
                    float distance = Vector3.Distance(hitTrans.position, transform.position);
                    bool windowObscure = Physics.Linecast(transform.position, hitTrans.position, LayerMask.GetMask("WindowLayer"));
                    bool wallObscure = Physics.Linecast(transform.position, hitTrans.position, LayerMask.GetMask("WallLayer"));
                    bool lineOfSight = Vector3.Dot(transform.TransformDirection(Vector3.forward), hitTrans.position - transform.position) > 0.35f;
                    bool transSneaking = transformController.sneaking;
                    bool transSprinting = transformController.sprinting;

                    //add to direct if in radius
                    if (distance < minAlertRange)
                    {
                        enemiesVisibleDirect.Add(hitTrans);
                    }
                    //check if infront of transform and unobscured
                    else if (lineOfSight == true && distance < maxAlertRange && windowObscure == true)
                    {
                        //seen through window
                        enemiesVisibleIndirect.Add(hitTrans);
                    }
                    else if (lineOfSight == true && distance < maxAlertRange && wallObscure == false)
                    {
                        //not obscured
                        enemiesVisibleDirect.Add(hitTrans);
                    }
                    //check if in walk and sprint range
                    else if (transSneaking == false && distance < medAlertRange && (transformController.verticalInput != 0 || transformController.horizontalInput != 0))
                    {
                        enemiesNotVisible.Add(hitTrans);
                    }
                    //check if in sprint range
                    else if (transSprinting == true && distance < maxAlertRange && (transformController.verticalInput != 0 || transformController.horizontalInput != 0))
                    {
                        enemiesNotVisible.Add(hitTrans);
                    }
                }

                //check if fighting noise radius or dead ally line of sight
                if ((hitTrans.GetComponent<EntityStats>().faction == "bad" && entStats.faction == "bad")
                || (hitTrans.GetComponent<EntityStats>().faction == "good" && entStats.faction == "good"))
                {
                    bool lineOfSight = Vector3.Dot(transform.TransformDirection(Vector3.forward), hitTrans.position - transform.position) > 0.35f;
                    float distance = Vector3.Distance(hitTrans.position, transform.position);
                    bool windowObscure = Physics.Linecast(transform.position, hitTrans.position, LayerMask.GetMask("WindowLayer"));
                    bool wallObscure = Physics.Linecast(transform.position, hitTrans.position, LayerMask.GetMask("WallLayer"));
                    
                    //check if ally fighting
                    if(distance < maxAlertRange && transformController.alertLevel == "hunting" && transformController.isRagdoll == false && alertLevel == "patrolling")
                    {
                        distressedAllyRadius.Add(hitTrans);
                    }

                    //check if new dead ally in line of sight
                    if (lineOfSight == true && distance < maxAlertRange && (windowObscure == true || wallObscure == false) 
                        && transformController.isRagdoll == true 
                        && transformController.deadSeen == false)
                    {
                        deadAllyLineOfSight.Add(hitTrans);

                        //instant alert, mark body as seen body
                        hitTrans.GetComponent<EntityController>().deadSeen = true;
                        alertPoints = 0;
                    }
                }
            }

            //if curious noise in medium radius, investigate
            else if (hitCollider.tag == "CuriousNoise")
            {
                Transform hitTrans = hitCollider.transform;
                float distance = Vector3.Distance(hitTrans.position, transform.position);

                if (distance < medAlertRange)
                {
                    curiousNoiseRadius.Add(hitTrans);
                }
            }

            //if loud noise in radius, run to noise
            else if (hitCollider.tag == "LoudNoise")
            {
                Transform hitTrans = hitCollider.transform;
                float distance = Vector3.Distance(hitTrans.position, transform.position);

                if (distance < maxAlertRange)
                {
                    loudNoiseRadius.Add(hitTrans);
                }
            }
        }
    }
}
