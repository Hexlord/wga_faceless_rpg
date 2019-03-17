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
 * 17.03.2019   brylov      Made SpawnProjectile method added multiple projectile storage
 * 
 */
[AddComponentMenu("ProjectFaceless/Creature/Shoot System")]
public class ShootSystem : MonoBehaviour
{    // Public

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
    //TO DO: randomize projectile spawner, add multiple projectile support
    public GameObject[] projectilePrefabs = new GameObject[1];
    public Transform ShootingPoint;
    public string targetTag;
    
    public float projectileSpeed = 12.0f;

    [Header("Animation Settings")]

    public int animationLayer = 0;

    public string shootAnimation = "shoot";
    public string shootRestoreAnimation = "shootRestore";
    public string shootAnimationTrigger = "shootTrigger";

    public string idleAnimation = "idle";

    //private

    private GameObject projectile;
    private Vector3 shootingDirection;

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
        projectile = Instantiate(projectilePrefabs[0], ShootingPoint.position, ShootingPoint.rotation);
        projectile.GetComponent<Rigidbody>().AddForce(shootingDirection * projectileSpeed);
    }


    void FixedUpdate()
    {
        bool transition = animator.IsInTransition(animationLayer);

        AnimatorClipInfo info = animator.GetCurrentAnimatorClipInfo(animationLayer)[0];
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayer);
        AnimationClip clip = info.clip;
        string clipName = clip.name;

        bool isDefaultClip = clipName == idleAnimation;
        isDefaultClip = true; // TODO: remove when animations ready

        if (transition) return;

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

    public void Shoot(Vector3 direction)
    {
        Debug.Assert(!Shooting);
        shootingDirection = direction.normalized;

        if (!canShoot) return;

        state = ShootSystemState.Shooting;
        animator.SetTrigger(shootAnimationTrigger);
    }


}
