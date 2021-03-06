﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   bkrylov     Created
 * 
 */
public class DumbEnemy : BaseCharacter
{

    //public Weapon rightFist, leftFist;
    public float fistDamage = 5.0f;

    [SerializeField]
    private float attackCooldown;

    [SerializeField]
    private float damage;

    [SerializeField]
    private float attackRange;

    private GameObject character;
    private NavMeshAgent navMeshAgent;
    //private BasicStatusSystem healthSystem;
    private Vector3 distanceToPlayer;
    private bool isNotified = false;

    public void Notify(string tag)
    {
        if (!isNotified)
        {
            isNotified = true;
            //rightFist.TargetTag = tag;
            //leftFist.TargetTag = tag;
            //character = GameObject.FindWithTag(tag);
            //healthSystem = character.GetComponent<BasicStatusSystem>();
            Debug.Log("notify");
        }
    }

    protected override void Start()
    {
        base.Start();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        //rightFist.Damage = fistDamage;
        //leftFist.Damage = fistDamage;
    }

    private void Awake()
    {
        InvokeRepeating("Attack", 0.0f, attackCooldown);
    }

    protected override void FixedUpdate()
    {
        if (isNotified)
        {
            distanceToPlayer = (character.transform.position - gameObject.transform.position);
            distanceToPlayer.y = 0;
            if (distanceToPlayer.magnitude > attackRange)
            {
                navMeshAgent.SetDestination(character.transform.position);
            }
        }
        base.Move();
    }

    protected virtual void Attack()
    {
        if (distanceToPlayer.magnitude >= attackRange || !isNotified)
            return;
        anim.SetTrigger("Attack");
    }
}
