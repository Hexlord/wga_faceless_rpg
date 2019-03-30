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
[AddComponentMenu("ProjectFaceless/GameLogic/Enemy Protected Gate")]
public class EnemyProtectedGate : MonoBehaviour
{
    

    [Header("Trigger Open Settings")]

    [Tooltip("Open gates when there are no alive enemies in that range")]
    public float protectionRange = 10.0f;


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
                        if(health.Alive)
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
