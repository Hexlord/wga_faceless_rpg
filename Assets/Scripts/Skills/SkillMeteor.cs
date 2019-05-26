using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/*
 * History:
 *
 * Date         Author      Description
 *
 * 03.03.2019   aknorre     Created
 *
 */

public class SkillMeteor : SkillBase
{
    private MovementSystem casterMovementSystem;

    public static float height = 5.0f;
    public static float raiseTime = 0.3f;
    public static float flyTime = 0.7f;
    public static float maximumDistance = 10.0f;
    private readonly GameObject explosionPrefab;

    public SkillMeteor() :
        base(Skill.Meteor, SkillAnimation.Meteor, true, 0.0f)
    {
        explosionPrefab = (GameObject)Resources.Load("Prefabs/Skills/CircleStrike", typeof(GameObject));

    }

    public override bool PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);
        PutOnCooldown();
        casterMovementSystem = caster.GetComponent<MovementSystem>();

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
        attractor.damagePerSecond = 3000.0f;
        attractor.distanceHighpass = 7.0f;
        attractor.distancePower = 0.0f;
        attractor.strength = -40000.0f;
        attractor.useForce = true;
        attractor.ignoreY = false;
        var lifespan = dummy.AddComponent<Lifespan>();
        lifespan.lifespan = 0.0f;

        var effect = UnityEngine.Object.Instantiate(explosionPrefab, caster.transform.position, Quaternion.identity);
        effect.transform.localScale = new Vector3(2, 2, 2);
    }

    public override void ChannelUpdate(GameObject caster, float delta, float time, float length)
    {
        
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);

        
    }

    public override void FinishEvent(GameObject caster)
    {
        
    }

}
