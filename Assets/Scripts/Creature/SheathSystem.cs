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

/*
 * Prevents AttackSystem or SkillSystem from attacking or casting until weapon is unsheathed
 */
public class SheathSystem : MonoBehaviour
{
    [AddComponentMenu("ProjectFaceless/Creature")]
    // Public

    public enum SheathSystemState
    {
        Sheathed,
        Unsheathing,
        Unsheathed,
        Sheathing,
    }


    /*
     * Expected animation configuration:
     * 
     * [unsheathTrigger] -> (unsheath) -> (default)
     * [sheathTrigger] -> (sheath) -> (default)
     * 
     */

    [Header("Basic Settings")]
    [Tooltip("Toggles whether creature can unsheathe")]
    public bool canUnsheathe = true;
    [Tooltip("Toggles whether creature can sheathe")]
    public bool canSheathe = true;

    [Header("Animation Settings")]

    public int animationLayer = 0;

    public string sheatheAnimation = "sheathe";
    public string sheatheAnimationTrigger = "sheatheTrigger";

    public string unsheatheAnimation = "unsheathe";
    public string unsheatheAnimationTrigger = "unsheatheTrigger";

    public string idleAnimation = "idle";
    public string unarmedIdleAnimation = "unarmedIdle";

    [Header("Advanced Settings")]
    [Tooltip("Sheathed weapon object")]
    public GameObject weaponSheathed;

    [Tooltip("Unsheathed weapon object")]
    public GameObject weaponUnsheathed;

    public bool Busy
    {
        get
        {
            return
              state == SheathSystemState.Unsheathing ||
              state == SheathSystemState.Sheathing;
        }
    }
    public bool Sheathed
    {
        get
        {
            return 
                state == SheathSystemState.Sheathed ||
                state == SheathSystemState.Sheathing;
        }
    }

    // Private

    [Header("Debug")]
    public SheathSystemState state = SheathSystemState.Sheathed;

    // Cache

    private Animator animator;

    private AttackSystem attackSystem;
    private SkillSystem skillSystem;

    void Start()
    {
        // Cache

        animator = GetComponent<Animator>();

        attackSystem = GetComponent<AttackSystem>();
        skillSystem = GetComponent<SkillSystem>();

        weaponSheathed.SetActive(true);
        weaponUnsheathed.SetActive(false);

        if (attackSystem) attackSystem.canAttack = false;
        if (skillSystem) skillSystem.canCast = false;
    }

    void FixedUpdate()
    {
        bool transition = animator.IsInTransition(animationLayer);

        AnimatorClipInfo info = animator.GetCurrentAnimatorClipInfo(animationLayer)[0];
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayer);
        AnimationClip clip = info.clip;
        string clipName = clip.name;

        if (transition) return;

        switch (state)
        {
            case SheathSystemState.Sheathed:
                break;
            case SheathSystemState.Unsheathing:
                if (clipName == idleAnimation)
                {
                    state = SheathSystemState.Unsheathed;
                    if (attackSystem) attackSystem.canAttack = true;
                    if (skillSystem) skillSystem.canCast = true;
                    
                    weaponSheathed.SetActive(false);
                    weaponUnsheathed.SetActive(true);
                }
                break;
            case SheathSystemState.Unsheathed:
                break;
            case SheathSystemState.Sheathing:
                if (clipName == unarmedIdleAnimation)
                {
                    state = SheathSystemState.Sheathed;
                    if (attackSystem) attackSystem.canAttack = false;
                    if (skillSystem) skillSystem.canCast = false;

                    weaponSheathed.SetActive(true);
                    weaponUnsheathed.SetActive(false);
                }
                break;
        }
    }

    public void Sheath()
    {
        Debug.Assert(!Busy);

        if (!canSheathe) return;

        state = SheathSystemState.Sheathing;
        animator.SetTrigger(sheatheAnimationTrigger);
    }

    public void Unsheath()
    {
        Debug.Assert(!Busy);

        if (!canUnsheathe) return;

        state = SheathSystemState.Unsheathing;
        animator.SetTrigger(unsheatheAnimationTrigger);
    }


}
