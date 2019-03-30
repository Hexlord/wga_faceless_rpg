using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 
 */

public static class ColliderExtensions
{
    public static bool IsPartOf(this Collider collider, GameObject gameObject)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            if (col == collider) return true;
        }

        return false;
    }
    
    public static void IgnoreCollisionsWith(this Collider collider, GameObject gameObject)
    {

        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            Physics.IgnoreCollision(collider, col);
        }
    }

    public static void IgnoreCollisionsWith(this Collider collider, string tag)
    {

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
        
        foreach (GameObject gameObject in gameObjects)
        {
            collider.IgnoreCollisionsWith(gameObject);
        }
    }
}
