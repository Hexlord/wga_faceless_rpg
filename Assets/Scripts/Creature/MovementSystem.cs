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
 * 
 */

public class MovementSystem : MonoBehaviour
{

    // Public

    [Header("Movement Settings")]
    [Tooltip("Movement desire threshold (default = 0.2)")]
    [Range(0.0f, 1.0f)]
    public float movementDesireThreshold = 0.2f;


    [Tooltip("Movement maximum speed")]
    [Range(0.0f, 30.0f)]
    public float movementSpeed = 7.0f;

    public enum MovementSystemState
    {
        Idle,


        MovingForward,
        MovingBackward,
        MovingLeft,
        MovingRight,
    }

    public MovementSystemState state = MovementSystemState.Idle;
    
    [Header("Animation Settings")]

    public string idleAnimation = "idle";
    public string movingForwardAnimation = "moveForward";
    public string movingBackwardAnimation = "moveBackward";
    public string movingLeftAnimation = "moveLeft";
    public string movingRightAnimation = "moveRight";

    public string idleAnimationTrigger = "idleTrigger";
    public string movingForwardAnimationTrigger = "moveForwardTrigger";
    public string movingBackwardAnimationTrigger = "moveBackwardTrigger";
    public string movingLeftAnimationTrigger = "moveLeftTrigger";
    public string movingRightAnimationTrigger = "moveRightTrigger";
       
    public bool Moving
    {
        get { return state != MovementSystemState.Idle; }
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
            state = CalculateState();
        }
    }

    // Private

    [Header("Debug")]
    public Vector2 desiredMovement = Vector2.zero;

    // Cache

    private Animator animator;
    private Rigidbody body;

    protected void Start()
    {
        // Cache

        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
    }

    private MovementSystemState CalculateState()
    {
        float forward = desiredMovement.y;
        float right = desiredMovement.x;

        float forwardAbs = Mathf.Abs(forward);
        float rightAbs = Mathf.Abs(right);

        if (forwardAbs < movementDesireThreshold &&
            rightAbs < movementDesireThreshold) return MovementSystemState.Idle;

        if (forwardAbs > rightAbs)
        {
            return forward > 0
                ? MovementSystemState.MovingForward
                : MovementSystemState.MovingBackward;
        }
        else
        {
            return right > 0
                ? MovementSystemState.MovingRight
                : MovementSystemState.MovingLeft;
        }
    }

    private void MoveBody(float delta)
    {
        if (state == MovementSystemState.Idle) return;

        Vector2 deltaVelocity = desiredMovement * movementSpeed -
            (new Vector2(body.velocity.x, body.velocity.z));

        Vector2 appliedVelocity = Vector2.MoveTowards(
            Vector2.zero,
            deltaVelocity,
            movementSpeed);

        body.AddForce(appliedVelocity.x, 0.0f, appliedVelocity.y, ForceMode.VelocityChange);
    }

    protected void FixedUpdate()
    {
        string trigger = idleAnimationTrigger;
        switch (state)
        {
            case MovementSystemState.Idle:
                trigger = idleAnimationTrigger;
                break;
            case MovementSystemState.MovingForward:
                trigger = movingForwardAnimationTrigger;
                break;
            case MovementSystemState.MovingBackward:
                trigger = movingBackwardAnimationTrigger;
                break;
            case MovementSystemState.MovingLeft:
                trigger = movingLeftAnimationTrigger;
                break;
            case MovementSystemState.MovingRight:
                trigger = movingRightAnimationTrigger;
                break;
        }

        animator.SetTrigger(trigger);

        MoveBody(Time.fixedDeltaTime);
    }

}
