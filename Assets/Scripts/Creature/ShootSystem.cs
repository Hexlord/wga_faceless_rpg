using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
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
    [Tooltip("Stores multiple prefabs to shoot with")]
    public GameObject[] projectilePrefabs = new GameObject[1];
    [Tooltip ("Transform from which the creature shoots")]
    public Transform ShootingPoint;
    [Tooltip("Speed with which a projectile flies. Warning: scales down with rigidbody mass")]
    public float projectileSpeed = 12.0f;
    [Tooltip("Fire rate in projectiles per second")]
    public float fireRate = 2.0f;

    [Header("Animation Settings")]

    public int animationLayer = 0;

    public string shootAnimation = "shoot";
    public string shootRestoreAnimation = "shootRestore";
    public string shootAnimationTrigger = "shootTrigger";

    public string idleAnimation = "idle";

    //private

    private GameObject projectile;
    private Vector3 shootingDirection;
    private float fireTime = 0.0f;
    private float fireTimer = 0.0f;

    public bool Shooting
    {
        get { return state != ShootSystemState.None; }
    }

    // Private

    [Header("Debug")]
    public ShootSystemState state = ShootSystemState.None;

    // Cache

    private Animator animator;

    void Awake()
    {
        // Cache

        animator = GetComponent<Animator>();

        fireTime = 1.0f / fireRate;
    }

    private void SpawnProjectile()
    {
        projectile = Instantiate(projectilePrefabs[0], ShootingPoint.position, ShootingPoint.rotation);
        //TO DO: Find the way to omitt next line
        projectile.GetComponent<CollisionDamageProjectile>().source = gameObject;
        projectile.GetComponent<Rigidbody>().AddForce(shootingDirection * projectileSpeed);
    }


    void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;
        fireTimer = Mathf.MoveTowards(fireTimer, fireTime, delta);

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

        if (!canShoot ||
            fireTimer < fireTime) return;

        fireTimer -= fireTime;

        state = ShootSystemState.Shooting;
        animator.SetTrigger(shootAnimationTrigger);
    }


}
