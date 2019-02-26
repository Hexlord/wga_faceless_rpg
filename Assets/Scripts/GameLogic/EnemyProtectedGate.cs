using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProtectedGate : MonoBehaviour
{

    [Header("Trigger Open Settings")]

    [Tooltip("Open gates when there are no alive enemies in that range")]
    public float protectionRange = 10.0f;

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
        bool good = true;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Faceless");
        foreach (GameObject enemy in enemies)
        {
            Rigidbody body = enemy.GetComponent<Rigidbody>();
            HealthSystem health = enemy.GetComponent<HealthSystem>();

            if (body)
            {
                float distance = Vector3.Distance(body.position, transform.position);
                if (distance > Mathf.Epsilon && distance < protectionRange)
                {
                    if (health)
                    {
                        if(health.HP > 0)
                        {
                            good = false;
                            break;
                        }
                    }
                }
            }
        }

        if(good)
        {
            Destroy(gameObject);
        }
    }
}
