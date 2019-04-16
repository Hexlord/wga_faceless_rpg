using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

public class SkillBlackBall : SkillBase
{
    private const float height = 2.0f;
    private Vector3 hand = new Vector3(0.4f, 0.0f, 0.5f);
    private readonly GameObject projectilePrefab;

    public SkillBlackBall() :
        base(Skill.BlackBall, SkillAnimation.First, false, 10.0f)
    {
        projectilePrefab = (GameObject)Resources.Load("Prefabs/Skills/BlackBall", typeof(GameObject));
    }

    public override void PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);
        Debug.Log("Preparing blackball, setting cooldawn");
        PutOnCooldawn();
    }

    public override void StartUpdate(GameObject caster, float delta, float time, float length)
    {
        base.StartUpdate(caster, delta, time, length);
        Debug.Log("Casting blackball " + time + " / " + length);
    }

    public override void CastEvent(GameObject caster)
    {
        base.CastEvent(caster);
        Debug.Log("Creating blackball GameObject and launching it from caster");

        var rotation = caster.transform.rotation;
        var position = rotation * hand + caster.transform.position + Vector3.up * height;

        var aim = caster.GetComponent<AimSystem>();
        if (aim)
        {
            rotation = aim.Aim;
        }

        var projectile =
            UnityEngine.Object.Instantiate(projectilePrefab,
                position, rotation);

        projectile.GetComponent<AttractorPoint>().source = caster;

    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
        Debug.Log("Ending blackball " + time + " / " + length);
    }

}

