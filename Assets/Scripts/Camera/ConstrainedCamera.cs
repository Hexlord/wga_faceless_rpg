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
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */

/*
 * 
 */
[AddComponentMenu("ProjectFaceless/Camera/Constrained")]
public class ConstrainedCamera : MonoBehaviour
{

    // Public

    [Header("Input Settings")]

    [Tooltip("Toggles whether player can control camera")]
    public bool playerControlled = false;

    [Header("Camera Constraint Settings")]

    [Tooltip("Camera target (if any)")]
    public GameObject constraintTarget;

    [Tooltip("Always look at camera target (if any)")]
    public bool constraintLookAtTarget = false;

    [Tooltip("Minimum distance to target (if any) for camera")]
    [Range(0.0f, 10.0f)]
    public float constraintMinimumDistanceToTarget = 1.0f;

    [Tooltip("Camera pitch baseline (center for limited range)")]
    [Range(0.0f, 90.0f)]
    public float constraintPitchBaseline = 0.0f;

    [Tooltip("Camera pitch limit")]
    [Range(0.0f, 90.0f)]
    public float constraintPitchLimit = 60.0f;

    [Tooltip("Camera yaw baseline (center for limited range)")]
    [Range(-180.0f, 180.0f)]
    public float constraintYawBaseline = 0.0f;

    [Tooltip("Camera yaw limit (180 = unlimited)")]
    [Range(0.0f, 180.0f)]
    public float constraintYawLimit = 180.0f;

    [Tooltip("Field of view (80 = default)")]
    [Range(1.0f, 115.0f)]
    public float constraintFieldOfView = 80.0f;

    

    public Vector3 Position
    {
        get { return transform.position; }
        set
        {
            if (!constraintTarget)
            {
                transform.position = value;
                return;
            }

            Vector3 fromTargetToCamera = value - constraintTarget.transform.position;
            float length = fromTargetToCamera.magnitude;
            if (length >= constraintMinimumDistanceToTarget)
            {
                transform.position = value;
                return;
            }

            transform.position = constraintTarget.transform.position +
                fromTargetToCamera * (constraintMinimumDistanceToTarget / length);
        }
    }

    public float Yaw
    {
        get { return transform.rotation.eulerAngles.y; }
        set
        {
            if (constraintYawLimit < 180.0f - Mathf.Epsilon)
            {
                float deltaAngle = Mathf.DeltaAngle(constraintYawBaseline, value);
                if (Mathf.Abs(deltaAngle) > constraintYawLimit)
                {
                    value = deltaAngle > 0
                        ? constraintYawBaseline + constraintYawLimit - Mathf.Epsilon
                        : constraintYawBaseline - constraintYawLimit + Mathf.Epsilon;
                }
            }

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                value,
                transform.rotation.eulerAngles.z);

            OnRotationChange();
        }
    }
    public float Pitch
    {
        get { return transform.rotation.eulerAngles.x; }
        set
        {
            if (constraintPitchLimit < 90.0f - Mathf.Epsilon)
            {
                float deltaAngle = Mathf.DeltaAngle(constraintPitchBaseline, value);
                if (Mathf.Abs(deltaAngle) > constraintPitchLimit)
                {
                    value = deltaAngle > 0
                        ? constraintPitchBaseline + constraintPitchLimit - Mathf.Epsilon
                        : constraintPitchBaseline - constraintPitchLimit + Mathf.Epsilon;
                }
            }

            transform.rotation = Quaternion.Euler(
                value,
                transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            OnRotationChange();
        }
    }

    // Cache

    protected new Camera camera;
    public Vector3 Target
    {
        get { return target; }
    }

    protected Vector3 target;

    protected virtual void Start()
    {
        // Cache

        camera = GetComponent<Camera>();

        // Constraints
        Position = Position;
        Yaw = Yaw;
        Pitch = Pitch;

        camera.fieldOfView = constraintFieldOfView;

        camera.enabled = false;
    }

    protected virtual void OnRotationChange()
    {
        target = constraintTarget.transform.position;
    }

    protected virtual void Update()
    {
        if (constraintLookAtTarget && constraintTarget)
        {
            camera.transform.LookAt(constraintTarget.transform);

            // Constraints
            Yaw = Yaw;
            Pitch = Pitch;
        }
    }

}
