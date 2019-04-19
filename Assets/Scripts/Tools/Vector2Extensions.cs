using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 27.03.2019   aknorre     Created
 * 
 */

public static class Vector2Extensions
{


    public static bool Project(Vector2 a, Vector2 b, Vector2 p, out Vector2 contactPoint)
    {
        contactPoint = Vector2.zero;

        var v = b - a;
        var w = p - a;

        var c1 = Vector2.Dot(w, v);
        var c2 = Vector2.Dot(v, v);
        if (c1 <= 0 || c2 <= c1)
        {
            return false;
        }

        contactPoint = a + (c1 / c2) * v;
        return true;
    }
}
