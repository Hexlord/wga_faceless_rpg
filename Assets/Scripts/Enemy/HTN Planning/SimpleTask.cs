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

    
    public SimpleTask(string name, 
        Func<bool>[] conditions, 
        Func<bool>[] rules, 
        Func<bool>[] finish, 
        TaskAction action) : base (name, conditions, rules)
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
        var coroutine = TaskExecution();
        StartCoroutine(coroutine);
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
                yield break;
            }

            yield return null;
        }
        
    }
}