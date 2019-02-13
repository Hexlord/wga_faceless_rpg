using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{

    //public variables
    public float speed = 10.0f;
    public Vector3 drag = new Vector3(1, 1, 1);
    public Quaternion previousRotation;
    public float mass = 10.0f;
    public float sensitivity = 60.0f;
    public float CameraFollowRadius = 5.0f;
    public float sprintModifier = 1.25f;

    // private variables
    Vector3 Direction = Vector3.zero;
    Vector3 CamDirectionForward, CamDirectionRight;
    Vector3 targetPosition;
    CharacterController charControl;
    ConcentrationSystem concentration;
    Character character;
    DefenseSystem defenseSystem;
    public Transform PlayerCharacter;
    Vector2 originalMousePosition;
    float horizontalAngle, verticalAngle;

    // Use this for initialization
    void Start()
    {
        concentration = gameObject.GetComponent<ConcentrationSystem>();
        character = gameObject.GetComponent<Character>();
        targetPosition = PlayerCharacter.position;
        charControl = gameObject.GetComponent<CharacterController>();
        originalMousePosition = Input.mousePosition;
        defenseSystem = gameObject.GetComponent<DefenseSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        CamDirectionForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        CamDirectionRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);
        if (!defenseSystem.isDashing)
        {
            Direction = Vector3.zero;
            Direction += CamDirectionRight * Input.GetAxis("Horizontal") * speed;
            Direction += CamDirectionForward * Input.GetAxis("Vertical") * speed * ((character.IsSprinting) ? sprintModifier : 1.0f);
            Direction += (!charControl.isGrounded) ? new Vector3(0, Physics.gravity.y * mass, 0) : Vector3.zero;
            if (character.Status == Character.CharacterState.MagicStance)
            {

                if (Input.GetButtonDown("Aim"))
                {
                    if (GetComponent<SmartController>().GetState() == SmartController.CameraState.Action)
                    {
                        GetComponent<SmartController>().SwitchState(SmartController.CameraState.Shoot);
                    }
                    else
                    {
                        GetComponent<SmartController>().SwitchState(SmartController.CameraState.Action);
                    }
                }


                GetComponent<SmartController>().TriggerPlayerAutoRotation();
            }


            if (Input.GetButtonDown("Attack"))
            {
                if (character.Status == Character.CharacterState.SheathedSword)
                {
                    character.DrawSword();
                }
                if ((character.Status == Character.CharacterState.SwordStance) && !defenseSystem.IsBlocking)
                {
                    character.SwingSword();
                }
                if (character.Status == Character.CharacterState.MagicStance)
                {
                    character.ShootProjectile();
                }
            }

            if (Input.GetButtonDown("SwitchState"))
            {
                character.SwapPlayerStatus();
            }

            if (Input.GetButtonDown("Block"))
            {
                if (character.Status == Character.CharacterState.SwordStance)
                {
                    defenseSystem.IsBlocking = true;
                }
            }

            if (Input.GetButtonUp("Block"))
            {
                defenseSystem.IsBlocking = false;
            }



            if ((Input.GetButtonDown("Sprint")) && (character.Status == Character.CharacterState.MagicStance) && (Direction != Vector3.zero))
            {
                defenseSystem.InitiateDash(Direction);
            }

            if (Input.GetButtonDown("Unsheathe"))
            {
                character.DrawSword();
            }

            if (Input.GetButton("Heal"))
            {
                concentration.SpendConcentration(Time.deltaTime);
                Direction = Vector3.zero;
            }

        }

        if (defenseSystem.isDashing)
        {
            Direction = defenseSystem.dashDirection;
        }

        horizontalAngle = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        verticalAngle = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        if (Input.GetButtonDown("Sprint"))
        {
            //Branches for states MagicStance and SwordStance are empty for future changes to sprinting logic;
            switch (character.Status)
            {
                case Character.CharacterState.MagicStance:
                    {
                        break;
                    }
                case Character.CharacterState.SwordStance:
                    {
                        break;
                    }
                case Character.CharacterState.SheathedSword:
                    {
                        character.IsSprinting = true;
                        break;
                    }
            }

        }

        if (Input.GetButtonUp("Sprint"))
        {
            switch (character.Status)
            {
                case Character.CharacterState.MagicStance:
                    {
                        break;
                    }
                case Character.CharacterState.SwordStance:
                    {
                        break;
                    }
                case Character.CharacterState.SheathedSword:
                    {
                        character.IsSprinting = false;
                        break;
                    }
            }

        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), ((int)(1.0f / Time.smoothDeltaTime)).ToString());
    }

    private void FixedUpdate()
    {

        if (Direction != Vector3.zero)
        {
            charControl.Move(Direction * Time.deltaTime);
            GetComponent<SmartController>().TriggerPlayerAutoRotation();
            previousRotation = transform.rotation;
        }
    }

    private void LateUpdate()
    {
        /*
        if (!GetComponent<SmartController>().isActiveAndEnabled)
        {
            Camera.main.transform.position = Camera.main.transform.position + (PlayerCharacter.position - targetPosition);
            if (horizontalAngle != 0 || verticalAngle != 0)
            {
                Camera.main.transform.RotateAround(PlayerCharacter.position, PlayerCharacter.up, horizontalAngle);
                Camera.main.transform.RotateAround(PlayerCharacter.position, CamDirectionRight, -verticalAngle);
                Camera.main.transform.LookAt(PlayerCharacter, PlayerCharacter.up);
            }

            targetPosition = PlayerCharacter.position;
        }
        */
    }
}
