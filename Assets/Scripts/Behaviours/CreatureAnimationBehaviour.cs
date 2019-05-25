using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimationBehaviour : StateMachineBehaviour
{
    // Private

    private int fromHash = 0;

    // Cache

    public static readonly int unarmedIdleHash = Animator.StringToHash("unarmedIdle");
    public static readonly int armedIdleHash = Animator.StringToHash("armedIdle");

    public static readonly int sheatheHash = Animator.StringToHash("sheathe");
    public static readonly int unsheatheHash = Animator.StringToHash("unsheathe");

    public static bool IsSheatheUnsheatheHash(int hash)
    {
        return hash == sheatheHash || hash == unsheatheHash;
    }

    public static readonly int attack1Hash = Animator.StringToHash("attack1");
    public static readonly int attack2Hash = Animator.StringToHash("attack2");

    public static bool IsAttackHash(int hash)
    {
        return hash == attack1Hash || hash == attack2Hash;
    }

    /*
     * Leads to normally finishing skill via SkillEnd or ChannelEnd
     */
    public static readonly int interruptTriggerHash = Animator.StringToHash("interruptTrigger");
    /*
     * Leads to idle animation
     */
    public static readonly int stopTriggerHash = Animator.StringToHash("stopTrigger");
    public static readonly int channelingBooleanHash = Animator.StringToHash("Channeling");
    public static readonly int currentSkillFloatHash = Animator.StringToHash("CurrentSkill");

    public static readonly int skillTriggerHash = Animator.StringToHash("skillTrigger");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*
        var go = animator.gameObject;
        var hash = stateInfo.shortNameHash;


        var attackRelated = IsAttackHash(fromHash);
        if (attackRelated)
        {
            var attackSystem = go.GetComponent<AttackSystem>();
            if (attackSystem)
            {
                attackSystem.FinalizeAttack();
            }
        }

        var skillSystem = go.GetComponent<SkillSystem>();
        if (skillSystem && skillSystem.Busy)
        {
            skillSystem.FinalizeTransition();
        }
        */
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fromHash = stateInfo.shortNameHash;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
