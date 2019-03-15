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
public class ShootSystem : MonoBehaviour
{

    // Public

    public enum ShootSystemState
    {
        None,

        Shooting,
        Restoring,
    }


    /*
     * Expected animation configuration:
     * 
     * [shootTrigger] -> (shoot) -> (shootRestore) -> (default)
     * 
     */

    [Header("Basic Settings")]
    [Tooltip("Toggles whether creature can shoot")]
    public bool canShoot = true;

    [Header("Animation Settings")]

    public int animationLayer = 0;

    public string shootAnimation = "shoot";
    public string shootRestoreAnimation = "shootRestore";
    public string shootAnimationTrigger = "shootTrigger";

    public string defaultAnimation = "default";

    public bool Shooting
    {
        get { return state != ShootSystemState.None; }
    }

    // Private

    [Header("Debug")]
    public ShootSystemState state = ShootSystemState.None;

    // Cache

    private Animator animator;

    void Start()
    {
        // Cache

        animator = GetComponent<Animator>();

    }

    private void SpawnProjectile()
    {

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
            case ShootSystemState.None:
                break;
            case ShootSystemState.Shooting:
                if(isDefaultClip || clipName == shootRestoreAnimation)
                {
                    SpawnProjectile();
                }

                if (clipName == shootRestoreAnimation) state = ShootSystemState.Restoring;
                if (isDefaultClip) state = ShootSystemState.None; // restoring already passed
                break;
            case ShootSystemState.Restoring:
                if (isDefaultClip) state = ShootSystemState.None;
                break;
        }
    }

    public void Shoot()
    {
        Debug.Assert(!Shooting);

        if (!canShoot) return;

        state = ShootSystemState.Shooting;
        animator.SetTrigger(shootAnimationTrigger);
    }


}
