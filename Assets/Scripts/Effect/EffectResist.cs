using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class EffectResist : EffectBase
{
    private readonly GameObject effectPrefab;
    private GameObject effect;

    public EffectResist() :
        base(Effect.Special1Invulnerable, 7.5f)
    {
        effectPrefab = (GameObject)Resources.Load("Prefabs/Skills/InvulnerableEffect", typeof(GameObject));
    }

    public override void Update(float delta)
    {
        base.Update(delta);

    }

    public override void OnApply(GameObject target, GameObject source)
    {
        base.OnApply(target, source);

        effect = UnityEngine.Object.Instantiate(effectPrefab,
            target.transform.position, Quaternion.identity, 
            target.transform);
    }

    public override void OnPurge(GameObject source)
    {
        base.OnPurge(source);

        if (effect)
        {
            Object.Destroy(effect);
        }
    }
}

