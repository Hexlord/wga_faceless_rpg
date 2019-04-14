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
    private static bool Matches(string tag, string target, bool partial)
    {
        if (tag == null || target == null) return false;
        if (tag == target) return true;
        if (partial && tag.Contains(target)) return true;
        return false;
    }

    public static GameObject TraverseParent(this GameObject gameObject)
    {
        while (true)
        {
            if (!gameObject.transform.parent) return gameObject;
            gameObject = gameObject.transform.parent.gameObject;
        }
    }

    public static GameObject TraverseParent(this GameObject gameObject, string tag, bool partial = true)
    {
        while (true)
        {
            if (!gameObject.transform.parent || !Matches(gameObject.transform.parent.tag, tag, partial))
            {
                return Matches(gameObject.transform.tag, tag, partial)
                    ? gameObject
                    : null;
            }
            gameObject = gameObject.transform.parent.gameObject;
        }
    }

    public static GameObject[] Children(this GameObject gameObject)
    {
        var result = new GameObject[gameObject.transform.childCount];

        for (var i = 0; i < result.Length; ++i)
        {
            result[i] = gameObject.transform.GetChild(i).gameObject;
        }

        return result;
    }

    /*
     * Can even find inactive
     */
    public static GameObject FindPrecise(this GameObject parent, string name, bool partial = true)
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
