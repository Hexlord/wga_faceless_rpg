using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            DumbEnemy enemyController = gameObject.GetComponentInParent<DumbEnemy>();
            enemyController.notify("Player");
        }
    }
}
