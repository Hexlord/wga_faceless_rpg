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
public class CollisionDamageProjectile : CollisionDamageBasic
{
    [AddComponentMenu("ProjectFaceless/GameLogic")]
    // Public

    [Header("Projectile Settings")]
    [Tooltip("Number of targets pierced (destroyed after hitting that number of target colliders)")]
    public int pierceCount = 1;
    
    [Tooltip("Projectile object (defaulted to owner object)")]
    public GameObject projectile;

    // Private

    private int pierceCounter = 0;

    // Cache


    protected override void OnDamage(GameObject source, float amount)
    {
        ++pierceCounter;

        if(pierceCounter >= pierceCount)
        {
            Destroy(projectile ? projectile : gameObject);
        }
    }
       


}
