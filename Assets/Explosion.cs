using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float damage = 75.0f;
    public float radius = 3.0f;

    private void OnTriggerEnter(Collider other)
    {
        var colliders = Physics.OverlapSphere(transform.position,
            radius,
            LayerMask.GetMask("Magical", "Physical", "Combined"));
        foreach (var col in colliders)
        {
            switch (col.tag)
            {
                case "Environment":
                    break;
                case "Critical":
                    col.transform.root.GetComponent<HealthSystem>().Damage(
                        GetComponent<CollisionDamageProjectile>().source, 
                        damage);
                    break;
                case "Weapon":
                    break;
                case "Shield":
                    break;
                case "Body":
                    col.transform.root.GetComponent<HealthSystem>().Damage(
                        GetComponent<CollisionDamageProjectile>().source, 
                        damage);
                    break;
            }
        }
    }
}
