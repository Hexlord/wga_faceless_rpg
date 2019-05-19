using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class Task
{
    private string taskName;
    
    private TaskStatus taskStatus;
    
    public delegate bool Condition(); 
    
    protected readonly Condition[] preConditionsList;
    protected readonly Condition[] integrityRules;
    protected readonly HTNplanner taskCouroutineStarter;

    public Task(string name, HTNplanner coroutineRunner, Condition[] conditions, Condition[] rules)
    {
        taskName = name;
        taskCouroutineStarter = coroutineRunner;
        preConditionsList = new Condition[conditions.Length];
        integrityRules = new Condition[rules.Length];
        conditions.CopyTo(preConditionsList, 0);
        rules.CopyTo(integrityRules, 0);
        taskStatus = TaskStatus.Planned;
    }
    
    public enum TaskType 
    {
        Empty,
        Simple,
        Complex
    }

    public enum TaskStatus
    {
        None,
        Planned,
        InProgress,
        Complete,
        Failure
    }

    public string Name
    {
        get { return taskName; }
    }
    public virtual TaskType Type
    {
        get { return TaskType.Empty; }
    }
    
    public virtual TaskStatus Status
    {
        get { return taskStatus; }
    }

    
    protected void SetStatus(TaskStatus val)
    {
        taskStatus = val;
    }
    
    
    #region Checks

    protected bool BasicCheck(Condition[] collection)
    {
            var result = true;
            foreach (var condition in collection)
            {
                result &= condition.Invoke();
                if (!result) return false;
            }
            return true;
    }


    protected bool CheckPreConditions()
    {
        return BasicCheck(preConditionsList);
    }

    protected bool CheckTaskIntegrity()
    {
        return BasicCheck(integrityRules);
    }

    #endregion


    protected virtual IEnumerator TaskExecution()
    {
        return null;
    }

    public virtual void StartExecution()
    {
        taskCouroutineStarter.StartRunningCoroutine(TaskExecution());
    }

    public virtual Task[] DecomposeTask()
    {
        return null;
    }

}
