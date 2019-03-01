using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBall : MonoBehaviour
{

    [Header("Basic Settings")]

    [Tooltip("Fly forward speed")]
    public float speed = 6.0f;

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
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }
}
