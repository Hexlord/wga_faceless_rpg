using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbEnemy : BaseCharacter
{
    [SerializeField]
    private float attackCooldown;

    [SerializeField]
    private float damage;

    [SerializeField]
    private float velocity;

    [SerializeField]
    private float attackRange;

    private GameObject character;
    private CharacterController characterController;
    private HealthSystem healthSystem;
    private Vector3 distanceToPlayer;
    private bool isNotified = false;

    public void notify()
    {
        isNotified = !isNotified;
    }

    void Start()
    {
        character = GameObject.FindGameObjectWithTag("Player");
        healthSystem = character.GetComponent<HealthSystem>();
        characterController = gameObject.GetComponent<CharacterController>();
        
    }

    private void Awake()
    {
        InvokeRepeating("Attack", 0.0f, attackCooldown);
    }

    void FixedUpdate()
    {
        distanceToPlayer = (character.transform.position - gameObject.transform.position);
        distanceToPlayer.y = 0;
        if (isNotified == true && distanceToPlayer.magnitude > attackRange * 0.5)
        {
            characterController.Move((character.transform.position - gameObject.transform.position).normalized * (velocity * Time.fixedDeltaTime));
            transform.forward = distanceToPlayer;
        }
    }

    private void Attack()
    {
        if (distanceToPlayer.magnitude >= attackRange || isNotified == false)
            return;
        healthSystem.DealDamage(damage);
    }
}
