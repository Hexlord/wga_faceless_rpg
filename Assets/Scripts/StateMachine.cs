using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine
{
    public delegate void Transition(string startState, string endState);

    private Dictionary<StateTransition, Transition> stateMachine;

    public StateMachine(string initialState, bool bidirectional = false)
    {
        stateMachine = new Dictionary<StateTransition, Transition>();
        curentState = initialState;
        isBdirectional = bidirectional;
    }

    // current state machine's state
    public string curentState { get; private set; }

    //is state graph bidirectional
    public bool isBdirectional { get; private set; }

    private void _ModifyTransition(string startState, string endState, Transition transition, bool isPresent)
    {
        if (CheckTransition(startState, endState) == isPresent)
        {
            stateMachine[new StateTransition(startState, endState)] = transition;
        }
    }

    //checks if transition between states is present
    public bool CheckTransition(string startState, string endState)
    {
        return stateMachine.TryGetValue(new StateTransition(startState, endState), out Transition dummyTransition);
    }

    //add transition between states if it wasn't present already
    public void AddTransition(string startState, string endState, Transition transition)
    {
        _ModifyTransition(startState, endState, transition, false);
        if (isBdirectional == true)
        {
            _ModifyTransition(endState, startState, transition, false);
        }
    }

    //change transition between states if it was present already
    public void ChangeTransition(string startState, string endState, Transition transition)
    {
        _ModifyTransition(startState, endState, transition, true);
        if (isBdirectional == true)
        {
            _ModifyTransition(endState, startState, transition, true);
        }
    }

    //remove transition between states
    public void RemoveTransition(string startState, string endState)
    {
        stateMachine.Remove(new StateTransition(startState, endState));
        if (isBdirectional == true)
        {
            stateMachine.Remove(new StateTransition(endState, startState));
        }
    }

    //invokes transition between states
    public bool Invoke(string nextState)
    {
        StateTransition stateTransition = new StateTransition(curentState, nextState);
        if (stateMachine.TryGetValue(stateTransition, out Transition transition) == true)
        {
            transition(curentState, nextState);
            curentState = nextState;
            return true;
        }
        return false;
    }
}
