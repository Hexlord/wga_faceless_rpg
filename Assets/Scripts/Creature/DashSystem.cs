using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 22.03.2019   bkrylov     Created
 * 
 */

[AddComponentMenu("ProjectFaceless/Creature/DashSystem")]
public class DashSystem : MonoBehaviour
{
    [Tooltip("Distance of dash")]
    public float dashDistance = 10.0f;
    [Tooltip("How fast the dash is performed")]
    public float timeForDash = 0.2f;
    [Tooltip("How many charges the character can have generally")]
    public int numberOfCharges = 3;
    [Tooltip("How many times the character can dash right now. Changefor testing puroses.")]
    public int chargesAvailable;
    [Tooltip("Time to reload dash")]
    public float reloadTime = 1.0f;
    [Header ("Animation Settings")]
    [Tooltip("Boolean that controls dash animation")]
    public string dashAnimationBool;
    [Tooltip("Name of dash animation")]
    public string animDash;
    //private 
    private float speed;
    private float lastTimeDashedOrReload;
    private float timeDashInit;
    private bool isDashing;
    private Animator animator;
    private MovementSystem movement;
    private Vector2 dashVector;

    private void Start()
    {
        speed = dashDistance / timeForDash;
        chargesAvailable = numberOfCharges;

        animator = GetComponent<Animator>();
        movement = GetComponent<MovementSystem>();
    }

    public bool CanDash()
    {
        return !isDashing && (chargesAvailable > 0);
    }

    private void Update()
    {
        if (!isDashing && (chargesAvailable < numberOfCharges))
        {
            if (Time.time > lastTimeDashedOrReload + reloadTime)
            {
                chargesAvailable++;
                lastTimeDashedOrReload = Time.time;
            }
        }

        if (Time.time < (timeDashInit + timeForDash))
        {
            movement.desiredMovement = dashVector;
            isDashing = true;
        }
        else
        {
            if (isDashing)
            {
                StopDashing();
                isDashing = false;
            }
        }
    }

    public void StartDashing(Vector2 desiredDashVector)
    {
        if (CanDash())
        {
            chargesAvailable--;
            timeDashInit = Time.time;
            animator.SetBool(dashAnimationBool, true);
            dashVector = desiredDashVector;
            movement.SetSpeed(speed);
            movement.Movement = dashVector;
        }
    }

    public void StopDashing()
    {
        lastTimeDashedOrReload = Time.time;
        animator.SetBool(dashAnimationBool, true);
        movement.RevertSpeed();
    }
}
