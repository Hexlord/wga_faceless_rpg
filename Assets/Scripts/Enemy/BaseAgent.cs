using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAgent : MonoBehaviour
{
    static uint entityIDGenerator = 0;
    private uint entityID = 0;
    protected Queue<Vector3> waypoints;
    protected bool systemsSet = false;
    protected bool isStunned = false;
    protected float stunDuration = 0.0f;
    protected float stunStart = 0.0f;
    protected NavigationSystem navSys;
    protected CollectiveAISystem AISys;
    private Vector3 rayDirection;
    [Header("Speed Settings")]
    public float baseSpeed = 5.0f;
    public float walkingBackSpeed = 4.0f;
    public float sprintingSpeed = 10.0f;

    [Header("Animation Settings")]
    Animator animator;
    public float epsilonWalking = 0.005f;
    public string walkDirectionInt = "Walking Direction";
    public string enemySpottedBool = "EnemySpotted";
    public string knockbackTrigger = "ThrownBack";
    public string getUpTrigger = "GetUp";
    public string[] idleActionTrigger = { "LookOverShoulder", "LookAround" };

    public bool allowedToAttack = false;
    protected MovementSystem movement;
    protected int directionAnim = 0;
    protected bool isAlerted;
    protected bool canSeeEnemy;
    protected float stoppingDistance;
    protected GameObject currentTarget;
    //protected CollectiveAISystem.AgentType type = CollectiveAISystem.AgentType.Base;

    public virtual CollectiveAISystem.AgentType AgentType()
    {
        return CollectiveAISystem.AgentType.Base;
    }

    public uint ID
    {
        get { return entityID; }
    }

    public  bool CanSeeEnemy
    {
        get { return canSeeEnemy; }
    }

    public virtual bool CanAttackEnemy()
    {
        return false;
    }

    // Start is called before the first frame update
    protected void Awake()
    {
        entityID = entityIDGenerator++;
    }

    protected virtual void Start()
    {
        movement = GetComponent<MovementSystem>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (!isStunned)
        {
            UpdateMove();
        }
        ResetStun();
    }

    protected void OnDestroy()
    {
        AISys.AgentDied(ID);
    }
    //

    public void SetControllingSystems(CollectiveAISystem ai, NavigationSystem nav)
    {
        if (!systemsSet)
        {
            AISys = ai;
            navSys = nav;
        }
    }

    #region Actions
    public void ThrownBackAndGetUp()
    {
        animator.SetTrigger(knockbackTrigger);
        animator.SetTrigger(getUpTrigger);
    }
    #endregion

    #region Movement
    protected void UpdateMove()
    {
        UpdateMovementDirection(movement.desiredMovement, new Vector2(transform.forward.x, transform.forward.z));
        movement.Movement = navSys.AskDirection(entityID);
        UpdateMovementAnimation();
        Vector3 lookingVector = currentTarget.transform.position - transform.position;
        lookingVector = new Vector3(lookingVector.x, 0, lookingVector.z);
        transform.forward = (canSeeEnemy) ? lookingVector : new Vector3(movement.desiredMovement.x, 0, movement.desiredMovement.y);
    }

    protected void UpdateMovementDirection(Vector2 walkingDirection, Vector2 lookingDirection)
    {

        if (walkingDirection.magnitude < epsilonWalking)
        {
            directionAnim = 0;
            movement.SetSpeed((isAlerted) ? sprintingSpeed : baseSpeed);
            return;
        }

        float angle = Vector2.SignedAngle(lookingDirection, walkingDirection);

        if (Mathf.Abs(angle) < 30)
        {
            directionAnim = 1;
        }
        if ((Mathf.Abs(angle) >= 30) && (Mathf.Abs(angle) < 150))
        {
            movement.SetSpeed(baseSpeed);
            if (Mathf.Sign(angle) == -1)
            {
                directionAnim = 3;
            }
            else
            {
                directionAnim = 2;
            }
        }
        if (Mathf.Abs(angle) >= 150)
        {
            movement.SetSpeed(walkingBackSpeed);
            directionAnim = 4;
        }
    }

    private void UpdateMovementAnimation()
    {
        if (animator.GetBool(enemySpottedBool) != isAlerted) animator.SetBool(enemySpottedBool, isAlerted);
        animator.SetInteger(walkDirectionInt, directionAnim);
    }
    #endregion

    #region State Functions 
    public void SawSomething(GameObject something)
    {
        Debug.Log("Agent" + ID + " saw " + something);
        isAlerted = true;
        canSeeEnemy = true;
        AISys.Notify(something);
    }

    public void LostTarget()
    {
        canSeeEnemy = false;
    }

    public void Alert(GameObject newTarget)
    {
        currentTarget = newTarget;
        isAlerted = true;
        allowedToAttack = true;
    }

    public void Dismiss()
    {
        isAlerted = false;
    }

    protected void ResetStun()
    {
        if (isStunned && Time.time > stunStart + stunDuration)
        {
            isStunned = false;
        }
    }

    public void Stun(float duration)
    {
        isStunned = true;
        stunDuration = duration;
        stunStart = Time.time;
    }

    public void SetTarget(GameObject go)
    {
        currentTarget = go;
    }
    #endregion
}
