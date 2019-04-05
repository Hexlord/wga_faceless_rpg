using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */
[AddComponentMenu("ProjectFaceless/Creature/Skill System")]
public class SkillSystem : MonoBehaviour
{
    
    // Public

    public enum SkillSystemState
    {
        None,

        SkillStart,
        SkillEnd,

        ChannelStart,
        ChannelUpdate,
        ChannelEnd
    }


    /*
     * Expected animation configuration:
     * 
     * [skillStartTrigger] -> (skillStart) ->
     * [interruptTrigger] -> (skillEnd) -> (default)
     * 
     * [channelStartTrigger] -> (channelStart) ->
     * (channelUpdate) <loop>
     * [interruptTrigger] -> (channelEnd) -> (default)
     */

    [Header("Basic Settings")]
    [Tooltip("Toggles whether creature can cast spells")]
    public bool canCast = true;

    [Header("Animation Settings")]

    public string skillAnimationStart = "skillStart";
    public string skillAnimationEnd = "skillEnd";

    public string channelAnimationStart = "channelStart";
    public string channelAnimationUpdate = "channelUpdate";
    public string channelAnimationEnd = "channelEnd";

    public string idleAnimation = "idle";

    public string skillAnimationStartTrigger = "skillStartTrigger";

    public string channelAnimationStartTrigger = "channelStartTrigger";
    // public string channelAnimationEndTrigger = "channelEndTrigger";

    public string interruptTrigger = "interruptTrigger";
    public string interruptInstantTrigger = "interruptInstantTrigger";

    public int animationLayer = 0;

    [Header("Precision Settings")]

    [Tooltip("Enabling makes skills tick even through missed ending frame")]
    public bool preciseEnding = false;
    [Tooltip("Enabling makes skills tick even through missed channeling frame")]
    public bool preciseChanneling = false;
    [Tooltip("Enabling makes skills use current animation time instead of internal state timer")]
    public bool useAnimationTime = false;

    
    [Tooltip("Skills known by this character")]
    public Skill[] startSkills;

    public bool Busy
    {
        get { return state != SkillSystemState.None; }
    }
    public bool Casting
    {
        get { return state == SkillSystemState.SkillStart ||
                     state == SkillSystemState.SkillEnd; }
    }
    public bool Channeling
    {
        get { return state == SkillSystemState.ChannelStart ||
                     state == SkillSystemState.ChannelUpdate ||
                     state == SkillSystemState.ChannelEnd; }
    }

    public IList<string> Skills
    {
        get { return skillNames; }
    }

    public int ActiveSkillNumber
    {
        get { return activeSkillNumber; }
    }

    public bool IsSkillSelected
    {
        get { return activeSkill != null; }
    }

    // Private

    [Header("Debug")]
    public SkillSystemState state = SkillSystemState.None;

    public float skillAnimationStartLength = 0.0f;
    public float skillAnimationEndLength = 0.0f;

    public float channelAnimationStartLength = 0.0f;
    public float channelAnimationUpdateLength = 0.0f;
    public float channelAnimationEndLength = 0.0f;

    private readonly IList<SkillBase> skills = new List<SkillBase>();
    private readonly IList<string> skillNames = new List<string>();
    private SkillBase activeSkill = null;
    private int activeSkillNumber = -1;

    private float stateTimer = 0.0f;
    private float time = 0.0f;

    // Cache

    private Animator animator;



    void Awake()
    {
        // Cache

        animator = GetComponent<Animator>();

        skillAnimationStartLength = animator.GetAnimationLength(skillAnimationStart);
        skillAnimationEndLength = animator.GetAnimationLength(skillAnimationEnd);

        channelAnimationStartLength = animator.GetAnimationLength(channelAnimationStart);
        channelAnimationUpdateLength = animator.GetAnimationLength(channelAnimationUpdate);
        channelAnimationEndLength = animator.GetAnimationLength(channelAnimationEnd);

        foreach(Skill skill in startSkills)
        {
            skills.Add(skill.Instantiate());
            skillNames.Add(skill.ToString());
        }
    }

    private void SwitchState(SkillSystemState state)
    {
        stateTimer = 0.0f;
        this.state = state;

        if (state == SkillSystemState.None)
        {
            activeSkillNumber = -1;
            activeSkill = null;
        }
    }

    /*
     * To support missing animations and increase time precision by 1 frame,
     * skill callbacks are called just after switch state, before switch-case analyzes
     */
    void FixedUpdate()
    {
       
        
        AnimatorClipInfo info = animator.GetCurrentAnimatorClipInfo(animationLayer)[0];
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayer);
        AnimationClip clip = info.clip;
        string clipName = clip.name;
        time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
        var delta = Time.fixedDeltaTime * animState.speed;

        foreach (SkillBase skill in skills)
        {
            skill.Update(delta);
        }

        bool transition = animator.IsInTransition(animationLayer);

        bool isDefaultClip = clipName == idleAnimation;

        stateTimer += delta;
        
