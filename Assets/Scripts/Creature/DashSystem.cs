using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 22.03.2019   bkrylov     Created
 * 25.03.2019   bkrylov     Made ChargesAvailable a property
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
    [Tooltip("How many times the character can dash right now. Change for testing puroses only.")]
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

    public int ChargesAvailabe
    {
        get { return chargesAvailable; }
    }

    public void RestoreCharges(int count)
    {
        chargesAvailable = Mathf.Min(chargesAvailable + count, numberOfCharges);
    }

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

    private void LateUpdate()
    {
        if ((Time.time > (timeDashInit + timeForDash)) && (isDashing))
        {
            StopDashing();
        }

        if (!isDashing && (Time.time > lastTimeDashedOrReload + reloadTime) && (chargesAvailable < numberOfCharges))
        {
            chargesAvailable++;
            lastTimeDashedOrReload = Time.time;
        }
    }

    private void Update()
    {
        if (isDashing)
        {
            movement.desiredMovement = dashVector;
        }
    }

    public void StartDashing(Vector2 desiredDashVector)
    {
        if (CanDash())
        {
            chargesAvailable--;
            isDashing = true;
            timeDashInit = Time.time;
            //Add additional conditions for setting up proper dash anim
            animator.SetBool(dashAnimationBool, true);
            dashVector = desiredDashVector;
            movement.SetSpeed(speed);
            movement.Movement = dashVector;
        }
    }

    public void StopDashing()
    {
        lastTimeDashedOrReload = Time.time;
        isDashing = false;
        animator.SetBool(dashAnimationBool, true);
        movement.ResetSpeed();
    }
}
