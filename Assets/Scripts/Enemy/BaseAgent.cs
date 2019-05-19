using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("ProjectFaceless/Enemy/BaseAgent")]
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
    protected HTNplanner AISys;
    private Vector3 rayDirection;

    [Header("AI Settings")] 
    [Range(1.0f, 10.0f)]
    public float AIPriority;

    public float waitTime = 1.0f;
    public float roamRadius = 5.0f;
    [Header("Speed Settings")]
    public float baseSpeed = 5.0f;
    public float walkingBackSpeed = 4.0f;
    public float sprintingSpeed = 10.0f;

    [Header("Animation Settings")]
    Animator animator;
    [Tooltip("Minimal vector length to determine the whether the agent is walking or not")]
    public float epsilonWalking = 0.005f;
    [Tooltip("Integer that determines the direction the agent is walking right now")]
    public string walkDirectionInt = "Walking Direction";
    [Tooltip("Animator boolean that is true if the enemy had seen the player")]
    public string enemySpottedBool = "EnemySpotted";
    [Tooltip("Trigger that toggles knockback animation")]
    public string knockbackTrigger = "ThrownBack";
    [Tooltip("Trigger that toggles getting up animation")]
    public string getUpTrigger = "GetUp";
    [Tooltip("Triggers that toggle idle animations")]
    public string idleActionInt = "IdleVariant";
    public int idles = 3;
    private bool isDoingIdle = false;
    [Tooltip("Determines whether the agent is allowed to attack or not")]
    public bool allowedToAttack = false;
    protected MovementSystem movement;
    protected int animatorEnemySpotted;
    protected bool isAlerted;
    protected bool canSeeEnemy;
    protected float stoppingDistance;
    protected GameObject currentTarget;
    protected float agentRadius;
    protected float waitTimer;

    
    public delegate Task taskCreation();
    protected Dictionary<string, taskCreation> AgentTaskDictionary;
    //protected CollectiveAISystem.AgentType type = CollectiveAISystem.AgentType.Base;

    public virtual CollectiveAISystem.AgentType AgentType()
    {
        return CollectiveAISystem.AgentType.Base;
    }

    public uint ID
    {
        get { return entityID; }
    }

    public float Radius
    {
        get { return agentRadius; }
    }
    
    public float Priority
    {
        get { return AIPriority; }
    }
    
    public  bool CanSeeEnemy
    {
        get { return canSeeEnemy; }
    }

    public virtual bool CanSeeTarget()
    {
        return false;
    }

    public virtual bool CanAttackEnemy()
    {
        return false;
    }

    public bool IsDoingIdleAnimation
    {
        get { return isDoingIdle; }
    }

    public void SetControllingSystems(HTNplanner ai, NavigationSystem nav)
    {
        if (!systemsSet)
        {
            AISys = ai;
            navSys = nav;
        }
    }

    public bool IsStunned
    {
        get
        {
            return isStunned;
        }
    }

    #region Monobehaviour voids
    // Start is called before the first frame update
    protected void Awake()
    {
        movement = GetComponent<MovementSystem>();
        animator = GetComponent<Animator>();
        entityID = entityIDGenerator++;
        agentRadius = GetComponent<CapsuleCollider>().radius;
        SetDictionary();
    }

    protected virtual void Start()
    {
        movement = GetComponent<MovementSystem>();
        animator = GetComponent<Animator>();
        animatorEnemySpotted = Animator.StringToHash(enemySpottedBool);
    }

    protected virtual void Update()
    {
        if (!isStunned && !isDoingIdle && waitTimer <= 0)
        {
            UpdateMove();
        }

        if (waitTimer > 0) waitTimer -= Time.deltaTime;
        ResetStun();
    }

    protected void OnDestroy()
    {
        AISys.AgentDied(ID);
    }
    //
    #endregion

    #region Actions
    public void ThrownBackAndGetUp()
    {
        animator.SetTrigger(knockbackTrigger);
        animator.SetTrigger(getUpTrigger);
    }

    public void StartWalkingToPlayer()
    {
        navSys.PlacePathRequest(this, currentTarget.transform.position);
    }
    
    public void StartWalkingToRandomPoint()
    {
        Vector3 vector = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * 
                         Vector3.forward * Random.Range(0, roamRadius);
        NavMeshHit hit;
        NavMesh.Raycast(this.transform.position, 
            vector, 
            out hit, 
            new NavMeshQueryFilter());
        navSys.PlacePathRequest(this, hit.position);
    }

    public void SetWaitTimerForNSeconds(float N)
    {
        waitTimer = N;
    }

    public void SetWaitForOneSec()
    {
        SetWaitTimerForNSeconds(1f);
    }
    
    private void StartDoingIdle()
    {
        int index = Random.Range(1, idles + 1);
        animator.SetInteger(idleActionInt, index);
        isDoingIdle = true;
    }
    
    public Task SetMeTask(string TaskName)
    {
        return AgentTaskDictionary[TaskName].Invoke();
    }
    
    #endregion
    protected void UpdateMove()
    {
        movement.Movement = navSys.AskDirection(entityID);
        Vector3 lookingVector = currentTarget.transform.position - transform.position;
        lookingVector = new Vector3(lookingVector.x, 0, lookingVector.z);
        if (movement.desiredMovement != Vector2.zero || isAlerted) 
            transform.forward = (isAlerted) ? 
                lookingVector : 
                new Vector3(movement.desiredMovement.x, 0, movement.desiredMovement.y);
    }

    #region State Functions 
    public void SawSomething(GameObject something)
    {
        //Debug.Log("Agent" + ID + " saw " + something);
        isAlerted = true;
        canSeeEnemy = true;
        FinalizeIdle();
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
        animator.SetFloat(animatorEnemySpotted, 1.0f);
        allowedToAttack = true;
    }

    public void Dismiss()
    {
        isAlerted = false;
        animator.SetFloat(animatorEnemySpotted, 0.0f);
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

    public void FinalizeIdle()
    {
        isDoingIdle = false;
        animator.SetInteger(idleActionInt, 0);
    }
    #endregion
    
    public static int ComparePriority(BaseAgent x, BaseAgent y)
    {
        if (x == null)
        {
            if (y == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y == null)
                return 1;
            else
            {
                if (x.Priority > y.Priority)
                    return 1;
                else if (x.Priority == y.Priority)
                    return 0;
                else
                    return -1;
            }
        }
    }
    
    #region general Tasks

    protected virtual void SetDictionary()
    {
        AgentTaskDictionary = new Dictionary<string, taskCreation>();
        AgentTaskDictionary.Add("WalkTowardsRandomPoint", WalkTowardsRandomPoint);
        AgentTaskDictionary.Add("DoRandomIdle", DoRandomIdle);
        AgentTaskDictionary.Add("WaitForOneSecond", WaitForOneSecond);
        AgentTaskDictionary.Add("RoamAround", RoamAround);
    }
    SimpleTask WalkTowardsRandomPoint()
    {
        Task.Condition[] preConditions = {() => movement.canMove};
        Task.Condition[] integrityRules =
        {
            () => !canSeeEnemy,
            () => !IsStunned
        };

        Task.Condition[] finishCondition =
        {
            () => navSys.HasAgentReachedDestination(ID)
        };

        SimpleTask.TaskAction action = StartWalkingToRandomPoint;
        
        var task = new SimpleTask(
            this.name + "WalkTowardsRandomPoint",
            AISys,
            preConditions, 
            integrityRules,
            finishCondition,
            action);
        return task;
    }
    
    SimpleTask DoRandomIdle()
    {
        Task.Condition[] preConditions = {() => idles > 0};
        Task.Condition[] integrityRules =
        {
            () => !canSeeEnemy,
            () => !IsStunned
        };

        Task.Condition[] finishCondition =
        {
            () => !isDoingIdle
        };

        SimpleTask.TaskAction action = StartDoingIdle;
        
        var task = new SimpleTask(
            this.name + "DoRandomIdle",
            AISys,
            preConditions, 
            integrityRules,
            finishCondition,
            action);
        return task;
    }

    SimpleTask WaitForOneSecond()
    {
        Task.Condition[] preConditions = {() => true};
        Task.Condition[] integrityRules =
        {
            () => !canSeeEnemy,
            () => !IsStunned
        };

        Task.Condition[] finishCondition =
        {
            () => waitTimer <= 0
        };

        SimpleTask.TaskAction action = SetWaitForOneSec;
        
        var task = new SimpleTask(
            this.name + "WaitForOneSecond",
            AISys,
            preConditions, 
            integrityRules,
            finishCondition,
            action);
        return task;
    }

    ComplexTask RoamAround()
    {
        Task.Condition[] preConditions = {() => true };
        Task.Condition[] integrityRules =
        {
            () => true
        };


        ComplexTask.DecompositionMethod method = DecomposeRoamAround;
        
        var task = new ComplexTask(
            this.name + "RoamAround",
            AISys,
            preConditions, 
            integrityRules,
            method);
        return task;
    }

    Task[] DecomposeRoamAround()
    {
        var tasks = new Task[3];
        tasks[0] = WalkTowardsRandomPoint();
        tasks[1] = DoRandomIdle();
        tasks[2] = SetMeTask("RoamAround");
        return tasks;
    }
    
    #endregion
    
    
}
