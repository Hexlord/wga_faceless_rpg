using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiveAISystem : MonoBehaviour
{

    public NavigationSystem navSystem;
    public BaseAgent[] agents;
    private Dictionary<uint, BaseAgent> agentsDictionary = new Dictionary<uint, BaseAgent>();
    private Dictionary<uint, AgentType> agentsTypes = new Dictionary<uint, AgentType>();
    private Dictionary<uint, Order> agentsOrders = new Dictionary<uint, Order>();
    private GameObject target;
    private BodyStateSystem statesOfTarget;
    private GameObject[] recreationalArea;

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
    public enum OrderType
    {
        None,
        ProtectArea,
        RoamAround,
        HuntDown,
    }

    public struct Order
    {
        public OrderType type;
        public GameObject target;
        public Order (OrderType t, GameObject go)
        {
            type = t;
            target = go;
        }
    }

    public int basicAttacksTickets;
    public int combinedAttacksTickets;

    private bool playerSpotted;

    private void Awake()
    {
        foreach(BaseAgent agent in agents)
        {
            agent.SetControllingSystems(this, navSystem);
            agentsDictionary.Add(agent.ID, agent);
            agentsTypes.Add(agent.ID, agent.AgentType());
            agentsOrders.Add(agent.ID, new Order(OrderType.RoamAround, ClosestRecreationalArea(agent.transform.position)));
        }
    }

    public AttackTikets GetTicket(uint agent)
    {
        if ((basicAttacksTickets == 0) && (combinedAttacksTickets == 0)) return AttackTikets.None;
        switch (agentsTypes[agent])
        {
            case AgentType.Base:
                return AttackTikets.None;
            case AgentType.Melee:
                if (basicAttacksTickets > 0)
                {
                    int rnd = Random.Range(0, 2);
                    return (rnd == 1) ? AttackTikets.MeleePhysical : AttackTikets.MeleeMagical;
                }
                else
                {
                    return AttackTikets.None;
                }
            case AgentType.EliteMelee:
                return AttackTikets.None;
            case AgentType.Ranged:
                return AttackTikets.None;
            case AgentType.EliteRanged:
                return AttackTikets.None;
        }
        return AttackTikets.None;

    }

    public void Notify(GameObject targetGO)
    {
        playerSpotted = true;
        target = targetGO;
        if (target != null)
        {
            statesOfTarget = target.GetComponent<BodyStateSystem>();
            foreach (BaseAgent agent in agents)
            {
                if (agent != null) agent.Alert();
                agentsOrders[agent.ID] = new Order(OrderType.HuntDown, target);
            }
        }
    }

    public void AgentDied(uint agent)
    {
        agentsDictionary.Remove(agent);
        agentsTypes.Remove(agent);
        agentsOrders.Remove(agent);
    }

    GameObject ClosestRecreationalArea(Vector3 agentPosition)
    {
        float maxDistance = 1000.0f;
        int index = 0;
        Vector3 vectorToRA;
        for(int i = 0; i < recreationalArea.Length; i++)
        {
            vectorToRA = agentPosition - recreationalArea[i].transform.position;
            if (vectorToRA.magnitude < maxDistance)
            {
                index = i;
                maxDistance = vectorToRA.magnitude;
            }
        }
        return recreationalArea[index];
    }

    void Update()
    {
        foreach (BaseAgent agent in agents)
        {
            if (agent != null)
            {
                switch (agentsOrders[agent.ID].type)
                {
                    case OrderType.None:
                        break;
                    case OrderType.HuntDown:
                        switch (agentsTypes[agent.ID])
                        {
                            case AgentType.Base:
                                break;
                            case AgentType.Melee:

                                break;
                            case AgentType.Ranged:
                                break;
                            case AgentType.EliteRanged:
                                break;
                            case AgentType.EliteMelee:
                                break;
                        }
                        break;
                    case OrderType.RoamAround:
                        break;
                    case OrderType.ProtectArea:
                        break;
                }
            }
        }
    }

    private void MeleeHuntDown(uint ID)
    {

    }
}
