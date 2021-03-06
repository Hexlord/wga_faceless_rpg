﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * History:
 *
 * Date         Author      Description
 *
 * 15.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 25.03.2019   bkrylov     Added dashing
 */

/*
 * Blended with other animations additively
 */
[AddComponentMenu("ProjectFaceless/Creature/Movement System")]
public class MovementSystem : MonoBehaviour
{

    // Public

    [Header("Movement Settings")]
    [Tooltip("Toggles ability to move")]
    public bool canMove = true;

    [Tooltip("Movement desire threshold (default = 0.2)")]
    [Range(0.0f, 1.0f)]
    public float movementDesireThreshold = 0.2f;

    [Tooltip("Movement maximum speed")]
    [Range(0.0f, 30.0f)]
    public float baseMovementSpeed = 7.0f;

    [Tooltip("Time to reach maximum speed")]
    [Range(0.0f, 5.0f)]
    public float accelerationTime = 0.3f;

    [Tooltip("Speed multiplier when attacking or casting")]
    [Range(0.0f, 1.0f)]
    public float busyMultiplier = 0.6f;

    [Tooltip("Body rotation stabilization")]
    public bool rotationStabilization = true;

    [Tooltip("Leg touch gravity max")]
    [Range(0.001f, 100.0f)]
    public float gravityMax = 20.0f;

    [Tooltip("Leg touch gravity time to fade")]
    [Range(0.001f, 10.0f)]
    public float gravityFadeTime = 1.0f;

    [Tooltip("Air walk fade time")]
    [Range(0.001f, 10.0f)]
    public float airwalkFadeTime = 1.0f;

    [Header("Animation Settings")]
    [Tooltip("Smoothing factor for transitions")]
    public float animationDamping = 0.15f;

    /*
     * Toggles whether we try to stop forces affecting us
     */
    public bool ResistForces { get; set; }

    private readonly int animatorHorizontal = Animator.StringToHash("Horizontal");
    private readonly int animatorVertical = Animator.StringToHash("Vertical");

    private float currentMovementSpeed;

    private float busyFactor;
    private float landFactor;
    private float gravity;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    private Vector2 appliedVelocity;

    public bool Moving
    {
        get { return desiredMovementBodySpace.sqrMagnitude > Mathf.Epsilon; }
    }

    /*
     * Y stands for forward movement (+W, -S)
     * X stands for right movement   (+D, -A)
     */
    public Vector2 Movement
    {
        get { return desiredMovement; }
        set
        {
            if (value.sqrMagnitude < Mathf.Epsilon) value = Vector2.zero;
            else if (value.sqrMagnitude > 1.0f) value = value.normalized;

            desiredMovement = value;
            OnDesiredMovementUpdate();
        }
    }

    // Private

    [Header("Debug")]
    public Vector2 desiredMovement = Vector2.zero;
    public Vector2 desiredMovementBodySpace = Vector2.zero;

    // Cache

    private Animator animator;
    private Rigidbody body;
    private SheathSystem sheathSystem;
    private TouchCondition legsTouchCondition;
    private SkillSystem skillSystem;
    private AttackSystem attackSystem;

    protected void Awake()
    {
        // Private

        currentMovementSpeed = baseMovementSpeed;
        ResistForces = true;

        // Cache

        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        sheathSystem = GetComponent<SheathSystem>();
        skillSystem = GetComponent<SkillSystem>();
        attackSystem = GetComponent<AttackSystem>();

        var legs = transform.Find("LegsCollider");
        if (legs)
        {
            legsTouchCondition = legs.GetComponent<TouchCondition>();
        }


    }

    private void OnDesiredMovementUpdate()
    {
        var desiredMovementBodySpaceTemp =
            Quaternion.Euler(0.0f, -transform.rotation.eulerAngles.y, 0.0f) *
            (new Vector3(desiredMovement.x, 0.0f, desiredMovement.y));

        desiredMovementBodySpace = new Vector2(desiredMovementBodySpaceTemp.x, desiredMovementBodySpaceTemp.z);

    }

    private void MoveBody(float delta)
    {
        busyFactor = 1.0f;
        if (skillSystem && skillSystem.Busy) busyFactor = busyMultiplier;
        if (attackSystem && attackSystem.Attacking) busyFactor = busyMultiplier;

        landFactor = 1.0f;
        gravity = 0.0f;

        if (legsTouchCondition)
        {
            if (!legsTouchCondition.Touch)
            {
                landFactor = Mathf.Max(0, airwalkFadeTime - legsTouchCondition.DetouchedTime) / airwalkFadeTime * landFactor;

                gravity = gravityMax * Mathf.Max(0, gravityFadeTime - legsTouchCondition.DetouchedTime) * delta;
            }
        }

        currentVelocity = (new Vector2(body.velocity.x, body.velocity.z));
        targetVelocity = desiredMovement * currentMovementSpeed * busyFactor;
        if (!ResistForces)
        {
            targetVelocity += currentVelocity;
        }

        appliedVelocity = Vector2.MoveTowards(
            Vector2.zero,
            targetVelocity - currentVelocity,
            currentMovementSpeed * delta * landFactor / accelerationTime);

        body.AddForce(appliedVelocity.x, -gravity, appliedVelocity.y, ForceMode.VelocityChange);
    }

    private void RotateBody(float delta)
    {
        if (rotationStabilization)
        {
            body.angularVelocity = new Vector3(0.0f, Mathf.Lerp(body.angularVelocity.y, 0.0f, delta), 0.0f);
        }
    }

    protected void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;

        if (animator) animator.SetFloat(animatorHorizontal, desiredMovementBodySpace.x, animationDamping, delta);
        if (animator) animator.SetFloat(animatorVertical, desiredMovementBodySpace.y, animationDamping, delta);

        if (canMove)
        {
            MoveBody(delta);
            RotateBody(delta);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        currentMovementSpeed = newSpeed;
    }

    public void ResetSpeed()
    {
        currentMovementSpeed = baseMovementSpeed;
    }

}
