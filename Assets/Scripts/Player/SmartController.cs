using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Basic character rotation algorithm is:
 *  
 *  1. After timeout of not moving mouse rotate character to match aim
 *  2. After X angle becoming 90+ degree rotate character to match aim continuously (aim can still move during character rotation) until reaching the aim
 *  3. After any movement rotate character to match aim
 *  
 *  Camera rotation is respecting configurable speed limit (protection from dizziness)
 *  Camera is ray casting from character to determine safe distance (protection from clipping)
 * 
 */
public class SmartController : MonoBehaviour
{
    [Header("Basic Settings")]

    [Tooltip("Player movement speed in meters per second")]
    public float moveSpeed = 7.0f;

    [Tooltip("Mouse sensitivity")]
    public Vector2 mouseSensitivity = new Vector2(5.0f, 5.0f);

    [Header("Advanced Settings")]

    [Tooltip("Limit character rotation speed in degree per second")]
    public Vector2 rotationSpeedLimit = new Vector2(
        180.0f,
        180.0f);
    
    [Tooltip("Limit camera rotation speed in degree per second")]
    public Vector2 lookSpeedLimit = new Vector2(
        260.0f,
        260.0f);

    [Tooltip("Limit aim movement speed in degree per second")]
    public Vector2 lookAimSpeedLimit = new Vector2(
        390.0f,
       390.0f);

    [Tooltip("Limit aim vertical in degree")]
    public float LookAimLimit = 60.0f;

    [Tooltip("Time in seconds of inactive mouse to trigger character rotation")]
    public float lookAFKTimeout = 2.0f;

    [Tooltip("Minimum angle difference in degree to allow character rotation due to timeout")]
    public float lookAFKThreshold = 0.0f;

    [Tooltip("Minimum angle difference in degree to trigger force character rotation")]
    public float lookAwayThreshold = 90.0f;

    [Tooltip("Distance between camera and player")]
    public float cameraDistance = 4.8f;

    [Tooltip("Vertical offset for camera")]
    public float cameraHeight = 2.2f;

    [Tooltip("Affects how safe distance for camera is calculated")]
    public float cameraAvoidanceFactor = 1.5f;

    [Tooltip("Affects how fast camera safe distance grows in meter per second")]
    public float cameraAvoidanceSpeed = 4.0f;



    // Private

    public float safeCameraDistance;
    private float lookAFKTimer;
    public bool isPlayerRotating = false;

    // Cache

    private GameObject player;
    private Transform playerTransform;
    private CharacterController playerController;

    private new GameObject camera;
    private Transform cameraTransform;

    // Single alloc

    public Vector2 lookAim = new Vector2();

    private Vector2 cameraLookCurrent = new Vector2();
    private Vector2 cameraLookDelta = new Vector2();
    private Vector2 cameraLookNext = new Vector2();

    private Vector2 playerCurrent = new Vector2();
    private Vector2 playerDelta = new Vector2();
    private Vector2 playerNext = new Vector2();

    private Vector2 inputDelta = new Vector2();

    private Vector3 playerMovement = new Vector3();
    private Vector3 cameraPosition = new Vector3();
    private Vector3 cameraTarget = new Vector3();

    public float cameraDistanceDebug = 0.0f;
    public float cameraRayDistanceDebug = 0.0f;

    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();
        playerController = GetComponent<CharacterController>();

        camera = GameObject.Find("Main Camera");
        cameraTransform = camera.GetComponent<Transform>();

        // Private

