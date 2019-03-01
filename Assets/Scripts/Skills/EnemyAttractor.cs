using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttractor : MonoBehaviour
{

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
    public float damagePerSecond = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Faceless");
        foreach(GameObject enemy in enemies)
        {
            Rigidbody body = enemy.GetComponent<Rigidbody>();
            HealthSystem health = enemy.GetComponent<HealthSystem>();

            if (body)
            {
                float distance = Vector3.Distance(body.position, transform.position);
                if (distance > Mathf.Epsilon && distance < distanceHighpass)
                {
                    Vector3 force = transform.position - body.position;
                    force.Normalize();
                    force = force / Mathf.Pow(distance, distancePower) * strength * Time.fixedDeltaTime;
                    
                    if(useForce)
                        body.AddForce(force, ForceMode.Impulse);
                    else
                        body.MovePosition(body.position + force);
                    
                    if (health)
                    {
                        health.DealDamage(damagePerSecond * Time.fixedDeltaTime);
                    }
                }
            }
        }
        
    }
}
