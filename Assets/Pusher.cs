using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    public float forcePushStrength = 25.0f;

    private CollisionDamageBasic hitbox;
    private bool pushes;
    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<CollisionDamageBasic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hitbox.DealsDamage)
            pushes = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pushes)
        {
            var rb = other.transform.root.GetComponent<Rigidbody>();
            if (rb)
            {
                switch (other.tag)
                {
                    case "Environment":
                        break;
                    case "Critical":
                        rb.AddForce(-transform.up * forcePushStrength);
                        break;
                    case "Weapon":
                        break;
                    case "Shield":
                        rb.AddForce(-transform.up * forcePushStrength * 0.1f);
                        break;
                    case "Body":
                        rb.AddForce(-transform.up * forcePushStrength);
                        break;
                }
            }
        }
    }
}
