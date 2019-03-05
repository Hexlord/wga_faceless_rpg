using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   bkrylov     Created
 * 
 */

public class Control : MonoBehaviour
{
    //Handles user input
    //public variables   

    // private variables
    private Vector3 Direction = Vector3.zero;
    private Vector3 CamDirectionForward, CamDirectionRight;
    private Camera mainCamera;

    private SmartController cameraController;
    private HealthSystemWithConcentration healthSystemWithConcentration;
    private Character character;
    private DefenseSystem defenseSystem;
    private SkillUser skillUser;
    private Transform PlayerCharacter;

    // Use this for initialization
    void Start()
    {
        PlayerCharacter = GameObject.Find("Player").transform;
        mainCamera = Camera.main;
        healthSystemWithConcentration = gameObject.GetComponent<HealthSystemWithConcentration>();
        character = gameObject.GetComponent<Character>();
        defenseSystem = gameObject.GetComponent<DefenseSystem>();
        skillUser = gameObject.GetComponent<SkillUser>();
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
            Direction += CamDirectionRight * (Input.GetAxis("Horizontal"));
            Direction += CamDirectionForward * Input.GetAxis("Vertical") * ((character.IsSprinting) ? character.SprintModifier : 1.0f);
            
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

            if (Input.GetButtonDown("Block") && (character.Status == Character.CharacterState.SwordStance) && (character.SwordStatus != Character.SwordState.SheathedSword))
            {
                defenseSystem.IsBlocking = true;
            }

            if(defenseSystem.IsBlocking)
            {
                cameraController.TriggerPlayerAutoRotation();
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

            if (Input.GetButton("Sprint") && (character.SwordStatus == Character.SwordState.SheathedSword) && (Input.GetAxis("Vertical") > 0))
            {
                character.IsSprinting = true;
            }

            if ((Input.GetButtonUp("Sprint") && (character.SwordStatus == Character.SwordState.SheathedSword)) || (Input.GetAxis("Vertical") == 0))
            {
                character.IsSprinting = false;
            }

            //Sheathing handling

            if (Input.GetButtonDown("Unsheathe"))
            {
                character.DrawSword();
            }

            // Skill handling
            if(!skillUser.Casting)
            {
                if (Input.GetButtonDown("Skill 1") && skillUser.Skills.Count >= 1)
                {
                    skillUser.Cast(0);
                }
                else if (Input.GetButtonDown("Skill 2") && skillUser.Skills.Count >= 2)
                {
                    skillUser.Cast(1);
                }
                else if (Input.GetButtonDown("Skill 3") && skillUser.Skills.Count >= 3)
                {
                    skillUser.Cast(2);
                }
                else if (Input.GetButtonDown("Skill 4") && skillUser.Skills.Count >= 4)
                {
                    skillUser.Cast(3);
                }
                else if (Input.GetButtonDown("Skill 5") && skillUser.Skills.Count >= 5)
                {
                    skillUser.Cast(4);
                }
                else if (Input.GetButtonDown("Skill 6") && skillUser.Skills.Count >= 6)
                {
                    skillUser.Cast(5);
                }
                else if (Input.GetButtonDown("Skill 7") && skillUser.Skills.Count >= 7)
                {
                    skillUser.Cast(6);
                }
                else if (Input.GetButtonDown("Skill 8") && skillUser.Skills.Count >= 8)
                {
                    skillUser.Cast(7);
                }
            }


            //Heal handling

            if (Input.GetButton("Heal"))
            {
                healthSystemWithConcentration.SpendConcentration(Time.deltaTime);
                Direction = Vector3.zero;
            }

            if (Direction.magnitude > 1) Direction.Normalize();

            character.CurrentDirection = Direction;
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
            cameraController.TriggerPlayerAutoRotation();
        }
    }
}
