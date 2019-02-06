using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform cameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        cameraPosition = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraPosition);
    }
}
