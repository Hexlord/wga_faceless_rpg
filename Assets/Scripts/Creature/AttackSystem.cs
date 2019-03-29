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

    public int animationLayer = 0;

    public string attackAnimation = "attack";
    public string[] attackAnimationTriggers = { "attackTrigger" };
    public string interruptAnimation = "interruptedAttack";
    public string interruptAnimationTrigger = "interruptedAttackTrigger";

    public string idleAnimation = "idle";

    public CollisionDamageBasic[] weapons;

    public bool Attacking
    {
        get { return state != AttackSystemState.None; }
    }

    // Private
    private int activeWeaponIndex = -1;
    //Testing
    [Header("Debug")]
    public AttackSystemState state = AttackSystemState.None;

    // Cache

    private Animator animator;
    private MovementSystem movementSystem;

    void Start()
    {
        // Cache

        animator = GetComponent<Animator>();
        movementSystem = GetComponent<MovementSystem>();
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Active = false;
        }
    }

    void FixedUpdate()
    {
        bool transition = animator.IsInTransition(animationLayer);
        AnimatorClipInfo info = animator.GetCurrentAnimatorClipInfo(animationLayer)[0];
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayer);
        AnimationClip clip = info.clip;
        string clipName = clip.name;

        bool isDefaultClip = clipName == idleAnimation;

        if (transition) return;

        switch (state)
        {
            case AttackSystemState.None:
                break;
            case AttackSystemState.Attacking:
                if (isDefaultClip)
                {
                    state = AttackSystemState.None;
                    weapons[activeWeaponIndex].Active = false;
                    weapons[activeWeaponIndex].ResetHitTargets();
                    activeWeaponIndex = -1;
                }
                break;
        }
    }

    public void Attack(int attackIndex, int weaponIndex)
    {
        activeWeaponIndex = weaponIndex;
        weapons[activeWeaponIndex].Active = true;
        Debug.Assert(!Attacking);

        if (!canAttack) return;

        if (movementSystem &&
            movementSystem.Moving) return;

        state = AttackSystemState.Attacking;
        animator.SetTrigger(attackAnimationTriggers[attackIndex]);
    }

    public void AttackInterrupted()
    {
        //TO DO: Interrupt attack;
    }
}
