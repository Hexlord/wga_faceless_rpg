using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifespan : MonoBehaviour
{
    [Header("Basic Settings")]

    [Tooltip("Time of life")]
    public float lifespan = 1.0f;

    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer >= lifespan)
        {
            Destroy(gameObject);
        }
    }
}
