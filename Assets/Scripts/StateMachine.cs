using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate bool Transition();
//class to store two states which connected by transition. S - enum.
public struct StatePair<S>
{
    public StatePair(S firstState, S secondState)
    {
        this.firstState = firstState;
        this.secondState = secondState;
    }

    public S firstState { get; private set; }

    public S secondState { get; private set; }

}

enum State
{
    Idle,
    Walk,
    Run
}

public class WeightedTransition
{
    public WeightedTransition(Transition transition, uint weight)
    {
        this.transition = transition;
        this.weight = weight;
    }
    public Transition transition
    {
        get; set;
    }
    public uint weight
    {
        get; set;
    }
}

public class Transitions
{
    private LinkedList<WeightedTransition> possibleTransitions;
    private int sumWeights = 0;
    private System.Random random;

    public Transitions()
    {
        possibleTransitions = new LinkedList<WeightedTransition>();
        random = new System.Random();
    }

    public void AddTransition(Transition transition, uint weight)
    {
        if (possibleTransitions.Count == 0)
            possibleTransitions.AddLast(new WeightedTransition(transition, weight));
        else
        {
            for (var node = possibleTransitions.First; node != null; node = node.Next)
            {
                if (node.Value.weight > weight)
                {
                    possibleTransitions.AddBefore(node, new WeightedTransition(transition, weight));
                    break;
                }
                if (node.Next == null)
                {
                    possibleTransitions.AddAfter(node, new WeightedTransition(transition, weight));
                }
            }

        }
        sumWeights += (int)weight;
    }

    public void RemoveTransiton(Transition transition, uint weight)
    {
        possibleTransitions.Remove(new WeightedTransition(transition, weight));
        sumWeights -= (int)weight;
    }

    public Transition GetRandomTransition()
    {
        if (sumWeights == 0)
        {
            throw new System.Exception("Sum of weights equals to 0");
        }
        int randomValue = random.Next(sumWeights - 1);
        uint passedWeightSum = 0;
        for (var node = possibleTransitions.First; node != null; node = node.Next)
        {
            passedWeightSum += node.Value.weight;
            if (randomValue < passedWeightSum)
            {
                return node.Value.transition;
            }
        }
        return null; // To tell compiler that all code path returns something, although it never should return null
    }
}

//V - number of verticies
public class Graph
{
    private int[,] graph;
    private bool isBidirectional;


    private void _SetVertex(uint startVertex, uint endVertex, int weight)
    {
        if (startVertex >= verticiesCount || endVertex >= verticiesCount)
        {
            throw new System.Exception("Vertex number out of range");
        }
        graph[startVertex, endVertex] = weight;
        if (isBidirectional == true)
        {
            graph[endVertex, startVertex] = weight;
        }
    }



    Graph(uint verticiesCount, bool isBidirectional = false)
    {
        graph = new int[verticiesCount, verticiesCount];
        this.verticiesCount = (int)verticiesCount;
        this.isBidirectional = isBidirectional;
    }


    public void ConnectVerticies(uint startVertex, uint endVertex)
    {
        if (startVertex >= verticiesCount || endVertex >= verticiesCount)
        {
            throw new System.Exception("Vertex number out of range");
        }
        graph[startVertex, endVertex]++;
        if (isBidirectional == true)
        {
            graph[endVertex, startVertex]++;
        }
    }

    public void DisconnectVerticies(uint startVertex, uint endVertex)
    {
        if (startVertex >= verticiesCount || endVertex >= verticiesCount)
        {
            throw new System.Exception("Vertex number out of range");
        }
        graph[startVertex, endVertex] = graph[startVertex, endVertex] == 0 ? 0 : graph[startVertex, endVertex] - 1;
        if (isBidirectional == true)
        {
            graph[endVertex, startVertex] = graph[endVertex, startVertex] == 0 ? 0 : graph[endVertex, startVertex] - 1;
        }
    }

    public int BFS(uint startVertex)
    {
        if (startVertex >= verticiesCount)
        {
            throw new System.Exception("Vertex number out of range");
        }

        List<bool> visited = new List<bool>(verticiesCount);
        visited[(int)startVertex] = true;
        Queue<uint> queue = new Queue<uint>();
        queue.Enqueue(startVertex);

        while (queue.Count != 0)
        {
            uint currentVertex = queue.Dequeue();
            for (int i = 0; i < verticiesCount; i++)
            {
                if (graph[currentVertex, i] != 0f && visited[i] == false)
                {
                    visited[i] = true;
                    queue.Enqueue((uint)i);
                }
            }
        }
        int countVisited = 0;
        for (int i = 0; i < verticiesCount; i++)
        {
            if (visited[i] == true)
                countVisited++;
        }
        return countVisited;
    }

