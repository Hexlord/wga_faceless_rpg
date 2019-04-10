using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */
[AddComponentMenu("ProjectFaceless/Skills/Base Attractor")]
public class AttractorBase : MonoBehaviour
{
    

    [Header("Target settings")]

    [Header("Attract Force Settings")]

    [Tooltip("Strength of attraction")]
    public float strength = 1.0f;
    [Tooltip("Increasing this value makes distance attenuation higher (more distance -> less attraction)")]
    public float distancePower = 0.5f;
    [Tooltip("Maximum distance for attraction to take place")]
    public float distanceHighpass = 10.0f;
    [Tooltip("If true, use impulse force, else use position attraction")]
    public bool useForce = true;
    
    [Header("Damage Settings")]

    [Tooltip("Damage of attraction per second")]
    public float damagePerSecond = 0.0f;

    [Tooltip("Damage source")]
    public GameObject source;

    // Cache
    private ChildCollection enemyCollection;

    protected virtual void Awake()
    {
        enemyCollection = GameObject.Find("Enemies").GetComponent<ChildCollection>();
    }

    protected virtual void Attract(Rigidbody body, HealthSystem health)
    {
        // To be overridden
    }


    protected void FixedUpdate()
    {
        foreach(var enemy in enemyCollection.Childs)
        {
            var body = enemy.GetComponent<Rigidbody>();
            var health = enemy.GetComponent<HealthSystem>();

            if (body)
            {
                Attract(body, health);
            }
        }
        
    }
}
