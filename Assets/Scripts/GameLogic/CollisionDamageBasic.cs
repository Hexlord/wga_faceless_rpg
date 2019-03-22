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
 * 
 */
[AddComponentMenu("ProjectFaceless/GameLogic/Collision Damage Basic")]
public class CollisionDamageBasic : MonoBehaviour
{

    // Public

    public enum DamageBodyState
    {
        Any,
        Physical,
        Magical
    }


    [Header("Basic Settings")]
    [Tooltip("Toggles whether it does damage")]
    public bool canDamage = true;

    [Tooltip("What body states can be damaged")]
    public DamageBodyState damageBodyState = DamageBodyState.Any;

    [Tooltip("Object that does the damage")]
    public GameObject source;
    
    [Tooltip("Object that never receives the damage (if any)")]
    public GameObject negativeFilterTarget;

    [Tooltip("Object tag that never receives the damage (if any)")]
    public string negativeFilterTargetTag;

    [Tooltip("Object that receives the damage (if any)")]
    public GameObject filterTarget;

    [Tooltip("Object tag that receives the damage (if any)")]
    public string filterTargetTag;

    [Tooltip("Amount of damage done on collision")]
    [Range(0.0f, 1000.0f, order = 2)]
    public float damage = 10.0f;

    [Tooltip("Unique damage (only damage each GameObject once per lifetime)")]
    public bool uniqueDamage = true;

    // Private

    [Header("Debug")]

    // Cache

    private Animator animator;
    private new Collider collider;

    private ArrayList hitTargets = new ArrayList();

    protected void Awake()
    {
        // Cache

        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();

        if (negativeFilterTarget) collider.IgnoreCollisionsWith(negativeFilterTarget);
        if (negativeFilterTargetTag.Length > 0) collider.IgnoreCollisionsWith(negativeFilterTargetTag);
    }

    protected virtual void OnDamage(GameObject source, float amount)
    {
        // Intentionally left empty
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (filterTarget &&
            !other.IsPartOf(filterTarget)) return;

        GameObject target = other.gameObject.TraverseParent();

        //if (filterTargetTag.Length > 0 &&
        //    target.tag != filterTargetTag) return;
        
        HealthSystem healthSystem = target.GetComponent<HealthSystem>();
        if (!healthSystem) return;

        if(damageBodyState != DamageBodyState.Any)
        {
            BodyStateSystem bodyStateSystem = target.GetComponent<BodyStateSystem>();
            if (!bodyStateSystem) return;

            if (bodyStateSystem.State == BodyStateSystem.BodyState.Physical &&
                damageBodyState == DamageBodyState.Magical ||
                bodyStateSystem.State == BodyStateSystem.BodyState.Magical &&
                damageBodyState == DamageBodyState.Physical)
            {
                //Add visual effects
                return;
            }
        }
        
        if (uniqueDamage &&
            hitTargets.Contains(target)) return;

        hitTargets.Add(target);
        healthSystem.Damage(source, damage);
        OnDamage(source, damage);
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
