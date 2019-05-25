using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public string shootAnimationTrigger = "shootTrigger";

    //private

    private GameObject projectile;
    private AimVectorCalculationDelegate shootingDirection;
    public delegate Vector3 AimVectorCalculationDelegate();
    private float fireLastTime = 0.0f;
    private int indexOfChosenProjectile = 0;

    public bool Shooting
    {
        get { return state != ShootSystemState.None || (Time.time < fireLastTime + fireRate); }
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
        projectile.GetComponent<Rigidbody>().AddForce(shootingDirection.Invoke() * projectileSpeed);
    }
    
    
    private void SpawnPellets(int n, int m)
    {
        float x = 0,
            y = 0, 
            k = 0;
        Vector3 direction = shootingDirection.Invoke();
        Vector3 tan;
        for (int j = 0; j < m; j++)
        {
            k += 0.05f;
            for (int i = 0; i < n; i++)
            {
                x = Random.Range(-Mathf.Sin(0.01f), Mathf.Sin(0.01f));
                y = Random.Range(-Mathf.Sin(0.01f), Mathf.Sin(0.01f));
                tan = new Vector3(x, y, 0);
                Vector3.OrthoNormalize(ref direction, ref tan);
                projectile = Instantiate(projectilePrefabs[indexOfChosenProjectile], ShootingPoint.position,
                    ShootingPoint.rotation);
                //TO DO: Find the way to omitt next line
                foreach (CollisionDamageProjectile p in projectile.GetComponentsInChildren<CollisionDamageProjectile>())
                {
                    p.source = gameObject;
                    p.traverseParentTag = targetTag;
                }

                projectile.GetComponent<Rigidbody>().AddForce((direction + tan * Mathf.Sin(k)) * projectileSpeed);
            }
        }
    }

    public void Spawn18Pellets()
    {
        SpawnPellets(18, 3);
    }
    
    public void EndShooting()
    {
        state = ShootSystemState.None;
    }

    public void Shoot(AimVectorCalculationDelegate direction, int projectileIndex)
    {
        //Debug.Assert(!Shooting);
        shootingDirection = direction;

        if (Shooting || !canShoot ||
            Time.time < fireLastTime + fireRate) return;

        fireLastTime = Time.time;

        state = ShootSystemState.Shooting;
        animator.SetTrigger(shootAnimationTrigger);
        indexOfChosenProjectile = projectileIndex;
    }

}
