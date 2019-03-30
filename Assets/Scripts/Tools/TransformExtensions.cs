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

public static class TransformExtensions
{

    /*
     * Can even find inactive
     */
    public static Transform FindPrecise(this Transform parent, string name)
    {
        var trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t;
            }
        }
        return null;
    }
}
