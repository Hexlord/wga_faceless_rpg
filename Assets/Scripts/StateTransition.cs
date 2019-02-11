using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to store two states which connected by transition
public struct StateTransition
{
    public StateTransition(string firstState, string secondState)
    {
        this.firstState = firstState;
        this.secondState = secondState;
    }

    public string firstState { get; private set; }

    public string secondState { get; private set; }

}
