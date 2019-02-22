using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbEnemy : BaseCharacter
{

    public Weapon rightFist, leftFist;
    public float fistDamage = 5.0f;
    public float mass = 1.0f;

    [SerializeField]
    private float attackCooldown;

    [SerializeField]
    private float damage;

    [SerializeField]
    private float velocity;

    [SerializeField]
    private float attackRange;

    private Animator anim;
    private GameObject character;
    private CharacterController characterController;
    private BasicStatusSystem healthSystem;
    private Vector3 distanceToPlayer;
    private bool isNotified = false;
    private string tag;

    public void Notify(string tag)
    {
        if (!isNotified)
        {
            isNotified = true;
            rightFist.TargetTag = tag;
            leftFist.TargetTag = tag;
            character = GameObject.FindWithTag(tag);
            healthSystem = character.GetComponent<BasicStatusSystem>();
            Debug.Log("notify");
        }
    }

    void Start()
    {
        
        characterController = gameObject.GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        rightFist.Damage = fistDamage;
        leftFist.Damage = fistDamage;
    }

    private void Awake()
    {
        InvokeRepeating("Attack", 0.0f, attackCooldown);
    }

    void FixedUpdate()
    {
        if (isNotified)
        {
            distanceToPlayer = (character.transform.position - gameObject.transform.position);
            distanceToPlayer.y = 0;
            if (distanceToPlayer.magnitude > attackRange * 0.5)
            {
                distanceToPlayer.Normalize();
                transform.forward = distanceToPlayer;
                Vector3 MoveDirection = distanceToPlayer + ((!characterController.isGrounded) ? new Vector3(0, Physics.gravity.y * mass, 0) : Vector3.zero);
                characterController.Move(MoveDirection.normalized * (velocity * Time.fixedDeltaTime));
            }
        }
    }

    private void Attack()
    {
        if (distanceToPlayer.magnitude >= attackRange || isNotified == false)
            return;
        rightFist.TriggerStricking();
        leftFist.TriggerStricking();
        anim.SetTrigger("Attack");
    }
}
