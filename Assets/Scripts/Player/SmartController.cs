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

    [Tooltip("Player rotation speed in degrees per second")]
    [Range(0.0f, 360.0f)]
    public float playerRotationSpeed = 180.0f;

    [Header("Player Auto Rotation On Different Camera Angle")]

    [Tooltip("Enables this feature")]
    public bool playerAutoRotateEnabled = false;

    [Tooltip("If checked, timeout only counts during mouse being inactive")]
    public bool playerAutoRotatePassive = false;

    [Tooltip("Timeout for player to rotate towards camera")]
    [Range(0.0f, 60.0f)]
    public float playerAutoRotateTimeout = 1.0f;

    [Header("Player Immediate Rotation On Very Different Camera Angle")]

    [Tooltip("Enables this feature")]
    public bool playerImmediateAutoRotateEnabled = false;

    [Tooltip("Angle threshold to trigger rotation")]
    [Range(0.0f, 180.0f)]
    public float playerImmediateAutoRotateThreshold = 90.0f;

    [Header("Camera State Settings")]

    [Tooltip("Camera state transition time in seconds")]
    [Range(0.001f, 10.0f)]
    public float transitionTime = 0.33f;


    [Header("Camera State Settings: Action")]

    [Tooltip("Action camera additive height")]
    [Range(0.0f, 10.0f)]
    public float actionHeight = 1.7f;

    [Tooltip("Action camera distance")]
    [Range(0.0f, 10.0f)]
    public float actionDistance = 7.0f;

    [Tooltip("Action camera pitch limit")]
    [Range(0.0f, 90.0f)]
    public float actionPitchLimit = 60.0f;

    [Tooltip("Reset camera pitch on switch to Action state")]
    public bool actionInitialPitchEnabled = true;
    
    [Tooltip("Action camera pitch on state switch")]
    [Range(-45.0f, 45.0f)]
    public float actionInitialPitch = -30.0f;

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
        
    [Tooltip("Reset camera pitch on switch to Shoot state")]
    public bool shootInitialPitchEnabled = true;

    [Tooltip("Shoot camera pitch on state switch")]
    [Range(-45.0f, 45.0f)]
    public float shootInitialPitch = 0.0f;

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
    
    /*
     * Last update data to use as start data on state switch
     */
    private float currentHeight = 0.0f;
    private Vector3 currentPosition = Vector3.zero;
    private Quaternion currentRotation = Quaternion.identity;


    /*
     * Last state switch data to interpolate from
     */
    private float startHeight = 0.0f;
    private Vector3 startPosition = Vector3.zero;
    private Quaternion startRotation = Quaternion.identity;

    /*
     * Camera rotation delta accumulated during state transition
     */
    private float transitionDeltaYaw = 0.0f;
    private float transitionDeltaPitch = 0.0f;

    // Cache

    private GameObject player;
    private Transform playerTransform;

    private new GameObject camera;
    private Transform cameraTransform;

    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();

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
        cameraRayDistanceDebug = float.MaxValue;
        float tLinear = 1.0f;

        if (cameraState != cameraStateNext)
        {
            tLinear = cameraStateTransition;
        }

        float desiredHeight = 0.0f;
        Vector3 desiredPosition = Vector3.zero;
        Quaternion desiredRotation = Quaternion.identity;
        
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

        if (transitionDeltaYaw == float.MaxValue)
            transitionDeltaYaw = Mathf.DeltaAngle(startRotation.eulerAngles.y, desiredRotation.eulerAngles.y);
        if (transitionDeltaPitch == float.MaxValue)
            transitionDeltaPitch = Mathf.DeltaAngle(startRotation.eulerAngles.x, desiredRotation.eulerAngles.x);

        switch (cameraStateNext)
        {
            case CameraState.Action:
                transitionDeltaYaw += inputDeltaX;
                transitionDeltaPitch -= inputDeltaY;
                transitionDeltaPitch = Mathf.DeltaAngle(0,
                    Mathf.Clamp(Mathf.DeltaAngle(0,
                    transitionDeltaPitch + startRotation.eulerAngles.x), -actionPitchLimit, actionPitchLimit) - startRotation.eulerAngles.x);
                break;
            case CameraState.Shoot:
                transitionDeltaYaw += inputDeltaX;
                transitionDeltaPitch -= inputDeltaY;
                transitionDeltaPitch = Mathf.DeltaAngle(0,
                    Mathf.Clamp(Mathf.DeltaAngle(0,
                    transitionDeltaPitch + startRotation.eulerAngles.x), -shootPitchLimit, shootPitchLimit) - startRotation.eulerAngles.x);
                break;
            case CameraState.Tactics:
                break;
        }

        currentHeight = Mathf.SmoothStep(startHeight, desiredHeight, tLinear);
        currentPosition = new Vector3(
                Mathf.SmoothStep(startPosition.x, desiredPosition.x, tLinear),
                Mathf.SmoothStep(startPosition.y, desiredPosition.y, tLinear),
                Mathf.SmoothStep(startPosition.z, desiredPosition.z, tLinear));
        if (cameraState != cameraStateNext)
        {
            Vector3 eulerDelta = new Vector3(
                (transitionDeltaPitch),
                (transitionDeltaYaw),
                (desiredRotation.eulerAngles.z - startRotation.eulerAngles.z)
                );
            currentRotation = Quaternion.Euler(
                Mathf.SmoothStep(startRotation.eulerAngles.x, startRotation.eulerAngles.x + eulerDelta.x, tLinear),
                Mathf.SmoothStep(startRotation.eulerAngles.y, startRotation.eulerAngles.y + eulerDelta.y, tLinear),
                Mathf.SmoothStep(startRotation.eulerAngles.z, startRotation.eulerAngles.z + eulerDelta.z, tLinear)
                );
        }
        else
        {
            currentRotation = desiredRotation;
        }


        Vector3 target = playerTransform.position + new Vector3(0, currentHeight, 0);

        Vector3 finalPosition = currentPosition;

        // Avoid clipping through simple forward directed camera ray hittest
        {
            Vector3 fromCamera = Vector3.zero;
            switch (cameraStateNext)
            {
                case CameraState.Action:
                    fromCamera = -currentPosition;
                    break;
                case CameraState.Shoot:
                    // This is not correct without
                    // + Quaternion.Euler(-shootPitch, playerTransform.eulerAngles.y, 0.0f) * (Vector3.right * shootRightOffset + Vector3.forward * -shootBackwardsOffset);
                    // part, but without it the visuals look smoother,
                    // until serious glitches occur, keep it commented
                    fromCamera = -currentPosition; // + Quaternion.Euler(-shootPitch, playerTransform.eulerAngles.y, 0.0f) * (Vector3.right * shootRightOffset + Vector3.forward * -shootBackwardsOffset);
                    break;
                case CameraState.Tactics:
                    fromCamera = -currentPosition;
                    break;
            }
            Vector3 toCamera = -fromCamera;

            Vector3 forward = fromCamera.normalized;
            Vector3 backwards = toCamera.normalized;

            Vector3 dir1 = Quaternion.Euler(cameraAvoidanceSmoothAngle, 0.0f, 0.0f) * backwards;
            Vector3 dir2 = Quaternion.Euler(-cameraAvoidanceSmoothAngle, 0.0f, 0.0f) * backwards;
            Vector3 dir3 = Quaternion.Euler(0.0f, cameraAvoidanceSmoothAngle, 0.0f) * backwards;
            Vector3 dir4 = Quaternion.Euler(0.0f, -cameraAvoidanceSmoothAngle, 0.0f) * backwards;

            float baseDistance = toCamera.magnitude;
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
            cameraStateTransition = Mathf.MoveTowards(cameraStateTransition, 1.0f, (1.0f / transitionTime) * Time.deltaTime);

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

            switch (cameraStateNext)
            {
                case CameraState.Action:
                    actionYaw = playerTransform.eulerAngles.y;
                    if (actionInitialPitchEnabled) actionPitch = actionInitialPitch;
                    break;
                case CameraState.Shoot:
                    if (shootInitialPitchEnabled) shootPitch = shootInitialPitch;
                    break;
                case CameraState.Tactics:
                    break;
            }

            startHeight = currentHeight;
            startPosition = currentPosition;
            startRotation = currentRotation;

            transitionDeltaYaw = float.MaxValue;
            transitionDeltaPitch = float.MaxValue;
        }
    }


}
