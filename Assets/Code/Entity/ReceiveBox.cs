using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Transactions;
using UnityEngine;

public class ReceiveBox : MonoBehaviour
{    
    public EntityController Body;
    public List<Collider> GiveBoxes;

    float Damage = 0;

    public CustomOnHitShaker camShaker;

    void Update()
    {
        //check for removal
        for(int i = 0; i < GiveBoxes.Count; i++)
        {
            if(GiveBoxes[i].enabled == false)
            {
                GiveBoxes.Remove(GiveBoxes[i]);
            }
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "GiveBoxMelee" && GiveBoxes.Contains(collider) == false)
        {
            if (Body.isRagdoll == false && Body.entityStats.faction != collider.transform.root.GetComponent<EntityStats>().faction)
            {
                ActionObject giveAction = collider.transform.GetComponent<GiveBoxMelee>().currentAction;

                //add hit force
                Body.entRigBody.AddForce((Body.entRigBody.transform.position - collider.transform.position).normalized * giveAction.force, ForceMode.Impulse);

                Vector3 direction;
                direction = UnityEngine.Random.insideUnitCircle.normalized;
                Body.entRigBody.AddTorque(direction * 30, ForceMode.Impulse);

                GiveBoxes.Add(collider);

                //replace with status managers
                //get weapon stats from collider's givebox script, replace with status managers
                Damage = giveAction.giveStatuses[0].debuffValue;
                Body.damageToApply = Damage;
                Damage = 0;

                camShaker.shake = true;
            }
        }

        if (collider.tag == "GiveBoxProj" && GiveBoxes.Contains(collider) == false)
        {
            if(Body.isRagdoll == false)
            {
                GiveBoxProj giveBox = collider.transform.GetComponent<GiveBoxProj>();

                //add hit force
                Body.entRigBody.AddForce((Body.entRigBody.transform.position - collider.transform.position).normalized * giveBox.currentAction.force, ForceMode.Impulse);

                Vector3 direction;
                direction = UnityEngine.Random.insideUnitCircle.normalized;
                Body.entRigBody.AddTorque(direction * 30, ForceMode.Impulse);

                GiveBoxes.Add(collider);

                //replace with status managers
                //get weapon stats from collider's givebox script, replace with status managers
                Damage = giveBox.currentAction.giveStatuses[0].debuffValue;
                Body.damageToApply = Damage;
                Damage = 0;
                //

                camShaker.shake = true;
                giveBox.disabled = true;
                giveBox.giveCollider.enabled = false;
                //collider.transform.root.GetComponent<BoxCollider>().enabled = false;
                collider.transform.root.position = new Vector3(transform.root.position.x, UnityEngine.Random.Range(0.4f, 0.5f), transform.root.position.z);

                Destroy(collider.transform.root.GetComponent<Rigidbody>());
                collider.transform.root.parent = transform.root;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        //check for removal if not disabled
        GiveBoxes.Remove(collider);
    }
}
