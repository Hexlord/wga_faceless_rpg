using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillChannelBehaviour : StateMachineBehaviour
{

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var skillSystem = animator.GetComponent<SkillSystem>();
        if (skillSystem)
        {
            skillSystem.OnChannelEnd();
        }
    }
}
