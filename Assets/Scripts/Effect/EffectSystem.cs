using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * History:
 *
 * Date         Author      Description
 *
 * 05.04.2019   aknorre     Created
 *
 */
[AddComponentMenu("ProjectFaceless/Creature/Effect System")]
public class EffectSystem : MonoBehaviour
{
    // Public

    [Tooltip("Effects applied to this character")]
    public Effect[] startEffects;

    private List<EffectBase> effects = new List<EffectBase>();
    private List<EffectBase> expiredEffects = new List<EffectBase>();

    public void Apply(Effect effect, GameObject source)
    {
        var e = effect.Instantiate();
        effects.Add(e);
        e.OnApply(gameObject, source);
    }

    public void Purge(Effect effect, GameObject source)
    {
        foreach(var e in effects.ToList())
        {
            if (e.Type == effect)
            {
                e.OnPurge(source);
                effects.Remove(e);
            }
        }
    }

    public void PurgeAll(GameObject source)
    {
        foreach (var e in effects)
        {
            e.OnPurge(source);
        }
        effects.Clear();
    }

    private void Awake()
    {
        // Cache

        foreach (var effect in startEffects)
        {
            effects.Add(effect.Instantiate());
        }
    }

    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;
        foreach (var effect in effects)
        {
            effect.Update(delta);
            if (effect.Expired)
            {
                effect.OnPurge(null);
                expiredEffects.Add(effect);
            }
        }

        foreach (var effect in expiredEffects)
        {
            effects.Remove(effect);
        }
        expiredEffects.Clear();
    }


}
