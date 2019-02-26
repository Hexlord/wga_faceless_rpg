using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Handles projectile logic
    /// </summary>
    
    [Tooltip("Time in seconds before the projectile will be destroyed.")]
    [SerializeField]
    private float timeBeforeDestruction = 10.0f;

    [Tooltip("How many times the projectile can bounce.")]
    [SerializeField]
    private int timesToBounce = 0;
    
    //Private

    int timesBounced;

    // Rig the projectile to destroy after a certain amount of time
    void Start()
    {
        Destroy(gameObject, timeBeforeDestruction);
    }

    //Bounce and/or deal damage
    private void OnCollisionEnter(Collision collision)
    {
        Weapon weapon = GetComponent<Weapon>();

        if (timesBounced >= timesToBounce)
        {
            Destroy(gameObject);
        }
        else
        {
            timesBounced++;
        }
    }

}
