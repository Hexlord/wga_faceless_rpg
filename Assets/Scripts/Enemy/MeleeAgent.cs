using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ProjectFaceless/Enemy/MeleeAgent")]
public class MeleeAgent : BaseAgent
{
    [Tooltip("Minimal range before attack starts")]
    public float attackRange = 0.7f;

    private AttackSystem attackSys;

    protected override void Start()
    {
        base.Start();
        attackSys = GetComponent<AttackSystem>();
    }

    public override CollectiveAISystem.AgentType AgentType()
    {
        return CollectiveAISystem.AgentType.Melee;
    }

    public override bool CanAttackEnemy()
    {
        return (!isStunned && (player.position - transform.position).magnitude <= attackRange);
    }
    // Update is called once per frame

    #region Actions

    

    #endregion

    #region Tasks

    protected override void SetDictionary()
    {
        base.SetDictionary();
        AgentTaskDictionary.Add("Physical Attack", PhysicalAttack);
        AgentTaskDictionary.Add("Magical Attack", MagicalAttack);
        AgentTaskDictionary.Add("Pursue", Pursue);
    }

    protected SimpleTask PhysicalAttack()
    {
        Task.Condition[] preConditions =
        {
            () => !IsStunned,
            CanAttackEnemy
        };

        Task.Condition[] finishCondition =
        {
            () => !attackSys.Attacking
        };

        SimpleTask.TaskAction action = () => attackSys.Attack(0, 0);
        
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
            CanAttackEnemy
        };

        Task.Condition[] finishCondition =
        {
            () => !attackSys.Attacking
        };

        SimpleTask.TaskAction action = () => attackSys.Attack(1, 1);
        
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
        Task.Condition[] integrityRule = {() => !IsStunned};
        
        ComplexTask.DecompositionMethod method = DecomposePursue;
        
        var task = new ComplexTask(
            this.name + "Pursue",
            AISys,
            method,
            Task.EmptyCondition,
            integrityRule
            );
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
        if (!CanAttackEnemy())
        {
            tasks[0] = WalkTowardsPlayer();
            tasks[1] = SetTask("Pursue");;
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
            tasks[1] = SetTask("Pursue");;
        }

        return tasks;
    }
    
    #endregion
    
}
