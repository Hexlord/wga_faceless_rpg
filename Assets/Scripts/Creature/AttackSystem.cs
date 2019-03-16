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
 * 
 */
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
    public string attackAnimationTrigger = "attackTrigger";

    public string idleAnimation = "idle";

    public bool Attacking
    {
        get { return state != AttackSystemState.None; }
    }

    // Private

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
                }
                break;
        }
    }

    public void Attack()
    {
        Debug.Assert(!Attacking);

        if (!canAttack) return;

        if (movementSystem &&
            movementSystem.Moving) return;

        state = AttackSystemState.Attacking;
        animator.SetTrigger(attackAnimationTrigger);
    }


}
