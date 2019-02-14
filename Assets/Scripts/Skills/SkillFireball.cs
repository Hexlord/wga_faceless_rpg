using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Skill casting is:
 * 1. Begin animation
 * 2. Channel or instant animation (perform)
 * 3. Return animation
 */
public class SkillFireball : SkillBase
{
    public SkillFireball() :
        base("fireball", false)
    {

    }
    
    public override void Prepare(GameObject caster) 
    {
        Debug.Log("Preparing fireball");
    }

    public override void Start(GameObject caster, float delta, float time, float length)
    {
        Debug.Log("Casting fireball");
    }

    public override void Cast(GameObject caster)
    {
        Debug.Log("Creating fireball GameObject and launching it from caster");
    }

    public override void Return(GameObject caster, float delta, float time, float length)
    {
        Debug.Log("Ending fireball");
    }

    private readonly float damage = 50.0f;
}

