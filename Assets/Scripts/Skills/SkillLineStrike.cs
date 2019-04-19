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
 
public class SkillLineStrike : SkillBase
{
    [Header("Line Segment Settings")]

    [Tooltip("Length of created strike line segment")]
    private const float SegmentLength = 12.0f;
    [Tooltip("Distance period of effect creation")]
    private const float EffectDistancePeriod = 1.6f;

    private const float Strength = 40000.0f;
    private const float Damage = 30.0f;
    private const float AOE = 3.0f;

    private readonly GameObject effectPrefab;

    public SkillLineStrike() :
        base(Skill.LineStrike, SkillAnimation.Second, false, 10.0f)
    {
        effectPrefab = (GameObject)Resources.Load("Prefabs/Skills/CircleStrike", typeof(GameObject));
    }
    
    public override void PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);
        PutOnCooldown();
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
        var attractor = dummy.AddComponent<AttractorLine>();
        attractor.damagePerSecond = Damage / Time.fixedDeltaTime;
        attractor.distanceHighpass = AOE;
        attractor.distancePower = 0.0f;
        attractor.strength = -Strength;
        attractor.useForce = true;
        attractor.lineFrom = caster.transform.position;
        attractor.lineTo = attractor.lineFrom + caster.transform.forward * SegmentLength;
        attractor.ignoreY = true;
        attractor.source = caster;
        var lifespan = dummy.AddComponent<Lifespan>();
        lifespan.lifespan = 0.0f;

        for(var d = 0.0f; d < SegmentLength; d += EffectDistancePeriod)
        {
            var position = caster.transform.position + Vector3.up * 2.0f +
                           caster.transform.forward * d;
            RaycastHit hitInfo;

            var mask = (1 << LayerMask.NameToLayer("Environment"));

            if (!Physics.Raycast(new Ray(position, -Vector3.up), out hitInfo, 100.0f, mask)) continue;
            
            UnityEngine.Object.Instantiate(
                effectPrefab,
                hitInfo.point, 
                Quaternion.identity);


        }

    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
    }
    
}

