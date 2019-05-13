using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class ComplexTask: Task
{
    public delegate Task[] DecompositionMethod();

    private Plan TaskExecutionPlan;
    
    
    
    readonly protected DecompositionMethod Decompose;
    
    public ComplexTask(string name, Func<bool>[] conditions, Func<bool>[] rules, DecompositionMethod decompose) :
        base(name, conditions, rules)
    {
        Decompose = decompose;
    }

    public override Task[] DecomposeTask()
    {
        return Decompose.Invoke();
    }

    public override void StartExecution()
    {
        SetStatus(TaskStatus.InProgress);
        TaskExecutionPlan = new Plan(this, this.Name);
    }

    protected override IEnumerator TaskExecution()
    {
        this.SetStatus(TaskStatus.InProgress);
        if (!CheckPreConditions())
        {
            this.SetStatus(TaskStatus.Failure);
            yield break;
        }

        while (this.Status != TaskStatus.Complete)
        {
            if (!CheckTaskIntegrity())
            {
                this.SetStatus(TaskStatus.Failure);
                yield break;
            }

            TaskExecutionPlan.PlanIterate();
            if (TaskExecutionPlan.Status == Plan.PlanStatus.Complete)
            {
                this.SetStatus(TaskStatus.Complete);
                yield break;
            }
        }

        yield return null;
    }
}