        switch (state)
        {
            case SkillSystemState.None:
                break;
            case SkillSystemState.SkillStart:
                if (clipName == skillAnimationStart)
                {
                    activeSkill.StartUpdate(gameObject, delta, time, skillAnimationStartLength);
                }
                else if (clipName == skillAnimationEnd)
                {
                    if (preciseEnding)
                    {
                        activeSkill.StartUpdate(gameObject, delta, skillAnimationStartLength, skillAnimationStartLength);
                    }
                    SwitchState(SkillSystemState.SkillEnd);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.CastEvent(gameObject);
                    activeSkill.EndUpdate(gameObject, delta, time, skillAnimationEndLength);
                }
                /*
                else if (isDefaultClip)
                {
                    if (preciseEnding)
                    {
                        activeSkill.StartUpdate(gameObject, delta, skillAnimationStartLength, skillAnimationStartLength);
                        activeSkill.EndUpdate(gameObject, delta, skillAnimationEndLength, skillAnimationEndLength);
                    }
                    SwitchState(SkillSystemState.None);
                    activeSkill.CastEvent(gameObject);
                }
                */
                break;
            case SkillSystemState.SkillEnd:
                if (clipName == skillAnimationEnd)
                {
                    activeSkill.EndUpdate(gameObject, delta, time, skillAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, delta, skillAnimationEndLength, skillAnimationEndLength);

                    SwitchState(SkillSystemState.None);
                }
                break;
            case SkillSystemState.ChannelStart:
                if (clipName == channelAnimationStart)
                {
                    activeSkill.StartUpdate(gameObject, delta, time, channelAnimationStartLength);
                }
                else if (clipName == channelAnimationUpdate)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, delta, channelAnimationStartLength, channelAnimationStartLength);

                    SwitchState(SkillSystemState.ChannelUpdate);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.CastEvent(gameObject);
                    activeSkill.ChannelUpdate(gameObject, delta, time, channelAnimationUpdateLength);

                }
                /* only happens due to no channel-loop animation */
                else if (clipName == channelAnimationEnd)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, delta, channelAnimationStartLength, channelAnimationStartLength);
                    activeSkill.CastEvent(gameObject);
                    if (preciseChanneling)
                    {
                        activeSkill.ChannelUpdate(gameObject, delta, channelAnimationUpdateLength, channelAnimationUpdateLength);
                    }

                    SwitchState(SkillSystemState.ChannelEnd);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.EndUpdate(gameObject, delta, time, channelAnimationEndLength);
                }
                /*
                 * Would be nice for handling zero length animation, but it requires separation of Idle animation and Cast start point(await start anim transition)
                 */
                /*
                else if (isDefaultClip)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, delta, channelAnimationStartLength, channelAnimationStartLength);
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, delta, channelAnimationUpdateLength, channelAnimationUpdateLength);
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, delta, channelAnimationEndLength, channelAnimationEndLength);

                    SwitchState(SkillSystemState.None);
                }
                */
                break;
            case SkillSystemState.ChannelUpdate:
                if (clipName == channelAnimationUpdate)
                {
                    activeSkill.ChannelUpdate(gameObject, delta, time, channelAnimationUpdateLength);
                }
                else if (clipName == channelAnimationEnd)
                {
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, delta, channelAnimationUpdateLength, channelAnimationUpdateLength);

                    SwitchState(SkillSystemState.ChannelEnd);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.EndUpdate(gameObject, delta, time, channelAnimationEndLength);

                }
                else if (isDefaultClip)
                {
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, delta, channelAnimationUpdateLength, channelAnimationUpdateLength);
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, delta, channelAnimationEndLength, channelAnimationEndLength);
                    activeSkill.FinishEvent(gameObject);

                    SwitchState(SkillSystemState.None);
                }
                break;
            case SkillSystemState.ChannelEnd:
                if (clipName == channelAnimationEnd)
                {
                    activeSkill.EndUpdate(gameObject, delta, time, channelAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    activeSkill.FinishEvent(gameObject);

                    SwitchState(SkillSystemState.None);
                }
                break;
        }
    }
    
    public void SelectSkill(string skillName)
    {
        Debug.Assert(!Busy);

        for(int i = 0; i < skills.Count; ++i)
        {
            if(skills[i].Name == skillName)
            {
                SelectSkill(i);
                return;
            }
        }

        Debug.LogError("Skill " + skillName + " not found!");
        Debug.Assert(false);
    }

    public void SelectSkill(int skillNumber)
    {
        Debug.Assert(!Busy);
        Debug.Assert(skillNumber >= 0);
        Debug.Assert(skills.Count > skillNumber);

        if (!canCast) return;

        SkillBase skill = skills[skillNumber];

        if (skill.OnCooldawn) return;

        animator.ResetTrigger(interruptTrigger);
        animator.ResetTrigger(interruptInstantTrigger);

        activeSkill = skill;
        activeSkillNumber = skillNumber;
        
    }

    public void UnselectSkill()
    {
        Debug.Assert(!Busy);

        activeSkill = null;
        activeSkillNumber = -1;

    }

    public void Cast()
    {
        Debug.Assert(!Busy);
        Debug.Assert(IsSkillSelected);

        activeSkill.PrepareEvent(gameObject);
        if (activeSkill.Channeling)
        {
            SwitchState(SkillSystemState.ChannelStart);
            animator.SetTrigger(channelAnimationStartTrigger);
            activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, 0.0f, channelAnimationStartLength);
        }
        else
        {
            SwitchState(SkillSystemState.SkillStart);
            animator.SetTrigger(skillAnimationStartTrigger);
            activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, 0.0f, skillAnimationStartLength);
        }
    }

    
    /*
     * Resets state and animation
     * 
     * instant means skip end animation
     */
    public void Interrupt(bool instant)
    {
        Debug.Assert(Busy);

        activeSkill.InterruptEvent(gameObject);

        if (!instant)
        {
            if (state == SkillSystemState.SkillStart)
            {
                SwitchState(SkillSystemState.SkillEnd);
            }
            else if (state == SkillSystemState.ChannelStart ||
                state == SkillSystemState.ChannelUpdate)
            {
                SwitchState(SkillSystemState.ChannelEnd);
            }

            animator.SetTrigger(interruptTrigger);
        }
        else
        {
            SwitchState(SkillSystemState.None);
            animator.SetTrigger(interruptInstantTrigger);
        }
    }

}
