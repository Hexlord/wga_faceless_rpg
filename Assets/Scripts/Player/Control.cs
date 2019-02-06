using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    //public variables
    public float speed = 10.0f;
    public Vector3 drag = new Vector3(1, 1, 1);
    public Quaternion previousRotation;
    public float mass = 10.0f;
    public float sensitivity = 60.0f;
    public float CameraFollowRadius = 5.0f;
    public float sprintModifier = 1.25f;
    public float dashModifier = 5.4f;
    public float dashTime = 0.2f;
    //public float radius = 5.0f;
    private float dashStart;

    // private variables
    Ray groundedRay;
    Vector3 Direction = Vector3.zero;
    Vector3 CamDirectionForward, CamDirectionRight;
    Vector3 targetPosition;
    CharacterController charControl;
    Character character;
    public Transform PlayerCharacter;
    Vector2 originalMousePosition;
    float horizontalAngle, verticalAngle;
    bool isDashing = false;

    // Use this for initialization
    void Start()
    {
        character = gameObject.GetComponent<Character>();
        targetPosition = PlayerCharacter.position;
        charControl = gameObject.GetComponent<CharacterController>();
        originalMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        isDashing = Time.time < dashStart + dashTime;
        CamDirectionForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        CamDirectionRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);
        if (!isDashing)
        {
            Direction = Vector3.zero;
            groundedRay = new Ray(transform.position, Vector3.down);
            Direction += CamDirectionRight * Input.GetAxis("Horizontal") * speed;
            Direction += CamDirectionForward * Input.GetAxis("Vertical") * speed * ((character.IsSprinting) ? sprintModifier : 1.0f);
            Direction += (!charControl.isGrounded) ? new Vector3(0, Physics.gravity.y * mass, 0) : Vector3.zero;

            if (Input.GetButtonDown("Attack"))
            {
                if(character.Status == Character.CharacterState.SwordStance)
                {
                    character.SwingSword();
                }
                if(character.Status == Character.CharacterState.MagicStance)
                {
                    character.ShootProjectile();
                }
            }

            if (Input.GetButtonDown("SwitchState"))
            {            
                character.SwapPlayerStatus();
            }

            //if (Input.GetButton("Block"))
            //{
            //    character.isBlocking = true;
            //}
            //else
            //{
            //    character.isBlocking = false;
            //}

            if (Input.GetButtonDown("Unsheathe"))
            {
                character.DrawSword();
            }

        }

        horizontalAngle = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        verticalAngle = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        if(Input.GetButtonDown("Sprint"))
        {
            switch (character.Status)
            {
                case Character.CharacterState.MagicStance:
                case Character.CharacterState.SwordStance:
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
                case Character.CharacterState.SwordStance:
                case Character.CharacterState.SheathedSword:
                    {
                        character.IsSprinting = false;
                        break;
                    }
            }

        }

        if(isDashing)
        {
            Direction *= dashModifier;
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
            //transform.forward = CamDirectionForward;
                GetComponent<SmartController>().TriggerPlayerAutoRotation();
                previousRotation = transform.rotation;
            }
            else
            {
                //transform.rotation = previousRotation;
                //GetComponent<SmartController>().TriggerPlayerAutoRotation();
        }
    }

    private void LateUpdate()
    {
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
    }
}
