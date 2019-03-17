using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */

public class Billboard : MonoBehaviour
{
    [AddComponentMenu("ProjectFaceless/Tools")]
    Transform cameraTransform;

    [Tooltip("Flip among z axis")]
    public bool flip = false;

    void Start()
    {
        cameraTransform = GameObject.Find("MainCamera").GetComponent<Camera>().transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.forward = flip ? cameraTransform.forward : -cameraTransform.forward;
    }
}
