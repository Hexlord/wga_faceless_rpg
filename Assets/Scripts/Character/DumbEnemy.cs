using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbEnemy : BaseCharacter
{

    public Weapon rightFist, leftFist;
    public float fistDamage = 5.0f;

    [SerializeField]
    private float attackCooldown;

    [SerializeField]
    private float damage;

    [SerializeField]
    private float attackRange;

    private GameObject character;
    private CharacterController characterController;
    private HealthSystem healthSystem;

    private Vector3 distanceToPlayer;
    private bool isNotified = false;

    public void Notify(string tag)
    {
        if (!isNotified)
        {
            isNotified = true;
            rightFist.TargetTag = tag;
            leftFist.TargetTag = tag;
            character = GameObject.FindWithTag(tag);
            healthSystem = character.GetComponent<HealthSystem>();
            Debug.Log("notify");
        }
    }

    protected override void Start()
    {
        base.Start();
        characterController = gameObject.GetComponent<CharacterController>();
        rightFist.Damage = fistDamage;
        leftFist.Damage = fistDamage;
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
                distanceToPlayer.Normalize();
                transform.forward = distanceToPlayer;
                CurrentDirection = distanceToPlayer.normalized;
            }
        }
        base.Move();
    }

    private void Attack()
    {
        if (distanceToPlayer.magnitude >= attackRange || isNotified == false)
            return;
        anim.SetTrigger("Attack");
    }
}
