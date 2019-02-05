using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    private string targetTag;
    private bool isStriking = false;
    private float damage;

    public float Damage
    {
        set
        {
            damage = value;
        }
    }

    public string TargetTag
    {
        set
        {
            try
            {
                targetTag = value;
            }
            catch
            {
                Debug.LogError("Something wrong with the tag \"" + value + "\"");
            }
        }
    }

    public void TriggerStricking()
    {
            isStriking = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (isStriking)
        {
            if (col.tag == targetTag)
            {
                try
                {
                    col.gameObject.GetComponent<HealthSystem>().DealDamage(damage);
                    isStriking = false;
                }
                catch
                {
                    Debug.LogError("NO HP SYSTEM ATTACHED");
                }
            }
        }
    }
}
