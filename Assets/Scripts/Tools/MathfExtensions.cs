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

public static class MathfExtensions
{
    public static float NormalizeAngle(float angle)
    {
        var num = Mathf.Repeat(angle, 360f);
        if (num > 180.0) num -= 360f;
        return num;
    }
}
