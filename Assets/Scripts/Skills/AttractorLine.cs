using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */
public class AttractorLine : AttractorBase
{

    [Header("Line Settings")]

    [Tooltip("Start of line segment")]
    public Vector3 lineFrom;
    public Vector3 lineTo;


    protected override void Attract(Rigidbody body, HealthSystem health)
    {
        base.Attract(body, health);

        bool insideSegment = true;
        Vector3 contactPoint = Vector3.zero;

        Vector3 v = lineTo - lineFrom;
        Vector3 w = body.position - lineFrom;

        float c1 = Vector3.Dot(w, v);
        float c2 = Vector3.Dot(v, v);
        if (c1 <= 0 || c2 <= c1)
        {
            insideSegment = false;
        }
        else
        {
            contactPoint = lineFrom + (c1 / c2) * v;
        }

        if (!insideSegment) return;

        float distance = Vector3.Distance(body.position, contactPoint);
        if (distance > Mathf.Epsilon && distance < distanceHighpass)
        {
            Vector3 force = contactPoint - body.position;
            force.Normalize();
            force = force / Mathf.Pow(distance, distancePower) * strength * Time.fixedDeltaTime;

            if (useForce)
                body.AddForce(force, ForceMode.Impulse);
            else
                body.MovePosition(body.position + force);

            if (health)
            {
                health.Damage(gameObject, damagePerSecond * Time.fixedDeltaTime);
            }
        }
    }

}
