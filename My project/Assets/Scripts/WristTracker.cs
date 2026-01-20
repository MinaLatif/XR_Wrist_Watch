using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;

/// <summary>
/// Tracks the left wrist joint using XR Hands and positions the watch transform accordingly.
/// Optionally shows watch only when palm faces up (like looking at a real watch).
/// </summary>
public class WristTracker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRHandTrackingEvents handTrackingEvents;
    [SerializeField] private Transform watchTransform;
    [SerializeField] private Transform headTransform;

    [Header("Positioning")]
    [Tooltip("Offset from wrist joint in local space (Y = up from wrist)")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 0.02f, 0f);

    [Tooltip("Rotation offset to face watch upward toward user")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(-90f, 0f, 180f);

    [Header("Palm Detection (VR Mode)")]
    [Tooltip("Only show watch when palm faces up (like looking at a real watch)")]
    [SerializeField] private bool requirePalmUp = false;

    [Tooltip("Angle threshold in degrees. Palm must be within this angle of 'up' to show watch")]
    [Range(15f, 90f)]
    [SerializeField] private float palmUpThreshold = 45f;

    [Tooltip("Also check if wrist is raised toward head level")]
    [SerializeField] private bool requireWristRaised = false;

    [Tooltip("Minimum height relative to head to consider wrist 'raised' (negative = below head)")]
    [SerializeField] private float minWristHeight = -0.3f;

    [Header("Smoothing")]
    [Tooltip("Enable smoothing to reduce jitter")]
    [SerializeField] private bool enableSmoothing = true;

    [Tooltip("Smoothing speed (higher = faster, less smooth)")]
    [SerializeField] private float smoothSpeed = 20f;

    [Header("Events")]
    public UnityEvent onTrackingAcquired;
    public UnityEvent onTrackingLost;
    public UnityEvent onWatchBecameVisible;
    public UnityEvent onWatchBecameHidden;

    private bool isTracking;
    private bool isWatchVisible;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool hasInitialPose;

    public bool IsTracking => isTracking;
    public bool IsWatchVisible => isWatchVisible;
    public bool RequirePalmUp { get => requirePalmUp; set => requirePalmUp = value; }

    private void OnEnable()
    {
        if (handTrackingEvents == null)
        {
            Debug.LogError("WristTracker: XRHandTrackingEvents reference is not set!");
            return;
        }

        handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
        handTrackingEvents.trackingAcquired.AddListener(OnTrackingAcquired);
        handTrackingEvents.trackingLost.AddListener(OnTrackingLost);
    }

    private void OnDisable()
    {
        if (handTrackingEvents == null) return;

        handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
        handTrackingEvents.trackingAcquired.RemoveListener(OnTrackingAcquired);
        handTrackingEvents.trackingLost.RemoveListener(OnTrackingLost);
    }

    private void Update()
    {
        if (!isTracking || watchTransform == null || !hasInitialPose) return;

        if (enableSmoothing)
        {
            // Smoothly interpolate to target pose
            watchTransform.position = Vector3.Lerp(
                watchTransform.position,
                targetPosition,
                Time.deltaTime * smoothSpeed
            );
            watchTransform.rotation = Quaternion.Slerp(
                watchTransform.rotation,
                targetRotation,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    private void OnJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        if (watchTransform == null) return;

        // Get wrist and palm joints from the hand
        XRHandJoint wristJoint = args.hand.GetJoint(XRHandJointID.Wrist);
        XRHandJoint palmJoint = args.hand.GetJoint(XRHandJointID.Palm);

        // Check if pose data is valid
        if (wristJoint.TryGetPose(out Pose wristPose))
        {
            // Calculate target position with offset applied in wrist's local space
            targetPosition = wristPose.position + wristPose.rotation * positionOffset;

            // Calculate target rotation with offset to face watch upward
            targetRotation = wristPose.rotation * Quaternion.Euler(rotationOffset);

            if (!enableSmoothing)
            {
                // Apply immediately without smoothing
                watchTransform.SetPositionAndRotation(targetPosition, targetRotation);
            }

            hasInitialPose = true;

            // Check palm orientation if required
            if (requirePalmUp)
            {
                bool shouldShowWatch = CheckPalmUpCondition(palmJoint, wristPose);
                UpdateWatchVisibility(shouldShowWatch);
            }
            else if (!isWatchVisible)
            {
                // If palm detection not required, always show watch when tracking
                UpdateWatchVisibility(true);
            }
        }
    }

    private bool CheckPalmUpCondition(XRHandJoint palmJoint, Pose wristPose)
    {
        // Get palm orientation
        if (!palmJoint.TryGetPose(out Pose palmPose))
        {
            // Fallback to wrist pose if palm not available
            palmPose = wristPose;
        }

        // Palm "up" direction is the local Y axis of the palm
        // For left hand, when palm faces up, the palm's up vector points toward ceiling
        Vector3 palmUp = palmPose.rotation * Vector3.up;

        // Calculate angle between palm up and world up
        float angleFromUp = Vector3.Angle(palmUp, Vector3.up);

        // Check if palm is facing up within threshold
        bool isPalmUp = angleFromUp <= palmUpThreshold;

        // Optionally check if wrist is raised toward head
        if (requireWristRaised && headTransform != null)
        {
            float wristHeightRelativeToHead = wristPose.position.y - headTransform.position.y;
            bool isWristRaised = wristHeightRelativeToHead >= minWristHeight;
            return isPalmUp && isWristRaised;
        }

        return isPalmUp;
    }

    private void UpdateWatchVisibility(bool visible)
    {
        if (visible != isWatchVisible)
        {
            isWatchVisible = visible;

            if (visible)
            {
                onWatchBecameVisible?.Invoke();
            }
            else
            {
                onWatchBecameHidden?.Invoke();
            }
        }
    }

    private void OnTrackingAcquired()
    {
        isTracking = true;
        hasInitialPose = false;
        onTrackingAcquired?.Invoke();
    }

    private void OnTrackingLost()
    {
        isTracking = false;
        onTrackingLost?.Invoke();
    }

    /// <summary>
    /// Manually set the hand tracking events reference (useful for runtime setup).
    /// </summary>
    public void SetHandTrackingEvents(XRHandTrackingEvents events)
    {
        if (handTrackingEvents != null)
        {
            handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
            handTrackingEvents.trackingAcquired.RemoveListener(OnTrackingAcquired);
            handTrackingEvents.trackingLost.RemoveListener(OnTrackingLost);
        }

        handTrackingEvents = events;

        if (handTrackingEvents != null && enabled)
        {
            handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
            handTrackingEvents.trackingAcquired.AddListener(OnTrackingAcquired);
            handTrackingEvents.trackingLost.AddListener(OnTrackingLost);
        }
    }
}
