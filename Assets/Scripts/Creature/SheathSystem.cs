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
[AddComponentMenu("ProjectFaceless/Creature/Sheath System")]
public class SheathSystem : MonoBehaviour
{
    
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

    private readonly int sheatheAnimationTrigger = Animator.StringToHash("sheatheTrigger");
    private readonly int unsheatheAnimationTrigger = Animator.StringToHash("unsheatheTrigger");

    [Tooltip("Smoothing factor for transitions")]
    public float animationDamping = 0.15f;

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
                state == SheathSystemState.Unsheathing;
        }
    }

    public SheathSystemState State
    {
        get { return state; }
    }

    // Private

    [Header("Debug")]
    public SheathSystemState state = SheathSystemState.Sheathed;
    
    private readonly int animatorWeapon = Animator.StringToHash("Weapon");

    // Cache

    private Animator animator;

    private AttackSystem attackSystem;
    private SkillSystem skillSystem;

    private void Start()
    {
        // Cache

        animator = GetComponent<Animator>();

        attackSystem = GetComponent<AttackSystem>();
        skillSystem = GetComponent<SkillSystem>();

        weaponSheathed.SetActive(true);
        weaponUnsheathed.SetActive(false);
    }

    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;
        
        if(animator) animator.SetFloat(animatorWeapon, Sheathed ? 0.0f : 1.0f, animationDamping, delta);
    }

    public void Sheathe()
    {
        Debug.Assert(!Busy);

        if (!canSheathe) return;

        state = SheathSystemState.Sheathing;
        animator.SetTrigger(sheatheAnimationTrigger);
    }

    public void Unsheathe()
    {
        Debug.Assert(!Busy);

        if (!canUnsheathe) return;

        state = SheathSystemState.Unsheathing;
        animator.SetTrigger(unsheatheAnimationTrigger);
    }

    public void FinalizeSheathing()
    {
        Debug.Assert(state == SheathSystemState.Sheathing);

        state = SheathSystemState.Sheathed;

        if (weaponSheathed) weaponSheathed.SetActive(true);
        if (weaponUnsheathed) weaponUnsheathed.SetActive(false);
    }
    public void FinalizeUnsheathing()
    {
        Debug.Assert(state == SheathSystemState.Unsheathing);

        state = SheathSystemState.Unsheathed;
        
        if(weaponSheathed) weaponSheathed.SetActive(false);
        if (weaponUnsheathed) weaponUnsheathed.SetActive(true);
    }
}
