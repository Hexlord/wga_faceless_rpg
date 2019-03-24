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

    protected virtual void OnContact()
    {
        // Intentionally left empty
    }

    protected void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject.TraverseParent();
        string hitTag = other.tag;

        //if (filterTargetTag.Length > 0 &&
        //    target.tag != filterTargetTag) return;

        HealthSystem healthSystem = target.GetComponent<HealthSystem>();
        if (!healthSystem) return;

        switch (hitTag)
        {
            case "Body":
                hitTargets.Add(target);
                healthSystem.Damage(source, damage);
                break;
            case "Environment":
                hitTargets.Add(target);
                break;
            case "Critical":
                hitTargets.Add(target);
                healthSystem.Damage(source, damage);
                break;
            case "Weapon":
                break;
            case "Shield":
                break;
        }
               
        if (uniqueDamage &&
            hitTargets.Contains(target)) return;

        
        OnContact();
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
