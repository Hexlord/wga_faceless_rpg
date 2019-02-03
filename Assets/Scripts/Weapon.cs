using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public string targetTag;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == targetTag)
        {
            try
            {
                collision.gameObject.GetComponent<HealthSystem>().DealDamage(25.0f);
            }
            catch
            {
                Debug.LogError("NO HP SYSTEM ATTACHED");
            }
        }
    }
}
