using System.Collections;
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

    public bool Freeze
    {
        get { return freeze; }
        set
        {
            freeze = value;
            if (freeze)
            {
                movementSystem.Movement = Vector2.zero;
            }
        }
    }

    // Private

    private bool freeze = false;

    private MovementSystem movementSystem;
    private new Camera camera;
    private PlayerCameraController cameraController;
    private SkillSystem skillSystem;
    private AttackSystem attackSystem;
    private SheathSystem sheathSystem;
    private BodyStateSystem bodyStateSystem;

    private bool aiming = false;
    private bool wantToAttack = false;
    private int wantToCast = -1;

    protected void Start()
    {
        movementSystem = GetComponent<MovementSystem>();
        skillSystem = GetComponent<SkillSystem>();
        attackSystem = GetComponent<AttackSystem>();
        sheathSystem = GetComponent<SheathSystem>();
        bodyStateSystem = GetComponent<BodyStateSystem>();
        cameraController = GetComponent<PlayerCameraController>();
        camera = GameObject.Find("MainCamera").GetComponent<Camera>();

        cameraController.ChangeCamera(cameraThirdPerson, preserveRotation: false, instant: true);
    }

    private void UpdateMovement()
    {
        Vector2 input = InputManager.GetMovement();
        Vector3 movement = new Vector3(
            input.x,
            0.0f,
            input.y);

        Vector3 desire = Quaternion.Euler(0.0f, camera.transform.rotation.eulerAngles.y, 0.0f)
            * movement;

        movementSystem.Movement = new Vector2(desire.x, desire.z);

        if (movementSystem.Moving)
        {
            cameraController.TriggerPlayerAutoRotation();
        }
    }

    private int GetUsedSkill()
    {
        if (InputManager.Get(InputAction.Skill_1) && skillSystem.Skills.Count >= 1)
        {
            return 0;
        }
        else if (InputManager.Get(InputAction.Skill_2) && skillSystem.Skills.Count >= 2)
        {
            return 1;
        }
        else if (InputManager.Get(InputAction.Skill_3) && skillSystem.Skills.Count >= 3)
        {
            return 2;
        }
        else if (InputManager.Get(InputAction.Skill_4) && skillSystem.Skills.Count >= 4)
        {
            return 3;
        }
        else if (InputManager.Get(InputAction.Skill_5) && skillSystem.Skills.Count >= 5)
        {
            return 4;
        }
        else if (InputManager.Get(InputAction.Skill_6) && skillSystem.Skills.Count >= 6)
        {
            return 5;
        }
        else if (InputManager.Get(InputAction.Skill_7) && skillSystem.Skills.Count >= 7)
        {
            return 6;
        }
        else if (InputManager.Get(InputAction.Skill_8) && skillSystem.Skills.Count >= 8)
        {
            return 7;
        }
        return -1;
    }

    private void UpdateSkills()
    {
        int usedSkill = GetUsedSkill();
        if (usedSkill >= 0) wantToCast = usedSkill;
        if (skillSystem.Casting || attackSystem.Attacking || movementSystem.Moving)
        {
            wantToCast = -1;
            return;
        }

        if (wantToCast >= 0)
        {
            if (sheathSystem.Sheathed)
            {
                if (!sheathSystem.Busy) sheathSystem.Unsheath();

                return;
            }

            if (wantToCast >= 0)
            {
                skillSystem.Cast(wantToCast);
            }
            wantToCast = -1;
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

    private void UpdateAttack()
    {

        if (InputManager.Get(InputAction.Attack) || wantToAttack)
        {
            if (sheathSystem.Sheathed)
            {
                if (!sheathSystem.Busy) sheathSystem.Unsheath();

                wantToAttack = true;
                return;
            }


            if (skillSystem.Casting || attackSystem.Attacking || movementSystem.Moving)
            {
                wantToAttack = false;
                return;
            }

            attackSystem.Attack();
            wantToAttack = false;
        }

    }

    private void UpdateBodyState()
    {
        if (InputManager.Get(InputAction.ChangeBodyState))
        {
            bodyStateSystem.ChangeState(
                bodyStateSystem.State == BodyStateSystem.BodyState.Magical
                ? BodyStateSystem.BodyState.Physical
                : BodyStateSystem.BodyState.Magical);
        }
    }

    protected void FixedUpdate()
    {
        if (!freeze)
        {
            UpdateMovement();
            UpdateSkills();
            UpdateAim();
            UpdateSheathe();
            UpdateBodyState();
            UpdateAttack();
        }


    }

}
