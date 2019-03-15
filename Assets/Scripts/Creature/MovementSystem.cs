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

/*
 * Blended with other animations additively
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

    [Tooltip("Body rotation stabilization")]
    public bool rotationStabilization = true;

    public enum MovementSystemState
    {
        None,


        MovingForward,
        MovingBackward,
        MovingLeft,
        MovingRight,
    }

    /*
     * Expected animation configuration:
     * 
     * [moveForwardTrigger] -> (moveForward)
     * [moveBackwardTrigger] -> (moveBackward)
     * [moveLeftTrigger] -> (moveLeft)
     * [moveRightTrigger] -> (moveRight)
     * 
     * [idleTrigger] -> (idle)
     */

    public MovementSystemState state = MovementSystemState.None;

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

    public int animationLayer = 1;

    public bool Moving
    {
        get { return state != MovementSystemState.None; }
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
        Vector2 desiredMovementBodySpace =
            body.rotation * desiredMovement;

        float forward = desiredMovementBodySpace.y;
        float right = desiredMovementBodySpace.x;

        float forwardAbs = Mathf.Abs(forward);
        float rightAbs = Mathf.Abs(right);

        if (forwardAbs < movementDesireThreshold &&
            rightAbs < movementDesireThreshold) return MovementSystemState.None;

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
        if (state == MovementSystemState.None) return;

        Vector2 deltaVelocity = desiredMovement * movementSpeed -
            (new Vector2(body.velocity.x, body.velocity.z));

        Vector2 appliedVelocity = Vector2.MoveTowards(
            Vector2.zero,
            deltaVelocity,
            movementSpeed);

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
        string trigger = idleAnimationTrigger;
        switch (state)
        {
            case MovementSystemState.None:
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
        RotateBody(Time.fixedDeltaTime);
    }

}
