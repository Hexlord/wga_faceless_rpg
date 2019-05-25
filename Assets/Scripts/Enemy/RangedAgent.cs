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
    private Vector3 shootingVector;
    private SmartAiming aimSystem;

    public override CollectiveAISystem.AgentType AgentType()
    {
        return CollectiveAISystem.AgentType.Ranged;
    }

    protected override void Start()
    {
        base.Start();
        shootSys = GetComponent<ShootSystem>();
        aimSystem = GetComponent<SmartAiming>();
    }


    public override void Alert(GameObject go)
    {
        base.Alert(go);
        aimSystem.SetTarget(go.transform);
    }

    #region Conditions
    public override bool CanSeeTarget()
    {
        if (isAlerted)
        {
            Ray ray = new Ray(shootSys.ShootingPoint.position, player.position - shootSys.ShootingPoint.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, seeingDistance, LayerMask.GetMask("Character", "Environment"), QueryTriggerInteraction.Ignore))
                if (player.gameObject == hit.collider.gameObject.TraverseParent(player.tag))
                {
                    Debug.DrawLine(shootSys.ShootingPointPosition, hit.point);
                    return true;
                }
        }

        return false;
    }
    
    #endregion

    
    #region Actions

    public void ShootPhysical()
    {
        shootSys.Shoot(aimSystem.StraightLineAimingVector, 0);
    }
    
    public void ShootMagical()
    {
        shootSys.Shoot(aimSystem.StraightLineAimingVector, 1);
    }

    #endregion
    
    
    #region Tasks

    protected override void SetDictionary()
    {
        base.SetDictionary();
        AgentTaskDictionary.Add("WalkUntilPlayerSeen", WalkUntilPlayerSeen);
        AgentTaskDictionary.Add("Physical Attack", PhysicalAttack);
        AgentTaskDictionary.Add("Magical Attack", MagicalAttack);
        AgentTaskDictionary.Add("Pursue", Pursue);
    }

    protected SimpleTask WalkUntilPlayerSeen()
    {
        Task.Condition[] preConditions =
        {
            () => movement.canMove,
            () => player != null,
            () => !shootSys.Shooting
        };
        Task.Condition[] integrityRules =
        {
            () => !IsStunned,
            () => (navSys.AgentDestination(ID) - player.position).magnitude < 
                    pathRecalculationDistance
        };
        
        Task.Condition[] finishCondition =
        {
            CanSeeTarget
        };
                    
        SimpleTask.TaskAction action = StartWalkingToPlayer;
                            
        var task = new SimpleTask(
            this.name + "WalkUntilPlayerSeen",
            AISys,
            action,
            preConditions, 
            integrityRules,
            finishCondition);
        return task;
        
    }
    
    protected SimpleTask PhysicalAttack()
    {
        Task.Condition[] preConditions =
        {
            () => !IsStunned,
            CanSeeTarget,
            () => shootSys.canShoot
        };

        Task.Condition[] finishCondition =
        {
            () => !shootSys.Shooting
        };

        SimpleTask.TaskAction action = ShootPhysical;
        
        var task = new SimpleTask(
            this.name + "PhysicalAttack",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }

    protected SimpleTask MagicalAttack()
    {
        Task.Condition[] preConditions =
        {
            () => !IsStunned,
            CanSeeTarget,
            () => shootSys.canShoot
        };

        Task.Condition[] finishCondition =
        {
            () => !shootSys.Shooting
        };

        SimpleTask.TaskAction action = ShootMagical;
        
        var task = new SimpleTask(
            this.name + "MagicalAttack",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    
    protected ComplexTask Pursue()
    {
        Task.Condition[] integrityRules =
        {
            () => !IsStunned
        };


        ComplexTask.DecompositionMethod method = DecomposePursue;
        
        var task = new ComplexTask(
            this.name + "Pursue",
            AISys,
            method,
            Task.EmptyCondition, 
            integrityRules);
        return task;
    }
    
    protected override Task[] DecomposeRoamAround()
    {
        Task[] tasks;
        if (isAlerted || CanSeeEnemy)
        {
            tasks = new Task[3];
            tasks[0] = TurnTowardsPlayer();
            tasks[1] = Howl();
            tasks[2] = SetTask("Pursue");
            return tasks;
        }

        if (isUnderAttack)
        {
            tasks = new Task[3];
            tasks[0] = TurnTowardsPlayer();
            tasks[0] = Howl();
            tasks[1] = SetTask("Pursue");
            return tasks;            
        }
        tasks = new Task[3];
        tasks[0] = WalkTowardsRandomPoint();
        tasks[1] = DoRandomIdle();
        tasks[2] = SetTask("RoamAround");
        return tasks;
    }

    protected virtual Task[] DecomposePursue()
    {
        Task[] tasks = new Task[2];
        if (!CanSeeTarget())
        {
            tasks = new Task[3];
            tasks[0] = WalkUntilPlayerSeen();
            tasks[1] = StopAgent();
            tasks[2] = SetTask("Pursue");
            return tasks;
        }
        else
        {
            if (AISys.TragetStateIsPhysical())
            {
                tasks[0] = PhysicalAttack();
            }
            else
            {
                tasks[0] = MagicalAttack();
            }
        }
        tasks[1] = SetTask("Pursue");
        return tasks;
    }
    
    #endregion
   
    
}

