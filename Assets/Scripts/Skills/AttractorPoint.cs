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
[AddComponentMenu("ProjectFaceless/Skills/Point Attractor")]
public class AttractorPoint : AttractorBase
{


    [Tooltip("Ignore Y in distance calculations")]
    public bool ignoreY = false;

    protected override void Attract(Rigidbody body, HealthSystem health)
    {
        base.Attract(body, health);

        var distance = 0.0f;
        var direction = Vector3.zero;

        if (ignoreY)
        {
            distance = Vector2.Distance(
                new Vector2(body.position.x, body.position.z),
                new Vector2(transform.position.x, transform.position.z));

            direction = new Vector3(
                transform.position.x - body.position.x,
                0.0f,
                transform.position.z - body.position.z);
        }
        else
        {
            distance = Vector3.Distance(body.position, transform.position);

            direction = transform.position - body.position;
        }

        if (!(distance > Mathf.Epsilon) || !(distance < distanceHighpass)) return;
        
        if(direction.sqrMagnitude > Mathf.Epsilon) direction.Normalize();
        
        var force = direction * strength / Mathf.Pow(distance, distancePower) * Time.fixedDeltaTime;

        if (useForce)
        {
            body.GetComponent<BaseAgent>().Stun(1.0f);
            body.AddForce(force, ForceMode.Impulse);
        }
        else
            body.MovePosition(body.position + force);

        if (health)
        {
            health.Damage(source ? source : gameObject, damagePerSecond * Time.fixedDeltaTime);
        }
    }
}
