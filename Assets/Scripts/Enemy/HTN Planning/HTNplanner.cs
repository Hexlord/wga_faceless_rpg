using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTNplanner : MonoBehaviour
{
    
    public BaseAgent[] agents;
    public GameObject player;
    [Tooltip("Points around which the agents wander")]
    public Transform[] nests;
    private BodyStateSystem statesOfTarget;
    private NavigationSystem navSystem;
    private Dictionary<uint, BaseAgent> AgentIdDictionary = new Dictionary<uint, BaseAgent>();
    private Dictionary<uint, Task> IdToTask;
    
    public enum AgentType
    {
        Base,
        Melee,
        Ranged,
        EliteMelee,
        EliteRanged,
    }
    
    public enum AttackTikets
    {
        None,
        MeleePhysical,
        MeleeMagical,
        MeleeCombined,
        RangedPhysical,
        RangedMagical,
        RangedCombined,
    }
    
    
    // Start is called before the first frame update
    void Awake()
    {
        navSystem = GetComponent<NavigationSystem>();
        AgentIdDictionary = new Dictionary<uint, BaseAgent>();
        IdToTask = new Dictionary<uint, Task>();
    }


    void Start()
    {
        foreach (var a in  agents)
        {
            AgentIdDictionary.Add(a.ID, a);
            a.SetControllingSystems(this, navSystem);
            AssignTask(a,a.GetTaskByName("RoamAround"));
        }
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var t in IdToTask.Values)
        {
            if (t.Status != Task.TaskStatus.InProgress )
                t.StartExecution();
        }
    }

    public void AssignTask(BaseAgent a, Task task)
    {
        if (IdToTask.ContainsKey(a.ID))
        {
            IdToTask[a.ID] = task;
        }
        else
        {
            IdToTask.Add(a.ID, task);
        }
    }
    
    
    public void Notify(GameObject targetGO)
    {
        if (player != null)
        {
            if (player.Equals(targetGO))
            {
                statesOfTarget = player.GetComponent<BodyStateSystem>();
                foreach (BaseAgent agent in agents)
                {
                    if (agent != null) agent.Alert(targetGO);
                }
            }
        }
    }
    
    public Transform ClosestNest(Vector3 agentPosition)
    {
        float maxDistance = 1000.0f;
        int index = 0;
        Vector3 vectorToNest;
        for(int i = 0; i < nests.Length; i++)
        {
            vectorToNest = agentPosition - nests[i].transform.position;
            if (vectorToNest.magnitude < maxDistance)
            {
                index = i;
                maxDistance = vectorToNest.magnitude;
            }
        }
        return nests[index];
    }
    
    public void AgentDied(uint agent)
    {
        AgentIdDictionary.Remove(agent);
        IdToTask.Remove(agent);
        Debug.Log("Agent" + agent + " has died and removed.");
    }

    public bool TragetStateIsPhysical()
    {
        return statesOfTarget.State == BodyStateSystem.BodyState.Physical;
    }
    
    public void StartRunningCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
