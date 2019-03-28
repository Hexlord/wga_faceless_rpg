using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Tooltip("Body rotation stabilization")]
    public bool rotationStabilization = true;


    [Header("Animation Settings")]
    [Tooltip("Smoothing factor for transitions")]
    public float animationDamping = 0.15f;

    private readonly int animatorHorizontal = Animator.StringToHash("Horizontal");
    private readonly int animatorVertical = Animator.StringToHash("Vertical");
    private readonly int animatorWeapon = Animator.StringToHash("Weapon");
    
    private float currentMovementSpeed;

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

    protected void Start()
    {
        // Private

        currentMovementSpeed = baseMovementSpeed;

        // Cache

        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        sheathSystem = GetComponent<SheathSystem>();

        
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
        var deltaVelocity = desiredMovement * currentMovementSpeed -
            (new Vector2(body.velocity.x, body.velocity.z));

        var appliedVelocity = Vector2.MoveTowards(
            Vector2.zero,
            deltaVelocity,
            currentMovementSpeed * delta * 10.0f);

        body.AddForce(appliedVelocity.x, 0.0f, appliedVelocity.y, ForceMode.VelocityChange);
    }

    private void RotateBody(float delta)
    {
        if(rotationStabilization)
        {
            body.angularVelocity = new Vector3(0.0f, Mathf.Lerp(body.angularVelocity.y, 0.0f, delta), 0.0f);
        }
    }

    protected void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;
        
        animator.SetFloat(animatorHorizontal, desiredMovementBodySpace.x, animationDamping, delta);
        animator.SetFloat(animatorVertical, desiredMovementBodySpace.y, animationDamping, delta);
        animator.SetFloat(animatorWeapon, sheathSystem.Sheathed ? 0.0f : 1.0f, animationDamping, delta);

        MoveBody(delta);
        RotateBody(delta);
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
