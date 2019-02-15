using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  
 * 
 * 
 */
public class SkillUser : MonoBehaviour
{

    // Public

    public enum SkillUserState
    {
        None,

        SkillStart,
        SkillEnd,

        ChannelStart,
        ChannelUpdate,
        ChannelEnd
    }

    public SkillUserState state = SkillUserState.None;

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

    public string skillAnimationStart = "skillStart";
    public string skillAnimationEnd = "skillEnd";

    public string channelAnimationStart = "channelStart";
    public string channelAnimationUpdate = "channelUpdate";
    public string channelAnimationEnd = "channelEnd";

    public string defaultAnimation = "default";

    public string skillAnimationStartTrigger = "skillStartTrigger";

    public string channelAnimationStartTrigger = "channelStartTrigger";
    // public string channelAnimationEndTrigger = "channelEndTrigger";

    public string interruptTrigger = "interruptTrigger";
    public string interruptInstantTrigger = "interruptInstantTrigger";

    public bool preciseEnding = false;
    public bool preciseChanneling = false;
    public bool useAnimationTime = false;

    public bool Casting
    {
        get { return state != SkillUserState.None; }
    }

    // Private

    public float skillAnimationStartLength = 0.0f;
    public float skillAnimationEndLength = 0.0f;

    public float channelAnimationStartLength = 0.0f;
    public float channelAnimationUpdateLength = 0.0f;
    public float channelAnimationEndLength = 0.0f;


    private readonly IList<SkillBase> skills = new List<SkillBase>();
    private SkillBase activeSkill = null;

    private float stateTimer = 0.0f;
    private float time = 0.0f;

    // Cache

    private Animator animator;

    private void LoadAnimationLength(out float result, string name)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                result = ac.animationClips[i].length;
                return;
            }
        }

        result = 0.0f;
        Debug.LogWarning("Animation " + name + " not found!");
        // Debug.Assert(false);
    }

    void Start()
    {
        // Cache

        animator = GetComponent<Animator>();

        LoadAnimationLength(out skillAnimationStartLength, skillAnimationStart);
        LoadAnimationLength(out skillAnimationEndLength, skillAnimationEnd);

        LoadAnimationLength(out channelAnimationStartLength, channelAnimationStart);
        LoadAnimationLength(out channelAnimationUpdateLength, channelAnimationUpdate);
        LoadAnimationLength(out channelAnimationEndLength, channelAnimationEnd);

        skills.Add(new SkillBlackBall());
    }

    private void SwitchState(SkillUserState state)
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

        AnimatorClipInfo info = animator.GetCurrentAnimatorClipInfo(0)[0];
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        AnimationClip clip = info.clip;
        string clipName = clip.name;
        time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;

        bool isDefaultClip = clipName == defaultAnimation;
        isDefaultClip = true;

        switch (state)
        {
            case SkillUserState.None:
                break;
            case SkillUserState.SkillStart:
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
                    SwitchState(SkillUserState.SkillEnd);
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
                    SwitchState(SkillUserState.None);
                    activeSkill.CastEvent(gameObject);
                }
                break;
            case SkillUserState.SkillEnd:
                if (clipName == skillAnimationEnd)
                {
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, skillAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, skillAnimationEndLength, skillAnimationEndLength);

                    SwitchState(SkillUserState.None);
                }
                break;
            case SkillUserState.ChannelStart:
                if (clipName == channelAnimationStart)
                {
                    activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationStartLength);
                }
                else if (clipName == channelAnimationUpdate)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, channelAnimationStartLength, channelAnimationStartLength);

                    SwitchState(SkillUserState.ChannelUpdate);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationUpdateLength);

                }
                /* only happens due to no channel-loop animation */
                // Debug.Assert(false);
                else if (clipName == channelAnimationEnd)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, channelAnimationStartLength, channelAnimationStartLength);
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, channelAnimationUpdateLength, channelAnimationUpdateLength);

                    SwitchState(SkillUserState.ChannelEnd);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    if (preciseEnding) activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, channelAnimationStartLength, channelAnimationStartLength);
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, channelAnimationUpdateLength, channelAnimationUpdateLength);
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, channelAnimationEndLength, channelAnimationEndLength);

                    SwitchState(SkillUserState.None);
                }
                break;
            case SkillUserState.ChannelUpdate:
                if (clipName == channelAnimationUpdate)
                {
                    activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationUpdateLength);
                }
                else if (clipName == channelAnimationEnd)
                {
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, channelAnimationUpdateLength, channelAnimationUpdateLength);

                    SwitchState(SkillUserState.ChannelEnd);
                    time = useAnimationTime ? clip.length * animState.normalizedTime : stateTimer;
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationEndLength);

                }
                else if (isDefaultClip)
                {
                    if (preciseChanneling) activeSkill.ChannelUpdate(gameObject, Time.fixedDeltaTime, channelAnimationUpdateLength, channelAnimationUpdateLength);
                    if (preciseEnding) activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, channelAnimationEndLength, channelAnimationEndLength);

                    SwitchState(SkillUserState.None);
                }
                break;
            case SkillUserState.ChannelEnd:
                if (clipName == channelAnimationEnd)
                {
                    activeSkill.EndUpdate(gameObject, Time.fixedDeltaTime, time, channelAnimationEndLength);
                }
                else if (isDefaultClip)
                {
                    SwitchState(SkillUserState.None);
                }
                break;
        }
        
        stateTimer += Time.fixedDeltaTime;
    }

    private void Update()
    {

        SkillBase pressedSkill = null;
        if (Input.GetButtonDown("Skill 1") && skills.Count >= 1)
        {
            pressedSkill = skills[0];
        }
        if (Input.GetButtonDown("Skill 2") && skills.Count >= 2)
        {
            pressedSkill = skills[1];
        }
        if(pressedSkill != null)
        {
            Cast(pressedSkill.Name);
        }
    }

    public void Cast(string skillName)
    {
        Debug.Assert(!Casting);

        foreach (SkillBase skill in skills)
        {
            if (skill.Name == skillName)
            {
                if (skill.OnCooldawn) return;

                activeSkill = skill;
                activeSkill.PrepareEvent(gameObject);
                if (activeSkill.Channeling)
                {
                    SwitchState(SkillUserState.ChannelStart);
                    animator.SetTrigger(channelAnimationStartTrigger);
                    activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, 0.0f, channelAnimationStartLength);
                }
                else
                {
                    SwitchState(SkillUserState.SkillStart);
                    animator.SetTrigger(skillAnimationStartTrigger);
                    activeSkill.StartUpdate(gameObject, Time.fixedDeltaTime, 0.0f, skillAnimationStartLength);
                }

                return;
            }
        }

        Debug.LogError("Skill " + skillName + " not found!");
        Debug.Assert(false);
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
            if (state == SkillUserState.SkillStart)
            {
                SwitchState(SkillUserState.SkillEnd);
            }
            else if (state == SkillUserState.ChannelStart ||
                state == SkillUserState.ChannelUpdate)
            {
                SwitchState(SkillUserState.ChannelEnd);
            }

            animator.SetTrigger(interruptTrigger);
        }
        else
        {
            stateTimer = 0.0f;
            state = SkillUserState.None;
            animator.SetTrigger(interruptInstantTrigger);
        }
    }

}
