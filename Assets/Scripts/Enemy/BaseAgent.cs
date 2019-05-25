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
    private Vector3 stunnedLookingVector;

    [Header("AI Settings")] 
    [Range(1.0f, 10.0f)]
    public float AIPriority;
    public float waitTime = 1.0f;
    public float healthTrackingThreshold = 5.0f;
    public float roamRadius = 5.0f;
    public float pathRecalculationDistance = 2.0f;
    public float storedHealthDeterioration = 1.0f;
    public float storedHealthThreshold = 0.25f;
    public float maxRetreatDistance = 7.0f;
    public Transform player;
    [Header("Speed Settings")]
    public float baseSpeed = 5.0f;
    public float walkingBackSpeed = 4.0f;
    public float sprintingSpeed = 10.0f;
    
    [Header("Animation Settings")]
    protected Animator animator;
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
    [Tooltip("Trigger that toggles howl animation")]
    public string howlTrigger = "Howl";
    [Tooltip("Trigger that toggles hurt animation")]
    public string hurtTrigger = "GotHurt";
    [Tooltip("Trigger that toggles stagger animation")]
    public string staggerTrigger = "Staggered";
    
    private bool isDoingIdle = false;
    private bool isHowling = false;
    [Tooltip("Determines whether the agent is allowed to attack or not")]
    public bool allowedToAttack = false;
    protected MovementSystem movement;
    protected HealthSystem health;
    protected Compass compass;
    protected int animatorEnemySpotted;
    protected bool isAlerted;
    protected bool canSeeEnemy;
    protected bool isUnderAttack;
    protected bool wasDamagedLastFrame;
    protected float agentRadius;
    protected float waitTimer;
    private float storedHealth;
    private float storedDamage;
    private float attackedTime; 
    
    public delegate Task TaskCreation();
    protected Dictionary<string, TaskCreation> AgentTaskDictionary;
    //protected CollectiveAISystem.AgentType type = CollectiveAISystem.AgentType.Base;

    #region Properties

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

    public bool IsHowling
    {
        get { return isHowling; }
    }

    public bool IsUnderAttack
    {
        get { return isUnderAttack; }
    }


    public bool CanRetreat()
    {
        Vector3 retreatVector = -transform.forward * maxRetreatDistance;
        var hit = new NavMeshHit();
        return NavMesh.Raycast(transform.position,
            transform.position + retreatVector,
            out hit,
            NavMesh.AllAreas);
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

    #endregion
    
    #region Monobehaviour voids
    // Start is called before the first frame update
    protected void Awake()
    {
        movement = GetComponent<MovementSystem>();
        animator = GetComponent<Animator>();
        health = GetComponent<HealthSystem>();
        compass = GetComponent<Compass>();
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

        if (isStunned)
        {
            movement.Movement = Vector3.zero;
            transform.forward = stunnedLookingVector;
        }

        if (waitTimer > 0) waitTimer -= Time.deltaTime;
        ResetStun();
    }

    private void LateUpdate()
    {
        TrackHealth();
    }

    protected void OnDestroy()
    {
        AISys.AgentDied(ID);
    }
    //
    #endregion

    #region Actions

    public Task GetTaskByName(string taskName)
    {
        return AgentTaskDictionary[taskName].Invoke();
    }
    public void ThrownBackAndGetUp()
    {
        animator.SetTrigger(knockbackTrigger);
        animator.SetTrigger(getUpTrigger);
    }

    public void StartWalkingToPlayer()
    {
        navSys.PlacePathRequest(this, player.position);
    }
    
    public void StartWalkingToRandomPoint()
    {
        Vector3 vector = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * 
                         Vector3.forward * Random.Range(0, roamRadius);
        NavMeshHit hit;
        NavMesh.Raycast(transform.position, 
            AISys.ClosestNest(transform.position).position + vector, 
            out hit,
            NavMesh.AllAreas);
        navSys.PlacePathRequest(this, hit.position);
    }

    public void StartRetreating()
    {
        Vector3 retreatVector = -transform.forward * maxRetreatDistance;
        navSys.PlacePathRequest(this, transform.position + retreatVector);
    }
    
    public void SetWaitTimerForNSeconds(float n)
    {
        waitTimer = n;
    }

    public void SetWaitForOneSec()
    {
        SetWaitTimerForNSeconds(1f);
    }
    
    public void StartDoingIdle()
    {
        int index = Random.Range(1, idles + 1);
        animator.SetInteger(idleActionInt, index);
        isDoingIdle = true;
    }

    public void StartHowling()
    {
        animator.SetFloat(animatorEnemySpotted, 1.0f);
        animator.SetTrigger(howlTrigger);
        isHowling = true;
    }

    public void TurnToPlayer()
    {
        compass.SetCompassStateTarget();
    }

    public void TurnForward()
    {
        compass.SetCompassStateForward();
    }
    
    #endregion
    protected void UpdateMove()
    {
        movement.Movement = navSys.AskDirection(entityID);
        if (movement.Moving)
        {
            compass.UpdateStoredVector(new Vector3(
                movement.desiredMovement.x, 
                0, 
                movement.desiredMovement.y));
        }
        if (compass != null)
        {
            transform.forward = compass.LookingVector;
        }
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

    protected void TrackHealth()
    {
        wasDamagedLastFrame = false;
        if (storedHealth > health.Health)
        {
            storedDamage += storedHealth - health.Health;
            attackedTime = Time.time;
            isUnderAttack = true;
            wasDamagedLastFrame = true;
            animator.SetTrigger(hurtTrigger);
        }

        if (storedDamage > storedHealthThreshold)
        {
            animator.SetTrigger(staggerTrigger);
            Stun(1.0f);
            storedDamage = 0;
        }
        
        if (!wasDamagedLastFrame && Time.time > attackedTime + healthTrackingThreshold)
        {
            isUnderAttack = false;
        }
        storedHealth = health.Health;
        storedDamage -= storedHealthDeterioration * Time.deltaTime;
    }

    public void LostTarget()
    {
        canSeeEnemy = false;
    }

    public virtual void Alert(GameObject go)
    {
        player = go.transform;
        compass.SetTarget(go.transform);
        isAlerted = true;
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
        stunnedLookingVector = transform.forward;
    }

    public void FinalizeIdle()
    {
        isDoingIdle = false;
        animator.SetInteger(idleActionInt, 0);
    }

    public void FinalizeHowling()
    {
        isHowling = false;
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
    
    #region Tasks

    protected virtual void SetDictionary()
    {
        AgentTaskDictionary = new Dictionary<string, TaskCreation>();
        AgentTaskDictionary.Add("WalkTowardsRandomPoint", WalkTowardsRandomPoint);
        AgentTaskDictionary.Add("DoRandomIdle", DoRandomIdle);
        AgentTaskDictionary.Add("WaitForOneSecond", WaitForOneSecond);
        AgentTaskDictionary.Add("RoamAround", RoamAround);
        AgentTaskDictionary.Add("Howl", Howl);
        AgentTaskDictionary.Add("WalkTowardsPlayer", WalkTowardsPlayer);
        AgentTaskDictionary.Add("LookForward", LookForward);
        AgentTaskDictionary.Add("TurnTowardsPlayer", TurnTowardsPlayer);
        AgentTaskDictionary.Add("StopAgent", StopAgent);
    }
    protected SimpleTask WalkTowardsRandomPoint()
    {
        Task.Condition[] preConditions = {() => movement.canMove};
        Task.Condition[] integrityRules =
        {
            () => !canSeeEnemy,
            () => !IsStunned,
            () => !IsUnderAttack,
        };

        Task.Condition[] finishCondition =
        {
            () => navSys.HasAgentReachedDestination(ID)
        };

        SimpleTask.TaskAction action = StartWalkingToRandomPoint;
        
        var task = new SimpleTask(
            this.name + "WalkTowardsRandomPoint",
            AISys,
            action,
            preConditions, 
            integrityRules,
            finishCondition);
        return task;
    }

    protected SimpleTask StopAgent()
    {
        Task.Condition[] preConditions =
        {
            () => movement.canMove,
        };
        Task.Condition[] integrityRules =
        {
            () => !IsStunned,
        };
        
        Task.Condition[] finishCondition =
        {
            () => navSys.HasAgentReachedDestination(ID)
        };
                    
        SimpleTask.TaskAction action = () => navSys.StopAgent(ID);
                            
        var task = new SimpleTask(
            this.name + "StopAgent",
            AISys,
            action,
            preConditions, 
            integrityRules,
            finishCondition);
        return task;
    }
    
    protected SimpleTask WalkTowardsPlayer()
    {
        Task.Condition[] preConditions =
        {
            () => movement.canMove,
            () => player != null
        };
        Task.Condition[] integrityRules =
        {
            () => !IsStunned,
            () => (navSys.AgentDestination(ID) - player.position).magnitude < 
            pathRecalculationDistance
        };
        
        Task.Condition[] finishCondition =
        {
            () => navSys.HasAgentReachedDestination(ID)
        };
                    
        SimpleTask.TaskAction action = StartWalkingToPlayer;
                            
        var task = new SimpleTask(
                            this.name + "WalkTowardsPlayer",
                            AISys,
                            action,
                            preConditions, 
                            integrityRules,
                            finishCondition);
        return task;
        
    }
    
    protected SimpleTask TurnTowardsPlayer()
    {
        Task.Condition[] preConditions =
        {
            () => player != null
        };
        Task.Condition[] integrityRules =
        {
            () => !IsStunned,
        };
                    
        SimpleTask.TaskAction action = TurnToPlayer;
                            
        var task = new SimpleTask(
            this.name + "TurnTowardsPlayer",
            AISys,
            action,
            preConditions, 
            integrityRules,
            Task.EmptyCondition);
        return task;
    }
    
    protected SimpleTask LookForward()
    {
        Task.Condition[] integrityRules =
        {
            () => !IsStunned,
        };
                    
        SimpleTask.TaskAction action = TurnForward;
                            
        var task = new SimpleTask(
            this.name + "TurnForward",
            AISys,
            action,
            Task.EmptyCondition, 
            integrityRules,
            Task.EmptyCondition);
        return task;
    }
    
    protected SimpleTask DoRandomIdle()
    {
        Task.Condition[] preConditions = {() => idles > 0};
        Task.Condition[] integrityRules =
        {
            () => !canSeeEnemy,
            () => !IsStunned,
            () => !IsUnderAttack,
        };

        Task.Condition[] finishCondition =
        {
            () => !isDoingIdle
        };

        SimpleTask.TaskAction action = StartDoingIdle;
        
        var task = new SimpleTask(
            this.name + "DoRandomIdle",
            AISys,
            action,
            preConditions, 
            integrityRules,
            finishCondition);
        return task;
    }

    protected SimpleTask WaitForOneSecond()
    {
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
            action,
            Task.EmptyCondition, 
            integrityRules,
            finishCondition);
        return task;
    }

    protected SimpleTask SetTask(string taskName)
    {
        SimpleTask.TaskAction action = 
            () => AISys.AssignTask(this, (ComplexTask)AgentTaskDictionary[taskName].Invoke());;
        
        var task = new SimpleTask(
            this.name + "SetTask " + taskName,
            AISys,
            action);
        return task;
    }

    protected SimpleTask Howl()
    {
        Task.Condition[] preConditions =
        {
            () => !IsStunned,
            () => isAlerted
        };
        Task.Condition[] finishCondition =
        {
            () => !IsHowling
        };

        SimpleTask.TaskAction action = StartHowling;
        
        var task = new SimpleTask(
            this.name + "Howl",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }

    protected SimpleTask WaitUntilPlayerSeen()
    {
        Task.Condition[] finishCondition =
        {
            () => canSeeEnemy,
        };

        SimpleTask.TaskAction action = () => { };
        
        var task = new SimpleTask(
            this.name + "PhysicalStrike",
            AISys,
            action,
            Task.EmptyCondition, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    protected SimpleTask Retreat()
    {
        Task.Condition[] preConditions =
        {
            () => !IsStunned,
            CanRetreat
        };
        
        Task.Condition[] integrityRules =
        {
            () => !IsStunned,
        };
        
        Task.Condition[] finishCondition =
        {
            () => navSys.HasAgentReachedDestination(ID)
        };

        SimpleTask.TaskAction action = StartHowling;
        
        var task = new SimpleTask(
            this.name + "Retreat",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    
    protected ComplexTask RoamAround()
    {
        ComplexTask.DecompositionMethod method = DecomposeRoamAround;

        
        var task = new ComplexTask(
            this.name + "RoamAround",
            AISys,
            method);
        return task;
    }
 
    protected virtual Task[] DecomposeRoamAround()
    {
        var tasks = new Task[3];
        tasks[0] = WalkTowardsRandomPoint();
        tasks[1] = DoRandomIdle();
        tasks[2] = SetTask("RoamAround");
        return tasks;
    }
    
    #endregion
    
    
}