    public bool isConnected()
    {
        for (int i = 0; i < verticiesCount; i++)
        {
            int visited = BFS((uint)i);
            if (visited == verticiesCount)
                return true;
        }
        return false;
    }

    public int verticiesCount { get; private set; }
}

//S - enum. There is no way to specify that in C# version built in Unity
public class StateMachine<S> where S : struct
{
    private Dictionary<StatePair<S>, Transitions> stateMachine;
    private Dictionary<S, uint> stateIDs;
    private Graph statesGraph;

    private void _InitializeStateIDs()
    {
        var valuesArray = System.Enum.GetValues(typeof(S));
        foreach (S obj in valuesArray)
        {
            System.Enum enumValue = System.Enum.Parse(typeof(S), obj.ToString()) as System.Enum;
            uint numericEnumValue = System.Convert.ToUInt32(enumValue);
            stateIDs[obj] = numericEnumValue;
        }
    }
    private void _InitializeLoops(List<Transition> loops, List<uint> weights)
    {
        if (loops.Count != statesGraph.verticiesCount || loops.Count != weights.Count)
        {
            throw new System.Exception("Number of loops does not equal to number of states or number of weights");
        }
        var valuesArray = System.Enum.GetValues(typeof(S));

        for (int i = 0; i < valuesArray.Length; i++)
        {
            CreateTransition((S)valuesArray.GetValue(i), (S)valuesArray.GetValue(i), loops[i], weights[i]);
        }
    }

    public int statesCount { get; private set; }

    public StateMachine(S initialState, List<Transition> loops, List<uint> weights, bool bidirectional = false)
    {
        if (!typeof(S).IsEnum)
        {
            throw new System.Exception("S must be an Enum");
        }
        stateMachine = new Dictionary<StatePair<S>, Transitions>();
        stateIDs = new Dictionary<S, uint>();
        statesCount = System.Enum.GetNames(typeof(S)).Length;
        currentState = initialState;
        isBdirectional = bidirectional;
        _InitializeStateIDs();
        _InitializeLoops(loops, weights);
    }

    // current state machine's state
    public S currentState { get; private set; }

    //is state graph bidirectional
    public bool isBdirectional { get; private set; }

    //checks if transition between states is present
    public bool CheckTransition(S startState, S endState)
    {
        return stateMachine.TryGetValue(new StatePair<S>(startState, endState), out Transitions dummyTransitions);
    }

    //add transition between states if it wasn't present already
    public void CreateTransition(S startState, S endState, Transition transition, uint weight)
    {
        Transitions transitions = new Transitions();
        transitions.AddTransition(transition, weight);
        if (CheckTransition(startState, endState) == false)
        {
            stateMachine.Add(new StatePair<S>(startState, endState), transitions);
            statesGraph.ConnectVerticies(stateIDs[startState], stateIDs[endState]);

            if (isBdirectional == true)
            {
                stateMachine.Add(new StatePair<S>(endState, startState), transitions);
                statesGraph.ConnectVerticies(stateIDs[endState], stateIDs[startState]);
            }
        }
    }

    //change transition between states if it was present already
    public void AddTransition(S startState, S endState, Transition transition, uint weight)
    {
        if (CheckTransition(startState, endState) == false)
        {
            stateMachine[new StatePair<S>(startState, endState)].AddTransition(transition, weight);
            statesGraph.ConnectVerticies(stateIDs[startState], stateIDs[endState]);
            if (isBdirectional == true)
            {
                stateMachine[new StatePair<S>(endState, startState)].AddTransition(transition, weight);
                statesGraph.ConnectVerticies(stateIDs[endState], stateIDs[startState]);
            }
        }
    }

    public void AddLoop(S state, Transition transition, uint weight)
    {
        AddTransition(state, state, transition, weight);
    }

    //remove transition between states
    public void RemoveTransition(S startState, S endState)
    {
        stateMachine.Remove(new StatePair<S>(startState, endState));
        statesGraph.ConnectVerticies(stateIDs[startState], stateIDs[endState]);
        if (isBdirectional == true)
        {
            stateMachine.Remove(new StatePair<S>(endState, startState));
            statesGraph.ConnectVerticies(stateIDs[endState], stateIDs[startState]);
        }
    }

    public bool isConnected()
    {
        return statesGraph.isConnected();
    }

    //invokes transition between states
    public bool Invoke(S nextState)
    {
        if (!isConnected())
        {
            throw new System.Exception("State Machine is not connected");
        }
        StatePair<S> stateTransition = new StatePair<S>(currentState, nextState);
        if (stateMachine.TryGetValue(stateTransition, out Transitions transitions) == true)
        {
            Transition transition = transitions.GetRandomTransition();
            transition();
            currentState = nextState;
            return true;
        }
        return false;
    }
}