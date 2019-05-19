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

public class SkillHeal : SkillBase
{
    private readonly GameObject hookPrefab;

    [Tooltip("Cost per health point")]
    public float concentrationPerHealthPoint = 1.0f;

    [Tooltip("Health point per second regeneration speed")]
    public float regenSpeed = 10.0f;

    private HealthSystem casterHealthSystem;
    private ConcentrationSystem casterConcentrationSystem;

    public SkillHeal() :
        base(Skill.Heal, SkillAnimation.Heal, true, 0.0f)
    {
        hookPrefab = (GameObject)Resources.Load("Prefabs/Skills/Hook", typeof(GameObject));
    }

    public override void PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);

        casterHealthSystem = caster.GetComponent<HealthSystem>();
        casterConcentrationSystem = caster.GetComponent<ConcentrationSystem>();
    }

    public override void StartUpdate(GameObject caster, float delta, float time, float length)
    {
        base.StartUpdate(caster, delta, time, length);
    }

    public override void CastEvent(GameObject caster)
    {
        base.CastEvent(caster);

        // SPAWN EFFECT? BUT THERE IS NO EFFECT :(
    }

    public override void ChannelUpdate(GameObject caster, float delta, float time, float length)
    {
        var maxRestore = casterHealthSystem.healthMaximum - casterHealthSystem.Health;
        maxRestore = Mathf.Min(maxRestore, regenSpeed * delta);
        var maxCost = concentrationPerHealthPoint * maxRestore;

        var amount = 0.0f;

        if (maxCost > float.Epsilon && casterConcentrationSystem.Concentration > float.Epsilon)
        {
            var cost = Mathf.Min(maxCost, casterConcentrationSystem.Concentration);
            casterConcentrationSystem.Concentration -= cost;

            amount = maxRestore * cost / maxCost;
            casterHealthSystem.Heal(caster, amount);
        }

        if (amount < float.Epsilon)
        {
            var skillSystem = caster.GetComponent<SkillSystem>();
            if (skillSystem.Channeling) skillSystem.Interrupt();
        }

    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
    }

    public override void FinishEvent(GameObject caster)
    {
    }

}
