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

    private static readonly int skill1TriggerHash = Animator.StringToHash("skill1Trigger");
    private static readonly int skill2TriggerHash = Animator.StringToHash("skill2Trigger");
    private static readonly int skill3TriggerHash = Animator.StringToHash("skill3Trigger");

    public static int GetSkillTriggerHash(SkillAnimation animation)
    {
        switch (animation)
        {
            case SkillAnimation.First:
                return skill1TriggerHash;
            case SkillAnimation.Second:
                return skill2TriggerHash;
            case SkillAnimation.Third:
                return skill3TriggerHash;
            default:
                throw new ArgumentOutOfRangeException("animation", animation, null);
        }
    }


    private static readonly int skill1Hash = Animator.StringToHash("skill1");
    private static readonly int skill2Hash = Animator.StringToHash("skill2");
    private static readonly int skill3Hash = Animator.StringToHash("skill3");

    public static int GetSkillHash(SkillAnimation animation)
    {
        switch (animation)
        {
            case SkillAnimation.First:
                return skill1Hash;
            case SkillAnimation.Second:
                return skill2Hash;
            case SkillAnimation.Third:
                return skill3Hash;
            default:
                throw new ArgumentOutOfRangeException("animation", animation, null);
        }
    }

    private static readonly int skill1UpdateHash = Animator.StringToHash("skill1Update");
    private static readonly int skill2UpdateHash = Animator.StringToHash("skill2Update");
    private static readonly int skill3UpdateHash = Animator.StringToHash("skill3Update");

    public static int GetSkillUpdateHash(SkillAnimation animation)
    {
        switch (animation)
        {
            case SkillAnimation.First:
                return skill1UpdateHash;
            case SkillAnimation.Second:
                return skill2UpdateHash;
            case SkillAnimation.Third:
                return skill3UpdateHash;
            default:
                throw new ArgumentOutOfRangeException("animation", animation, null);
        }
    }

    private static readonly int skill1EndHash = Animator.StringToHash("skill1End");
    private static readonly int skill2EndHash = Animator.StringToHash("skill2End");
    private static readonly int skill3EndHash = Animator.StringToHash("skill3End");

    public static int GetSkillEndHash(SkillAnimation animation)
    {
        switch (animation)
        {
            case SkillAnimation.First:
                return skill1EndHash;
            case SkillAnimation.Second:
                return skill2EndHash;
            case SkillAnimation.Third:
                return skill3EndHash;
            default:
                throw new ArgumentOutOfRangeException("animation", animation, null);
        }
    }
    

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
