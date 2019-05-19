using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ProjectFaceless/Enemy/MeleeAgent")]
public class MeleeAgent : BaseAgent
{
    [Tooltip("Minimal range before attack starts")]
    public float attackRange = 0.7f;
    [Tooltip("Frequency of attacks")]
    public float attackCooldown = 3.0f;
    private float attackStartTime;

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
        return (!isStunned && (currentTarget.transform.position - transform.position).magnitude < attackRange);
    }
    // Update is called once per frame


    #region Actions

    

    #endregion

    #region Tasks

    protected override void SetDictionary()
    {
        base.SetDictionary();
        
    }

    protected SimpleTask PhysicalAttack()
    {
        Task.Condition[] preConditions = {() => CanAttackEnemy()};
        Task.Condition[] integrityRules =
        {
            () => !IsStunned
        };

        Task.Condition[] finishCondition =
        {
            () => !attackSys.Attacking
        };

        SimpleTask.TaskAction action = () => attackSys.Attack(0, 0);
        
        var task = new SimpleTask(
            this.name + "PhysicalAttack",
            AISys,
            preConditions, 
            integrityRules,
            finishCondition,
            action);
        return task;
    }
    
    protected override Task[] DecomposeRoamAround()
    {
        var n = 0;
        Task[] tasks;
        if (isAlerted || CanSeeEnemy)
        {
            n = 3;
            tasks = new Task[n];
            tasks[0] = ;
            tasks[2] = SetTask("Pursue");
            return tasks;
        }
        tasks = new Task[n];
        tasks[0] = WalkTowardsRandomPoint();
        tasks[1] = DoRandomIdle();
        tasks[2] = SetTask("RoamAround");
        return tasks;
    }

    #endregion
    
}
