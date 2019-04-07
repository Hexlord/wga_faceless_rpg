﻿using System.Collections;
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
    public static Transform FindChildPrecise(this Transform parent, string name, bool partial = true)
    {
        var trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name ||
                partial && t.name.Contains(name))
            {
                return t;
            }
        }
        return null;
    }
}
