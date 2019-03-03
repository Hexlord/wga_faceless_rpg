using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   bkrylov     Created
 * 
 */
public class EnemyTrigger : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            DumbEnemy enemyController = gameObject.GetComponentInParent<DumbEnemy>();
            enemyController.Notify("Player");
        }
    }
}