        safeCameraDistance = cameraDistance;
    }

    void ProcessInput()
    {
        inputDelta.Set(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"));
    }

    void MoveAim()
    {
        // Move aim with speed limited by parameter

        float aimLimitX = lookAimSpeedLimit.x * Time.deltaTime;
        float aimLimitY = lookAimSpeedLimit.y * Time.deltaTime;

        lookAim.Set(
            lookAim.x + Mathf.Clamp(inputDelta.x * mouseSensitivity.x, -aimLimitX, aimLimitX),
            lookAim.y + Mathf.Clamp(-inputDelta.y * mouseSensitivity.y, -aimLimitY, aimLimitY));

        while (lookAim.x < 0) lookAim.x += 360.0f;
        while (lookAim.x > 360) lookAim.x -= 360.0f;

        while (lookAim.y < -LookAimLimit + Mathf.Epsilon) lookAim.y = -LookAimLimit + Mathf.Epsilon;
        while (lookAim.y > LookAimLimit - Mathf.Epsilon) lookAim.y = LookAimLimit - Mathf.Epsilon;
    }

    float FilterDistance(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, cameraDistance * (1.0f + cameraAvoidanceFactor), ~LayerMask.NameToLayer("Environment")))
        {
            float angle = Vector3.Angle(hit.normal, camera.transform.forward);
            if (angle > 90.0f) angle = 90.0f;
            float factor = 1.0f - angle / 90.0f;
            float factorSqrt = Mathf.Pow(factor, 1.0f / (1.0f + cameraAvoidanceFactor));

            float d = hit.distance * factorSqrt;
            cameraRayDistanceDebug = Mathf.Min(cameraRayDistanceDebug, hit.distance);

            return d;
        }

        return float.MaxValue;
    }

    void MoveCamera()
    {
        // Move camera towards aim with speed limited by parameter

        float lookLimitX = lookSpeedLimit.x * Time.deltaTime;
        float lookLimitY = lookSpeedLimit.y * Time.deltaTime;
        
        cameraLookDelta.Set(
            Mathf.DeltaAngle(cameraLookCurrent.x, lookAim.x),
            Mathf.DeltaAngle(cameraLookCurrent.y, lookAim.y));

        cameraLookNext.Set(
            cameraLookCurrent.x + Mathf.Clamp(cameraLookDelta.x, -lookLimitX, lookLimitX),
            cameraLookCurrent.y + Mathf.Clamp(cameraLookDelta.y, -lookLimitY, lookLimitY));

        cameraLookCurrent.Set(cameraLookNext.x, cameraLookNext.y);
        
        while (cameraLookCurrent.x < 0) cameraLookCurrent.x += 360.0f;
        while (cameraLookCurrent.y < 0) cameraLookCurrent.y += 360.0f;
        while (cameraLookCurrent.x > 360) cameraLookCurrent.x -= 360.0f;
        while (cameraLookCurrent.y > 360) cameraLookCurrent.y -= 360.0f;

        // TODO: make camera_distance movement due to clipping protection smoother
        float distance = cameraDistance;
        cameraRayDistanceDebug = float.MaxValue;

        // character-widening
        // works bad due to points going inside geometry
        // leave here as comment to consider alternatives
        /*
        {
            Vector3 origin1 = player_transform.position + camera_transform.right + camera_transform.up;
            Vector3 origin2 = player_transform.position - camera_transform.right + camera_transform.up;
            Vector3 origin3 = player_transform.position - camera_transform.right - camera_transform.up;
            Vector3 origin4 = player_transform.position + camera_transform.right - camera_transform.up;

            distance = Mathf.Min(distance, filter_distance(new Ray(origin1, -camera_transform.forward)));
            distance = Mathf.Min(distance, filter_distance(new Ray(origin2, -camera_transform.forward)));
            distance = Mathf.Min(distance, filter_distance(new Ray(origin3, -camera_transform.forward)));
            distance = Mathf.Min(distance, filter_distance(new Ray(origin4, -camera_transform.forward)));
        }
        */

        // direction-based
        {
            Vector3 dir1 = Quaternion.Euler(5.0f, 0.0f, 0.0f) * -cameraTransform.forward;
            Vector3 dir2 = Quaternion.Euler(-5.0f, 0.0f, 0.0f) * -cameraTransform.forward;
            Vector3 dir3 = Quaternion.Euler(0.0f, 5.0f, 0.0f) * -cameraTransform.forward;
            Vector3 dir4 = Quaternion.Euler(0.0f, -5.0f, 0.0f) * -cameraTransform.forward;

            distance = Mathf.Min(distance, FilterDistance(new Ray(playerTransform.position, dir1)));
            distance = Mathf.Min(distance, FilterDistance(new Ray(playerTransform.position, dir2)));
            distance = Mathf.Min(distance, FilterDistance(new Ray(playerTransform.position, dir3)));
            distance = Mathf.Min(distance, FilterDistance(new Ray(playerTransform.position, dir4)));
        }

        cameraDistanceDebug = distance;

        if(safeCameraDistance > distance)
        {
            safeCameraDistance = distance;
        }
        else
        {
            float growthSpeed = cameraAvoidanceSpeed * Time.deltaTime;

            safeCameraDistance = safeCameraDistance + Mathf.Clamp(distance - safeCameraDistance, -growthSpeed, growthSpeed);
        }

        Quaternion rotation = Quaternion.Euler(cameraLookCurrent.y, cameraLookCurrent.x, 0);

        cameraTarget.Set(playerTransform.position.x, playerTransform.position.y + cameraHeight, playerTransform.position.z);
        cameraPosition.Set(0, 0, safeCameraDistance);
        cameraPosition = cameraTarget - rotation * cameraPosition;

        cameraTransform.position = cameraPosition;
        cameraTransform.LookAt(cameraTarget);
        
    }

    void MoveСharacter()
    {
        // Move character towards aim

        float playerRotationLimitX = rotationSpeedLimit.x * Time.deltaTime;
        float playerRotationLimitY = rotationSpeedLimit.y * Time.deltaTime;

        // TODO: consider using Y component to affect character' head vertical rotation

        playerCurrent.Set(
            playerTransform.rotation.eulerAngles.y,
            playerTransform.rotation.eulerAngles.x);

        playerDelta.Set(
            Mathf.DeltaAngle(playerCurrent.x, cameraLookNext.x),
            Mathf.DeltaAngle(playerCurrent.y, cameraLookNext.y) * 0.0f);

        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon ||
            Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Epsilon;

        if (!isPlayerRotating)
        {
            if (isMoving || Mathf.Abs(playerDelta.x) > lookAwayThreshold)
            {
                isPlayerRotating = true;
            }
            else
            {
                if (Mathf.Abs(inputDelta.x) > Mathf.Epsilon ||
                    Mathf.Abs(inputDelta.y) > Mathf.Epsilon)
                {
                    lookAFKTimer = lookAFKTimeout;
                }
                else
                {
                    lookAFKTimer -= Time.deltaTime;
                    if (lookAFKTimer <= 0.0f &&
                        Mathf.Abs(Mathf.DeltaAngle(playerCurrent.x, cameraLookNext.x)) > lookAFKThreshold)
                    {
                        isPlayerRotating = true;
                    }
                }
            }
        }

        if (isPlayerRotating)
        {
            if (Mathf.Abs(playerDelta.x) > Mathf.Epsilon ||
                Mathf.Abs(playerDelta.y) > Mathf.Epsilon)
            {
                playerNext.Set(
                    playerCurrent.x + Mathf.Clamp(playerDelta.x, -playerRotationLimitX, playerRotationLimitX),
                    playerCurrent.y + Mathf.Clamp(playerDelta.y, -playerRotationLimitY, playerRotationLimitY));

                Quaternion q = Quaternion.Euler(0.0f, playerNext.x, 0.0f);

                playerTransform.rotation = q;

                if (Mathf.Abs(playerDelta.x) <= playerRotationLimitX &&
                    Mathf.Abs(playerDelta.y) <= playerRotationLimitY)
                {
                    // Done in this step
                    isPlayerRotating = false;
                }
            }
            else
            {
                isPlayerRotating = false;
            }
        }

    }

    void Movement()
    {
        // Renew after rotation, not a scope of rotation itself
        playerCurrent.Set(
            playerTransform.rotation.eulerAngles.y,
            playerTransform.rotation.eulerAngles.x);

        float angle = (playerCurrent.x / 180.0f) * Mathf.PI;

        float cosf = Mathf.Cos(angle);
        float sinf = Mathf.Sin(angle);

        playerMovement.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float x = playerMovement.x * cosf + playerMovement.z * sinf;
        float z = playerMovement.z * cosf + -playerMovement.x * sinf;
        playerMovement.Set(x, 0.0f, z);
        if (x != 0.0f || z != 0.0f) playerMovement.Normalize();
        playerMovement.y = -1.0f;

        playerController.Move(playerMovement * Time.fixedDeltaTime * moveSpeed);
    }

    void Update()
    {
        ProcessInput();

        MoveAim();

        MoveCamera();

        MoveСharacter();
    }

    void FixedUpdate()
    {
        Movement();
    }

}
