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
public class PointAttractor : AttractorBase
{
    protected override void Attract(Rigidbody body, HealthSystem health)
    {
        base.Attract(body, health);
        float distance = Vector3.Distance(body.position, transform.position);
        if (distance > Mathf.Epsilon && distance < distanceHighpass)
        {
            Vector3 force = transform.position - body.position;
            force.Normalize();
            force = force / Mathf.Pow(distance, distancePower) * strength * Time.fixedDeltaTime;

            if (useForce)
                body.AddForce(force, ForceMode.Impulse);
            else
                body.MovePosition(body.position + force);

            if (health)
            {
                health.DealDamage(damagePerSecond * Time.fixedDeltaTime);
            }
        }
    }
}
