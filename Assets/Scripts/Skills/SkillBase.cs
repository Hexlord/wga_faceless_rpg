using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public SkillBase(string name, bool channeling)
    {
        this.name = name;
        this.channeling = channeling;
    }
    
    /*
     * Called before casting starts
     */
    public virtual void Prepare(GameObject caster)
    {

    }

    /*
     * time is how much time passed since cast start
     * length is begin animation length
     * 
     */
    public virtual void Start(GameObject caster, float delta, float time, float length)
    {

    }

    /*
     * time is how much time passed since channel start
     * length is begin animation length
     * 
     */
    public virtual void Cast(GameObject caster)
    {

    }

    /*
     * time is how much time passed since channel start
     * length is begin animation length
     * 
     * Returning false ends the channeling
     */
    public virtual bool Channel(GameObject caster, float delta, float time, float length)
    {
        return false;
    }

    /*
     * time is how much time passed since cast end
     * length is return animation length
     */
    public virtual void Return(GameObject caster, float delta, float time, float length)
    {

    }

    public virtual void Interrupt(GameObject caster)
    {

    }

    public string Name { get { return name; } }

    public bool Channeling { get { return channeling; } }

    private readonly string name;
    private readonly bool channeling;


}

