using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 *
 * Date         Author      Description
 *
 * 15.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 25.03.2019   bkrylov     Remade Component to better collider filtration
 *
 */
[AddComponentMenu("ProjectFaceless/GameLogic/Collision Damage Basic")]
public class CollisionDamageBasic : MonoBehaviour
{

    // Public

    [Header("Basic Settings")]
    [Tooltip("Toggles whether it does damage")]
    public bool canDamage = true;

    [Tooltip("Object that does the damage")]
    public GameObject source;

    [Tooltip("Object that never receives the damage (if any)")]
    public GameObject negativeFilterTarget;

    [Tooltip("Object tag that never receives the damage (if any)")]
    public string negativeFilterTargetTag;

    [Tooltip("Amount of damage done on collision")]
    [Range(0.0f, 1000.0f, order = 2)]
    public float damage = 10.0f;

    [Tooltip("Amount of damage to shield HP done on collision with shields. Always unique")]
    [Range(0.0f, 1000.0f, order = 2)]
    public float shieldDamage = 10.0f;

    [Tooltip("Unique damage (only damage each GameObject once per lifetime)")]
    public bool uniqueDamage = true;

    [Tooltip("Effects to apply on damage")]
    public Effect[] effectsOnDamage;

    public bool Active
    {
        get { return active; }
        set { active = value; }
    }


    // Cache
    private AttackSystem sourceAttackSystem;
    private bool sourceAttackSystemCached = false;
    private new Collider collider;

    // Private

    [Header("Debug")]

    private ArrayList hitTargets = new ArrayList();

    private bool active = true;

    private AttackSystem SourceAttackSystem
    {
        get
        {
            if (!sourceAttackSystemCached && source)
            {
                sourceAttackSystem = source.GetComponent<AttackSystem>();
            }

            sourceAttackSystemCached = true;

            return sourceAttackSystem;
        }
    }


    protected void Awake()
    {
        collider = GetComponent<Collider>();

        if (negativeFilterTarget) collider.IgnoreCollisionsWith(negativeFilterTarget);
        if (negativeFilterTargetTag.Length > 0) collider.IgnoreCollisionsWith(negativeFilterTargetTag);
    }

    protected virtual void Interrupted(Collider other)
    {
        if (SourceAttackSystem)
        {
            SourceAttackSystem.AttackInterrupted();
        }
    }

    public virtual void OnContact()
    {
        // Intentionally left empty
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!canDamage || !active) return;

        var target = other.gameObject.TraverseParent("Faceless");
        if (!target) return;

        var hitTag = other.tag;

        //if (filterTargetTag.Length > 0 &&
        //    target.tag != filterTargetTag) return;

        BodyStateSystem bodyState = target.GetComponent<BodyStateSystem>();
        if ((!bodyState) || (BodyStateSystem.StateToLayer(bodyState.State) == this.gameObject.layer))
        {

            if (uniqueDamage &&
                hitTargets.Contains(target)) return;

            var hit = hitTag == "Critical" || hitTag == "Body";

            /*
             * Apply effects before the damage
             */
            if (hit)
            {
                var effectSystem = target.GetComponent<EffectSystem>();
                if (effectSystem)
                {
                    foreach(var effect in effectsOnDamage)
                    {
                        effectSystem.Apply(effect, source);
                    }
                }
            }

            switch (hitTag)
            {
                case "Environment":
                    hitTargets.Add(target);
                    break;
                case "Critical":
                    hitTargets.Add(target);
                    target.GetComponent<HealthSystem>().Damage(source, damage);
                    break;
                case "Weapon":
                    break;
                case "Shield":
                    Interrupted(other);
                    target.GetComponent<ShieldSystem>().RecieveDamage(shieldDamage);
                    break;
                case "Body":
                    hitTargets.Add(target);
                    target.GetComponent<HealthSystem>().Damage(source, damage);
                    break;
            }

            OnContact();
        }
        else
        {
            //TO DO: Show VFX if damage isn't dealt;
        }

    }

    /*
     * Should be used at the end of attack animation
     * To allow hitting same target on the next attack
     */
    public void ResetHitTargets()
    {
        hitTargets.Clear();
    }
}
