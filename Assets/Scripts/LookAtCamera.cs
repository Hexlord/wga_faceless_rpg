using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    //Automatically makes an object face towards main camera.
    //Used for UI elements that are rendered in world space.
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
