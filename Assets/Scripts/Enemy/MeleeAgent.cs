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
    
    
}
