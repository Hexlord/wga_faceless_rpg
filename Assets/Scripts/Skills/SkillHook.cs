using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

public class SkillHook : SkillBase
{
    private readonly GameObject hookPrefab;

    private GameObject hook;
    private Hook hookComponent;

    public SkillHook() :
        base(Skill.Hook.ToString(), true, 1.0f)
    {
        hookPrefab = (GameObject)Resources.Load("Prefabs/Skills/Hook", typeof(GameObject));
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

        var rotation = caster.transform.rotation;
        var position = caster.transform.position + Vector3.up * 2.0f;

        var aim = caster.GetComponent<AimSystem>();
        if (aim)
        {
            rotation = aim.Aim;
        }

        hook =
            UnityEngine.Object.Instantiate(hookPrefab,
                position, rotation);
        hookComponent = hook.GetComponent<Hook>();
    }

    public override void ChannelUpdate(GameObject caster, float delta, float time, float length)
    {
        if (!hook)
        {
            var skillSystem = caster.GetComponent<SkillSystem>();
            if (skillSystem.Channeling) skillSystem.Interrupt(false);
            return;
        }

        var state = hookComponent.State;

        switch (state)
        {
            case Hook.HookState.Fly:
                break;
            case Hook.HookState.Hit:
                hookComponent.Hit.GetComponent<Rigidbody>().AddForce(
                    Vector3.MoveTowards(Vector3.zero, caster.transform.position - hookComponent.Hit.transform.position, 20.0f * Time.fixedDeltaTime), ForceMode.Impulse);
                break;
            case Hook.HookState.Returning:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);

        if (!hook)
        {
            var skillSystem = caster.GetComponent<SkillSystem>();
            if(skillSystem.Channeling) skillSystem.Interrupt(false);
            return;
        }

        var state = hookComponent.State;

        switch (state)
        {
            case Hook.HookState.Fly:
                hookComponent.Return();
                break;
            case Hook.HookState.Hit:
                if (Vector3.Distance(caster.transform.position,
                    hookComponent.Hit.transform.position) > 2.5f)
                {
                    caster.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(caster.transform.position,
                        hookComponent.Hit.transform.position, 0.1f * (time / length + 0.5f)));
                }

                break;
            case Hook.HookState.Returning:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void FinishEvent(GameObject caster)
    {
        if (hook) Object.Destroy(hook);
    }

}

