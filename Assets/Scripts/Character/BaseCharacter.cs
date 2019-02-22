using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseCharacter : MonoBehaviour
{
    [Tooltip ("The position in world space where the object will appear after reset.")]
    [SerializeField]
    private Vector3 designatedPosition;

    private Vector3 originalPosition;
    
    public virtual void ResetPosition()
    {
        transform.position = originalPosition;
    }

    void Start()
    {
        originalPosition = transform.position;
    }

    void FixedUpdate()
    {

    }
}


