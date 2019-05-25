using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartAiming : MonoBehaviour
{
    public float collectingFrequency = 1.0f;
    public float storageTime = 5.0f;
    public float projectileMass;
    private float ShootForce;
    public int queueSize;
    private Queue<Vector3> targetPositionsDelta;
    private Transform target;
    private ShootSystem shootSys;
    private Vector3 storedPos;


    private void Awake()
    {
        targetPositionsDelta = new Queue<Vector3>();
        queueSize = Mathf.RoundToInt( storageTime / collectingFrequency);
        shootSys = GetComponent<ShootSystem>();
    }

    public void SetTarget(Transform t)
    {
        target = t;
        targetPositionsDelta.Clear();
        StartCoroutine(CollectObservation());
    }

    public Vector3 StraightLineAimingVector()
    {
        return (target.position + Vector3.up - shootSys.ShootingPoint.position).normalized;
    }

    public Vector3 PredictAimingVector()
    {
        Vector3[] arr = targetPositionsDelta.ToArray();
        Vector3 dir = Vector3.zero;
        foreach (var v in arr)
        {
            dir += v;
        }
        
        return dir;
    }

    private IEnumerator CollectObservation()
    {
        targetPositionsDelta.Enqueue(target.position - storedPos);
        storedPos = target.position;
        if (targetPositionsDelta.Count > queueSize)
        {
            targetPositionsDelta.Dequeue();
        }

        yield return new WaitForSeconds(collectingFrequency);
    }
}
