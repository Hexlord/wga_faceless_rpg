using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase
{
    public bool Expired
    {
        get { return timer >= lifetime; }
    }

    public Effect Type
    {
        get { return type; }
    }

    public EffectBase(Effect type, float lifetime)
    {
        this.name = type.ToString();
        this.lifetime = lifetime;
        this.timer = 0.0f;
        this.type = type;
    }

    public virtual void Update(float delta)
    {
        timer += delta;
    }
    
    public virtual void OnApply(GameObject target, GameObject source)
    {
        this.source = source;
        this.target = target;
        targetHealth = this.target.GetComponent<HealthSystem>();

    }

    public virtual void OnPurge(GameObject source)
    {

    }

    private readonly string name;
    private readonly float lifetime;
    private float timer;
    protected GameObject source;
    protected GameObject target;
    protected HealthSystem targetHealth;
    private Effect type;




}

