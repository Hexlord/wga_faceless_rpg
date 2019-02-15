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
public class SkillBlackBall : SkillBase
{
    public SkillBlackBall() :
        base("blackball", false, 2.0f)
    {
        projectilePrefab = (GameObject)Resources.Load("Prefabs/Fireball", typeof(GameObject));
    }
    
    public override void PrepareEvent(GameObject caster) 
    {
        Debug.Log("Preparing blackball, setting cooldawn");
        PutOnCooldawn();
    }

    public override void StartUpdate(GameObject caster, float delta, float time, float length)
    {
        Debug.Log("Casting blackball " + time + " / " + length);
    }

    public override void CastEvent(GameObject caster)
    {
        Debug.Log("Creating blackball GameObject and launching it from caster");
        GameObject projectile = UnityEngine.Object.Instantiate(projectilePrefab, caster.transform.position + new Vector3(0, 2, 0) + caster.transform.forward * 2.0f, caster.transform.rotation);

        projectile.GetComponent<Rigidbody>().velocity = caster.transform.forward * 1.0f;
        projectile.GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, caster.transform.eulerAngles.y + 180.0f, 0);
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        Debug.Log("Ending blackball " + time + " / " + length);
    }

    private readonly float damage = 50.0f;
    private readonly GameObject projectilePrefab;
}

