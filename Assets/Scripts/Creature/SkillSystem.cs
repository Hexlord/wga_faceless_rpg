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
 * 
 */
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

    public bool Casting
    {
        get { return state != SkillSystemState.None; }
    }

    public IList<string> Skills
    {
        get { return skillNames; }
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

    private float stateTimer = 0.0f;
    private float time = 0.0f;

    // Cache

    private Animator animator;

    

    void Start()
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
    }

    /*
     * To support missing animations and increase time precision by 1 frame,
     * skill callbacks are called just after switch state, before switch-case analyzes
     */
    void FixedUpdate()
    {
        foreach (SkillBase skill in skills)
        {
            skill.Update(Time.fixedDeltaTime);
        }

        AnimatorClipInfo info = animator.GetCurrentAnimatorClipInfo(animationLayer)[0];
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayer);
        AnimationClip clip = info.clip;
        string clipName = clip.name;
        time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;

        bool isDefaultClip = clipName == idleAnimation;
        isDefaultClip = true; // TODO: remove when animations ready

        switch (state)
        {
            case SkillSystemState.None:
                break;
            case SkillSystemState.SkillStart:
                if (clipName == skillAnimationStart)
                {
                    activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, time, skillAnimationStartLength);
                }
                else if (clipName == skillAnimationEnd)
                {
                    if (preciseEnding)
                    {
                        activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, skillAnimationStartLength, skillAnimationStartLength);
                    }
                    SwitchState(SkillSystemState.SkillEnd);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.CastEvent(gameObject);
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, skillAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    if (preciseEnding)
                    {
                        activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, skillAnimationStartLength, skillAnimationStartLength);
                        activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, skillAnimationEndLength, skillAnimationEndLength);
                    }
                    SwitchState(SkillSystemState.None);
                    activeSkill.CastEvent(gameObject);
                }
                break;
            case SkillSystemState.SkillEnd:
                if (clipName == skillAnimationEnd)
                {
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, skillAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, skillAnimationEndLength, skillAnimationEndLength);

                    SwitchState(SkillSystemState.None);
                }
                break;
            case SkillSystemState.ChannelStart:
                if (clipName == channelAnimationStart)
                {
                    activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationStartLength);
                }
                else if (clipName == channelAnimationUpdate)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, channelAnimationStartLength, channelAnimationStartLength);

                    SwitchState(SkillSystemState.ChannelUpdate);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationUpdateLength);

                }
                /* only happens due to no channel-loop animation */
                // Debug.Assert(false);
                else if (clipName == channelAnimationEnd)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, channelAnimationStartLength, channelAnimationStartLength);
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, channelAnimationUpdateLength, channelAnimationUpdateLength);

                    SwitchState(SkillSystemState.ChannelEnd);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, channelAnimationStartLength, channelAnimationStartLength);
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, channelAnimationUpdateLength, channelAnimationUpdateLength);
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, channelAnimationEndLength, channelAnimationEndLength);

                    SwitchState(SkillSystemState.None);
                }
                break;
            case SkillSystemState.ChannelUpdate:
                if (clipName == channelAnimationUpdate)
                {
                    activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationUpdateLength);
                }
                else if (clipName == channelAnimationEnd)
                {
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, channelAnimationUpdateLength, channelAnimationUpdateLength);

                    SwitchState(SkillSystemState.ChannelEnd);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationEndLength);

                }
                else if (isDefaultClip)
                {
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, channelAnimationUpdateLength, channelAnimationUpdateLength);
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, channelAnimationEndLength, channelAnimationEndLength);

                    SwitchState(SkillSystemState.None);
                }
                break;
            case SkillSystemState.ChannelEnd:
                if (clipName == channelAnimationEnd)
                {
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    SwitchState(SkillSystemState.None);
                }
                break;
        }
        
        stateTimer += Time.fixedDeltaTime;
    }
    
    public void Cast(string skillName)
    {
        Debug.Assert(!Casting);

        if (!canCast) return;

        for(int i = 0; i < skills.Count; ++i)
        {
            if(skills[i].Name == skillName)
            {
                Cast(i);
                return;
            }
        }

        Debug.LogError("Skill " + skillName + " not found!");
        Debug.Assert(false);
    }

    public void Cast(int skillNumber)
    {
        Debug.Assert(!Casting);
        Debug.Assert(skillNumber >= 0);
        Debug.Assert(skills.Count > skillNumber);

        if (!canCast) return;

        SkillBase skill = skills[skillNumber];

        if (skill.OnCooldawn) return;

        activeSkill = skill;
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
        Debug.Assert(Casting);

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
            stateTimer = 0.0f;
            state = SkillSystemState.None;
            animator.SetTrigger(interruptInstantTrigger);
        }
    }

}
