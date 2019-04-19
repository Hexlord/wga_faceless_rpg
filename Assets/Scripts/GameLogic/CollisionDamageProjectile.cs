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
 * 25.03.2019   bkrylov     Remade Component for better collider filtration;
 * 
 */
[AddComponentMenu("ProjectFaceless/GameLogic/Projectile Damage")]
public class CollisionDamageProjectile : CollisionDamageBasic
{

    // Public

    [Header("Projectile Settings")]
    [Tooltip("Number of targets pierced (destroyed after hitting that number of target colliders)")]
    public int pierceCount = 1;
    
    [Tooltip("Projectile object (defaulted to owner object)")]
    public GameObject projectile;

    // Private

    private int pierceCounter = 0;

    public override void OnContact()
    {

        ++pierceCounter;

        if(pierceCounter >= pierceCount)
        {
            Destroy(projectile ? projectile : gameObject);
        }
    }

    protected override void Interrupted(Collider other)
    {
        Destroy(this.gameObject);
    }



}
