using System;
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
public class SmartController : MonoBehaviour
{
    //--------------------------------------------------- AFTER REVISION -----------------------------------------------------------//

    // Public

    [Header("Input Settings")]

    [Tooltip("Mouse horizontal sensitivity")]
    [Range(1.0f, 10.0f)]
    public float mouseHorizontalSensitivity = 5.0f;

    [Tooltip("Mouse vertical sensitivity")]
    [Range(1.0f, 10.0f)]
    public float mouseVerticalSensitivity = 5.0f;

    [Header("Player Settings")]

    //[Tooltip("Player movement speed in meters per second")]
    //[Range(0.0f, 10.0f)]
    //public float playerMoveSpeed = 7.0f;

    [Tooltip("Player rotation speed in degrees per second")]
    [Range(0.0f, 360.0f)]
    public float playerRotationSpeed = 180.0f;

    [Header("Player Auto Rotation On Different Camera Angle")]

    [Tooltip("Enables this feature")]
    public bool playerAutoRotateEnabled = true;

    [Tooltip("If checked, timeout only counts during mouse being inactive")]
    public bool playerAutoRotatePassive = false;

    [Tooltip("Timeout for player to rotate towards camera")]
    [Range(0.0f, 60.0f)]
    public float playerAutoRotateTimeout = 1.0f;

    [Header("Player Immediate Rotation On Very Different Camera Angle")]

    [Tooltip("Enables this feature")]
    public bool playerImmediateAutoRotateEnabled = true;

    [Tooltip("Angle threshold to trigger rotation")]
    [Range(0.0f, 180.0f)]
    public float playerImmediateAutoRotateThreshold = 90.0f;

    [Header("Camera State Settings")]

    [Tooltip("Camera state transition speed. Default = 1.0")]
    [Range(0.0f, 10.0f)]
    public float transitionSpeed = 1.0f;


    [Header("Camera State Settings: Action")]

    [Tooltip("Action camera additive height")]
    [Range(0.0f, 10.0f)]
    public float actionHeight = 1.7f;

    [Tooltip("Action camera distance")]
    [Range(0.0f, 10.0f)]
    public float actionDistance = 7.0f;

    [Tooltip("Action camera pitch on state switch")]
    [Range(-45.0f, 45.0f)]
    public float actionInitialPitch = -30.0f;

    [Tooltip("Action camera pitch limit")]
    [Range(0.0f, 90.0f)]
    public float actionPitchLimit = 60.0f;

    [Header("Camera State Settings: Shoot")]

    [Tooltip("Shoot camera additive height")]
    [Range(0.0f, 10.0f)]
    public float shootHeight = 2.17f;

    [Tooltip("Shoot camera backwards offset")]
    [Range(0.0f, 1.0f)]
    public float shootBackwardsOffset = 0.3f;

    [Tooltip("Shoot camera right offset")]
    [Range(0.0f, 2.0f)]
    public float shootRightOffset = 0.777f;

    [Tooltip("Shoot camera pitch limit")]
    [Range(0.0f, 90.0f)]
    public float shootPitchLimit = 80.0f;

    [Header("Camera State Settings: Tactics")]

    [Tooltip("Tactics camera additive height")]
    [Range(0.0f, 10.0f)]
    public float tacticsHeight = 1.7f;

    [Tooltip("Tactics camera distance")]
    [Range(0.0f, 30.0f)]
    public float tacticsDistance = 18.0f;

    [Tooltip("Tactics camera pitch")]
    [Range(0.0f, 90.0f)]
    public float tacticsPitch = 90.0f;

    [Header("Camera Clipping Settings")]

    [Tooltip("Affects how safe distance for camera is calculated. Increasing prevents clipping, but makes camera closer to player")]
    [Range(1.0f, 8.0f)]
    public float cameraAvoidanceSmoothAngle = 2.0f;

    public enum CameraState
    {
        Action,
        Shoot,
        Tactics
    }
    [Header("Debug")]
    public CameraState cameraState = CameraState.Action;
    public CameraState cameraStateNext = CameraState.Action;
    public float cameraStateTransition = 0.0f;
    public float cameraRayDistanceDebug = 0.0f;

    // Private

    private float inputDeltaX;
    private float inputDeltaY;

    private float actionYaw = 0.0f;
    private float actionPitch = 0.0f;
    private float shootPitch = 0.0f;

    private float playerAutoRotateTimer = 0.0f;
    private bool playerAutoRotateActive = false;


    private Vector3 actionPosition = Vector3.zero;
    private Vector3 shootPosition = Vector3.zero;
    private Vector3 tacticsPosition = Vector3.zero;

    private Quaternion actionRotation = Quaternion.identity;
    private Quaternion shootRotation = Quaternion.identity;
    private Quaternion tacticsRotation = Quaternion.identity;

    /*
     * Last update data to use as start data on state switch
     */
    public float currentHeight = 0.0f;
    public Vector3 currentPosition = Vector3.zero;
    public Quaternion currentRotation = Quaternion.identity;


    /*
     * Last state switch data to interpolate from
     */
    public float startHeight = 0.0f;
    public Vector3 startPosition = Vector3.zero;


