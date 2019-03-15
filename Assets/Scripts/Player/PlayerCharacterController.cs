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
 * 
 */

public class PlayerCharacterController : MonoBehaviour
{

    // Public

    // Private

    private MovementSystem movement;
    private new Camera camera;
    private PlayerCameraController cameraController;

    protected void Start()
    {
        movement = GetComponent<MovementSystem>();
        cameraController = GetComponent<PlayerCameraController>();
        camera = Camera.main;
    }
    protected void FixedUpdate()
    {
        Vector3 input = new Vector3(
            Input.GetAxis("Horizontal"),
            0.0f,
            Input.GetAxis("Vertical"));

        Vector3 desire = Quaternion.Euler(0.0f, camera.transform.rotation.eulerAngles.y, 0.0f)
            * input;

        movement.Movement = new Vector2(desire.x, desire.z);

        if(movement.Moving)
        {
            cameraController.TriggerPlayerAutoRotation();
        }
    }

}
