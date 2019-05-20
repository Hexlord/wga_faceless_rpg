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
 
public class SkillCircleStrike : SkillBase
{

    private readonly GameObject effectPrefab;

    private const float Strength = 40000.0f;
    private const float Damage = 30.0f;
    private const float AOE = 5.0f;

    public SkillCircleStrike() :
        base(Skill.CircleStrike, SkillAnimation.CircleStrike, false, 10.0f)
    {
        effectPrefab = (GameObject)Resources.Load("Prefabs/Skills/CircleStrike", typeof(GameObject));
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
        var attractor = dummy.AddComponent<AttractorPoint>();
        attractor.damagePerSecond = Damage / Time.fixedDeltaTime;
        attractor.distanceHighpass = AOE;
        attractor.distancePower = 0.0f;
        attractor.strength = -Strength;
        attractor.useForce = true;
        attractor.ignoreY = true;
        attractor.source = caster;
        var lifespan = dummy.AddComponent<Lifespan>();
        lifespan.lifespan = 0.0f;

        var effect = UnityEngine.Object.Instantiate(effectPrefab, caster.transform.position, Quaternion.identity);
        effect.transform.localScale = new Vector3(2, 2, 2);
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
    }
    
}

