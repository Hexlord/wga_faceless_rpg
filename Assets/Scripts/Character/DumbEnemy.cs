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
    private float rotationVelocity;

    [SerializeField]
    private float attackRange;

    private GameObject character;
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

        InvokeRepeating("Attack", 0.0f, attackCooldown);
    }

    private bool isFar = false;
    void FixedUpdate()
    {
        if ((character.transform.position - gameObject.transform.position).magnitude > 20 && !isFar)
        {
            velocity += 100;
            isFar = true;
        }
        if ((character.transform.position - gameObject.transform.position).magnitude < 20 && isFar )
        {
            isFar = false;
            velocity -= 100;
        }
        if (isNotified)
        {
            if (Rotate())
            {
                Go();
            }
        }
    }

    private bool checkDistance()
    {
        Vector3 characterPos = character.transform.position;
        Vector3 enemyPos = gameObject.transform.position;
        characterPos.y = 0;
        enemyPos.y = 0;
        if (Vector3.Distance(characterPos, enemyPos) < attackRange)
            return true;
        else
            return false;
    }

    private void Attack()
    {
        if (!checkDistance())
            return;
        healthSystem.DealDamage(damage);
    }

    private Vector3 getDirection()
    {
        Vector3 direction = character.transform.position - gameObject.transform.position;
        direction.y = 0;
        return direction.normalized;
    }

    private float getAngle(Vector3 direction)
    {
        return  Vector3.SignedAngle(gameObject.transform.forward, direction, gameObject.transform.up);
    }

    private bool Rotate()
    {
        float angle = getAngle(getDirection());
        float angleToRotate = Mathf.Abs(angle) <= Mathf.Abs(rotationVelocity * Time.deltaTime) ? Mathf.Abs(angle) : Mathf.Abs(rotationVelocity * Time.deltaTime);
        angleToRotate *= angle >= 0 ? 1 : -1;
        bool isDone = angleToRotate == angle;

        Quaternion rotation = Quaternion.Euler(0, angleToRotate, 0);
        gameObject.transform.rotation *= rotation;
        return isDone;
    }

    private void Go()
    {
        gameObject.transform.Translate(Vector3.forward * (velocity * Time.fixedDeltaTime));
    }
}
