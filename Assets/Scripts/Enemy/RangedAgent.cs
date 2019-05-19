using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ProjectFaceless/Enemy/MeleeAgent")]
public class RangedAgent : BaseAgent
{
    public float seeingDistance;
    private ShootSystem shootSys;
    private bool ticketRequested = false;
    private bool canSeeTarget = false;

    public override CollectiveAISystem.AgentType AgentType()
    {
        return CollectiveAISystem.AgentType.Ranged;
    }

    protected override void Start()
    {
        base.Start();
        shootSys = GetComponent<ShootSystem>();
    }

    public override bool CanSeeTarget()
    {
        if (isAlerted)
        {
            Ray ray = new Ray(shootSys.ShootingPoint.position, currentTarget.transform.position - shootSys.ShootingPoint.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, seeingDistance, LayerMask.GetMask("Character", "Environment"), QueryTriggerInteraction.Ignore))
                if (currentTarget == hit.collider.gameObject.TraverseParent(currentTarget.tag))
                {
                    Debug.DrawLine(shootSys.ShootingPointPosition, hit.point);
                    return true;
                }
            Debug.DrawLine(shootSys.ShootingPointPosition, hit.point);
        }

        return false;
    }


   
    
}

