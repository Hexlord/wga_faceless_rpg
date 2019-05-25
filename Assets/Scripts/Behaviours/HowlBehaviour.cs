using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlBehaviour : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<BaseAgent>().FinalizeHowling();
    }

}
