using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavensEye : MonoBehaviour
{
    public Transform target;
    public float height = 10.0f;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + Vector3.up * height;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            other.transform.root.up = -transform.up;
            other.transform.root.GetComponent<Rigidbody>().velocity = 
                -transform.up * other.transform.root.GetComponent<Rigidbody>().velocity.magnitude;
        }
    }
}
