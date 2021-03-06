﻿using System;
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

public class SkillFireBreath : SkillBase
{
    private static float forwardOffset = 1.0f;

    private readonly GameObject firePrefab;

    public SkillFireBreath() :
        base(Skill.FireBreath, SkillAnimation.First, false, 5.0f)
    {
        firePrefab = (GameObject)Resources.Load("Prefabs/Skills/FireBreath", typeof(GameObject));
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
        var sourceRotation = caster.transform.rotation;

        var aim = caster.GetComponent<AimSystem>();
        if (aim) sourceRotation = aim.Aim;

        var rotation = Quaternion.Euler(0.0f, sourceRotation.eulerAngles.y, 0.0f);
        var position = caster.transform.position + rotation * Vector3.forward * forwardOffset;

        var projectile =
            UnityEngine.Object.Instantiate(firePrefab,
                position, rotation);

        projectile.GetComponent<CollisionDamageBasic>().source = caster;

    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
    }

}
