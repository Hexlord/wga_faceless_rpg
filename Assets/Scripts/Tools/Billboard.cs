using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

public class Billboard : MonoBehaviour
{
    Transform cameraTransform;

    [Tooltip("Flip among z axis")]
    public bool flip = false;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = flip ? cameraTransform.forward : -cameraTransform.forward;
    }
}