    public Vector3 eulerDelta = Vector3.zero;
    public Quaternion startRotation = Quaternion.identity;
    public Quaternion desiredRotation = Quaternion.identity;

    public float desiredHeight = 0.0f;
    public Vector3 desiredPosition = Vector3.zero;


    // Cache

    private GameObject player;
    private Transform playerTransform;
    private CharacterController playerController;

    private new GameObject camera;
    private Transform cameraTransform;

    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();
        playerController = GetComponent<CharacterController>();

        camera = GameObject.Find("Main Camera");
        cameraTransform = camera.GetComponent<Transform>();

    }

    void UpdateNextState()
    {
        switch (cameraStateNext)
        {
            case CameraState.Action:
                // Affect camera rotation
                actionYaw = Mathf.DeltaAngle(0.0f, actionYaw + inputDeltaX);
                actionPitch = Mathf.DeltaAngle(0.0f, actionPitch + inputDeltaY);
                actionPitch = Mathf.Clamp(actionPitch, -actionPitchLimit, actionPitchLimit);

                float yawDelta = Mathf.DeltaAngle(actionYaw, playerTransform.eulerAngles.y);

                if (playerImmediateAutoRotateEnabled)
                {
                    if (Mathf.Abs(Mathf.DeltaAngle(playerTransform.eulerAngles.y, actionYaw)) >= playerImmediateAutoRotateThreshold)
                    {
                        playerAutoRotateActive = true;
                    }
                }

                if (playerAutoRotateEnabled)
                {
                    if (yawDelta <= Mathf.Epsilon)
                    {
                        playerAutoRotateTimer += Time.deltaTime;
                        if (playerAutoRotateTimer >= playerAutoRotateTimeout)
                        {
                            playerAutoRotateActive = true;
                        }
                    }
                    else
                    {
                        playerAutoRotateTimer = 0.0f;
                    }
                }

                if (playerAutoRotateActive)
                {
                    playerTransform.rotation = Quaternion.Euler(
                                playerTransform.eulerAngles.x,
                                Mathf.MoveTowardsAngle(playerTransform.eulerAngles.y, actionYaw, playerRotationSpeed * Time.deltaTime),
                                playerTransform.eulerAngles.z);

                    if (Mathf.Abs(Mathf.DeltaAngle(playerTransform.eulerAngles.y, actionYaw)) <= Mathf.Epsilon)
                    {
                        playerAutoRotateActive = false;
                        playerAutoRotateTimer = 0.0f;
                    }
                }

                break;
            case CameraState.Shoot:
                // Affect camera, player rotation
                float shootYaw = playerTransform.eulerAngles.y + inputDeltaX;
                shootPitch = Mathf.DeltaAngle(0.0f, shootPitch + inputDeltaY);
                shootPitch = Mathf.Clamp(shootPitch, -shootPitchLimit, shootPitchLimit);

                playerTransform.rotation = Quaternion.Euler(playerTransform.eulerAngles.x, shootYaw, playerTransform.eulerAngles.z);


                break;
            case CameraState.Tactics:
                // Affect vector aim


                break;
        }
    }

    float FilterDistance(Ray ray, float distance, Vector3 forward)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance * 3.0f, ~LayerMask.NameToLayer("Environment")))
        {
            float angle = Vector3.Angle(hit.normal, forward);
            if (angle > 90.0f) angle = 90.0f;
            float factor = 1.0f - angle / 90.0f;
            float factorSqrt = Mathf.Pow(factor, 1.0f / 3.0f);

            float d = hit.distance * factorSqrt;

            cameraRayDistanceDebug = Mathf.Min(cameraRayDistanceDebug, d);

            return d;
        }

        return float.MaxValue;
    }

    void Update()
    {
        float tLinear = 1.0f;
        float tHermite = 1.0f;

        if (cameraState != cameraStateNext)
        {
            tLinear = cameraStateTransition;
            tHermite = Mathf.SmoothStep(0.0f, 1.0f, cameraStateTransition);
        }

        cameraRayDistanceDebug = float.MaxValue;

        inputDeltaX = Input.GetAxis("Mouse X") * mouseHorizontalSensitivity;
        inputDeltaY = Input.GetAxis("Mouse Y") * mouseVerticalSensitivity;

        UpdateNextState();

        switch (cameraStateNext)
        {
            case CameraState.Action:
                desiredHeight = actionHeight;
                desiredPosition = Quaternion.Euler(-actionPitch, actionYaw, 0.0f) * (Vector3.forward * -actionDistance);
                desiredRotation = Quaternion.Euler(-actionPitch, actionYaw, 0.0f);
                break;
            case CameraState.Shoot:
                desiredHeight = shootHeight;
                desiredPosition = Quaternion.Euler(-shootPitch, playerTransform.eulerAngles.y, 0.0f) * (Vector3.right * shootRightOffset + Vector3.forward * -shootBackwardsOffset);
                desiredRotation = Quaternion.Euler(-shootPitch, playerTransform.eulerAngles.y, 0.0f);
                break;
            case CameraState.Tactics:
                desiredHeight = tacticsHeight;
                desiredPosition = Quaternion.Euler(tacticsPitch, playerTransform.eulerAngles.y, 0.0f) * (Vector3.forward * -tacticsDistance);
                desiredRotation = Quaternion.Euler(tacticsPitch, playerTransform.eulerAngles.y, 0.0f);
                break;
        }

        currentHeight = Mathf.Lerp(currentHeight, desiredHeight, tHermite);
        currentPosition = new Vector3(
                Mathf.SmoothStep(startPosition.x, desiredPosition.x, tLinear),
                Mathf.SmoothStep(startPosition.y, desiredPosition.y, tLinear),
                Mathf.SmoothStep(startPosition.z, desiredPosition.z, tLinear));
        //currentRotation = Quaternion.Lerp(startRotation, desiredRotation, tHermite);
        //currentRotation = Quaternion.Euler(
        //    Mathf.SmoothStep(startRotation.eulerAngles.x, desiredRotation.eulerAngles.x, tLinear),
        //    Mathf.SmoothStep(startRotation.eulerAngles.y, desiredRotation.eulerAngles.y, tLinear),
        //    Mathf.SmoothStep(startRotation.eulerAngles.z, desiredRotation.eulerAngles.z, tLinear)
        //    );
        eulerDelta = new Vector3(
            Mathf.DeltaAngle(startRotation.eulerAngles.x, desiredRotation.eulerAngles.x),
            Mathf.DeltaAngle(startRotation.eulerAngles.y, desiredRotation.eulerAngles.y),
            Mathf.DeltaAngle(startRotation.eulerAngles.z, desiredRotation.eulerAngles.z)
            );
        //eulerDelta = new Vector3(
        //    (desiredRotation.eulerAngles.x - startRotation.eulerAngles.x),
        //    (desiredRotation.eulerAngles.y - startRotation.eulerAngles.y),
        //    (desiredRotation.eulerAngles.z - startRotation.eulerAngles.z)
        //    );
        currentRotation = Quaternion.Euler(
            Mathf.SmoothStep(startRotation.eulerAngles.x, startRotation.eulerAngles.x + eulerDelta.x, tLinear),
            Mathf.SmoothStep(startRotation.eulerAngles.y, startRotation.eulerAngles.y + eulerDelta.y, tLinear),
            Mathf.SmoothStep(startRotation.eulerAngles.z, startRotation.eulerAngles.z + eulerDelta.z, tLinear)
            );

        Vector3 target = playerTransform.position + new Vector3(0, currentHeight, 0);

        Vector3 finalPosition = currentPosition;

        if (true)
        {
            // Avoid clipping when no transition

            Vector3 forward = currentRotation * Vector3.forward;
            Vector3 backwards = currentRotation * (-Vector3.forward);
            Vector3 dir1 = Quaternion.Euler(cameraAvoidanceSmoothAngle, 0.0f, 0.0f) * backwards;
            Vector3 dir2 = Quaternion.Euler(-cameraAvoidanceSmoothAngle, 0.0f, 0.0f) * backwards;
            Vector3 dir3 = Quaternion.Euler(0.0f, cameraAvoidanceSmoothAngle, 0.0f) * backwards;
            Vector3 dir4 = Quaternion.Euler(0.0f, -cameraAvoidanceSmoothAngle, 0.0f) * backwards;

            // TODO: target is different for Shoot state, but this seem not to affect the player yet
            float baseDistance = currentPosition.magnitude;
            float distance = baseDistance;
            distance = Mathf.Min(distance, FilterDistance(new Ray(target, dir1), baseDistance, forward));
            distance = Mathf.Min(distance, FilterDistance(new Ray(target, dir2), baseDistance, forward));
            distance = Mathf.Min(distance, FilterDistance(new Ray(target, dir3), baseDistance, forward));
            distance = Mathf.Min(distance, FilterDistance(new Ray(target, dir4), baseDistance, forward));

            float compensation = baseDistance - distance;
            finalPosition += forward * compensation;
        }

        cameraTransform.position = finalPosition + target;
        cameraTransform.rotation = currentRotation;

        if (cameraState != cameraStateNext)
        {
            cameraStateTransition = Mathf.MoveTowards(cameraStateTransition, 1.0f, transitionSpeed * Time.deltaTime);

            if (cameraStateTransition >= 1.0f - Mathf.Epsilon)
            {
                cameraState = cameraStateNext;
                cameraStateTransition = 0.0f;
            }
        }
    }

    /*
     * Calling this forces player to rotate towards camera when in Action state
     */
    public void TriggerPlayerAutoRotation()
    {
        playerAutoRotateActive = true;
    }

    public void SwitchState(CameraState state)
    {
        if (cameraStateNext != state)
        {
            cameraState = cameraStateNext;
            cameraStateNext = state;
            cameraStateTransition = 0.0f;

            if (cameraStateNext == CameraState.Action)
            {
                actionYaw = playerTransform.eulerAngles.y;
                actionPitch = actionInitialPitch;
            }

            startHeight = currentHeight;
            startPosition = currentPosition;
            startRotation = currentRotation;
        }
    }


}
