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
public class SkillCircleStrike : SkillBase
{

    private readonly GameObject effectPrefab;

    public SkillCircleStrike() :
        base("circlestrike", false, 10.0f)
    {
        effectPrefab = (GameObject)Resources.Load("Prefabs/CircleStrike", typeof(GameObject));
    }
    
    public override void PrepareEvent(GameObject caster) 
    {
        Debug.Log("Preparing circlestrike, setting cooldawn");
        PutOnCooldawn();
    }

    public override void StartUpdate(GameObject caster, float delta, float time, float length)
    {
        Debug.Log("Casting circlestrike " + time + " / " + length);
    }

    public override void CastEvent(GameObject caster)
    {
        var dummy = new GameObject("Dummy");
        dummy.transform.position = caster.transform.position;
        var attractor = dummy.AddComponent<EnemyAttractor>();
        attractor.damagePerSecond = 3000.0f;
        attractor.distanceHighpass = 5.0f;
        attractor.distancePower = 0.0f;
        attractor.strength = -500.0f;
        attractor.useForce = true;
        var lifespan = dummy.AddComponent<Lifespan>();
        lifespan.lifespan = 0.0f;

        var effect = UnityEngine.Object.Instantiate(effectPrefab, caster.transform.position, Quaternion.identity);
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        Debug.Log("Ending circlestrike " + time + " / " + length);
    }
    
}

