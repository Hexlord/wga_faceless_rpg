using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class Task : MonoBehaviour
{
    private string taskName;
    
    private TaskStatus taskStatus;
    
    protected delegate bool Condition(); 
    
    protected readonly Condition[] preConditionsList;
    protected readonly Condition[] integrityRules;


    public Task(string name, Func<bool>[] conditions, Func<bool>[] rules)
    {
        taskName = name;
        preConditionsList = new Condition[conditions.Length];
        integrityRules = new Condition[rules.Length];
        conditions.CopyTo(preConditionsList, 0);
        rules.CopyTo(integrityRules, 0);
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
        get { return TaskStatus.None; }
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
    
    #endregion
    
    protected bool CheckTaskIntegrity()
    {
        return BasicCheck(integrityRules);
    }

    protected virtual IEnumerator TaskExecution()
    {
        return null;
    }

    public virtual void StartExecution()
    {
    }

    public virtual Task[] DecomposeTask()
    {
        return null;
    }

}
