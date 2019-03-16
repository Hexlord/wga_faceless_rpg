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

public static class GameObjectExtensions
{
    public static GameObject TraverseParent(this GameObject gameObject)
    {
        if (gameObject.transform.parent) return gameObject.transform.parent.gameObject.TraverseParent();
        return gameObject;
    }
}
