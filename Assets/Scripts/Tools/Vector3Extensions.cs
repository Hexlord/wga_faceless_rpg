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
}
