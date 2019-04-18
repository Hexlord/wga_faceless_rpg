using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Observer : MonoBehaviour
{
    public Transform[] shootingPositions;
    public Dictionary<Transform, bool> ShootingPositionSeeTarget;
    public Vector3 targetOffset = Vector3.up;
    private Vector3 chosenObserver;

    private void Awake()
    {
        ShootingPositionSeeTarget = new Dictionary<Transform, bool>();
        foreach (Transform pos in shootingPositions)
        {
            ShootingPositionSeeTarget.Add(pos, false);
        }
    }

    public Vector3 GetRandomShootingPosition(Transform target)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Transform pos in shootingPositions)
        {
            Ray ray = new Ray(pos.position, (target.position + targetOffset) - pos.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000.0f, LayerMask.GetMask("Character", "Environment"), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == target.tag)
                {
                    positions.Add(pos.position);
                }
            }
        }
        int index = Random.Range(0, positions.Count);
        if (positions.Count == 0)
        {
            return target.position;
        }
        chosenObserver = positions[index];
        return positions[index];
    }

    public bool ChosenObserverCanStillSeeTarget(Transform target)
    {
        Ray ray = new Ray(chosenObserver, (target.position + targetOffset) - chosenObserver);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, LayerMask.GetMask("Character", "Environment"), QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.tag == target.tag)
            {
                return true;
            }
        }
        return false;
    }
}
