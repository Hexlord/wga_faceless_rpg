using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    //Handles user input
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
    Camera mainCamera;
    CharacterController charControl;
    SmartController cameraController;
    HealthSystemWithConcentration concentration;
    Character character;
    DefenseSystem defenseSystem;
    public Transform PlayerCharacter;

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
        concentration = gameObject.GetComponent<HealthSystemWithConcentration>();
        character = gameObject.GetComponent<Character>();
        charControl = gameObject.GetComponent<CharacterController>();
        defenseSystem = gameObject.GetComponent<DefenseSystem>();
        cameraController = gameObject.GetComponent<SmartController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Determining camera associated vectors
        CamDirectionForward = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
        CamDirectionRight = new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z);

        if (!defenseSystem.isDashing)
        {
            Direction = Vector3.zero;
            Direction += CamDirectionRight * Input.GetAxis("Horizontal") * speed;
            Direction += CamDirectionForward * Input.GetAxis("Vertical") * speed * ((character.IsSprinting) ? sprintModifier : 1.0f);
            Direction += (!charControl.isGrounded) ? new Vector3(0, Physics.gravity.y * mass, 0) : Vector3.zero;
            
            //Aiming down sights handling

            if (Input.GetButtonDown("Aim"))
            {
                if (character.Status == Character.CharacterState.MagicStance)
                {
                    if (cameraController.GetState() == SmartController.CameraState.Action)
                    {
                        cameraController.SwitchState(SmartController.CameraState.Shoot);
                    }
                    else
                    {
                        cameraController.SwitchState(SmartController.CameraState.Action);
                    }
                    cameraController.TriggerPlayerAutoRotation();
                }   
            }

            //Attack handling

            if (Input.GetButtonDown("Attack"))
            {
                if (character.SwordStatus == Character.SwordState.SheathedSword)
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
            
            //Switching states

            if (Input.GetButtonDown("SwitchState"))
            {
                character.SwapPlayerStatus();
            }

            //Blocking handling

            if (Input.GetButtonDown("Block") && (character.Status == Character.CharacterState.SwordStance))
            {
                if (character.SwordStatus == Character.SwordState.SheathedSword)
                {
                    character.DrawSword();
                }
                defenseSystem.IsBlocking = true;
            }

            if (Input.GetButtonUp("Block") && (character.Status == Character.CharacterState.SwordStance) && defenseSystem.IsBlocking)
            {
                defenseSystem.IsBlocking = false;
            }

            //Sprint handling

            if ((Input.GetButtonDown("Sprint")) && (character.Status == Character.CharacterState.MagicStance) 
                && (Direction != Vector3.zero) && (character.SwordStatus == Character.SwordState.UnsheathedSword))
            {
                defenseSystem.InitiateDash(Direction);
            }

            if (Input.GetButtonDown("Sprint") && (character.SwordStatus == Character.SwordState.SheathedSword))
            {
                character.IsSprinting = true;
            }

            if (Input.GetButtonUp("Sprint") && character.SwordStatus == Character.SwordState.SheathedSword)
            {
                character.IsSprinting = false;
            }

            //Sheathing handling

            if (Input.GetButtonDown("Unsheathe"))
            {
                character.DrawSword();
            }

            //Heal handling

            if (Input.GetButton("Heal"))
            {
                concentration.SpendConcentration(Time.deltaTime);
                Direction = Vector3.zero;
            }
        }
        else
        {
            Direction = defenseSystem.dashDirection;
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
            cameraController.TriggerPlayerAutoRotation();
            previousRotation = transform.rotation;
        }
    }
}
