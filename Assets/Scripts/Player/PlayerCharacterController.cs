using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    // Cache

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
    private AimSystem aimSystem;

    // Private

    private bool freeze = false;
    private bool aiming = false;
    private bool wantToAttack = false;

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
        aimSystem = GetComponent<AimSystem>();
        //Main camera can be accessed via Camera.main. Why do you use such an unorthodox and heavy method?
        camera = GameObject.Find("MainCamera").GetComponent<Camera>();

        cameraController.ChangeCamera(cameraThirdPerson, preserveRotation: false, instant: true);
    }

    private void UpdateMovement()
    {
        var input = InputManager.GetMovement();
        var movement = new Vector3(
            input.x,
            0.0f,
            input.y);

        var desire = Quaternion.Euler(0.0f, camera.transform.rotation.eulerAngles.y, 0.0f)
            * movement;
        if ((bodyStateSystem.State == BodyStateSystem.BodyState.Magical) && 
            InputManager.Pressed(InputAction.Defend))
        {
            Debug.Log("Dashed");
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

    private void UpdateDefense()
    {
        if (bodyStateSystem.State == BodyStateSystem.BodyState.Physical && sheathSystem.state == SheathSystem.SheathSystemState.Unsheathed)
        {
            if (shieldSystem.CanShield)
            {
                if (InputManager.Pressed(InputAction.Defend))
                {
                    shieldSystem.RaiseShield();
                    Debug.Log("Full shield Charges: " + shieldSystem.GetFullShieldCharges());
                    Debug.Log("Current shield charge HP: " + shieldSystem.GetRemainingHPInCharge());
                }
            }
            else
            {
                if (InputManager.Released(InputAction.Defend))
                    shieldSystem.LowerShield();
                else
                {
                    cameraController.TriggerPlayerAutoRotation();
                }
            }
        }
    }

    private int GetSkillSelection()
    {
        var magicalOffset = bodyStateSystem.State == BodyStateSystem.BodyState.Magical ? 2 : 0;

        var key = -1;
        if (InputManager.Pressed(InputAction.Skill_1))
        {
            key = 0;
        }
        else if (InputManager.Pressed(InputAction.Skill_2))
        {
            key = 1;
        }

        if (key >= 0)
        {
            key += magicalOffset;
            if (skillSystem.Skills.Count <= key) key = -1;
        }
       
        return key;
    }

    private void UpdateSkills()
    {
        var selectedSkill = GetSkillSelection();

        if (selectedSkill >= 0 &&
            !skillSystem.Busy)
        {
            if (skillSystem.SelectedSkill == selectedSkill)
            {
                skillSystem.UnselectSkill();
            }
            else
            {
                skillSystem.SelectSkill(selectedSkill);
            }
        }
    }

    private void UpdateAim()
    {
        aimSystem.Aim = cameraController.Camera.transform.rotation;

        if (InputManager.Released(InputAction.Aim))
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

        if (InputManager.Released(InputAction.Sheathe))
        {
            if (sheathSystem.Sheathed) sheathSystem.Unsheathe();
            else sheathSystem.Sheathe();
        }
    }

    private void Attack()
    {
        if (skillSystem.IsSkillSelected)
        {
            skillSystem.Cast();
            return;
        }

        if (bodyStateSystem.State == BodyStateSystem.BodyState.Physical)
        {
                attackSystem.Attack(-1, 0);
        }
        else if (bodyStateSystem.State == BodyStateSystem.BodyState.Magical)
        {

            var rayFromCenterOfTheScreen = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            //Vector3 aimingPointOffset = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            //aimingPointOffset = /*(shootSystem.ShootingPointPosition - aimingPointOffset).magnitude */ camera.transform.forward;
            //Ray rayForAiming = new Ray(aimingPointOffset, camera.transform.forward);
            RaycastHit hit;
            Vector3 shootingDirection;
            var mask = LayerMask.GetMask("Enemy", "Environment");
            const float dist = 1000.0f;

            if (Physics.Raycast(rayFromCenterOfTheScreen, out hit, dist, mask, QueryTriggerInteraction.Ignore))
            {
                shootingDirection = (hit.point - (shootSystem.ShootingPoint.position));
                if (shootingDirection.sqrMagnitude > Mathf.Epsilon) shootingDirection.Normalize();
            }
            else
            {
                shootingDirection = camera.transform.forward;
            }
            cameraController.TriggerPlayerAutoRotation();
            shootSystem.Shoot(shootingDirection, 0);
        }
    }

    private void UpdateAttack()
    {

        if (InputManager.Down(InputAction.Attack) || wantToAttack)
        {
            if (sheathSystem.Sheathed)
            {
                if (!sheathSystem.Busy) sheathSystem.Unsheathe();

                wantToAttack = true;
                return;
            }


            if (skillSystem.Busy || attackSystem.Attacking || shieldSystem.IsRaised)
            {
                wantToAttack = false;
                return;
            }

            Attack();
            
            wantToAttack = false;
        }
        else
        {
            if (skillSystem.Channeling) skillSystem.Interrupt();
        }

    }

    private void UpdateBodyState()
    {
        if (InputManager.Released(InputAction.ChangeBodyState))
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
            UpdateDefense();

            if (InputManager.Down(InputAction.Menu))
            {
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
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