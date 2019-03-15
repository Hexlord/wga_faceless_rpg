using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 14.03.2019   aknorre     Created
 * 
 */

/*
 * 
 */
public class ThirdPersonCamera : ConstrainedCamera
{
    // Public

    [Header("Player Settings")]


    [Header("Third Person Camera Settings")]
    [Tooltip("Distance from player anchor")]
    [Range(0.0f, 10.0f)]
    public float thirdPersonDistance = 5.0f;

    [Tooltip("Player anchor offset from player origin")]
    public Vector3 thirdPersonAnchorOffset = Vector3.zero;

    protected override void Start()
    {
        base.Start();

        Yaw = constraintTarget.transform.rotation.eulerAngles.y;
        Pitch = constraintTarget.transform.rotation.eulerAngles.x;
    }

    protected override void OnRotationChange()
    {
        Position = constraintTarget.transform.position + constraintTarget.transform.rotation * thirdPersonAnchorOffset +
            Quaternion.Euler(Pitch, Yaw, 0.0f) * -Vector3.forward * thirdPersonDistance;
    }

    protected override void Update()
    {
        base.Update();
    }

}
