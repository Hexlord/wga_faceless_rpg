using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float timeBeforeDestruction = 10.0f;
    public int timesToBounce = 0;
    int timesBounced;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeBeforeDestruction);
    }

    private void OnCollisionEnter(Collision collision)
    {
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
