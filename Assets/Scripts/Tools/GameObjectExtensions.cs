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

    public static GameObject TraverseParent(this GameObject gameObject, string tag, bool partial = true)
    {
        if (gameObject.transform.parent &&
            (gameObject.transform.parent.tag == tag
             || partial && gameObject.transform.parent.tag.Contains(tag))) return gameObject.transform.parent.gameObject.TraverseParent(tag, partial);
        return gameObject;
    }

    /*
     * Can even find inactive
     */
    public static GameObject FindChildPrecise(this GameObject parent, string name, bool partial = true)
    {
        if (parent == null)
        {
            return null;
        }

        var trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name ||
                partial && t.name.Contains(name))
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
