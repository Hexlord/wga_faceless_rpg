using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Observer : MonoBehaviour
{
    public Transform[] shootingPositions;
    public Dictionary<Transform, bool> ShootingPositionCanSeeTarget;
    public Vector3 targetOffset = Vector3.up;
    private Vector3 chosenObserver;

    private void Awake()
    {
        ShootingPositionCanSeeTarget = new Dictionary<Transform, bool>();
        foreach (Transform pos in shootingPositions)
        {
            ShootingPositionCanSeeTarget.Add(pos, false);
        }
    }

    private List<Vector3> ShootingPositionsThatSeeTarget(Transform target)
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
        return positions;
    }

    public Vector3 GetClosestShootingPosition(Transform target, Vector3 position)
    {
        List<Vector3> positions = ShootingPositionsThatSeeTarget(target);
        var min = 0;
        for(int i = 0; i < positions.Count; i++)
        {
            if ((position - positions[i]).magnitude < (position - positions[min]).magnitude)
            {
                min = i;
            }
        }
        chosenObserver = positions[min];
        return positions[min];
    }

    public Vector3 GetRandomShootingPosition(Transform target)
    {
        List<Vector3> positions = ShootingPositionsThatSeeTarget(target);
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
