﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 25.03.2019   bkrylov     fixed bug: player can damage mobs while being idle by brushing sword hitbox against enemy hitbox
 * 
 */
[AddComponentMenu("ProjectFaceless/Creature/Attack System")]
public class AttackSystem : MonoBehaviour
{
    
    // Public

    public enum AttackSystemState
    {
        None,

        Attacking,
    }


    /*
     * Expected animation configuration:
     * 
     * [attackTrigger] -> (attack) -> (idle)
     * 
     */
    
    [Header("Basic Settings")]
    [Tooltip("Toggles whether creature can attack")]
    public bool canAttack = true;

    [Header("Animation Settings")]

    //public string attackAnimation = "attack";
    public string[] attackAnimationTriggers = { "attackTrigger" };
    //public string interruptAnimation = "interruptedAttack";
    public string interruptAnimationTrigger = "interruptedAttackTrigger";

    public CollisionDamageBasic[] weapons;

    public bool Attacking
    {
        get { return state != AttackSystemState.None; }
    }

    // Private
    private int activeWeaponIndex = 0;
    //Testing
    [Header("Debug")]
    public AttackSystemState state = AttackSystemState.None;

    // Cache

    private Animator animator;
    private MovementSystem movementSystem;
    private Random random = new Random();

    void Awake()
    {
        // Cache

        animator = GetComponent<Animator>();
        movementSystem = GetComponent<MovementSystem>();
        for (var i = 0; i < weapons.Length; i++)
        {
            weapons[i].DealsDamage = false;
        }
    }

    public void Attack(int attackIndex, int weaponIndex)
    {
        activeWeaponIndex = weaponIndex;
        weapons[activeWeaponIndex].DealsDamage = true;
        //Debug.Assert(!Attacking);

        if (!canAttack) return;

        /*
         if (movementSystem &&
            movementSystem.Moving) return;
            */

        state = AttackSystemState.Attacking;
        if (attackIndex == -1)
        {
            attackIndex = random.Next(attackAnimationTriggers.Length);
        }
        animator.SetTrigger(attackAnimationTriggers[attackIndex]);
    }

    public void AttackInterrupted()
    {
        animator.SetTrigger(interruptAnimationTrigger);
    }

    public void FinalizeAttack()
    {
        if (activeWeaponIndex != -1)
        {
            state = AttackSystemState.None;
            weapons[activeWeaponIndex].DealsDamage = false;
            weapons[activeWeaponIndex].ResetHitTargets();
            activeWeaponIndex = -1;
        }
    }
}
