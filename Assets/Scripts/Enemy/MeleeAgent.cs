using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAgent : BaseAgent
{
    public float attackRange = 0.7f;
    private AttackSystem attackSys;

    public override CollectiveAISystem.AgentType AgentType()
    {
        return CollectiveAISystem.AgentType.Melee;
    }

    public override bool CanAttackEnemy()
    {
        return (!isStunned && (currentTarget.transform.position - transform.position).magnitude < attackRange);
    }
    // Update is called once per frame
    protected override void Update()
    {
        if (isStunned)
        {
            UpdateMove();
            if ((currentTarget.transform.position - transform.position).magnitude < attackRange)
            {
                switch (AISys.GetTicket(ID))
                {
                    case CollectiveAISystem.AttackTikets.MeleeMagical:
                        attackSys.Attack(1, 1);
                        break;
                    case CollectiveAISystem.AttackTikets.MeleePhysical:
                        attackSys.Attack(0, 0);
                        break;
                    case CollectiveAISystem.AttackTikets.None:
                        break;
                }
            }
        }
        ResetStun();

    }
}
