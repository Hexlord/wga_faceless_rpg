using System;
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
[AddComponentMenu("ProjectFaceless/Skills/Hook")]
public class Hook : MonoBehaviour
{

    [Tooltip("Hook speed")]
    public float hookSpeed = 15.0f;

    [Tooltip("Hook range")]
    public float hookRange = 15.0f;
    
    private GameObject hit;
    private float distanceTraveled = 0.0f;
    private Vector3 hitOffset;
    private Quaternion hitRotation;
    private Quaternion hitHookRotation;

    public enum HookState
    {
        Fly,
        Hit,
        Returning
    }

    public HookState State
    {
        get { return state; }
    }

    public GameObject Hit
    {
        get { return hit; }
    }

    private HookState state = HookState.Fly;

    public void Return()
    {
        state = HookState.Returning;
    }

    public float Distance
    {
        get { return distanceTraveled; }
    }

    protected void FixedUpdate()
    {
        var d = hookSpeed * Time.fixedDeltaTime;

        switch (state)
        {
            case HookState.Fly:
                transform.position += transform.forward * d;
                distanceTraveled += d;

                if (distanceTraveled > hookRange) state = HookState.Returning;
                break;
            case HookState.Hit:
                var rot = hit.transform.rotation * hitRotation;
                transform.position = hit.transform.position + rot * hitOffset;
                transform.rotation = hitHookRotation * rot;
                break;
            case HookState.Returning:
                transform.position += -transform.forward * d;
                distanceTraveled -= d;

                if (distanceTraveled <= 0.0f)
                {
                    Destroy(gameObject);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == HookState.Fly)
        {
            Debug.Log("Touched " + other.gameObject.name);
            if (other.tag.Contains("Faceless"))
            {
                hit = other.gameObject;
                hitHookRotation = transform.rotation;
                hitOffset = transform.position - hit.transform.position;
                hitRotation = Quaternion.Inverse(hit.transform.rotation);
                state = HookState.Hit;
            } else if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                state = HookState.Returning;
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {

        OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {

    }


}
