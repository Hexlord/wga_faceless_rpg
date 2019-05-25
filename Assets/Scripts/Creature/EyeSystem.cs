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

    bool Spotted(string spottingTag, out GameObject spottedObject)
    {
        spottedObject = null;
        rayDirection = eyes.forward;
        rayDirection.y = 0;

        for (float i = 0.0f; i < fieldOfViewWidth * 0.5; i += rayStep)
        {
            rayDirection = Quaternion.AngleAxis(i, Vector3.up) * eyes.forward;
            if (Physics.Raycast(eyes.position, rayDirection, out hit, maxDistance, LayerMask.GetMask("Character"), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag(spottingTag))
                {
                    spottedObject = hit.collider.gameObject.TraverseParent(targetTag);
                    return true;
                }
            }

            rayDirection = Quaternion.AngleAxis(i, Vector3.down) * eyes.forward;
            if (Physics.Raycast(eyes.position, rayDirection, out hit, maxDistance, LayerMask.GetMask("Character"), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag(spottingTag))
                {
                    spottedObject = hit.collider.gameObject.TraverseParent(targetTag);
                    return true;
                }

            }
        }
        return false;
    }

}
