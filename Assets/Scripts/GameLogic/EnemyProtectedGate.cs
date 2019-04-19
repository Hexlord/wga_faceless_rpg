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

    // Cache
    private ChildCollection enemyCollection;

    private void Awake()
    {
        enemyCollection = GameObject.Find("Enemies").GetComponent<ChildCollection>();
    }

    void FixedUpdate()
    {
        var good = true;

        foreach (var enemy in enemyCollection.Childs)
        {
            var body = enemy.GetComponent<Rigidbody>();
            var health = enemy.GetComponent<HealthSystem>();

            if (!body) continue;

            var distance = Vector3.Distance(body.position, transform.position);

            if (!(distance > Mathf.Epsilon) || !(distance < protectionRange) || !health || !health.Alive) continue;

            good = false;
            break;
        }

        if (good)
        {
            Destroy(gameObject);
        }
    }
}
