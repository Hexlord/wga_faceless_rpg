using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;
using Assets.Scripts.Tools;

public class NavigationSystem : MonoBehaviour
{

    Thread pathFindingThread;

    public BaseAgent[] agents;
    public Dictionary<BaseAgent, NavMeshPath> AgentsToPaths;
    public Dictionary<BaseAgent, Vector3> AgentsToDestinations;
    //Private
    Vector3[] agentsPositions;
    Vector3[] agentsDestinations;
    NavMeshPath[] agentsPaths;
    delegate void pathFindingDelegate(Vector3[] starts, Vector3[] finishes, NavMeshPath[] paths);
    bool isCalculating;

    void Start()
    {
        agentsPositions = new Vector3[agents.Length];
        foreach(BaseAgent agent in agents)
        {
            AgentsToPaths.Add(agent, new NavMeshPath());
        }
        isCalculating = false;
    }

    public void SetNewDestination(BaseAgent agent, Vector3 Destination)
    {
        if (AgentsToDestinations.ContainsKey(agent)) AgentsToDestinations.Remove(agent);
        AgentsToDestinations.Add(agent, Destination);
    }

    private void CollectPositions()
    {
        for (int i = 0; i < agentsPositions.Length; i++)
        {
            agentsPositions[i] = agents[i].transform.position;
        }
    }

    private void FixData()
    {
        agentsPaths = new List<NavMeshPath>(AgentsToPaths.Values).ToArray();
        agentsDestinations = new List<Vector3>(AgentsToDestinations.Values).ToArray();
    }

    // Update is called once per frame

    void StartPathFinding()
    {
        isCalculating = true;
        StartCoroutine(Synchronise());

        FixData();
        pathFindingThread = new Thread(new ThreadStart(FindAllPaths));
        pathFindingThread.Start();
    }

    void FinalizePathFinding()
    {

    }

    IEnumerator Synchronise()
    {
        while (isCalculating)
        {
            yield return null;
        }
        FinalizePathFinding();
    }

    public void FindAllPaths()
    {
        if (agentsPositions.Length != agentsDestinations.Length || agentsDestinations.Length != agentsPaths.Length || agentsPaths.Length != agentsPositions.Length) return; //Error
        for (int i = 0; i < agentsPositions.Length; i++)
        {
            NavMesh.CalculatePath(agentsPositions[i], agentsDestinations[i], NavMesh.AllAreas, agentsPaths[i]);
        }
        isCalculating = false;
    }
}
