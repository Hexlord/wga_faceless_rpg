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
        InvokeRepeating("Attack", 0.0f, attackCooldown);
    }

    void FixedUpdate()
    {
        if(isNotified == true)
            characterController.Move((character.transform.position - gameObject.transform.position).normalized * (velocity * Time.fixedDeltaTime));
    }

    private void Attack()
    {
        if ((character.transform.position - gameObject.transform.position).magnitude >= attackRange || isNotified == false)
            return;
        healthSystem.DealDamage(damage);
    }
}
