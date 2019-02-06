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
 * !NB!
 * Please use SwitchState(newState) to switch state!!!
 * 
 */
public class SmartControllerOld : MonoBehaviour
{
    [Header("Player Settings")]

    [Tooltip("Player movement speed in meters per second")]
    [Range(0.0f, 10.0f)]
    public float moveSpeed = 7.0f;

    [Header("Input Settings")]

    [Tooltip("Mouse horizontal sensitivity")]
    [Range(1.0f, 10.0f)]
    public float mouseHorizontalSensitivity = 5.0f;

    [Tooltip("Mouse vertical sensitivity")]
    [Range(1.0f, 10.0f)]
    public float mouseVerticalSensitivity = 5.0f;

    [Header("Limits")]

    [Tooltip("Limit mouse movement speed in degree per second")]
    [Range(1.0f, 3600.0f, order = 2)]
    public float mouseSpeedLimit = 390.0f;

    [Tooltip("Limit character rotation speed in degree per second")]
    [Range(1.0f, 3600.0f, order = 2)]
    public float rotationSpeedLimit = 180.0f;

    [Tooltip("Limit camera rotation speed in degree per second")]
    [Range(1.0f, 3600.0f, order = 2)]
    public float cameraRotationSpeedLimit = 260.0f;

    [Tooltip("Limit aim vertical in degree")]
    [Range(0.0f, 90.0f)]
    public float LookAimLimit = 60.0f;

    [Tooltip("Affects how fast camera distance changes in meter per second")]
    [Range(1.0f, 20.0f)]
    public float cameraDistanceSpeed = 10.0f;

    [Header("Trigger Settings")]

    [Tooltip("Time in seconds of inactive mouse to trigger character rotation")]
    [Range(0.0f, 3600.0f)]
    public float lookAFKTimeout = 2.0f;

    [Tooltip("Minimum angle difference in degree to allow character rotation due to timeout")]
    [Range(0.0f, 180.0f)]
    public float lookAFKThreshold = 0.0f;

    [Tooltip("Minimum angle difference in degree to trigger force character rotation")]
    [Range(0.0f, 180.0f)]
    public float lookAwayThreshold = 90.0f;

    [Header("Camera State Settings")]

    [Tooltip("State transition speed for angles coefficient")]
    [Range(0.0f, 1.0f)]
    public float stateAngularTransitionSpeed = 0.5f;
    
    [Tooltip("State transition speed for distance coefficient")]
    [Range(0.0f, 1.0f)]
    public float stateDistanceTransitionSpeed = 0.5f;

    [Tooltip("State transition speed for distance coefficient during lerp interpolation")]
    [Range(0.0f, 10.0f)]
    public float stateDistanceTransitionSpeedLerp = 3.5f;

    [Tooltip("State transition speed for angles coefficient during lerp interpolation")]
    [Range(0.0f, 10.0f)]
    public float stateAngularTransitionSpeedLerp = 3.5f;

    [Tooltip("State transition interpolation: toggles whether to use lerp interpolation")]
    public bool stateTransitionLerp = true;

    [Tooltip("Always use lerp interpolation")]
    public bool transitionLerp = false;

    [Tooltip("State transition speed for distance multiplier during switch to/from shooting")]
    [Range(0.0f, 100.0f)]
    public float stateDistanceTransitionSpeedShooting = 10.0f;

    [Tooltip("State transition speed for angles multiplier during switch to/from shooting")]
    [Range(0.0f, 10.0f)]
    public float stateAngularTransitionSpeedShooting = 3.0f;

    [Tooltip("State transition speed for distance multiplier during switch to/from shooting during lerp interpolation")]
    [Range(0.0f, 10.0f)]
    public float stateDistanceTransitionSpeedShootingLerp = 1.5f;

    [Tooltip("State transition speed for angles multiplier during switch to/from shooting during lerp interpolation")]
    [Range(0.0f, 10.0f)]
    public float stateAngularTransitionSpeedShootingLerp = 1.5f;

