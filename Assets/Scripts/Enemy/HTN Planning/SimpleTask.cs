using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;


public class SimpleTask:Task
{
    public delegate void TaskAction();
    
    readonly TaskAction taskAction;
    readonly protected Condition[] endingConditions;

    public override TaskType Type
    {
        get { return TaskType.Simple; }
    }
    
    public SimpleTask(string name,
        HTNplanner coroutineRunner,
        Condition[] conditions, 
        Condition[] rules, 
        Condition[] finish, 
        TaskAction action) : base (name, coroutineRunner, conditions, rules)
    {
        taskAction = action;
        endingConditions = new Condition[finish.Length];
        finish.CopyTo(endingConditions, 0);
    }
    
    protected bool CheckEndTask()
    {
        return BasicCheck(endingConditions);
    }

    public override void StartExecution()
    {
        base.StartExecution();
    }
    
    protected override IEnumerator TaskExecution()
    {
        this.SetStatus(TaskStatus.InProgress);
        if (!CheckPreConditions())
        {
            this.SetStatus(TaskStatus.Failure);
            yield break;
        }
        
        taskAction.Invoke();
        
        while (this.Status != TaskStatus.Complete)
        {
            if (!CheckTaskIntegrity())
            {
                this.SetStatus(TaskStatus.Failure);
                yield break;
            }

            if (CheckEndTask())
            {
                this.SetStatus(TaskStatus.Complete);
            }

            yield return null;
        }
        
    }
}