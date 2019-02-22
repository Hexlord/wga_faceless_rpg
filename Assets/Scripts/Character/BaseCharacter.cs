using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseCharacter : MonoBehaviour
{
    [Tooltip ("The position in world space where the object will appear after reset.")]
    [SerializeField]
    private Vector3 designatedPosition;

    private Vector3 direction;

    [Tooltip("Character normal speed")]
    [SerializeField]
    private float speed = 10.0f;
    private float mass;

    private CharacterController charController;
    private Vector3 originalPosition;
    
    public virtual void ResetPosition()
    {
        transform.position = originalPosition;
    }

    void Start()
    {
        originalPosition = transform.position;
        charController = GetComponent<CharacterController>();
        mass = GetComponent<Rigidbody>().mass;
    }

    void FixedUpdate()
    {
        Move(direction);
    }

    public Vector3 CurrentDirection
    {
        set
        {
            direction = value;
            if (direction.y == 0)
            {
                direction += (charController.isGrounded) ? Physics.gravity : Vector3.zero;
            }
        }
    }

    public virtual void Move(Vector3 direction)
    {
        direction *= speed;
        
        charController.Move(direction * Time.deltaTime);
    }
}


