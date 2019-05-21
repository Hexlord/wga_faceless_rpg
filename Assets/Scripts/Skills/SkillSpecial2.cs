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

public class SkillSpecial2 : SkillBase
{
    public static float concentrationCost = 99.9f;

    public override float ConcentrationCost
    {
        get { return concentrationCost; }
    }

    private EffectSystem casterEffectSystem;
    private ConcentrationSystem casterConcentrationSystem;

    public SkillSpecial2() :
        base(Skill.SkillSpecial2, SkillAnimation.Heal, false, 2.0f)
    {

    }

    public override bool PrepareEvent(GameObject caster)
    {
        base.PrepareEvent(caster);

        casterEffectSystem = caster.GetComponent<EffectSystem>();
        casterConcentrationSystem = caster.GetComponent<ConcentrationSystem>();

        if (casterConcentrationSystem.Concentration < concentrationCost)
        {
            return false;
        }

        casterConcentrationSystem.Use(concentrationCost);
        PutOnCooldown();
        return true;
    }

    public override void StartUpdate(GameObject caster, float delta, float time, float length)
    {
        base.StartUpdate(caster, delta, time, length);
    }

    public override void CastEvent(GameObject caster)
    {
        base.CastEvent(caster);

        casterEffectSystem.Apply(Effect.Special1Invulnerable, caster);
        casterEffectSystem.Apply(Effect.Special1Speed, caster);
    }

    public override void EndUpdate(GameObject caster, float delta, float time, float length)
    {
        base.EndUpdate(caster, delta, time, length);
    }

    public override void FinishEvent(GameObject caster)
    {
    }

}
