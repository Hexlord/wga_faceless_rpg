﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */

public class PlayerCharacterController : MonoBehaviour
{
    [AddComponentMenu("ProjectFaceless/Player")]
    // Public
    [Header("Basic Settings")]

    [Tooltip("Camera for basic look around")]
    public ConstrainedCamera cameraThirdPerson;

    [Tooltip("Camera for aiming")]
    public ConstrainedCamera cameraThirdPersonAim;


    // Private

    private MovementSystem movementSystem;
    private new Camera camera;
    private PlayerCameraController cameraController;
    private SkillSystem skillSystem;
    private SheathSystem sheathSystem;

    private bool aiming = false;

    protected void Start()
    {
        movementSystem = GetComponent<MovementSystem>();
        skillSystem = GetComponent<SkillSystem>();
        sheathSystem = GetComponent<SheathSystem>();
        cameraController = GetComponent<PlayerCameraController>();
        camera = Camera.main;

        cameraController.ChangeCamera(cameraThirdPerson, preserveRotation: false, instant: true);
    }

    private void UpdateMovement()
    {
        Vector3 input = new Vector3(
            Input.GetAxis("Horizontal"),
            0.0f,
            Input.GetAxis("Vertical"));

        Vector3 desire = Quaternion.Euler(0.0f, camera.transform.rotation.eulerAngles.y, 0.0f)
            * input;

        movementSystem.Movement = new Vector2(desire.x, desire.z);

        if (movementSystem.Moving)
        {
            cameraController.TriggerPlayerAutoRotation();
        }
    }

    private void UpdateSkills()
    {
        if (!skillSystem.Casting)
        {
            if (InputManager.Get(InputAction.Skill_1) && skillSystem.Skills.Count >= 1)
            {
                skillSystem.Cast(0);
            }
            else if (InputManager.Get(InputAction.Skill_2) && skillSystem.Skills.Count >= 2)
            {
                skillSystem.Cast(1);
            }
            else if (InputManager.Get(InputAction.Skill_3) && skillSystem.Skills.Count >= 3)
            {
                skillSystem.Cast(2);
            }
            else if (InputManager.Get(InputAction.Skill_4) && skillSystem.Skills.Count >= 4)
            {
                skillSystem.Cast(3);
            }
            else if (InputManager.Get(InputAction.Skill_5) && skillSystem.Skills.Count >= 5)
            {
                skillSystem.Cast(4);
            }
            else if (InputManager.Get(InputAction.Skill_6) && skillSystem.Skills.Count >= 6)
            {
                skillSystem.Cast(5);
            }
            else if (InputManager.Get(InputAction.Skill_7) && skillSystem.Skills.Count >= 7)
            {
                skillSystem.Cast(6);
            }
            else if (InputManager.Get(InputAction.Skill_8) && skillSystem.Skills.Count >= 8)
            {
                skillSystem.Cast(7);
            }
        }
    }

    private void UpdateAim()
    {
        if (InputManager.Get(InputAction.Aim))
        {
            aiming = !aiming;
        }

        cameraController.ChangeCamera(aiming
            ? cameraThirdPersonAim
            : cameraThirdPerson,
            preserveRotation: true);
    }

    private void UpdateSheathe()
    {
        if (sheathSystem.Busy) return;

        if (InputManager.Get(InputAction.Sheathe))
        {
            if (sheathSystem.Sheathed) sheathSystem.Unsheath();
            else sheathSystem.Sheath();
        }
    }

    protected void FixedUpdate()
    {
        UpdateMovement();
        UpdateSkills();
        UpdateAim();
        UpdateSheathe();


    }

}
