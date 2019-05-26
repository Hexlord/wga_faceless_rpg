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
 
public class SkillShiftStrike : SkillBase
{
    private const float Strength = 80000.0f;
    private const float Damage = 30.0f;
    private const float AOE = 6.0f;

    public SkillShiftStrike() :
        base(Skill.ShiftStrike, SkillAnimation.ShiftLeft, false, 10.0f)
    {
    }
    
    public override bool PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);
        PutOnCooldown();

        return true;
    }

    public override void StartUpdate(GameObject caster, float delta, float time, float length)
    {
        base.StartUpdate(caster, delta, time, length);

    }

    public override void CastEvent(GameObject caster)
    {
        base.CastEvent(caster);
        var dummy = new GameObject("Dummy");
        dummy.transform.position = caster.transform.position;
        var attractor = dummy.AddComponent<AttractorRadial>();
        attractor.damagePerSecond = Damage / Time.fixedDeltaTime;
        attractor.distanceHighpass = AOE;
        attractor.distancePower = 0.0f;
        attractor.strength = -Strength;
        attractor.useForce = true;
        attractor.ignoreY = true;
        attractor.source = caster;
        var lifespan = dummy.AddComponent<Lifespan>();
        lifespan.lifespan = 0.0f;
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
    }
    
}

