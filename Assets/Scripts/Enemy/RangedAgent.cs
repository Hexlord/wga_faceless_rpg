using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ProjectFaceless/Enemy/MeleeAgent")]
public class RangedAgent : BaseAgent
{
    public float seeingDistance;
    private ShootSystem shootSys;
    private bool ticketRequested = false;

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
            Physics.Raycast(ray, out hit, seeingDistance, LayerMask.GetMask("Character", "Environment", "Enemy"), QueryTriggerInteraction.Ignore);
            if (currentTarget == hit.collider.gameObject.TraverseParent(currentTarget.tag)) return true;
        }
        return false;
    }

    protected override void Update()
    {
        Vector3 shootingVector = currentTarget.transform.position + Vector3.up * 2f - shootSys.ShootingPoint.position;

        if (!isStunned && !IsDoingIdleAnimation)
        {
            UpdateMove();
            if (!shootSys.Shooting)
            {
                if (CanSeeTarget() && !ticketRequested)
                {
                    //TO BE REWORKED

                    //
                    CollectiveAISystem.AttackTikets tiket = AISys.GetTicket(ID);
                    ticketRequested = true;
                    switch (tiket)
                    {
                        case CollectiveAISystem.AttackTikets.RangedMagical:
                            //shootSys.ShootingDirection = shootingVector;
                            //shootSys.ShootingProjectileIndex = 1;
                            shootSys.Shoot(shootingVector, 1);
                            break;
                        case CollectiveAISystem.AttackTikets.RangedPhysical:
                            shootSys.Shoot(shootingVector, 0);
                            break;
                        case CollectiveAISystem.AttackTikets.None:
                            break;
                    }
                }
            }
            shootSys.ShootingDirection = shootingVector;
        }
        ResetStun();

        if (!shootSys.Shooting)
        {
            ticketRequested = false;
        }
    }
    
}

