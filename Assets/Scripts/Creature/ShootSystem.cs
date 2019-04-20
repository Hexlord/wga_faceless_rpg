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
    [Tooltip("Stores multiple prefabs to shoot with")]
    public GameObject[] projectilePrefabs = new GameObject[1];
    [Tooltip ("Transform from which the creature shoots")]
    public Transform ShootingPoint;
    [Tooltip("Speed with which a projectile flies. Warning: scales down with rigidbody mass")]
    public float projectileSpeed = 12.0f;
    [Tooltip("Fire rate in projectiles per second")]
    public float fireRate = 2.0f;

    public string targetTag = "Faceless";

    [Header("Animation Settings")]

    public int animationLayer = 0;

    public string shootAnimation = "shoot";
    public string shootRestoreAnimation = "shootRestore";
    public string shootAnimationTrigger = "shootTrigger";

    public string idleAnimation = "idle";

    //private

    private GameObject projectile;
    private Vector3 shootingDirection;
    private float fireLastTime = 0.0f;
    private int indexOfChosenProjectile = 0;

    public bool Shooting
    {
        get { return state != ShootSystemState.None || (Time.time < fireLastTime + fireRate); }
    }

    public Vector3 ShootingDirection
    {
        get
        {
            return shootingDirection;
        }
        set
        {
            shootingDirection = value.normalized;
        }
    }

    public int ShootingProjectileIndex
    {
        get
        {
            return indexOfChosenProjectile;
        }
        set
        {
            indexOfChosenProjectile = value;
        }
    }

    // Private

    [Header("Debug")]
    public ShootSystemState state = ShootSystemState.None;

    // Cache

    public Vector3 ShootingPointPosition
    {
        get
        {
            return ShootingPoint.position;
        }
    }

    private Animator animator;

    void Awake()
    {
        // Cache

        animator = GetComponent<Animator>();
    }

    private void SpawnProjectile()
    {

        projectile = Instantiate(projectilePrefabs[indexOfChosenProjectile], ShootingPoint.position, ShootingPoint.rotation);
        //TO DO: Find the way to omitt next line
        foreach (CollisionDamageProjectile p in projectile.GetComponentsInChildren<CollisionDamageProjectile>())
        {
            p.source = gameObject;
            p.traverseParentTag = targetTag;
        }
        projectile.GetComponent<Rigidbody>().AddForce(shootingDirection * projectileSpeed);
    }

    public void EndShooting()
    {
        state = ShootSystemState.None;
    }

    public void Shoot(Vector3 direction, int projectileIndex)
    {
        Debug.Assert(!Shooting);
        shootingDirection = direction.normalized;

        if (Shooting || !canShoot ||
            Time.time < fireLastTime + fireRate) return;

        fireLastTime = Time.time;

        state = ShootSystemState.Shooting;
        animator.SetTrigger(shootAnimationTrigger);
        indexOfChosenProjectile = projectileIndex;
    }

}
