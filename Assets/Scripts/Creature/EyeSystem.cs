using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSystem : MonoBehaviour
{
    [Header("Line of sight settings")]
    public Transform target;
    public float maxDistance = 100.0f;
    public float fieldOfViewWidth = 270.0f;
    public float rayDensity = 90.0f;
    public string targetTag = "Player";
    private float rayStep;
    private RaycastHit hit;
    private Vector3 rayDirection;
    private BaseAgent agent;
    private GameObject spotted;
    private Transform eyes;



    public void SetTargetTag (string tag)
    {
        targetTag = tag;
    }

    // Start is called before the first frame update
    void Start()
    {
        eyes = this.transform;
        GameObject gm = gameObject.TraverseParent(transform.tag);
        agent = gameObject.TraverseParent(transform.tag).GetComponent<BaseAgent>();
        rayStep = fieldOfViewWidth / rayDensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Spotted(targetTag, out spotted))
        {
            agent.SawSomething(spotted);
        }
        else
        {
            if(agent.CanSeeEnemy)
            {
                agent.LostTarget();
            }
        }
    }

    //bool Spotted(Transform tar)
    //{
    //    rayDirection = target.position - this.transform.position;
    //    return Physics.Raycast(eyes.position, rayDirection, out hit, maxDistance, LayerMask.GetMask("Character"), QueryTriggerInteraction.Ignore);
    //}

    bool Spotted(string spottingTag, out GameObject spottedObject)
    {
        spottedObject = null;
        rayDirection = eyes.forward;
        //for (float j = 0.0f; j < fieldOfViewWidth * 0.5; j += rayStep)
        //{
        for (float i = 0.0f; i < fieldOfViewWidth * 0.5; i += rayStep)
        {
            rayDirection = Quaternion.AngleAxis(i, Vector3.up) * eyes.forward;
            //rayDirection = Quaternion.AngleAxis(j, Vector3.right) * rayDirection;
            if (Physics.Raycast(eyes.position, rayDirection, out hit, maxDistance, LayerMask.GetMask("Character"), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == spottingTag)
                {
                    spottedObject = hit.collider.gameObject.TraverseParent(targetTag);
                    return true;
                }
            }
            Debug.DrawLine(eyes.position, eyes.position + rayDirection * 100);

            rayDirection = Quaternion.AngleAxis(i, Vector3.down) * eyes.forward;
            //rayDirection = Quaternion.AngleAxis(j, Vector3.left) * rayDirection;
            if (Physics.Raycast(eyes.position, rayDirection, out hit, maxDistance, LayerMask.GetMask("Character"), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == spottingTag)
                {
                    spottedObject = hit.collider.gameObject.TraverseParent(targetTag);
                    return true;
                }

            }
            Debug.DrawLine(eyes.position, eyes.position + rayDirection * 100);
        }
        //}
        return false;
    }

}
