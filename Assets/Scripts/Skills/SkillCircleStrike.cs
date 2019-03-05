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

    public SkillCircleStrike() :
        base(Skill.CircleStrike.ToString(), false, 10.0f)
    {
        effectPrefab = (GameObject)Resources.Load("Prefabs/CircleStrike", typeof(GameObject));
    }
    
    public override void PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);
        Debug.Log("Preparing circlestrike, setting cooldawn");
        PutOnCooldawn();
    }

    public override void StartUpdate(GameObject caster, float delta, float time, float length)
    {
        base.StartUpdate(caster, delta, time, length);
        Debug.Log("Casting circlestrike " + time + " / " + length);
    }

    public override void CastEvent(GameObject caster)
    {
        base.CastEvent(caster);
        var dummy = new GameObject("Dummy");
        dummy.transform.position = caster.transform.position;
        var attractor = dummy.AddComponent<PointAttractor>();
        attractor.damagePerSecond = 3000.0f;
        attractor.distanceHighpass = 5.0f;
        attractor.distancePower = 0.0f;
        attractor.strength = -500.0f;
        attractor.useForce = true;
        var lifespan = dummy.AddComponent<Lifespan>();
        lifespan.lifespan = 0.0f;

        var effect = UnityEngine.Object.Instantiate(effectPrefab, caster.transform.position, Quaternion.identity);
        effect.transform.localScale = new Vector3(2, 2, 2);
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
        Debug.Log("Ending circlestrike " + time + " / " + length);
    }
    
}

