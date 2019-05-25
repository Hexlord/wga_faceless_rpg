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
        TaskAction action = null,
        Condition[] conditions = null, 
        Condition[] rules = null, 
        Condition[] finish = null) : base (name, coroutineRunner, conditions, rules)
    {
        taskAction = action;
        if (finish == null)
        {
            finish = EmptyCondition;
        }
        else
        {
            endingConditions = new Condition[finish.Length];
            finish.CopyTo(endingConditions, 0);
        }

    }
    
    private bool CheckEndTask()
    {
        return BasicCheck(endingConditions);
    }
    
    protected override IEnumerator TaskExecution()
    {
        SetStatus(TaskStatus.InProgress);
        if (!CheckPreConditions())
        {
            SetStatus(TaskStatus.Failure);
            yield break;
        }
        
        taskAction.Invoke();
        
        while (Status != TaskStatus.Complete)
        {
            if (!CheckTaskIntegrity())
            {
                SetStatus(TaskStatus.Failure);
                yield break;
            }

            if (CheckEndTask())
            {
                SetStatus(TaskStatus.Complete);
            }

            yield return null;
        }
        
    }
}