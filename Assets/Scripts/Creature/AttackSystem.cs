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
     * [attackTrigger] -> (attack) -> (default)
     * 
     */

    [Header("Basic Settings")]
    [Tooltip("Toggles whether creature can attack")]
    public bool canAttack = true;

    [Header("Animation Settings")]

    public int animationLayer = 0;

    public string attackAnimation = "attack";
    public string attackAnimationTrigger = "attackTrigger";

    public string defaultAnimation = "default";

    public bool Attacking
    {
        get { return state != AttackSystemState.None; }
    }

    // Private

    [Header("Debug")]
    public AttackSystemState state = AttackSystemState.None;

    // Cache

    private Animator animator;

    void Start()
    {
        // Cache

        animator = GetComponent<Animator>();

    }

    void FixedUpdate()
    {
        AnimatorClipInfo info = animator.GetCurrentAnimatorClipInfo(animationLayer)[0];
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayer);
        AnimationClip clip = info.clip;
        string clipName = clip.name;

        bool isDefaultClip = clipName == defaultAnimation;
        isDefaultClip = true; // TODO: remove when animations ready

        switch (state)
        {
            case AttackSystemState.None:
                break;
            case AttackSystemState.Attacking:
                if (isDefaultClip) state = AttackSystemState.None;
                break;
        }
    }

    public void Attack()
    {
        Debug.Assert(!Attacking);

        if (!canAttack) return;

        state = AttackSystemState.Attacking;
        animator.SetTrigger(attackAnimationTrigger);
    }


}
