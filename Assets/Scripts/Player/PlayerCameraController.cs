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
 * 14.03.2019   aknorre     Reworked (separation from camera module)
 * 16.03.2019   bkrylov     Allocated to Component Menu
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
[AddComponentMenu("ProjectFaceless/Player/Camera Controller")]
public class PlayerCameraController : MonoBehaviour
{
    // Public

    [Header("Input Settings")]

    [Tooltip("Mouse horizontal sensitivity")]
    [Range(1.0f, 10.0f)]
    public float mouseHorizontalSensitivity = 5.0f;

    [Tooltip("Mouse vertical sensitivity")]
    [Range(1.0f, 10.0f)]
    public float mouseVerticalSensitivity = 5.0f;

    [Tooltip("Mouse vertical inversion")]
    public bool mouseVerticalInversion = true;

    [Header("Player Settings")]

    [Tooltip("Player rotation speed in degrees per second")]
    [Range(0.0f, 3600.0f)]
    public float playerRotationSpeed = 360.0f;

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

    [Header("Camera Transition Settings")]

    [Tooltip("Camera transition time in seconds")]
    [Range(0.001f, 10.0f)]
    public float transitionTime = 0.33f;

    [Header("Camera Clipping Settings")]

    [Tooltip("Affects how safe distance for camera is calculated")]
    [Range(1.0f, 8.0f)]
    public float clippingAvoidanceSmoothAngle = 3.0f;

    [Tooltip("Camera distance change speed raw")]
    [Range(0.1f, 100.0f)]
    public float clippingAvoidanceSpeed = 0.5f;

    [Tooltip("Camera distance change speed linear interpolation per frame")]
    [Range(0.1f, 1.0f)]
    public float clippingAvoidanceSpeedLerp = 0.05f;

    [Tooltip("Camera distance immediate snap for solid clipping")]
    public bool clippingAvoidanceInstantSnap = true;

    [Tooltip("Maximum camera distance offset from object for perpendicular normal conditions")]
    [Range(0.1f, 5.0f)]
    public float clippingAvoidanceRayAngleOffset = 2.0f;

    [Tooltip("Camera distance offset normal angle factor reversed")]
    [Range(1.0f, 5.0f)]
    public float clippingAvoidanceNormalFactorReversed = 2.0f;

    [Tooltip("Camera distance offset normal angle factor baseline")]
    [Range(0.0f, 1.0f)]
    public float clippingAvoidanceNormalFactorBaseline = 0.1f;

    public bool Freeze
    {
        get { return freeze; }
        set
        {
            freeze = value;
        }
    }

    public Camera Camera
    {
        get { return camera; }
    }

    public ConstrainedCamera ConstrainedCamera
    {
        get { return constrainedCamera; }
    }

    // Private

    [Header("Debug")]

    public bool freeze = false;

    public float playerAutoRotateTimer = 0.0f;
    public bool playerAutoRotateActive = false;

    public float clippingAvoidanceOffset = 0.0f;

    public ConstrainedCamera constrainedCamera;
    public float cameraTransition = 1.0f;

    /*
     * Camera rotation delta accumulated during state transition
     */
    public float transitionDeltaYaw = 0.0f;
    public float transitionDeltaPitch = 0.0f;


    /*
     * Camera transform on start of transition
     */
    public float transitionStartYaw = 0.0f;
    public float transitionStartPitch = 0.0f;
    public Vector3 transitionStartPosition = Vector3.zero;
    public float transitionStartFOV = 80.0f;

    public float transitionSpeed = 0.0f;

    private Quaternion desiredRotation = Quaternion.identity;

    public float desiredCameraOffset = 0.0f;

    // Cache

    private new Camera camera;
    private Rigidbody body;

    void Awake()
    {
        // Cache

        camera = GameObject.Find("MainCamera").GetComponent<Camera>();
        body = GetComponent<Rigidbody>();

        desiredRotation = body.rotation;

        transitionSpeed = 1.0f / transitionTime;
    }

