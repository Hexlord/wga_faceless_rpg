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


    private readonly ICollection<SkillBase> skills = new HashSet<SkillBase>();
    private SkillBase activeSkill = null;

    private float timer = 0.0f;

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
        Debug.LogError("Animation " + name + " not found!");
        Debug.Assert(false);
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

        skills.Add(new SkillFireball());
    }

    private void SwitchState(SkillUserState state)
    {
        timer = 0.0f;
        this.state = state;
    }

    void FixedUpdate()
    {
        string clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        switch (state)
        {
            case SkillUserState.None:
                break;
            case SkillUserState.SkillStart:
                if(clip == skillAnimationEnd)
                {
                    SwitchState(SkillUserState.SkillEnd);
                    activeSkill.Cast(gameObject);
                } else if(clip == defaultAnimation)
                {
                    SwitchState(SkillUserState.None);
                    activeSkill.Cast(gameObject);
                }
                break;
            case SkillUserState.SkillEnd:
                if (clip == defaultAnimation)
                {
                    SwitchState(SkillUserState.None);
                }
                break;
            case SkillUserState.ChannelStart:
               if (clip == channelAnimationUpdate)
                {
                    SwitchState(SkillUserState.ChannelUpdate);
                    if(!activeSkill.Channel(gameObject, Time.fixedDeltaTime, timer, channelAnimationUpdateLength))
                    {
                        animator.SetTrigger(interruptTrigger);
                    }
                }
                break;
            case SkillUserState.ChannelUpdate:
                if (clip == channelAnimationUpdate)
                {
                    if (!activeSkill.Channel(gameObject, Time.fixedDeltaTime, timer, channelAnimationUpdateLength))
                    {
                        animator.SetTrigger(interruptTrigger);
                    }
                }
                else if (clip == channelAnimationEnd)
                {
                    SwitchState(SkillUserState.ChannelEnd);
                }
                else if (clip == defaultAnimation)
                {
                    SwitchState(SkillUserState.None);
                }
                break;
            case SkillUserState.ChannelEnd:
                if (clip == defaultAnimation)
                {
                    SwitchState(SkillUserState.None);
                }
                break;
        }


        timer += Time.fixedDeltaTime;

    }

    public void Cast(string skillName)
    {
        Debug.Assert(!Casting);

        foreach(SkillBase skill in skills)
        {
            if(skill.Name == skillName)
            {
                activeSkill = skill;
                activeSkill.Prepare(gameObject);
                if (activeSkill.Channeling)
                {
                    SwitchState(SkillUserState.ChannelStart);
                    animator.SetTrigger(channelAnimationStartTrigger);
                }
                else
                {
                    SwitchState(SkillUserState.SkillStart);
                    animator.SetTrigger(skillAnimationStartTrigger);
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

        activeSkill.Interrupt(gameObject);

        if (!instant)
        {
            if (state == SkillUserState.SkillStart)
            {
                SwitchState(SkillUserState.SkillEnd);
            }
            else if(state == SkillUserState.ChannelStart ||
                state == SkillUserState.ChannelUpdate)
            {
                SwitchState(SkillUserState.ChannelEnd);
            }

            animator.SetTrigger(interruptTrigger);
        }
        else
        {
            timer = 0.0f;
            state = SkillUserState.None;
            animator.SetTrigger(interruptInstantTrigger);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 20), "Hello");
    }

}
