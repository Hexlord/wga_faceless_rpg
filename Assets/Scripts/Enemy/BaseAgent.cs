using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAgent : MonoBehaviour
{
    static uint entityIDGenerator = 0;
    private uint entityID = 0;
    Queue<Vector3> waypoints;
    public NavigationSystem navSys;

    public float attackRange = 0.7f;
    private MovementSystem movement;
    float stoppingDistance;
    GameObject currentTarget;


    enum Order
    {
        Idle,
        Move,
        Protect,
        Hunt,
    }

    private Order currentOrders = Order.Idle;

    public uint ID
    {
        get { return entityID; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        entityID = entityIDGenerator++;
        movement = GetComponent<MovementSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();

    }

    void UpdateMove()
    {
        navSys.AskDirection(entityID);
    }


    public void OrderProtectArea(Vector3 position, float radius)
    {
        currentOrders = Order.Protect;
        if ((transform.position - position).magnitude < stoppingDistance) return;
        navSys.PlacePathRequest(ID, position);
    }

    public void OrderMoveTo(Vector3 position)
    {
        currentOrders = Order.Move;
        if ((transform.position - position).magnitude < stoppingDistance) return;
        navSys.PlacePathRequest(ID, position);
    }

    public void OrderAttackTarget(GameObject target)
    {
        currentOrders = Order.Hunt;
        currentTarget = target;
        if ((transform.position - currentTarget.transform.position).magnitude < stoppingDistance)
        {
            navSys.PlacePathRequest(this, currentTarget.transform.position);
        }

    }
}