    [Tooltip("Angle difference to match new state")]
    [Range(0.0f, 5.0f)]
    public float stateAngularEpsilon = 0.25f;

    [Tooltip("Distance difference to match new state")]
    [Range(0.0f, 1.0f)]
    public float stateDistanceEpsilon = 0.015f;

    [Tooltip("Lerp interpolation raw (non-lerp) extra component to raise base level")]
    [Range(0.0f, 1.0f)]
    public float stateTransitionRaw = 0.0067f;

    /*
     * Action camera is a camera, located backwards of the player, 
     * looking at player' center with vertical offset and constrained spherical rotation
     */
    [Header("Action Camera Settings")]

    [Tooltip("Distance between camera and player")]
    [Range(0.0f, 20.0f)]
    public float actionCameraDistance = 7.0f;

    [Tooltip("Vertical offset for camera target")]
    [Range(0.0f, 5.0f)]
    public float actionCameraTargetHeight = 3.0f;

    /*
     * To the right of the head
     */
    [Header("Shooting Camera Settings")]

    [Tooltip("Distance between camera and player")]
    [Range(0.0f, 1.0f)]
    public float shootingCameraDistance = 1.0f;

    [Tooltip("Vertical offset for camera")]
    [Range(0.0f, 5.0f)]
    public float shootingCameraHeight = 2.4f;

    [Tooltip("Target distance for camera")]
    [Range(0.0f, 100.0f)]
    public float shootingCameraTargetDistance = 25.0f;

    [Tooltip("Camera backwards offset")]
    [Range(0.0f, 1.0f)]
    public float shootingCameraBackwardsDistance = 1.0f;

    /*
     * Tactics camera is a camera, located backwards and to the top of the player, 
     * looking at player' center with vertical offset and locked rotation
     * 
     * Iso-like, allows precise skill aiming and tactical analyzes of current position
     */
    [Header("Tactics Camera Settings")]

    [Tooltip("Distance between camera and player")]
    [Range(0.0f, 10.0f)]
    public float tacticsCameraDistance = 11.0f;

    [Tooltip("Vertical offset for camera target")]
    [Range(0.0f, 5.0f)]
    public float tacticsCameraTargetHeight = 3.0f;

    [Tooltip("Vertical angle in degree")]
    [Range(0.0f, 90.0f)]
    public float tacticsCameraPitch = 65.0f;




    [Header("Clipping Prevention Settings")]

    [Tooltip("Affects how safe distance for camera is calculated. Decreasing prevents clipping, but makes camera closer to player")]
    [Range(0.0f, 5.0f)]
    public float cameraAvoidancePrecision = 2.0f;

    [Tooltip("Affects how safe distance for camera is calculated. Increasing prevents clipping, but makes camera closer to player")]
    [Range(1.0f, 8.0f)]
    public float cameraAvoidanceSmoothAngle = 2.0f;

    public enum CameraState
    {
        Action,
        Shoot,
        Tactics
    }
    public CameraState cameraState = CameraState.Action;
    public CameraState cameraStateNext = CameraState.Action;



    // Private

    public Vector2 actionAim = new Vector2();
    public Vector2 tacticsAim = new Vector2();
    public Vector2 shootAim = new Vector2();

    public float cameraYaw = 0.0f;
    public float cameraPitch = 0.0f;

    public float actionCameraYaw = 0.0f;
    public float actionCameraPitch = 0.0f;

    public float shootingCameraYaw = 0.0f;

    public float cameraDistance = 0.0f;
    public float lookAFKTimer = 0.0f;
    public bool isPlayerRotating = false;

    // Cache

    private GameObject player;
    private Transform playerTransform;
    private CharacterController playerController;

    private new GameObject camera;
    private Transform cameraTransform;

    // Single alloc

    public Vector2 deltaAim = new Vector2();

    private Vector2 inputDelta = new Vector2();

    private Vector3 playerMovement = new Vector3();
    private Vector3 cameraPosition = new Vector3();
    private Vector3 cameraTarget = new Vector3();

    public float cameraRayDistanceDebug = 0.0f;

