using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAgent : MonoBehaviour
{
    static uint entityIDGenerator = 0;
    private uint entityID = 0;
    Queue<Vector3> waypoints;
    bool systemsSet = false;
    bool isStunned = false;
    float stunDuration = 0.0f;
    float stunStart = 0.0f;
    private NavigationSystem navSys;
    private CollectiveAISystem AISys;

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

    public void Stun(float duration)
    {
        isStunned = true;
        stunDuration = duration;
        stunStart = Time.time;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        entityID = entityIDGenerator++;
    }

    void Start()
    {
        
        movement = GetComponent<MovementSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStunned)
        {
            UpdateMove();
        }

        if (isStunned && Time.time > stunStart + stunDuration)
        {
            isStunned = false;
        }

    }

    void UpdateMove()
    {
        movement.Movement = navSys.AskDirection(entityID);
    }


    public void SetControllingSystems(CollectiveAISystem ai, NavigationSystem nav)
    {
        if (!systemsSet)
        {
            AISys = ai;
            navSys = nav;
        }
    }

    //public void OrderProtectArea(Vector3 position, float radius)
    //{
    //    currentOrders = Order.Protect;
    //    if ((transform.position - position).magnitude < stoppingDistance) return;
    //    navSys.PlacePathRequest(ID, position);
    //}

    //public void OrderMoveTo(Vector3 position)
    //{
    //    currentOrders = Order.Move;
    //    if ((transform.position - position).magnitude < stoppingDistance) return;
    //    navSys.PlacePathRequest(ID, position);
    //}

    //public void OrderAttackTarget(GameObject target)
    //{
    //    currentOrders = Order.Hunt;
    //    currentTarget = target;
    //    if ((transform.position - currentTarget.transform.position).magnitude < stoppingDistance)
    //    {
    //        navSys.PlacePathRequest(entityID, currentTarget.transform.position);
    //    }

    //}
}