    void UpdatePlayerRotation(float delta)
    {
        float yawDelta = Mathf.DeltaAngle(body.rotation.eulerAngles.y, camera.transform.rotation.eulerAngles.y);

        if (playerImmediateAutoRotateEnabled)
        {
            if (Mathf.Abs(yawDelta) >= playerImmediateAutoRotateThreshold)
            {
                playerAutoRotateActive = true;
            }
        }

        if (playerAutoRotateEnabled)
        {
            if (Mathf.Abs(yawDelta) > Mathf.Epsilon)
            {
                playerAutoRotateTimer += delta;
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
            desiredRotation = Quaternion.Euler(
                body.rotation.eulerAngles.x,
                Mathf.MoveTowards(desiredRotation.eulerAngles.y, desiredRotation.eulerAngles.y + MathfExtensions.NormalizeAngle(camera.transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y), playerRotationSpeed * delta),
                body.rotation.eulerAngles.z);
            yawDelta = Mathf.DeltaAngle(body.rotation.eulerAngles.y, camera.transform.rotation.eulerAngles.y);
            if (Mathf.Abs(yawDelta) <= Mathf.Epsilon)
            {
                playerAutoRotateActive = false;
                playerAutoRotateTimer = 0.0f;
            }
        }

        body.rotation = desiredRotation;
    }

    struct RayEntry
    {
        public Collider collider;
        public float angleFactor;
        public float distance;
    };
    class SegmentEntry
    {
        public float distanceStart;
        public float distanceEnd;
    };

    private void ClipAnalyze(out float outDistance, out float outSnap, Vector3 cameraTarget, Vector3 fromCameraToTarget, float cameraDistance, Vector3 cameraForward)
    {
        var fromTargetToCamera = -fromCameraToTarget;

        const float range = 100.0f;

        var mask = LayerMask.GetMask("Environment");

        // Collect collisions 

        var hitsForwardIntoTarget = Physics.RaycastAll(new Ray(cameraTarget + fromTargetToCamera * range, fromCameraToTarget), range, mask);
        var hitsBackwardsIntoCamera = Physics.RaycastAll(new Ray(cameraTarget, fromTargetToCamera), range, mask);
        var hitsBackwardsIntoTarget = Physics.RaycastAll(new Ray(cameraTarget + fromCameraToTarget * range, fromTargetToCamera), range, mask);

        var rayEntries = new List<RayEntry>();

        // Go both ways, add planar objects twice (to compensate for their single side)
        foreach (var hit in hitsBackwardsIntoCamera)
        {
            RayEntry entry;
            entry.collider = hit.collider;
            entry.distance = hit.distance;
            var angle = Mathf.Abs(Vector3.Angle(hit.normal, cameraForward));
            if (angle > 90.0f) angle = 180.0f - angle;
            var factor = 1.0f - angle / 90.0f;
            entry.angleFactor = Mathf.Min(1.0f, clippingAvoidanceNormalFactorBaseline + Mathf.Pow(factor, 1.0f / clippingAvoidanceNormalFactorReversed));

            if (entry.collider.gameObject.CompareTag("Planar"))
            {
                rayEntries.Add(entry);
            }
            rayEntries.Add(entry);
        }
        foreach (var hit in hitsForwardIntoTarget)
        {
            RayEntry entry;
            entry.collider = hit.collider;
            entry.distance = range - hit.distance;
            var angle = Mathf.Abs(Vector3.Angle(hit.normal, cameraForward));
            if (angle > 90.0f) angle = 180.0f - angle;
            var factor = 1.0f - angle / 90.0f;
            entry.angleFactor = Mathf.Min(1.0f, clippingAvoidanceNormalFactorBaseline + Mathf.Pow(factor, 1.0f / clippingAvoidanceNormalFactorReversed));
            if (entry.collider.gameObject.CompareTag("Planar"))
            {
                rayEntries.Add(entry);
            }
            rayEntries.Add(entry);
        }

        rayEntries.Sort((a, b) => a.distance.CompareTo(b.distance));

        // If object is stuck inside other object, ignore that object
        // Example is player being in cube of ice, when we should ignore ice cube boundaries in our calculations
        var objectStuckColliders = new List<Collider>();
        {
            foreach (var hit in hitsForwardIntoTarget)
            {
                foreach (var hit2 in hitsBackwardsIntoTarget)
                {
                    if (hit.collider == hit2.collider)
                    {
                        objectStuckColliders.Add(hit.collider);
                    }
                }
            }
        }

        var currentColliders = new List<Collider>();
        var segmentEntries = new List<SegmentEntry>();

        var start = float.MaxValue;
        var snap = float.MaxValue;

        // Go starting from closest to target
        foreach (var entry in rayEntries)
        {
            if (objectStuckColliders.Contains(entry.collider)) continue;

            if (currentColliders.Contains(entry.collider))
            {
                // Second collision means we are now out of boundaries for that collider
                currentColliders.Remove(entry.collider);
            }
            else
            {
                // First appearance means we must wait for the end of collider (furthest side collision)
                currentColliders.Add(entry.collider);
            }

            if (start == float.MaxValue)
            {
                // Started segment (first distance is the lowest one, keep it)
                start = entry.distance;
            }

            if (currentColliders.Count == 0)
            {

                // Finished segment
                var segmentEntry = new SegmentEntry
                {
                    // Widen snap entries by cameraAvoidanceOffset

                    distanceStart = start - clippingAvoidanceRayAngleOffset * entry.angleFactor,
                    distanceEnd = entry.distance + clippingAvoidanceRayAngleOffset * entry.angleFactor
                };

                segmentEntries.Add(segmentEntry);
                start = float.MaxValue;
            }


            if (!entry.collider.gameObject.CompareTag("Small") &&
                !entry.collider.gameObject.CompareTag("Player"))
            {
                // If object is NOT marked as small, then we must minimize camera immediate position change to distance of any collision with this collider
                // In other words: camera can be above small stone column blocking vision for target
                // camera can NEVER be above big cave or mountain, it immediately goes through it to watch target without big object blocking its vision
                snap = Mathf.Min(snap, entry.distance - clippingAvoidanceRayAngleOffset * entry.angleFactor);
            }
        }

        // Can not go above snap
        cameraDistance = Mathf.Min(cameraDistance, snap);

        // Join touching segments
        // As we are only interested in valid position segments now
        for (var i = 0; i < segmentEntries.Count - 1; ++i)
        {
            if (segmentEntries[i].distanceEnd < segmentEntries[i + 1].distanceStart) continue;
            segmentEntries[i].distanceEnd = segmentEntries[i + 1].distanceEnd;
            segmentEntries.RemoveAt(i + 1);
            --i;
        }

        var stuck = false;
        var stuckDown = float.MinValue;
        var stuckUp = float.MaxValue;

        foreach (var entry in segmentEntries)
        {
            if (cameraDistance < entry.distanceStart || cameraDistance > entry.distanceEnd) continue;

            // Mark that desired camera position is inside some collider
            stuck = true;
            stuckDown = entry.distanceStart;
            stuckUp = entry.distanceEnd;
            break;
        }

        outSnap = snap;

        if (stuck)
        {
            if (snap > stuckUp) outDistance = stuckDown; // As we usually move forward, prefer going closer to target
            else if (snap > stuckDown) outDistance = stuckDown;
            else outDistance = snap;
        }
        else
        {
            outDistance = cameraDistance;
        }

    }

    private static Vector3 NearestPointOnFiniteLine(Vector3 start, Vector3 end, Vector3 pnt)
    {
        var line = (end - start);
        var len = line.magnitude;
        line.Normalize();

        var v = pnt - start;
        var d = Vector3.Dot(v, line);
        d = Mathf.Clamp(d, 0f, len);
        return start + line * d;
    }

    private static void LineToLineIntersection(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, out Vector3 Pa, out Vector3 Pb)
    {
        Vector3 P13 = P3 - P1;
        Vector3 P43 = P4 - P3;
        Vector3 P21 = P2 - P1;

        float mua = (
            Vector3.Dot(P13, P43) * Vector3.Dot(P43, P21) -
            Vector3.Dot(P13, P21) * Vector3.Dot(P43, P43)) /
            (Vector3.Dot(P21, P21) * Vector3.Dot(P43, P43) -
            Vector3.Dot(P43, P21) * Vector3.Dot(P43, P21));

        float mub = (Vector3.Dot(P13, P43) + mua * Vector3.Dot(P43, P21)) /
            Vector3.Dot(P43, P43);

        Pa = P1 + mua * (P2 - P1);
        Pb = P3 + mub * (P4 - P3);
    }

    protected void FixedUpdate()
    {
        UpdatePlayerRotation(Time.fixedDeltaTime);
        
        clippingAvoidanceOffset = Mathf.Lerp(clippingAvoidanceOffset, desiredCameraOffset, clippingAvoidanceSpeedLerp);
    }

    protected void LateUpdate()
    {
        float delta = Time.deltaTime;

        if (constrainedCamera.playerControlled && !freeze)
        {
            float deltaYaw = Input.GetAxis("Mouse X") * mouseHorizontalSensitivity;
            float deltaPitch = Input.GetAxis("Mouse Y") * mouseVerticalSensitivity
                * (mouseVerticalInversion
                ? -1
                : 1);

            constrainedCamera.Yaw += deltaYaw;
            constrainedCamera.Pitch += deltaPitch;

            /*
             * Rotation during transition can denormalize angle
             */
            transitionDeltaYaw += deltaYaw;
            transitionDeltaPitch += deltaPitch;
        }

        float targetPitch = cameraTransition < 1.0f
            ? transitionStartPitch + transitionDeltaPitch
            : constrainedCamera.Pitch;

        float targetYaw = cameraTransition < 1.0f
            ? transitionStartYaw + transitionDeltaYaw
            : constrainedCamera.Yaw;

        Vector3 position = Vector3Extensions.SmoothStep(transitionStartPosition, constrainedCamera.Position, cameraTransition);
        camera.transform.rotation = Quaternion.Euler(
            Mathf.SmoothStep(transitionStartPitch, targetPitch, cameraTransition),
            Mathf.SmoothStep(transitionStartYaw, targetYaw, cameraTransition),
            0.0f);
        camera.fieldOfView = Mathf.Lerp(transitionStartFOV, constrainedCamera.constraintFieldOfView, cameraTransition);

        cameraTransition = Mathf.MoveTowards(cameraTransition, 1.0f, delta * transitionSpeed);

        // Prevent camera clipping by moving camera position

        {
            Vector3 forward = (constrainedCamera.Target - position).normalized;

            Vector3 dir1 = Quaternion.Euler(clippingAvoidanceSmoothAngle, 0.0f, 0.0f) * forward;
            Vector3 dir2 = Quaternion.Euler(-clippingAvoidanceSmoothAngle, 0.0f, 0.0f) * forward;
            Vector3 dir3 = Quaternion.Euler(0.0f, clippingAvoidanceSmoothAngle, 0.0f) * forward;
            Vector3 dir4 = Quaternion.Euler(0.0f, -clippingAvoidanceSmoothAngle, 0.0f) * forward;

            float distance = 100.0f;
            float minDistance = 0.0f;
            Vector3 target = camera.transform.position + forward * distance;
            if (constrainedCamera.constraintTarget)
            {
                // keep at constraintMinimumDistanceToTarget distance from constraintTarget
                target = constrainedCamera.Target;

                distance = Vector3.Distance(position, target);
                minDistance = constrainedCamera.constraintMinimumDistanceToTarget;
            }

            float d1, s1;
            float d2, s2;
            float d3, s3;
            float d4, s4;
            ClipAnalyze(out d1, out s1, target, dir1, distance, forward);
            ClipAnalyze(out d2, out s2, target, dir2, distance, forward);
            ClipAnalyze(out d3, out s3, target, dir3, distance, forward);
            ClipAnalyze(out d4, out s4, target, dir4, distance, forward);

            var dist = d1;
            dist = Mathf.Min(dist, d2);
            dist = Mathf.Min(dist, d3);
            dist = Mathf.Min(dist, d4);

            var snap = s1;
            snap = Mathf.Min(snap, s2);
            snap = Mathf.Min(snap, s3);
            snap = Mathf.Min(snap, s4);

            desiredCameraOffset = Mathf.Min(distance - minDistance, distance - dist);
            var snapCompensation = 0.0f;
            if (snap < float.MaxValue)
            {
                snapCompensation = Mathf.Min(distance - minDistance, distance - snap);
            }

            if (clippingAvoidanceInstantSnap && clippingAvoidanceOffset < snapCompensation)
            {
                clippingAvoidanceOffset = snapCompensation;
            }

            clippingAvoidanceOffset = Mathf.MoveTowards(clippingAvoidanceOffset, desiredCameraOffset, clippingAvoidanceSpeed * delta);

            position += forward * clippingAvoidanceOffset;
        }

        camera.transform.position = position;
    }

    /*
     * Calling this forces player to rotate towards camera when in Action state
     */
    public void TriggerPlayerAutoRotation()
    {
        playerAutoRotateActive = true;
    }

    public void ChangeCamera(ConstrainedCamera otherCamera, bool preserveRotation = false, bool instant = false)
    {
        if (constrainedCamera == otherCamera) return;
        Debug.Assert(otherCamera);

        if (preserveRotation && constrainedCamera)
        {
            otherCamera.Pitch = constrainedCamera.Pitch;
            otherCamera.Yaw = constrainedCamera.Yaw;
        }

        transitionStartFOV = camera.fieldOfView;
        transitionStartYaw = camera.transform.eulerAngles.y;
        transitionStartPitch = camera.transform.eulerAngles.x;
        transitionStartPosition = camera.transform.position;

        transitionDeltaYaw = MathfExtensions.NormalizeAngle(otherCamera.Yaw - transitionStartYaw);
        transitionDeltaPitch = MathfExtensions.NormalizeAngle(otherCamera.Pitch - transitionStartPitch);
        
        cameraTransition = instant
            ? 1.0f
            : 0.0f;
        constrainedCamera = otherCamera;
    }

}
