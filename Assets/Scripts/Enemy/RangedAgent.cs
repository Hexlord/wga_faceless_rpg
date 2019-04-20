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

    //public override bool CanSeeTarget()
    //{
    //    return canSeeEnemy;
    //}

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

    //private void OnTriggerStay(Collider other)
    //{
    //    if (currentTarget == null)
    //    {
    //        if (other.gameObject.TraverseParent(currentTarget.tag) == currentTarget)
    //        {
    //            //Debug.Log("placed");
    //            canSeeEnemy = true;
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (currentTarget)
    //    {
    //        if (other.gameObject.TraverseParent(currentTarget.tag) == currentTarget)
    //        {
    //            canSeeEnemy = false;
    //        }
    //    }
    //}

    protected override void Update()
    {
        Vector3 shootingVector = Vector3.zero;
        if (currentTarget)
        {
            shootingVector = currentTarget.transform.position + Vector3.up - shootSys.ShootingPoint.position;
        }

        if (!isStunned && !IsDoingIdleAnimation)
        {
            if (!shootSys.Shooting)
            {
                UpdateMove();
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
                            movement.Movement = Vector2.zero;
                            break;
                        case CollectiveAISystem.AttackTikets.RangedPhysical:
                            shootSys.Shoot(shootingVector, 0);
                            movement.Movement = Vector2.zero;

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

