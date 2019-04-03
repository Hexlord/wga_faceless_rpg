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

    public static GameObject TraverseParent(this GameObject gameObject, string tag)
    {
        if (gameObject.transform.parent &&
            gameObject.transform.parent.tag == tag) return gameObject.transform.parent.gameObject.TraverseParent(tag);
        return gameObject;
    }

    /*
     * Can even find inactive
     */
    public static GameObject FindPrecise(this GameObject parent, string name)
    {
        var trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
