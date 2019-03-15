using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 
 */

public class VisibilityLOD : MonoBehaviour
{
    Transform cameraTransform;

    [Tooltip("Maximum distance from camera to be visible")]
    [Range(0.0f, 1000.0f)]
    public float maximumDistance = 100.0f;
    
    [Tooltip("Object of visibility")]
    public GameObject visibilityObject;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(visibilityObject)
        {
            visibilityObject.SetActive(Vector3.Distance(cameraTransform.position, transform.position) <= maximumDistance);
        }
    }
}
