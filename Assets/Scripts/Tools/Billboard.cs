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
[AddComponentMenu("ProjectFaceless/Tools/Billboard")]
public class Billboard : MonoBehaviour
{
    
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
