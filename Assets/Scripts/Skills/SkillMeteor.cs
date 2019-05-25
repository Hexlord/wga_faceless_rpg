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

    public SkillMeteor() :
        base(Skill.Meteor, SkillAnimation.Meteor, true, 0.0f)
    {

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

        Debug.Log("Meteor: " + time.ToString() + " / " + length.ToString());
    }

    public override void CastEvent(GameObject caster)
    {
        base.CastEvent(caster);

        var rotation = caster.transform.rotation;
        var position = caster.transform.position + Vector3.up * 2.0f;

        var aim = caster.GetComponent<AimSystem>();
        if (aim)
        {
            rotation = aim.Aim;
        }
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
