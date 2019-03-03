using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   bkrylov     Created
 * 
 */
public class DumbSniperComponent : BaseCharacter
{
    [SerializeField]
    private Transform ShootingPoint;

    [SerializeField]
    private GameObject magicProjectile;

    [SerializeField]
    private float magicalDamage = 25.0f;

    [SerializeField]
    private GameObject physicalProjectile;

    [SerializeField]
    private float physicalDamage = 25.0f;

    [SerializeField]
    private float projectileSpeed = 12.0f;

    [SerializeField]
    private float attackCooldown = 1.0f;

    [SerializeField]
    private string target = "Player";

    private GameObject instanceProjectile;

    private Transform playerTransform;

    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.Find("Player").transform;
    }

    private void Awake()
    {
        InvokeRepeating("Attack", 0.0f, attackCooldown);
    }

    //protected override void FixedUpdate()
    //{
    //    distanceToPlayer = (character.transform.position - gameObject.transform.position);
    //    distanceToPlayer.y = 0;
    //    if (distanceToPlayer.magnitude > attackRange)
    //    {
    //        distanceToPlayer.Normalize();
    //        transform.forward = distanceToPlayer;
    //        CurrentDirection = distanceToPlayer.normalized;
    //    }
    //    base.Move();
    //}

    protected virtual void Attack()
    {
        Ray rayToPlayer = new Ray(ShootingPoint.position, playerTransform.position + Vector3.up - ShootingPoint.position);
        RaycastHit hit;
        Vector3 shootingDirection;
        int mask = LayerMask.GetMask("Environment", "Enemy");
        float dist = 1000.0f;

        Physics.Raycast(rayToPlayer, out hit, dist, mask, QueryTriggerInteraction.Ignore);
        shootingDirection = (hit.point - ShootingPoint.position).normalized;

        
        
            if (Random.value > 0.5f)
            {
                instanceProjectile = Instantiate(physicalProjectile, ShootingPoint.position, ShootingPoint.rotation);
                Weapon projectileWeaponComponent = instanceProjectile.GetComponent<Weapon>();
                projectileWeaponComponent.SetWielder(this);
                projectileWeaponComponent.TargetTag = target;
                projectileWeaponComponent.Damage = physicalDamage;
            }
            else
            {
                instanceProjectile = Instantiate(magicProjectile, ShootingPoint.position, ShootingPoint.rotation);
                Weapon projectileWeaponComponent = instanceProjectile.GetComponent<Weapon>();
                projectileWeaponComponent.SetWielder(this);
                projectileWeaponComponent.TargetTag = target;
                projectileWeaponComponent.Damage = magicalDamage;
            }

            instanceProjectile.GetComponent<Rigidbody>().AddForce(shootingDirection * projectileSpeed);
        
    }
}
