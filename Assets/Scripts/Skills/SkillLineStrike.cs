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
    public float segmentLength = 12.0f;
    [Tooltip("Distance period of effect creation")]
    public float effectDistancePeriod = 1.6f;

    private readonly GameObject effectPrefab;

    public SkillLineStrike() :
        base(Skill.LineStrike.ToString(), false, 10.0f)
    {
        effectPrefab = (GameObject)Resources.Load("Prefabs/CircleStrike", typeof(GameObject));
    }
    
    public override void PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);
        PutOnCooldawn();
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
        var attractor = dummy.AddComponent<LineAttractor>();
        attractor.damagePerSecond = 2000.0f;
        attractor.distanceHighpass = 3.0f;
        attractor.distancePower = 0.0f;
        attractor.strength = -500.0f;
        attractor.useForce = true;
        attractor.lineFrom = caster.transform.position;
        attractor.lineTo = attractor.lineFrom + caster.transform.forward * segmentLength;
        var lifespan = dummy.AddComponent<Lifespan>();
        lifespan.lifespan = 0.0f;

        for(float d = 0.0f; d < segmentLength; d += effectDistancePeriod)
        {
            UnityEngine.Object.Instantiate(
                effectPrefab, 
                Vector3.Lerp(attractor.lineFrom, attractor.lineTo, d / segmentLength), 
                Quaternion.identity);
        }

    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
    }
    
}

