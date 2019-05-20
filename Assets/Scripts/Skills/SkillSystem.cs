using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
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

        Preparing,

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


    [Tooltip("Skills known by this character")]
    public Skill[] startSkills;

    public bool Busy
    {
        get { return state != SkillSystemState.None; }
    }

    public SkillSystemState State
    {
        get { return state; }
    }

    public bool Casting
    {
        get
        {
            return state == SkillSystemState.SkillStart ||
                   state == SkillSystemState.SkillEnd;
        }
    }
    public bool Channeling
    {
        get
        {
            return state == SkillSystemState.ChannelStart ||
                   state == SkillSystemState.ChannelUpdate ||
                   state == SkillSystemState.ChannelEnd;
        }
    }

    public IList<Skill> SkillTypes
    {
        get { return skillTypes; }
    }
    public IList<SkillBase> Skills
    {
        get { return skills; }
    }

    public int ActiveSkillNumber
    {
        get { return activeSkillNumber; }
    }
    public bool IsSkillSelected
    {
        get { return activeSkill != null; }
    }

    public int SelectedSkillNumber
    {
        get { return activeSkillNumber; }
    }
    public SkillBase SelectedSkill
    {
        get { return activeSkillNumber == -1 ? null : skills[activeSkillNumber]; }
    }

    // Private

    [Header("Debug")]
    public SkillSystemState state = SkillSystemState.None;

    public bool interrupt = false;
    public bool stop = false;

    private readonly IList<SkillBase> skills = new List<SkillBase>();
    private readonly IList<Skill> skillTypes = new List<Skill>();
    private SkillBase activeSkill = null;
    private int activeSkillNumber = -1;

    private float stateTimer = 0.0f;
    private float stateLength = 0.0f;

    // Cache

    private Animator animator;

    private MovementSystem movementSystem;

    private void Awake()
    {
        // Cache

        animator = GetComponent<Animator>();
        movementSystem = GetComponent<MovementSystem>();
        
        foreach (var skill in startSkills)
        {
            skills.Add(skill.Instantiate());
            skillTypes.Add(skill);
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

    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;

        foreach (var skill in skills)
        {
            skill.Update(delta);
        }

        stateTimer += delta;

        switch (state)
        {
            case SkillSystemState.None:
                break;
            case SkillSystemState.SkillStart:
                activeSkill.StartUpdate(gameObject, delta, stateTimer, stateLength);
                break;
            case SkillSystemState.SkillEnd:
                activeSkill.EndUpdate(gameObject, delta, stateTimer, stateLength);
                break;
            case SkillSystemState.ChannelStart:
                activeSkill.StartUpdate(gameObject, delta, stateTimer, stateLength);
                break;
            case SkillSystemState.ChannelUpdate:
                activeSkill.ChannelUpdate(gameObject, delta, stateTimer, stateLength);
                break;
            case SkillSystemState.ChannelEnd:
                activeSkill.EndUpdate(gameObject, delta, stateTimer, stateLength);
                break;
            case SkillSystemState.Preparing:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool HasSkill(Skill skill)
    {
        foreach(var skillIt in skills)
        {
            if (skillIt.Type == skill) return true;
        }

        return false;
    }

    public void Learn(Skill skill)
    {
        Debug.Assert(!HasSkill(skill));

        skills.Add(skill.Instantiate());
        skillTypes.Add(skill);
    }

    public void SelectSkill(Skill skill)
    {
        Debug.Assert(!Busy);

        for (var i = 0; i < skills.Count; ++i)
        {
            if (skills[i].Type == skill)
            {
                SelectSkill(i);
                return;
            }
        }

        Debug.LogError("Skill " + skill.ToString() + " not found!");
        Debug.Assert(false);
    }

    public void SelectSkill(string skillName)
    {
        Debug.Assert(!Busy);

        for (var i = 0; i < skills.Count; ++i)
        {
            if (skills[i].Type.ToString() == skillName)
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

        var skill = skills[skillNumber];

        if (skill.OnCooldown) return;

        activeSkill = skill;
        activeSkillNumber = skillNumber;

    }

    public float GetCooldownNormalized(Skill skill)
    {
        for (var i = 0; i < skills.Count; ++i)
        {
            if (skills[i].Type == skill)
            {
                return skills[i].CooldownTimerNormalized;
            }
        }

        return 0.0f;
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

        if (!activeSkill.PrepareEvent(gameObject))
        {
            UnselectSkill();
            return;
        }
        animator.SetTrigger(CreatureAnimationBehaviour.skillTriggerHash);
        animator.SetFloat(CreatureAnimationBehaviour.currentSkillFloatHash, Convert.ToSingle(activeSkill.Animation));

        animator.ResetTrigger(CreatureAnimationBehaviour.interruptTriggerHash);
        interrupt = false;
        animator.ResetTrigger(CreatureAnimationBehaviour.stopTriggerHash);
        stop = false;

        SwitchState(SkillSystemState.Preparing);
    }

    public void OnCastStart()
    {
        Debug.Log("Cast start");
        switch (state)
        {
            case SkillSystemState.Preparing:
                // activeSkill.PrepareEvent(gameObject);
                SwitchState(activeSkill.Channeling ? SkillSystemState.ChannelStart : SkillSystemState.SkillStart);
                animator.SetBool(CreatureAnimationBehaviour.channelingBooleanHash, activeSkill.Channeling);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnCastEnd()
    {
        Debug.Log("Cast end");
        switch (state)
        {
            case SkillSystemState.SkillStart:
                activeSkill.CastEvent(gameObject);
                SwitchState(SkillSystemState.SkillEnd);
                break;
            case SkillSystemState.ChannelStart:
                activeSkill.CastEvent(gameObject);
                SwitchState(SkillSystemState.ChannelUpdate);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public void OnChannelEnd()
    {
        Debug.Log("Channel end");
        switch (state)
        {
            case SkillSystemState.ChannelUpdate:
                SwitchState(SkillSystemState.ChannelEnd);
                break;
            case SkillSystemState.SkillEnd:
            case SkillSystemState.None:
            case SkillSystemState.Preparing:
            case SkillSystemState.ChannelStart:
            case SkillSystemState.ChannelEnd:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnRestoreEnd()
    {
        Debug.Log("Restore end");
        activeSkill.FinishEvent(gameObject);
        SwitchState(SkillSystemState.None);
    }
    
    public void Interrupt()
    {
        Debug.Assert(Busy);

        activeSkill.InterruptEvent(gameObject);

        animator.SetTrigger(CreatureAnimationBehaviour.interruptTriggerHash);
        interrupt = true;
    }

    public void Stop()
    {
        Debug.Assert(Busy);

        animator.SetTrigger(CreatureAnimationBehaviour.stopTriggerHash);
        stop = true;
    }

}