    public float cameraRemainingTargetDistance;
    public float cameraRemainingDistance;
    public float cameraRemainingYaw;
    public float cameraRemainingPitch;

    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();
        playerController = GetComponent<CharacterController>();

        camera = GameObject.Find("Main Camera");
        cameraTransform = camera.GetComponent<Transform>();

        // Private

        cameraDistance = actionCameraDistance;
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

        float speed;

        // Aim, camera and player are constrained together during shooting
        if (cameraStateNext == CameraState.Shoot)
        {
            speed = Mathf.Min(mouseSpeedLimit, rotationSpeedLimit, cameraRotationSpeedLimit) * Time.deltaTime;
        }
        else
        {
            speed = mouseSpeedLimit * Time.deltaTime;
        }

        deltaAim.Set(
            Mathf.Clamp(inputDelta.x * mouseHorizontalSensitivity, -speed, speed),
            Mathf.Clamp(inputDelta.y * mouseVerticalSensitivity, -speed, speed));

        switch (cameraStateNext)
        {
            case CameraState.Action:
                actionAim.Set(actionAim.x + deltaAim.x, actionAim.y + deltaAim.y);
                actionAim.x = Mathf.DeltaAngle(0.0f, actionAim.x);
                while (actionAim.y < -LookAimLimit + Mathf.Epsilon) actionAim.y = -LookAimLimit + Mathf.Epsilon;
                while (actionAim.y > LookAimLimit - Mathf.Epsilon) actionAim.y = LookAimLimit - Mathf.Epsilon;
                break;
            case CameraState.Shoot:
                shootAim.Set(shootAim.x + deltaAim.x, shootAim.y + deltaAim.y);
                break;
            case CameraState.Tactics:
                tacticsAim.Set(tacticsAim.x + deltaAim.x, tacticsAim.y + deltaAim.y);
                break;
        }
    }

    float FilterDistance(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, actionCameraDistance * (1.0f + cameraAvoidancePrecision), ~LayerMask.NameToLayer("Environment")))
        {
            float angle = Vector3.Angle(hit.normal, camera.transform.forward);
            if (angle > 90.0f) angle = 90.0f;
            float factor = 1.0f - angle / 90.0f;
            float factorSqrt = Mathf.Pow(factor, 1.0f / (1.0f + cameraAvoidancePrecision));

            float d = hit.distance * factorSqrt;
            cameraRayDistanceDebug = Mathf.Min(cameraRayDistanceDebug, hit.distance);

            return d;
        }

        return float.MaxValue;
    }

    void UpdateCamera()
    {
        // Move camera towards aim with speed limited by parameter


        float desiredYaw = 0.0f;
        float desiredPitch = 0.0f;

        float distanceFactor = 1.0f;
        float angularFactor = 1.0f;
        float distanceFactorLerp = 1.0f;
        float angularFactorLerp = 1.0f;
        if (cameraState != cameraStateNext)
        {
            distanceFactor *= stateDistanceTransitionSpeed;
            angularFactor *= stateAngularTransitionSpeed;
            distanceFactorLerp *= stateDistanceTransitionSpeedLerp;
            angularFactorLerp *= stateAngularTransitionSpeedLerp;
        }

        float speedStateDistance = cameraDistanceSpeed * distanceFactor * Time.deltaTime;
        float speedStateAngular = cameraRotationSpeedLimit * angularFactor * Time.deltaTime;
        float speedStateDistanceLerp = distanceFactorLerp * Time.deltaTime;
        float speedStateAngularLerp = angularFactorLerp * Time.deltaTime;

        // shooting aim must be instant
        if (cameraState == CameraState.Shoot &&
                cameraStateNext == CameraState.Shoot)
        {
            speedStateDistance *= 1000.0f;
            speedStateAngular *= 1000.0f;
            speedStateDistanceLerp *= 1000.0f;
            speedStateAngularLerp *= 1000.0f;
        }

        if (cameraState != cameraStateNext &&
            cameraState == CameraState.Shoot ||
                cameraStateNext == CameraState.Shoot)
        {
            speedStateDistance *= stateDistanceTransitionSpeedShooting;
            speedStateAngular *= stateAngularTransitionSpeedShooting;
            speedStateDistanceLerp *= stateDistanceTransitionSpeedShootingLerp;
            speedStateAngularLerp *= stateAngularTransitionSpeedShootingLerp;
        }

        float speedAngular = cameraRotationSpeedLimit * Time.deltaTime;

        // Action

        actionCameraYaw = Mathf.MoveTowardsAngle(actionCameraYaw, actionAim.x, speedAngular);
        actionCameraPitch = Mathf.MoveTowardsAngle(actionCameraPitch, actionAim.y, speedAngular);

        // Tactics



        // Shooting

        shootingCameraYaw = Mathf.MoveTowardsAngle(shootingCameraYaw, shootAim.x, speedAngular);

        // States

        bool avoidClipping = true;
        float desiredDistance = 0.0f;
        Vector3 desiredTarget = Vector3.zero;

        switch (cameraStateNext)
        {
            case CameraState.Action:
                desiredDistance = actionCameraDistance;
                desiredTarget = playerTransform.position + new Vector3(0.0f, actionCameraTargetHeight, 0.0f);
                desiredYaw = actionCameraYaw;
                desiredPitch = actionCameraPitch;
                break;
            case CameraState.Shoot:
                desiredDistance = shootingCameraTargetDistance + shootingCameraBackwardsDistance;
                avoidClipping = false;
                desiredTarget = playerTransform.position +
                    Quaternion.Euler(0.0f, shootingCameraYaw, 0.0f) *
                    Vector3.forward * shootingCameraTargetDistance +
                    new Vector3(0, shootingCameraHeight, 0);

                desiredYaw = shootingCameraYaw - Mathf.Atan(shootingCameraDistance / shootingCameraTargetDistance) * Mathf.Rad2Deg;
                break;
            case CameraState.Tactics:
                desiredDistance = tacticsCameraDistance;
                desiredTarget = playerTransform.position + new Vector3(0.0f, tacticsCameraTargetHeight, 0.0f);
                desiredYaw = playerTransform.eulerAngles.y;
                desiredPitch = -tacticsCameraPitch;
                break;
        }

        float rawCoef = 1.0f;

        if (cameraState != cameraStateNext && stateTransitionLerp ||
            transitionLerp)
        {
            cameraTarget = Vector3.Lerp(cameraTarget, desiredTarget, speedStateDistanceLerp);
            UpdateCameraDistance(speedStateDistanceLerp, desiredDistance, avoidClipping, true);
            cameraYaw = Mathf.LerpAngle(cameraYaw, desiredYaw, speedStateAngularLerp);
            cameraPitch = Mathf.LerpAngle(cameraPitch, desiredPitch, speedStateAngularLerp);

            rawCoef = stateTransitionRaw;
        }
        
        if(rawCoef > Mathf.Epsilon)
        {
            cameraTarget = Vector3.MoveTowards(cameraTarget, desiredTarget, speedStateDistance * rawCoef);
            UpdateCameraDistance(speedStateDistance * rawCoef, desiredDistance, avoidClipping, false);
            cameraYaw = Mathf.MoveTowardsAngle(cameraYaw, desiredYaw, speedStateAngular * rawCoef);
            cameraPitch = Mathf.MoveTowardsAngle(cameraPitch, desiredPitch, speedStateAngular * rawCoef);
        }

        cameraRemainingTargetDistance = Vector3.Distance(cameraTarget, desiredTarget);
        cameraRemainingDistance = Mathf.Abs(cameraDistance - desiredDistance);
        cameraRemainingYaw = Mathf.Abs(Mathf.DeltaAngle(cameraYaw, desiredYaw));
        cameraRemainingPitch = Mathf.Abs(Mathf.DeltaAngle(cameraPitch, desiredPitch));
        if (cameraRemainingTargetDistance < stateDistanceEpsilon &&
            cameraRemainingDistance < stateDistanceEpsilon &&
            cameraRemainingYaw < stateAngularEpsilon &&
            cameraRemainingPitch < stateAngularEpsilon)
        {
            cameraState = cameraStateNext;
        }

        Quaternion rotation = Quaternion.Euler(-cameraPitch, cameraYaw, 0);

        cameraPosition.Set(0, 0, cameraDistance);
        cameraPosition = cameraTarget - rotation * cameraPosition;

        cameraTransform.position = cameraPosition;
        cameraTransform.LookAt(cameraTarget);
    }

    void UpdateCameraDistance(float speed, float desiredDistance, bool avoidClipping = true, bool useLerp = false)
    {
        float distance = desiredDistance;
        cameraRayDistanceDebug = float.MaxValue;

        if (avoidClipping)
        {
            Vector3 backwards = -cameraTransform.forward;
            Vector3 dir1 = Quaternion.Euler(cameraAvoidanceSmoothAngle, 0.0f, 0.0f) * backwards;
            Vector3 dir2 = Quaternion.Euler(-cameraAvoidanceSmoothAngle, 0.0f, 0.0f) * backwards;
            Vector3 dir3 = Quaternion.Euler(0.0f, cameraAvoidanceSmoothAngle, 0.0f) * backwards;
            Vector3 dir4 = Quaternion.Euler(0.0f, -cameraAvoidanceSmoothAngle, 0.0f) * backwards;

            distance = Mathf.Min(distance, FilterDistance(new Ray(cameraTarget, dir1)));
            distance = Mathf.Min(distance, FilterDistance(new Ray(cameraTarget, dir2)));
            distance = Mathf.Min(distance, FilterDistance(new Ray(cameraTarget, dir3)));
            distance = Mathf.Min(distance, FilterDistance(new Ray(cameraTarget, dir4)));
        }

        float speedCoef = 1.0f;

        if (cameraDistance > distance)
        {
            if (cameraState == cameraStateNext)
            {
                speedCoef *= 16.0f;
            }
            else
            {
                speedCoef *= 2.0f;
            }
        }
        if (!useLerp)
        {
            cameraDistance = Mathf.MoveTowards(cameraDistance, distance, speed * speedCoef);
        }
        else
        {
            cameraDistance = Mathf.Lerp(cameraDistance, distance, Mathf.Clamp(speed, 0, 1));
        }
    }

    void MoveСharacter()
    {
        // Move character towards aim

        float speed = rotationSpeedLimit * Time.deltaTime;

        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon ||
            Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Epsilon;

        float playerYaw = playerTransform.eulerAngles.y;
        float playerPitch = playerTransform.eulerAngles.x;

        switch (cameraStateNext)
        {
            case CameraState.Action:
                if (isMoving ||
                    Mathf.Abs(Mathf.DeltaAngle(playerYaw, actionCameraYaw)) >= lookAwayThreshold)
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
                            Mathf.Abs(Mathf.DeltaAngle(playerYaw, actionCameraYaw)) >= lookAFKThreshold)
                        {
                            isPlayerRotating = true;
                        }
                    }
                }
                break;
            case CameraState.Shoot:
                isPlayerRotating = true;
                break;
            case CameraState.Tactics:
                isPlayerRotating = false;
                break;
        }

        if (isPlayerRotating)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(playerYaw, cameraYaw));
            if (diff > Mathf.Epsilon)
            {
                Quaternion q = Quaternion.Euler(0.0f, Mathf.MoveTowardsAngle(playerYaw, cameraYaw, speed), 0.0f);

                playerTransform.rotation = q;

                if (diff <= speed)
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
        float angle = playerTransform.eulerAngles.y * Mathf.Deg2Rad;

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

    void SwitchState(CameraState newState)
    {
        cameraState = cameraStateNext;
        cameraStateNext = newState;
    }

    void Update()
    {
        ProcessInput();

        MoveAim();

        UpdateCamera();

        MoveСharacter();
    }

    void FixedUpdate()
    {
        Movement();
    }

}
