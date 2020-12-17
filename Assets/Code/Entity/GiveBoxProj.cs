using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBoxProj : MonoBehaviour
{
    public ItemObject thisItem;
    public ActionObject currentAction;
    public Rigidbody rigid_body;
    public BoxCollider giveCollider;
    public bool disabled;

    void Start()
    {
        disabled = false;
    }

    void Update()
    {
        if (disabled == false && rigid_body != null)
        {
            if (rigid_body.velocity.magnitude < 0.1f)
            {
                giveCollider.enabled = false;
            }
        }
    }
}
