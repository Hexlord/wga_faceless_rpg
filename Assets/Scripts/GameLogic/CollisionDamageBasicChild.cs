using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 05.04.2019   aknorre     Created
 * 
 */
[AddComponentMenu("ProjectFaceless/GameLogic/Collision Damage Basic Child")]
public class CollisionDamageBasicChild : MonoBehaviour
{
    private CollisionDamageBasic owner;

    protected void Awake()
    {
        owner = transform.parent.GetComponent<CollisionDamageBasic>();
        Debug.Assert(owner != null);
    }

    public void OnContact()
    {
        // Intentionally left empty
    }

    public void OnTriggerEnter(Collider other)
    {
        owner.OnTriggerEnter(other);
    }
}
