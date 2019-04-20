using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCastBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var skillSystem = animator.GetComponent<SkillSystem>();
        if (skillSystem)
        {
            skillSystem.OnCastStart();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var skillSystem = animator.GetComponent<SkillSystem>();
        if (skillSystem)
        {
            skillSystem.OnCastEnd();
        }
    }
}
