using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AwarenessIndicatorManager : MonoBehaviour
{
    Dictionary<Transform, GameObject> lastHashSet;
    Dictionary<Transform, GameObject> tempHashSet;
    List<Transform> transformsThisFrame;
    List<Transform> toRemoveThisFrame;

    public GameObject IndicatorPrefab;
    public GameObject Player;
    float maxWidth;

    // Start is called before the first frame update
    void Start()
    {
        lastHashSet = new Dictionary<Transform, GameObject>();
        tempHashSet = new Dictionary<Transform, GameObject>();
        transformsThisFrame = new List<Transform>();
        toRemoveThisFrame = new List<Transform>();

        maxWidth = 350;
    }

    // Update is called once per frame
    void Update()
    {
        //clear transforms for loop
        transformsThisFrame.Clear();
        
        //get all enemies in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10);

        //add to transform list
        foreach (var hitCollider in hitColliders)
        {
            Transform hitTrans = hitCollider.transform;
            EntityController transformController = hitTrans.GetComponent<EntityController>();

            if (hitCollider.tag == "Entity" && hitCollider.gameObject != Player && transformController.isRagdoll == false)
            {
                //check if enemy in radius
                transformsThisFrame.Add(hitTrans);
            }
        }
    
        //use dummy dict for add
        tempHashSet = lastHashSet;
        foreach (Transform enemy in transformsThisFrame)
        {
            //if transform in transforms and not in hash
            if (tempHashSet.ContainsKey(enemy) == false)
            {
                //instantiate new indicator instance, add as child to this transform
                GameObject newIndicator = Instantiate(IndicatorPrefab, transform);

                //add new hash to hashset
                lastHashSet.Add(enemy, newIndicator);
            }
        }

        foreach (KeyValuePair<Transform, GameObject> hash in tempHashSet)
        {
            //Rotate Indicator to point towards transform
            Vector3 indicatorPos = transform.position;
            Vector3 targetPos = hash.Key.position;
            Vector3 distance = targetPos - indicatorPos;
            Quaternion rot = Quaternion.LookRotation(distance.normalized);
            Transform indic = hash.Value.transform;
            indic.rotation = Quaternion.Euler(90, rot.eulerAngles.y, 0);

            //determine width by closeness
            RectTransform rt = indic.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(maxWidth * (1 - (Mathf.Abs(distance.magnitude) / 10)), 300);

            //determine color
            EntityController entCont = hash.Key.GetComponent<EntityController>();
            string alertLevel = entCont.alertLevel;
            string traversalType = entCont.traversalType;

            if (alertLevel == "patrolling")
            {
                //white
                indic.GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 0.9f);
            }
            else if (alertLevel == "alerting" || alertLevel == "searching" || alertLevel == "thinking")
            {
                //yellow
                indic.GetComponent<RawImage>().color = new Color(1.0f, 0.92f, 0.016f, 0.9f);
            }
            else if (alertLevel == "hunting" && traversalType == "direct")
            {
                //red
                indic.GetComponent<RawImage>().color = new Color(1.0f, 0, 0.0f, 0.9f);
            }
            else if (alertLevel == "hunting")
            {
                //orange
                indic.GetComponent<RawImage>().color = new Color(1.0f, (156.0f / 255.0f), 0.0f, 0.9f);
            }


            //if transform in hash and not in current transforms
            if (transformsThisFrame.Contains(hash.Key) == false)
            {
                //remove hash from hash set
                toRemoveThisFrame.Add(hash.Key);
            }
        }

        for(int i = 0; i < toRemoveThisFrame.Count; i++)
        {
            //Debug.Log("Are we getting here?");

            //instantiate new indicator instance, add as child to this transform
            Destroy(lastHashSet[toRemoveThisFrame[i]]);

            //add new hash to hashset
            lastHashSet.Remove(toRemoveThisFrame[i]);
        }

        toRemoveThisFrame.Clear();
    }
}
