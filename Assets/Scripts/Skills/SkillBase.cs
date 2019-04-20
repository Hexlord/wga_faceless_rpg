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

/*
 * Skill casting is:
 * 1. Begin animation
 * 2. Channel or instant animation (perform)
 * 3. Return animation
 * 
 * caster is not cached because it could be a component, 
 * which could be added to scene after this class creation
 * 
 */
public class SkillBase
{

    private readonly GameObject textPrefab;

    public SkillBase(Skill type, SkillAnimation animation, bool channeling, float cooldown)
    {
        this.type = type;
        this.animation = animation;
        this.channeling = channeling;
        this.cooldown = cooldown;
        this.cooldownTimer = 0.0f;
    }

    public void Update(float delta)
    {
        cooldownTimer -= delta;
    }
    
    /*
     * Called before casting starts
     */
    public virtual void PrepareEvent(GameObject caster)
    {
    }

    /*
     * time is how much time passed since cast start
     * length is begin animation length
     * 
     */
    public virtual void StartUpdate(GameObject caster, float delta, float time, float length)
    {

    }

    /*
     * 
     */
    public virtual void CastEvent(GameObject caster)
    {
    }

    /*
     * time is how much time passed since channel start
     * length is begin animation length
     * 
     */
    public virtual void ChannelUpdate(GameObject caster, float delta, float time, float length)
    {
    }

    /*
     * time is how much time passed since cast end
     * length is return animation length
     */
    public virtual void EndUpdate(GameObject caster, float delta, float time, float length)
    {

    }

    public virtual void FinishEvent(GameObject caster)
    {

    }

    public virtual void InterruptEvent(GameObject caster)
    {

    }

    public Skill Type { get { return type; } }

    public SkillAnimation Animation
    {
        get { return animation; }
    }

    public bool Channeling { get { return channeling; } }

    public bool OnCooldown { get { return cooldownTimer > 0; } }
    public float CooldownTimerNormalized { get { return cooldownTimer / cooldown; } }

    protected void PutOnCooldown()
    {
        cooldownTimer = cooldown;
    }

    private readonly Skill type;
    private readonly SkillAnimation animation;
    private readonly bool channeling;
    private readonly float cooldown;
    private float cooldownTimer;




}

