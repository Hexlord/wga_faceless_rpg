using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;
using Assets.Scripts.Tools;
[AddComponentMenu("ProjectFaceless/Enemy/Navigation System")]
public class NavigationSystem : MonoBehaviour
{
    [Tooltip("Minimal range before agent start moving towards new point")]
    public float agentWaypointStoppingDistance = 0.05f;
    [Tooltip("Minimal range when agent stops moving")]
    public float agentDestinationStoppingDistance = 0.5f;
    [Tooltip("Maximal range for searching navmesh point")]
    public float offNavMeshPointSearchRadius = 50.0f;
    public int numberOfTries = 3;

    enum RequestStatus
    {
        None,
        Placed,
        Completed,
        Failed,
    }
    
    private float agentStoppingDistance;
    private Dictionary<uint, BaseAgent> agents;
    private Dictionary<uint, Queue<Vector3>> AgentsToPaths;
    private Dictionary<uint, RequestStatus> AgentsRequestStatuses;
    private Dictionary<uint, Vector3> AgentsToDestinations;
    //Private
    Queue<PathFindingRequestInfo> pathFindingQueue;
    NavMeshPath processingPath;
    bool isCalculating;

    public void Awake()
    {
        processingPath = new NavMeshPath();
        pathFindingQueue = new Queue<PathFindingRequestInfo>(); ;
        AgentsRequestStatuses = new Dictionary<uint, RequestStatus>();
        AgentsToDestinations = new Dictionary<uint, Vector3>();
        AgentsToPaths = new Dictionary<uint, Queue<Vector3>>();
        agents = new Dictionary<uint, BaseAgent>();
    }

    public struct PathFindingRequestInfo
    {
        public BaseAgent agent;
        public Vector3 agentStart;
        public Vector3 agentDestination;


        public PathFindingRequestInfo(BaseAgent a, Vector3 s, Vector3 f)
        {
            agent = a;
            agentStart = s;
            agentDestination = f;
        }
    }

    private void RegisterAgent(BaseAgent agent)
    {
        agents.Add(agent.ID, agent);
        AgentsToPaths.Add(agent.ID, new Queue<Vector3>());
        AgentsRequestStatuses.Add(agent.ID, RequestStatus.None);
    }
    
    public void ClearRequest(uint ID)
    {
        AgentsRequestStatuses[ID] = RequestStatus.None;
    }

    public void PlacePathRequest(BaseAgent agent, Vector3 finish)
    {
        if (!agents.ContainsKey(agent.ID))
        {
            RegisterAgent(agent);
        }
        if (AgentsRequestStatuses[agent.ID] != RequestStatus.Failed)
        {
            PathFindingRequestInfo newRequest = new PathFindingRequestInfo(agent, finish, agent.transform.position);
            pathFindingQueue.Enqueue(newRequest);
            AgentsRequestStatuses[agent.ID] = RequestStatus.Placed;

        }

        if (AgentsToDestinations.ContainsKey(agent.ID))
        {
            AgentsToDestinations[agent.ID] = finish;
        }
        else
        {
            AgentsToDestinations.Add(agent.ID, finish);
        }
    }

    public Vector2 AskDirection(uint ID)
    {
        Vector3 direction = new Vector3(0, 0, 0);
        if (!AgentsRequestStatuses.ContainsKey(ID)) return Vector2.zero;
        if (AgentsRequestStatuses[ID] == RequestStatus.Completed)
        {
            if (AgentsToPaths[ID].Count > 0)
            {
                direction = AgentsToPaths[ID].Peek() - agents[ID].transform.position;
            }
            direction.y = 0;
            agentStoppingDistance = (AgentsToPaths[ID].Count == 1) ? agentDestinationStoppingDistance : agentWaypointStoppingDistance;

            if (direction.magnitude <= agentStoppingDistance)
            {
                //AgentsToPaths[ID].Dequeue();
                Debug.Log("Reached corner" + AgentsToPaths[ID].Dequeue());
                if (AgentsToPaths[ID].Count > 0)
                {
                    direction = AgentsToPaths[ID].Peek() - transform.position;
                    direction.y = 0;
                }
                else
                {
                    direction = Vector3.zero;
                    AgentsRequestStatuses[ID] = RequestStatus.None;
                }
            }
        }
        return new Vector2(direction.x, direction.z);
    }

    // Update is called once per frame
    private void Update()
    {
        if (pathFindingQueue.Count > 0 && !isCalculating)
        {
            ProcessRequests();
            //StartPathFinding();
        }
    }

    private void ProcessRequests()
    {
        PathFindingRequestInfo infoChunk;
        Vector3[] cornersOfPath = { Vector3.zero };

        while (pathFindingQueue.Count > 0)
        {
            infoChunk = pathFindingQueue.Dequeue();
            NavMesh.CalculatePath(infoChunk.agentStart, infoChunk.agentDestination, NavMesh.AllAreas, processingPath);
            int attempts = 0;

            if (processingPath.status != NavMeshPathStatus.PathInvalid)
            {
                cornersOfPath = processingPath.corners;
                //Array.Copy(processingPath.corners, cornersOfPath, processingPath.corners.Length - 2);
                Array.Reverse(cornersOfPath);
                AgentsToPaths[infoChunk.agent.ID] = new Queue<Vector3>(cornersOfPath);
                AgentsToPaths[infoChunk.agent.ID].Dequeue();
                AgentsRequestStatuses[infoChunk.agent.ID] = RequestStatus.Completed;
                Debug.Log("Finished calculating path for " + infoChunk.agent);
            }
            else
            {
                NavMeshHit hit, hit1;
                if ((NavMesh.SamplePosition(infoChunk.agentStart, out hit, offNavMeshPointSearchRadius, NavMesh.AllAreas))
                    && (NavMesh.SamplePosition(infoChunk.agentDestination, out hit1, offNavMeshPointSearchRadius, NavMesh.AllAreas)))
                    NavMesh.CalculatePath(hit.position, hit1.position, NavMesh.AllAreas, processingPath);
                else
                {
                    Debug.Log("Failed To process path for " + infoChunk.agent);
                    AgentsRequestStatuses[infoChunk.agent.ID] = RequestStatus.Failed;
                }
            }
        }
        isCalculating = false;
        Debug.Log("AI. All agents recieved their paths.");
    }

    public Vector3 AgentDestination(uint ID)
    {
        if (AgentsToDestinations.ContainsKey(ID))
        {
            return AgentsToDestinations[ID];
        }
        return Vector3.zero;
    }

    public bool hasAgentReachedDestination(uint ID)
    {
        return (AgentsRequestStatuses.ContainsKey(ID) && (AgentsRequestStatuses[ID] == RequestStatus.None)) || (!AgentsRequestStatuses.ContainsKey(ID));
    }
}
