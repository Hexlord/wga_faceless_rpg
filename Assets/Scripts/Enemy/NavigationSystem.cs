using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;
using Assets.Scripts.Tools;

public class NavigationSystem : MonoBehaviour
{

    Thread pathFindingThread;

    public float agentWaypointStoppingDistance = 0.005f;
    public float agentDestinationStoppingDistance = 0.5f;

    enum RequestStatus
    {
        None,
        Placed,
        Completed,
        Failed,
    }

    private float agentStoppingDistance;
    private Dictionary<uint, BaseAgent> agents = new Dictionary<uint, BaseAgent>();
    private Dictionary<uint, Queue<Vector3>> AgentsToPaths = new Dictionary<uint, Queue<Vector3>>();
    private Dictionary<uint, RequestStatus> AgentsRequestStatuses = new Dictionary<uint, RequestStatus>();
    //Private
    Queue<PathFindingRequestInfo> pathFindingQueue = new Queue<PathFindingRequestInfo>();

    bool isCalculating;

    public struct PathFindingRequestInfo
    {
        public BaseAgent agent;
        public Vector3 agentDestination;

        public PathFindingRequestInfo(BaseAgent a, Vector3 f)
        {
            agent = a;
            agentDestination = f;
        }
    }

    private void RegisterAgent(BaseAgent agent)
    {
        agents.Add(agent.ID, agent);
        AgentsToPaths.Add(agent.ID, new Queue<Vector3>());
        AgentsRequestStatuses.Add(agent.ID, RequestStatus.None);
    }

    public void PlacePathRequest(uint ID, Vector3 finish)
    {
        if (AgentsRequestStatuses[ID] != RequestStatus.Failed)
        {
            if (!agents.ContainsKey(ID)) RegisterAgent(agents[ID]);

            PathFindingRequestInfo newRequest = new PathFindingRequestInfo(agents[ID], finish);
            pathFindingQueue.Enqueue(newRequest);
            AgentsRequestStatuses[ID] = RequestStatus.Placed;
        }
    }

    public Vector2 AskDirection(uint ID)
    {
        Vector3 direction = new Vector3(0, 0, 0);
        if (AgentsRequestStatuses[ID] == RequestStatus.Completed)
        {
            direction = AgentsToPaths[ID].Peek() - transform.position;
            agentStoppingDistance = (AgentsToPaths[ID].Count == 1) ? agentDestinationStoppingDistance : agentWaypointStoppingDistance;

            if (direction.magnitude <= agentStoppingDistance)
            {
                AgentsToPaths[ID].Dequeue();
                if (AgentsToPaths[ID].Count > 0)
                {
                    direction = AgentsToPaths[ID].Peek() - transform.position;
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
            StartPathFinding();
        }
    }

    private void StartPathFinding()
    {
        isCalculating = true;
        pathFindingThread = new Thread(new ThreadStart(ProcessRequests));
        pathFindingThread.Start();
    }

    private void ProcessRequests()
    {
        PathFindingRequestInfo infoChunk;
        NavMeshPath path = new NavMeshPath();

        while (pathFindingQueue.Count > 0)
        {
            infoChunk = pathFindingQueue.Dequeue();
            NavMesh.CalculatePath(infoChunk.agent.transform.position, infoChunk.agentDestination, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                AgentsToPaths[infoChunk.agent.ID] = new Queue<Vector3>(path.corners);
                AgentsRequestStatuses[infoChunk.agent.ID] = RequestStatus.Completed;
                Debug.Log("Finished calculating path for " + infoChunk.agent);
            }
            else
            {
                AgentsRequestStatuses[infoChunk.agent.ID] = RequestStatus.Failed;
                Debug.Log("Failed To process path for " + infoChunk.agent);
            }
        }
        isCalculating = false;
        Debug.Log("AI. All agents recieved their paths.");
    }

}
