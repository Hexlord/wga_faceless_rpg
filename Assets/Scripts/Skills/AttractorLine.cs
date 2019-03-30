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
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */
[AddComponentMenu("ProjectFaceless/Skills/Line Attractor")]
public class AttractorLine : AttractorBase
{


    [Header("Line Settings")]

    [Tooltip("Start of line segment")]
    public Vector3 lineFrom;
    public Vector3 lineTo;

    [Tooltip("Ignore Y in distance calculations")]
    public bool ignoreY = false;
    

    protected override void Attract(Rigidbody body, HealthSystem health)
    {
        base.Attract(body, health);

        var distance = 0.0f;
        var direction = Vector3.zero;
        if (ignoreY)
        {
            var contactPoint = Vector2.zero;
            var inSegment = Vector2Extensions.Project(
                new Vector2(lineFrom.x, lineFrom.z),
                new Vector2(lineTo.x, lineTo.z),
                new Vector2(body.position.x, body.position.z),
                out contactPoint);

            if (!inSegment) return;
            distance = Vector2.Distance(new Vector2(body.position.x, body.position.z), contactPoint);
            direction = (new Vector3(contactPoint.x, body.position.y, contactPoint.y) - body.position);
        }
        else
        {
            var contactPoint = Vector3.zero;
            var inSegment = Vector3Extensions.Project(
                lineFrom,
                lineTo,
                body.position,
                out contactPoint);

            if (!inSegment) return;
            distance = Vector3.Distance(body.position, contactPoint);
            direction = contactPoint - body.position;
        }

        if(direction.sqrMagnitude > Mathf.Epsilon) direction.Normalize();
        
        if (distance > Mathf.Epsilon && distance < distanceHighpass)
        {
            var force = direction * strength / Mathf.Pow(distance, distancePower) * Time.fixedDeltaTime;

            if (useForce)
                body.AddForce(force, ForceMode.Impulse);
            else
                body.MovePosition(body.position + force);

            if (health)
            {
                health.Damage(source ? source : gameObject, damagePerSecond * Time.fixedDeltaTime);
            }
        }
    }

}
