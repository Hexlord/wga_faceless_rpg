using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{

    public float strength = 1.0f;

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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Faceless");
        foreach(GameObject enemy in enemies)
        {
            Rigidbody body = enemy.GetComponent<Rigidbody>();

            float distance = Vector3.Distance(body.position, transform.position);
            if(distance < 10.0f)
            {
                Vector3 force = Vector3.MoveTowards(Vector3.zero, transform.position - body.position, 1.0f / (distance + 1.0f) * strength);
                body.AddForce(force);
            }
        }
        
    }
}
