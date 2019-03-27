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
    public string idleAnimationTrigger = "idleTrigger";

    public string moveForwardAnimation = "moveForward";
    public string moveForwardAnimationTrigger = "moveForwardTrigger";
    public string moveBackwardAnimation = "moveBackward";
    public string moveBackwardAnimationTrigger = "moveBackwardTrigger";
    public string moveLeftAnimation = "moveLeft";
    public string moveLeftAnimationTrigger = "moveLeftTrigger";
    public string moveRightAnimation = "moveRight";
    public string moveRightAnimationTrigger = "moveRightTrigger";

    public string unarmedIdleAnimation = "unarmedIdle";
    public string unarmedIdleAnimationTrigger = "unarmedIdleTrigger";
    public string unarmedMoveForwardAnimation = "unarmedMoveForward";
    public string unarmedMoveBackwardAnimation = "unarmedMoveBackward";
    public string unarmedMoveLeftAnimation = "moveLeft";
    public string unarmedMoveRightAnimation = "moveRight";



    public int animationLayer = 0;

    private float currentMovementSpeed;

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
    private SheathSystem sheathSystem;
    private AttackSystem attackSystem;
    private SkillSystem skillSystem;

    protected void Start()
    {
        currentMovementSpeed = baseMovementSpeed;
        // Cache
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        sheathSystem = GetComponent<SheathSystem>();
        attackSystem = GetComponent<AttackSystem>();
        skillSystem = GetComponent<SkillSystem>();
    }

    private MovementSystemState CalculateState()
    {
        Vector3 desiredMovementBodySpace =
            Quaternion.Euler(0.0f, -transform.rotation.eulerAngles.y, 0.0f) * 
            (new Vector3(desiredMovement.x, 0.0f, desiredMovement.y));

        float forward = desiredMovementBodySpace.z;
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

        Vector2 deltaVelocity = desiredMovement * currentMovementSpeed -
            (new Vector2(body.velocity.x, body.velocity.z));

        Vector2 appliedVelocity = Vector2.MoveTowards(
            Vector2.zero,
            deltaVelocity,
            currentMovementSpeed);

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
        if (attackSystem &&
            attackSystem.Attacking ||
            skillSystem &&
            skillSystem.Busy) Movement = Vector2.zero;

        bool transition = animator.IsInTransition(animationLayer);

        bool sheathed = sheathSystem.Sheathed;

        AnimatorClipInfo info = animator.GetCurrentAnimatorClipInfo(animationLayer)[0];
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayer);
        AnimationClip clip = info.clip;
        string clipName = clip.name;

        string trigger = idleAnimationTrigger;

        bool idle = clipName == (sheathed
            ? unarmedIdleAnimation
            : idleAnimation);

        bool matches = false;
        switch (state)
        {
            case MovementSystemState.None:
                matches = clipName == (sheathed
                    ? unarmedIdleAnimation
                    : idleAnimation);
                break;
            case MovementSystemState.MovingForward:
                matches = clipName == (sheathed
                    ? unarmedMoveForwardAnimation
                    : moveForwardAnimation);
                break;
            case MovementSystemState.MovingBackward:
                matches = clipName == (sheathed
                    ? unarmedMoveBackwardAnimation
                    : moveBackwardAnimation);
                break;
            case MovementSystemState.MovingLeft:
                matches = clipName == (sheathed
                    ? unarmedMoveLeftAnimation
                    : moveLeftAnimation);
                break;
            case MovementSystemState.MovingRight:
                matches = clipName == (sheathed
                    ? unarmedMoveRightAnimation
                    : moveRightAnimation);
                break;
        }


        switch (state)
        {
            case MovementSystemState.None:
                trigger = idleAnimationTrigger;
                break;
            case MovementSystemState.MovingForward:
                trigger = idle || matches
                    ? moveForwardAnimationTrigger
                    : idleAnimationTrigger;
                break;
            case MovementSystemState.MovingBackward:
                trigger = idle || matches
                    ? moveBackwardAnimationTrigger
                    : idleAnimationTrigger;
                break;
            case MovementSystemState.MovingLeft:
                trigger = idle || matches
                    ? moveLeftAnimationTrigger
                    : idleAnimationTrigger;
                break;
            case MovementSystemState.MovingRight:
                trigger = idle || matches
                    ? moveRightAnimationTrigger
                    : idleAnimationTrigger;
                break;
        }

        animator.ResetTrigger(moveForwardAnimationTrigger);
        animator.ResetTrigger(moveBackwardAnimationTrigger);
        animator.ResetTrigger(moveLeftAnimationTrigger);
        animator.ResetTrigger(moveRightAnimationTrigger);
        animator.ResetTrigger(idleAnimationTrigger);

        animator.SetTrigger(trigger);

        MoveBody(Time.fixedDeltaTime);
        RotateBody(Time.fixedDeltaTime);
    }

    public void SetSpeed(float newSpeed)
    {
        currentMovementSpeed = newSpeed;
    }

    public void RevertSpeed()
    {
        currentMovementSpeed = baseMovementSpeed;
    }

}
