using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    private Transform target;
    private Vector3 toTargetVector;
    private Vector3 storedVector;
    private CompassState status;
    
    private enum CompassState
    {
        Forward,
        Target
    }

    public void SetCompassStateTarget()
    {
        status = CompassState.Target;
    }

    public void SetTarget(Transform go)
    {
        target = go;
    }
    
    public void SetCompassStateForward()
    {
        status = CompassState.Forward;
    }
    
    public Vector3 LookingVector
    {
        get
        {
            switch (status)
            {
                case CompassState.Target:
                    toTargetVector = target.position - transform.position;
                    return toTargetVector;
                case CompassState.Forward:
                    toTargetVector = storedVector;
                    return toTargetVector;
                default:
                    return Vector3.forward;
            }
        }
    }

    public void UpdateStoredVector(Vector3 v)
    {
        storedVector = v.normalized;
    }


}
