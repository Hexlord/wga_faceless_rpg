using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 01.01.2019   aknorre     Created
 * 
 */

/*
 *  Basic character rotation algorithm is:
 *  
 *  1. After timeout of not moving mouse rotate character to match aim
 *  2. After X angle becoming 90+ degree rotate character to match aim continuously (aim can still move during character rotation) until reaching the aim
 *  3. After any movement rotate character to match aim
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

    [Tooltip("Minimum distance to target for camera")]
    [Range(0.0f, 10.0f)]
    public float actionMinimumDistance = 1.65f;

    [Tooltip("Action camera additive height")]
    [Range(0.0f, 10.0f)]
    public float actionHeight = 1.7f;

    [Tooltip("Action camera additive right offset")]
    [Range(0.0f, 10.0f)]
    public float actionRightOffset = 0.0f;

    [Tooltip("Action camera distance")]
    [Range(0.0f, 20.0f)]
    public float actionDistance = 7.0f;

    [Tooltip("Action camera pitch limit")]
    [Range(0.0f, 90.0f)]
    public float actionPitchLimit = 60.0f;

    [Tooltip("Reset camera pitch on switch to Action state")]
    public bool actionInitialPitchEnabled = true;

    [Tooltip("Action camera pitch on state switch")]
    [Range(-45.0f, 45.0f)]
    public float actionInitialPitch = -15.0f;

    [Tooltip("Action camera field of view")]
    [Range(1.0f, 115.0f)]
    public float actionFOV = 90.0f;

    [Header("Camera State Settings: Shoot")]

    [Tooltip("Minimum distance to target for camera")]
    [Range(0.0f, 10.0f)]
    public float shootMinimumDistance = 0.0f;

    [Tooltip("Shoot camera additive height")]
    [Range(0.0f, 10.0f)]
    public float shootHeight = 2.17f;

    [Tooltip("Shoot camera backwards offset")]
    [Range(0.0f, 5.0f)]
    public float shootBackwardsOffset = 0.3f;

    [Tooltip("Shoot camera right offset")]
    [Range(0.0f, 5.0f)]
    public float shootRightOffset = 0.777f;

    [Tooltip("Shoot camera pitch limit")]
    [Range(0.0f, 90.0f)]
    public float shootPitchLimit = 80.0f;

    [Tooltip("Reset camera pitch on switch to Shoot state")]
    public bool shootInitialPitchEnabled = true;

    [Tooltip("Shoot camera pitch on state switch")]
    [Range(-45.0f, 45.0f)]
    public float shootInitialPitch = 0.0f;
       
    [Tooltip("Shoot camera field of view")]
    [Range(1.0f, 115.0f)]
    public float shootFOV = 90.0f;

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
       
    [Tooltip("Tactics camera field of view")]
    [Range(1.0f, 115.0f)]
    public float tacticsFOV = 90.0f;

    [Header("Camera Clipping Settings")]

    [Tooltip("Affects how safe distance for camera is calculated")]
    [Range(1.0f, 8.0f)]
    public float cameraAvoidanceSmoothAngle = 3.0f;

    [Tooltip("Camera distance change speed")]
    [Range(0.1f, 50.0f)]
    public float cameraAvoidanceSpeed = 12.0f;

    [Tooltip("Camera distance immediate snap for solid clipping")]
    public bool cameraAvoidanceInstantSnap = true;

    [Tooltip("Maximum camera distance offset from object for perpendicular normal conditions")]
    [Range(0.1f, 5.0f)]
    public float cameraAvoidanceOffset = 2.0f;

    [Tooltip("Camera distance offset normal angle factor reversed")]
    [Range(1.0f, 5.0f)]
    public float cameraAvoidanceNormalFactorReversed = 2.0f;

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

    // Private

    private float inputDeltaX;
    private float inputDeltaY;

    private float actionYaw = 0.0f;
    private float actionPitch = 0.0f;
    private float shootYaw = 0.0f;
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
    private float startFOV = 0.0f;

    /*
     * Camera rotation delta accumulated during state transition
     */
    private float transitionDeltaYaw = 0.0f;
    private float transitionDeltaPitch = 0.0f;

    private float avoidanceOffset = 0.0f;

    // Cache

    private GameObject player;
    private Transform playerTransform;

    private new GameObject camera;
    private Transform cameraTransform;
    private Camera cameraComponent;

    void Start()
    {
        // Cache

        player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();

        camera = GameObject.Find("Main Camera");
        cameraTransform = camera.GetComponent<Transform>();
        cameraComponent = camera.GetComponent<Camera>();

        actionYaw = playerTransform.eulerAngles.y;
        actionPitch = actionInitialPitch;

        shootPitch = shootInitialPitch;

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
                shootYaw = Mathf.DeltaAngle(0.0f, shootYaw + inputDeltaX);
                shootPitch = Mathf.DeltaAngle(0.0f, shootPitch + inputDeltaY);
                shootPitch = Mathf.Clamp(shootPitch, -shootPitchLimit, shootPitchLimit);
                               
                break;
            case CameraState.Tactics:
                // Affect vector aim


                break;
        }
    }

    struct RayEntry
    {
        public Collider collider;
        public bool small;
        public float angleFactor;
        public float distance;
    };
    class SnapEntry
    {
        public float distanceStart;
        public float distanceEnd;
    };

    void ClipAnalyze(out float outDistance, out float outSnap, Vector3 cameraObject, Vector3 rayDirection, float cameraDistance, Vector3 cameraForward)
    {
        Vector3 rayObject = cameraObject;

        Vector3 cameraPosition = rayObject + rayDirection * cameraDistance;

        float range = 100.0f;

        int mask = (1 << LayerMask.NameToLayer("Environment"));

        RaycastHit[] hitsForward = Physics.RaycastAll(new Ray(cameraObject + rayDirection * range, -rayDirection), range, mask);
        RaycastHit[] hitsBackwards = Physics.RaycastAll(new Ray(cameraObject, rayDirection), range, mask);
        RaycastHit[] hitsObjectForward = Physics.RaycastAll(new Ray(cameraObject, -rayDirection), range, mask);
        
        List<RayEntry> rayEntries = new List<RayEntry>();

        {
            foreach (RaycastHit hit in hitsBackwards)
            {
                RayEntry entry;
                entry.collider = hit.collider;
                entry.small = hit.collider.gameObject.CompareTag("Small");
                entry.distance = hit.distance;
                float angle = Mathf.Abs(Vector3.Angle(hit.normal, cameraForward));
                if (angle > 90.0f) angle = 180.0f - angle;
                float factor = 1.0f - angle / 90.0f;
                entry.angleFactor = Mathf.Pow(factor, 1.0f / cameraAvoidanceNormalFactorReversed);

                if (entry.collider.gameObject.CompareTag("Planar"))
                {
                    rayEntries.Add(entry);
                }
                rayEntries.Add(entry);
            }
            foreach (RaycastHit hit in hitsForward)
            {
                RayEntry entry;
                entry.collider = hit.collider;
                entry.small = hit.collider.gameObject.CompareTag("Small");
                entry.distance = range - hit.distance;
                float angle = Mathf.Abs(Vector3.Angle(hit.normal, cameraForward));
                if (angle > 90.0f) angle = 180.0f - angle;
                float factor = 1.0f - angle / 90.0f;
                entry.angleFactor = Mathf.Pow(factor, 1.0f / cameraAvoidanceNormalFactorReversed);
                if (entry.collider.gameObject.CompareTag("Planar"))
                {
                    rayEntries.Add(entry);
                }
                rayEntries.Add(entry);
            }
        }

        rayEntries.Sort(delegate (RayEntry a, RayEntry b)
        {
            return a.distance.CompareTo(b.distance);
        });

        List<Collider> objectStuckColliders = new List<Collider>();
        {
            foreach (RaycastHit hit in hitsBackwards)
            {
                foreach (RaycastHit hit2 in hitsObjectForward)
                {
                    if (hit.collider == hit2.collider)
                    {
                        objectStuckColliders.Add(hit.collider);
                    }
                }
            }
        }

        List<Collider> currentColliders = new List<Collider>();
        List<SnapEntry> snapEntries = new List<SnapEntry>();

        float start = float.MaxValue;
        float snap = float.MaxValue;
        bool stuck = false;

        for (int i = 0; i < rayEntries.Count; ++i)
        {
            RayEntry entry = rayEntries[i];
            if (objectStuckColliders.Contains(entry.collider)) continue;

            if (currentColliders.Contains(entry.collider))
            {
                currentColliders.Remove(entry.collider);
            }
            else
            {
                currentColliders.Add(entry.collider);
            }

            if (currentColliders.Count == 0)
            {
                // Finished segment
                SnapEntry snapEntry = new SnapEntry
                {

                    // Widen snap entries by cameraAvoidanceOffset
                    distanceStart = start - cameraAvoidanceOffset * entry.angleFactor,
                    distanceEnd = entry.distance + cameraAvoidanceOffset * entry.angleFactor
                };

                if (cameraDistance >= snapEntry.distanceStart &&
                    cameraDistance <= snapEntry.distanceEnd)
                {
                    stuck = true;
                }

                snapEntries.Add(snapEntry);
                start = float.MaxValue;
            }
            else
            {
                // Started segment (first distance is the lowest one, keep it)
                if(start == float.MaxValue)
                {
                    start = entry.distance;
                }
            }

            if (!entry.collider.gameObject.CompareTag("Small") &&
                !entry.collider.gameObject.CompareTag("Player"))
            {
                snap = Mathf.Min(snap, entry.distance);
            }
        }

        // Join touching segments
        for (int i = 0; i < snapEntries.Count - 1; ++i)
        {
            if(snapEntries[i].distanceEnd >=
                snapEntries[i+1].distanceStart)
            {
                snapEntries[i].distanceEnd = snapEntries[i + 1].distanceEnd;
                snapEntries.RemoveAt(i + 1);
            }
        }

        if (!stuck &&
            cameraDistance <= snap)
        {
            outDistance = cameraDistance;
            outSnap = snap;
        }
        else // Need to find an extremum, which is both below snap and closest to cameraDistance
        {
            float distanceUp = float.MaxValue;
            float distanceDown = float.MaxValue;
            for (int i = 0; i < snapEntries.Count; ++i)
            {
                SnapEntry entry = snapEntries[i];
                if (cameraDistance <= entry.distanceStart &&
                    entry.distanceStart <= snap)
                {
                    distanceUp = entry.distanceStart;
                    break;
                }
                if (cameraDistance <= entry.distanceEnd &&
                    entry.distanceEnd <= snap)
                {
                    distanceUp = entry.distanceEnd;
                    break;
                }
            }

            for (int i = snapEntries.Count - 1; i >= 0; --i)
            {
                SnapEntry entry = snapEntries[i];
                if (cameraDistance >= entry.distanceStart &&
                    entry.distanceStart <= snap)
                {
                    distanceDown = entry.distanceStart;
                    break;
                }
                if (cameraDistance >= entry.distanceEnd &&
                    entry.distanceEnd <= snap)
                {
                    distanceDown = entry.distanceEnd;
                    break;
                }
            }

            /*
             * Shortest snap is disabled for inconsistency with multiple ray result accumulation 
             * (both avg and min/max lead to poor results)
            // Shortest snap
            float deltaUp = Mathf.Abs(distanceUp - cameraDistance);
            float deltaDown = Mathf.Abs(distanceDown - cameraDistance);
            if (deltaUp < deltaDown)
            {
                distance = distanceUp;
            }
            else
            {
                distance = distanceDown;
            }
            */

            // Closest snap
            if(distanceDown != float.MaxValue)
            {
                outDistance = distanceDown;
                outSnap = snap;
            } else
            {
                outDistance = distanceUp;
                outSnap = snap;
            }
        }

    }

    void Update()
    {
        float tLinear = 1.0f;

        if (cameraState != cameraStateNext)
        {
            tLinear = cameraStateTransition;
        }

        float desiredHeight = 0.0f;
        Vector3 desiredPosition = Vector3.zero;
        Quaternion desiredRotation = Quaternion.identity;
        float desiredFOV = 0.0f;
        float desiredDistance = 0.0f;
        float minDistance = 0.0f;
        inputDeltaX = Input.GetAxis("Mouse X") * mouseHorizontalSensitivity;
        inputDeltaY = Input.GetAxis("Mouse Y") * mouseVerticalSensitivity;

        UpdateNextState();

        switch (cameraStateNext)
        {
            case CameraState.Action:
                desiredHeight = actionHeight;

                desiredPosition = Quaternion.Euler(-actionPitch, actionYaw, 0.0f) * (Vector3.right * actionRightOffset + Vector3.forward * -actionDistance);
                desiredRotation = Quaternion.Euler(-actionPitch, actionYaw, 0.0f);

                desiredFOV = actionFOV;
                desiredDistance = actionDistance;
                minDistance = actionMinimumDistance;
                break;
            case CameraState.Shoot:
                desiredHeight = shootHeight;
                //desiredPosition = Quaternion.Euler(-shootPitch, playerTransform.eulerAngles.y, 0.0f) * (Vector3.right * shootRightOffset + Vector3.forward * -shootBackwardsOffset);
                //desiredRotation = Quaternion.Euler(-shootPitch, playerTransform.eulerAngles.y, 0.0f);

                desiredPosition = Quaternion.Euler(-shootPitch, shootYaw, 0.0f) * (Vector3.right * shootRightOffset + Vector3.forward * -shootBackwardsOffset);
                desiredRotation = Quaternion.Euler(-shootPitch, shootYaw, 0.0f);

                desiredFOV = shootFOV;
                desiredDistance = shootBackwardsOffset;
                minDistance = shootMinimumDistance;
                break;
            case CameraState.Tactics:
                desiredHeight = tacticsHeight;
                desiredPosition = Quaternion.Euler(tacticsPitch, playerTransform.eulerAngles.y, 0.0f) * (Vector3.forward * -tacticsDistance);
                desiredRotation = Quaternion.Euler(tacticsPitch, playerTransform.eulerAngles.y, 0.0f);

                desiredFOV = tacticsFOV;
                desiredDistance = tacticsDistance;
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
        cameraComponent.fieldOfView = Mathf.SmoothStep(startFOV, desiredFOV, tLinear);
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
            Vector3 forward = desiredRotation * Vector3.forward;
            Vector3 backwards = -forward;

            Vector3 dir1 = Quaternion.Euler(cameraAvoidanceSmoothAngle, 0.0f, 0.0f) * backwards;
            Vector3 dir2 = Quaternion.Euler(-cameraAvoidanceSmoothAngle, 0.0f, 0.0f) * backwards;
            Vector3 dir3 = Quaternion.Euler(0.0f, cameraAvoidanceSmoothAngle, 0.0f) * backwards;
            Vector3 dir4 = Quaternion.Euler(0.0f, -cameraAvoidanceSmoothAngle, 0.0f) * backwards;

            float distance = desiredDistance;

            float d1, s1;
            float d2, s2;
            float d3, s3;
            float d4, s4;
            ClipAnalyze(out d1, out s1, target, dir1, desiredDistance, forward);
            ClipAnalyze(out d2, out s2, target, dir2, desiredDistance, forward);
            ClipAnalyze(out d3, out s3, target, dir3, desiredDistance, forward);
            ClipAnalyze(out d4, out s4, target, dir4, desiredDistance, forward);

            distance = Mathf.Min(distance, d1);
            distance = Mathf.Min(distance, d2);
            distance = Mathf.Min(distance, d3);
            distance = Mathf.Min(distance, d4);

            float snap = s1;
            snap = Mathf.Min(snap, s2);
            snap = Mathf.Min(snap, s3);
            snap = Mathf.Min(snap, s4);

            float compensation = desiredDistance - distance;
            float snapCompensation = desiredDistance - snap;

            if(cameraAvoidanceInstantSnap)
            {
                if (avoidanceOffset < snapCompensation)
                {
                    avoidanceOffset = snapCompensation;
                }
            }

            avoidanceOffset = Mathf.MoveTowards(avoidanceOffset, compensation, cameraAvoidanceSpeed * Time.deltaTime);

            avoidanceOffset = Mathf.Min(avoidanceOffset, desiredDistance - minDistance);

            finalPosition += forward * avoidanceOffset;
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

    public CameraState GetState()
    {
        return cameraStateNext;
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
                    shootYaw = playerTransform.eulerAngles.y;
                    if (shootInitialPitchEnabled) shootPitch = shootInitialPitch;
                    break;
                case CameraState.Tactics:
                    break;
            }

            startHeight = currentHeight;
            startPosition = currentPosition;
            startRotation = currentRotation;
            startFOV = cameraComponent.fieldOfView;

            transitionDeltaYaw = float.MaxValue;
            transitionDeltaPitch = float.MaxValue;
        }
    }


}
