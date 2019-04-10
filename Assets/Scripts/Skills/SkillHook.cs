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
    private MovementSystem casterMovementSystem;

    public static float power = 17.0f;
    public static float powerToBoss = 300.0f;
    public static float targetDistance = 1.5f;

    public static float power = 20000.0f;

    public SkillHook() :
        base(Skill.Hook, true, 1.0f)
    {
        hookPrefab = (GameObject)Resources.Load("Prefabs/Skills/Hook", typeof(GameObject));
    }

    public override void PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);
        PutOnCooldawn();
        casterMovementSystem = caster.GetComponent<MovementSystem>();
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


        var step = power * Time.fixedDeltaTime;

        switch (state)
        {
            case Hook.HookState.Fly:
                break;
            case Hook.HookState.Hit:
                var distanceFactor = Mathf.Clamp(
                    Vector3.Distance(hookComponent.Hit.transform.position, caster.transform.position) / targetDistance - 1.0f,
                    0.0f, 1.0f);

                if (hookComponent.Hit.tag.Contains("Boss"))
                {
                    casterMovementSystem.ResistForces = false; // let us be hooked
                    var direction = (hookComponent.Hit.transform.position - caster.transform.position);
                    direction.SafeNormalize();

                    caster.GetComponent<Rigidbody>().AddForce(
                        direction * powerToBoss * distanceFactor * delta, ForceMode.Impulse);
                }
                else
                {
                    var direction = (caster.transform.position - hookComponent.Hit.transform.position);
                    hookComponent.Hit.GetComponent<Rigidbody>().AddForce(
                        direction * powerToBoss * distanceFactor * delta, ForceMode.Impulse);
                }

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

        casterMovementSystem.ResistForces = true;

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
