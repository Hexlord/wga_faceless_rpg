using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class EffectFast : EffectBase
{

    public EffectFast() :
        base(Effect.Special1Speed, 7.5f)
    {
    }

    public override void Update(float delta)
    {
        base.Update(delta);

    }

    public override void OnApply(GameObject target, GameObject source)
    {
        base.OnApply(target, source);
    }

    public override void OnPurge(GameObject source)
    {
        base.OnPurge(source);
    }
}

