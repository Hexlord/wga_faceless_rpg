using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class BossAgent : BaseAgent
{
    public float seeingDistance = 100.0f;
    public float attackRange = 1f;
    public float maxAimTime = 2f;
    public Transform heavenEye;
    public Transform[] shootingTowers;
    private ShootSystem shootSys;
    private AttackSystem attackSys;
    private SmartAiming aimSys;
    private Observer observer;
    private int chosenPost = 0;
    public string aimTrigger = "DrawBow";
    public string castTrigger = "Cast";
    protected override void Start()
    {
        base.Start();
        shootSys = GetComponent<ShootSystem>();
        aimSys = GetComponent<SmartAiming>();
        observer = GetComponent<Observer>();
    }

    #region Checks

    public override bool CanSeeTarget()
    {
        if (isAlerted)
        {
            Ray ray = new Ray(shootSys.ShootingPoint.position, player.position - shootSys.ShootingPoint.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, seeingDistance, LayerMask.GetMask("Character", "Environment"), QueryTriggerInteraction.Ignore))
                if (player.gameObject == hit.collider.gameObject.TraverseParent(player.tag))
                {
                    Debug.DrawLine(shootSys.ShootingPointPosition, hit.point);
                    return true;
                }
        }

        return false;
    }

    public override bool CanAttackEnemy()
    {
        return (!isStunned && (player.position - transform.position).magnitude <= attackRange);
    }
    
    #endregion

    #region Actions

    public void Aim()
    {
        animator.SetTrigger(aimTrigger);
    }

    public void Teleport()
    {
        transform.position = shootingTowers[chosenPost].position;
    }

    public void Cast()
    {
        ChooseShootingPosition();
        animator.SetTrigger(castTrigger);
    }

    private void ChooseShootingPosition()
    {
        List<int> posList = new List<int>();
        Vector3[] eligableShootingPositions = new Vector3[3];
        eligableShootingPositions[0] = observer.GetClosestShootingPosition(player, shootingTowers[0].position);
        var pos2 = observer.GetClosestShootingPosition(player, shootingTowers[1].position);
        var pos3 = observer.GetClosestShootingPosition(player, shootingTowers[2].position);
        for (int i = 0; i < shootingTowers.Length; i++)
        {
            var hit = new NavMeshHit();
            if (NavMesh.Raycast(shootingTowers[i].position,
                eligableShootingPositions[i],
                out hit,   
                NavMesh.AllAreas) &&
                i != chosenPost)
                posList.Add(i);
        }
        chosenPost = posList[UnityEngine.Random.Range(0, posList.Count)]; 
    }
    
    #endregion

    #region Tasks

    protected SimpleTask WalkTowardsChosenTower()
    {
        Task.Condition[] finishCondition =
        {
            () => navSys.HasAgentReachedDestination(ID)
        };

        SimpleTask.TaskAction action = () => { };
        
        var task = new SimpleTask(
            this.name + "WalkTowardsChosenTower",
            AISys,
            action,
            Task.EmptyCondition, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    
    protected SimpleTask WaitUntilAlerted()
    {
        Task.Condition[] finishCondition =
        {
            () => isAlerted,
            () => isUnderAttack
        };

        SimpleTask.TaskAction action = () => { };
        
        var task = new SimpleTask(
            this.name + "WaitUntilAlerted",
            AISys,
            action,
            Task.EmptyCondition, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
     
    protected SimpleTask WaitUntilPlayerStopped()
    {
        Task.Condition[] finishCondition =
        {
            () => player.GetComponent<Rigidbody>().velocity == Vector3.zero,
            () => waitTimer <= 0
        };

        SimpleTask.TaskAction action = () => { SetWaitTimerForNSeconds(maxAimTime);};
        
        var task = new SimpleTask(
            this.name + "WaitUntilAlerted",
            AISys,
            action,
            Task.EmptyCondition, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    
    protected SimpleTask Kick()
    {
        Task.Condition[] preConditions =
        {
            () => !IsStunned,
            CanAttackEnemy
        };

        Task.Condition[] finishCondition =
        {
            () => !attackSys.Attacking
        };

        SimpleTask.TaskAction action = () => attackSys.Attack(1, 2);
        
        var task = new SimpleTask(
            this.name + "PhysicalStrike",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    
    protected SimpleTask PhysicalStrike()
    {
        Task.Condition[] preConditions =
        {
            () => !IsStunned,
            CanAttackEnemy
        };

        Task.Condition[] finishCondition =
        {
            () => !attackSys.Attacking
        };

        SimpleTask.TaskAction action = () => attackSys.Attack(0, 0);
        
        var task = new SimpleTask(
            this.name + "PhysicalStrike",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }

    protected SimpleTask MagicalStrike()
    {
        Task.Condition[] preConditions =
        {
            () => !IsStunned,
            CanAttackEnemy
        };

        Task.Condition[] finishCondition =
        {
            () => !attackSys.Attacking
        };

        SimpleTask.TaskAction action = () => attackSys.Attack(0, 1);
        
        var task = new SimpleTask(
            this.name + "MagicalStrike",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    
    protected SimpleTask ShootPhysical()
    {
        Task.Condition[] preConditions = 
        {
            () => !IsStunned,
            () => shootSys.canShoot
            
        };
        
        Task.Condition[] finishCondition =
        {
            () => !shootSys.Shooting
        };

        SimpleTask.TaskAction action = 
            () => shootSys.Shoot(aimSys.StraightLineAimingVector, 0);
        
        var task = new SimpleTask(
            this.name + "ShootPhysical",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    
    protected SimpleTask ShootMagical()
    {
        Task.Condition[] preConditions = 
        {
            () => !IsStunned,
            CanSeeTarget,
            () => shootSys.canShoot
            
        };
        
        Task.Condition[] finishCondition =
        {
            () => !shootSys.Shooting
        };

        SimpleTask.TaskAction action = 
            () => shootSys.Shoot(aimSys.StraightLineAimingVector, 1);
        
        var task = new SimpleTask(
            this.name + "ShootMagical",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }

    protected SimpleTask ShootCombined()
    {
        Task.Condition[] preConditions = 
        {
            () => !IsStunned,
            () => shootSys.canShoot
            
        };
        
        Task.Condition[] finishCondition =
        {
            () => !shootSys.Shooting
        };

        SimpleTask.TaskAction action = 
            () => shootSys.Shoot(
                () => { return (heavenEye.position - shootSys.ShootingPointPosition).normalized; }, 
                2);
        
        var task = new SimpleTask(
            this.name + "ShootCombined",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }

    protected SimpleTask CallReinforcements()
    {
        Task.Condition[] preConditions = 
        {
            () => !IsStunned,
            
        };
        
        Task.Condition[] finishCondition =
        {
            () => !false
        };

        SimpleTask.TaskAction action =
            () => { throw new NotImplementedException();};
        
        var task = new SimpleTask(
            this.name + "CallReinforcements",
            AISys,
            action,
            preConditions, 
            Task.EmptyCondition,
            finishCondition);
        return task;
    }
    
    protected ComplexTask FirstTask()
    {
        ComplexTask.DecompositionMethod method = FirstTaskDecomposition;

        
        var task = new ComplexTask(
            this.name + "FirstTask",
            AISys,
            method);
        return task;
    }

    protected Task[] FirstTaskDecomposition()
    {
       var tasks = new Task[4];
               tasks[0] = WalkTowardsRandomPoint();
               tasks[1] = CallReinforcements();
               tasks[2] = Howl();
               tasks[3] = SetTask("Battle");
               return tasks; 
    }
    
    #endregion


}
