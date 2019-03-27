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

public static class Vector3Extensions
{
    public static Vector3 SmoothStep(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(
            Mathf.SmoothStep(a.x, b.x, t),
            Mathf.SmoothStep(a.y, b.y, t),
            Mathf.SmoothStep(a.z, b.z, t)
            );
    }

    public static bool Project(Vector3 a, Vector3 b, Vector3 p, out Vector3 contactPoint)
    {
        contactPoint = Vector3.zero;

        var v = b - a;
        var w = p - a;

        var c1 = Vector3.Dot(w, v);
        var c2 = Vector3.Dot(v, v);
        if (c1 <= 0 || c2 <= c1)
        {
            return false;
        }

        contactPoint = a + (c1 / c2) * v;
        return true;
    }
}
