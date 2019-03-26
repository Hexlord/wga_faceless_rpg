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
 * 17.03.2019   bkrylov     Added shooting
 * 25.03.2019   bkrylov     Added dashing and defence. Splited UpdateX procedures for optimisation purposes
 */
[AddComponentMenu("ProjectFaceless/Player/Character Controller")]
public class PlayerCharacterController : MonoBehaviour
{
    
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
    private ShootSystem shootSystem;
    private SheathSystem sheathSystem;
    private DashSystem dashSystem;
    private ShieldSystem shieldSystem;
    private BodyStateSystem bodyStateSystem;

    private bool aiming = false;
    private bool wantToAttack = false;
    private int wantToCast = -1;

    protected void Start()
    {
        movementSystem = GetComponent<MovementSystem>();
        skillSystem = GetComponent<SkillSystem>();
        attackSystem = GetComponent<AttackSystem>();
        shootSystem = GetComponent<ShootSystem>();
        sheathSystem = GetComponent<SheathSystem>();
        dashSystem = GetComponent<DashSystem>();
        shieldSystem = GetComponent<ShieldSystem>();
        bodyStateSystem = GetComponent<BodyStateSystem>();
        cameraController = GetComponent<PlayerCameraController>();
        //Main camera can be accessed via Camera.main. Why do you use such an unorthodox and heavy method?
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
        if ((bodyStateSystem.State == BodyStateSystem.BodyState.Magical) && 
            InputManager.GetInputRelease(InputAction.Defend))
        {
            if (sheathSystem.state == SheathSystem.SheathSystemState.Unsheathed)
            {
                dashSystem.StartDashing(new Vector2(desire.x, desire.z));
            }
        }
        else
        {
            movementSystem.Movement = new Vector2(desire.x, desire.z);
        }


        if (movementSystem.Moving)
        {
            cameraController.TriggerPlayerAutoRotation();
        }
    }

    private void UpdateDefence()
    {
        if (bodyStateSystem.State == BodyStateSystem.BodyState.Physical && sheathSystem.state == SheathSystem.SheathSystemState.Unsheathed)
        {
            if (!shieldSystem.CanShield)
            {
                if (InputManager.GetInput(InputAction.Defend))
                {
                    shieldSystem.RaiseShield();
                    Debug.Log("Full shield Charges: " + shieldSystem.GetFullShieldCharges());
                    Debug.Log("Current shield charge HP: " + shieldSystem.GetRemainingHPInCharge());
                }
                else shieldSystem.RegenerateShieldHP(Time.deltaTime);
            }
            else
            {
                if (InputManager.GetInputRelease(InputAction.Defend)) shieldSystem.LowerShield();
                else
                {
                    cameraController.TriggerPlayerAutoRotation();
                }
            }
        }
    }

    private int GetUsedSkill()
    {
        if (InputManager.GetInputRelease(InputAction.Skill_1) && skillSystem.Skills.Count >= 1)
        {
            return 0;
        }
        else if (InputManager.GetInputRelease(InputAction.Skill_2) && skillSystem.Skills.Count >= 2)
        {
            return 1;
        }
        else if (InputManager.GetInputRelease(InputAction.Skill_3) && skillSystem.Skills.Count >= 3)
        {
            return 2;
        }
        else if (InputManager.GetInputRelease(InputAction.Skill_4) && skillSystem.Skills.Count >= 4)
        {
            return 3;
        }
        else if (InputManager.GetInputRelease(InputAction.Skill_5) && skillSystem.Skills.Count >= 5)
        {
            return 4;
        }
        else if (InputManager.GetInputRelease(InputAction.Skill_6) && skillSystem.Skills.Count >= 6)
        {
            return 5;
        }
        else if (InputManager.GetInputRelease(InputAction.Skill_7) && skillSystem.Skills.Count >= 7)
        {
            return 6;
        }
        else if (InputManager.GetInputRelease(InputAction.Skill_8) && skillSystem.Skills.Count >= 8)
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
        if (InputManager.GetInputRelease(InputAction.Aim))
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

        if (InputManager.GetInputRelease(InputAction.Sheathe))
        {
            if (sheathSystem.Sheathed) sheathSystem.Unsheath();
            else sheathSystem.Sheath();
        }
    }

    private void UpdateAttack()
    {

        if (InputManager.GetInputRelease(InputAction.Attack) || wantToAttack)
        {
            if (sheathSystem.Sheathed)
            {
                if (!sheathSystem.Busy) sheathSystem.Unsheath();

                wantToAttack = true;
                return;
            }


            if (skillSystem.Casting || attackSystem.Attacking || (movementSystem.Moving && bodyStateSystem.State != BodyStateSystem.BodyState.Magical) || shieldSystem.IsRaised)
            {
                wantToAttack = false;
                return;
            }

            if (bodyStateSystem.State == BodyStateSystem.BodyState.Physical)
            {
                attackSystem.Attack();
            }
            
            if (bodyStateSystem.State == BodyStateSystem.BodyState.Magical)
            {
                Ray rayFromCenterOfTheScreen = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                Vector3 shootingDirection;
                int mask = LayerMask.GetMask("Enemy", "Environment", "Character");
                float dist = 1000.0f;

                if (Physics.Raycast(rayFromCenterOfTheScreen, out hit, dist, mask, QueryTriggerInteraction.Ignore))
                {
                    shootingDirection = (hit.point - (shootSystem.ShootingPoint.position)).normalized;
                }
                else
                {
                    shootingDirection = camera.transform.forward;
                }
                cameraController.TriggerPlayerAutoRotation();
                shootSystem.Shoot(shootingDirection);
            }

            wantToAttack = false;
        }

    }

    private void UpdateBodyState()
    {
        if (InputManager.GetInputRelease(InputAction.ChangeBodyState))
        {
            bodyStateSystem.ChangeState(
                bodyStateSystem.State == BodyStateSystem.BodyState.Magical
                ? BodyStateSystem.BodyState.Physical
                : BodyStateSystem.BodyState.Magical);
        }
    }

    protected void Update()
    {
        if (!freeze)
        {
            UpdateSkills();
            UpdateAim();
            UpdateSheathe();
            UpdateBodyState();
            UpdateAttack();
            UpdateDefence();
        }


    }

    protected void FixedUpdate()
    {
        if (!freeze)
        {
            UpdateMovement();
        }


    }

}
