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
        base(Skill.BlackBall.ToString(), false, 10.0f)
    {
        projectilePrefab = (GameObject)Resources.Load("Prefabs/Fireball", typeof(GameObject));
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
        GameObject projectile =
            UnityEngine.Object.Instantiate(projectilePrefab,
                Quaternion.Euler(caster.transform.eulerAngles) * hand +
                caster.transform.position + new Vector3(0, height, 0), caster.transform.rotation);

        projectile.transform.rotation = Quaternion.Euler(0, caster.transform.eulerAngles.y, 0);        
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
        Debug.Log("Ending blackball " + time + " / " + length);
    }

}

