using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   bkrylov     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */
[AddComponentMenu("ProjectFaceless/Enemy/Basic")]
public class BaseCharacter : MonoBehaviour
{
    

    [Tooltip ("The position in world space where the object will appear after reset.")]
    [SerializeField]
    private Vector3 designatedPosition;

    protected Vector3 direction;

    [Tooltip("Character normal speed")]
    [SerializeField]
    protected float speed = 10.0f;

    [Tooltip("Names of animation clips that are used for attacks")]
    [SerializeField]
    private string[] strikeAnimationClipsNames;

    protected CharacterController charController;
    protected Vector3 originalPosition;
    protected Animator anim;
    
    public virtual void ResetPosition()
    {
        transform.position = originalPosition;
    }

    protected virtual void Start()
    {
        originalPosition = transform.position;
        charController = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        if (direction != Vector3.zero) Move();
    }

    public Vector3 CurrentDirection
    {
        set
        {
            direction = value;
        }
    }

    protected virtual void Move()
    {
        direction *= speed;
        if (charController != null)
        {
            direction += (!charController.isGrounded) ? Physics.gravity : Vector3.zero;
        }
        if (charController != null) charController.Move(direction * Time.deltaTime);
        CurrentDirection = Vector3.zero;
    }

    public virtual bool IsStriking()
    {
        string currentAnimation = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        for (int i = 0; i < strikeAnimationClipsNames.Length; i++)
        {
            if (strikeAnimationClipsNames[i] == currentAnimation)
            {
                return true;
            }
        }
        return false;
    }
}


