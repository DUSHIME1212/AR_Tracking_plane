using UnityEngine;

/// Handles touch gestures for manipulating the spawned AR object
/// Implements pinch-to-scale and twist-to-rotate functionality

public class ObjectManipulator : MonoBehaviour
{
    [Header("Manipulation Settings")]
    [Tooltip("Speed of scaling (smaller = slower, more precise)")]
    [SerializeField] private float scaleSpeed = 0.01f;

    [Tooltip("Speed of rotation (smaller = slower, more precise)")]
    [SerializeField] private float rotationSpeed = 0.3f;

    [Header("Scale Limits")]
    [Tooltip("Minimum scale of object")]
    [SerializeField] private float minScale = 0.1f;

    [Tooltip("Maximum scale of object")]
    [SerializeField] private float maxScale = 5.0f;

    [Header("Object Spawner Reference")]
    [Tooltip("Reference to get the spawned object")]
    [SerializeField] private ObjectSpawner objectSpawner;

    // Touch tracking variables
    private float previousTouchDistance = 0f;
    private float previousTouchAngle = 0f;
    private GameObject targetObject = null;


    /// Validates references
 
    private void Start()
    {
        if (objectSpawner == null)
        {
            Debug.LogError("Object Spawner not assigned in ObjectManipulator!");
        }

        Debug.Log("Object Manipulator initialized. Use two fingers to scale and rotate.");
    }


    /// Checks for two-finger touch input every frame

    private void Update()
    {
        // Get reference to spawned object
        if (objectSpawner != null && objectSpawner.HasSpawned())
        {
            targetObject = objectSpawner.GetSpawnedObject();
        }
        else
        {
            targetObject = null;
            return; // No object to manipulate yet
        }

        // Check if we have exactly 2 touches (required for gestures)
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Handle pinch to scale
            HandlePinchToScale(touch0, touch1);

            // Handle twist to rotate
            HandleTwistToRotate(touch0, touch1);
        }
        else
        {
            // Reset tracking when not using two fingers
            previousTouchDistance = 0f;
            previousTouchAngle = 0f;
        }
    }

 
    /// Handles pinch gesture to scale the object
    /// Pinching fingers together = smaller
    /// Spreading fingers apart = larger
   
    /// <param name="touch0">First touch point</param>
    /// <param name="touch1">Second touch point</param>
    private void HandlePinchToScale(Touch touch0, Touch touch1)
    {
        if (targetObject == null) return;

        // Calculate current distance between the two touches
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);

        // Initialize on first frame of two-finger touch
        if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
        {
            previousTouchDistance = currentDistance;
            return;
        }

        // Only scale when fingers are moving
        if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
        {
            if (previousTouchDistance > 0)
            {
                // Calculate how much the distance changed
                float distanceDelta = currentDistance - previousTouchDistance;

                // Calculate scale multiplier
                float scaleFactor = 1.0f + (distanceDelta * scaleSpeed);

                // Apply scale to object
                Vector3 newScale = targetObject.transform.localScale * scaleFactor;

                // Clamp scale to min/max limits
                float scaleValue = Mathf.Clamp(newScale.x, minScale, maxScale);
                newScale = Vector3.one * scaleValue;

                // Apply the new scale
                targetObject.transform.localScale = newScale;

                // Debug feedback
                if (Mathf.Abs(distanceDelta) > 1f) // Only log significant changes
                {
                    Debug.Log($"Scaling: {scaleValue:F2}");
                }
            }

            // Update for next frame
            previousTouchDistance = currentDistance;
        }
    }

   
    /// Handles twist gesture to rotate the object
    /// Rotating two fingers clockwise/counter-clockwise rotates object
   
    /// <param name="touch0">First touch point</param>
    /// <param name="touch1">Second touch point</param>
    private void HandleTwistToRotate(Touch touch0, Touch touch1)
    {
        if (targetObject == null) return;

        // Calculate the angle between the two touches
        Vector2 touchDelta = touch1.position - touch0.position;
        float currentAngle = Mathf.Atan2(touchDelta.y, touchDelta.x) * Mathf.Rad2Deg;

        // Initialize on first frame of two-finger touch
        if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
        {
            previousTouchAngle = currentAngle;
            return;
        }

        // Only rotate when fingers are moving
        if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
        {
            // Calculate angle change
            float angleDelta = Mathf.DeltaAngle(previousTouchAngle, currentAngle);

            // Apply rotation around Y axis (vertical axis)
            // Negative because we want natural rotation direction
            targetObject.transform.Rotate(Vector3.up, -angleDelta * rotationSpeed, Space.World);

            // Debug feedback
            if (Mathf.Abs(angleDelta) > 1f) // Only log significant changes
            {
                Debug.Log($"Rotating: {angleDelta:F1}°");
            }

            // Update for next frame
            previousTouchAngle = currentAngle;
        }
    }


    /// Optional: Reset object to original scale and rotation
    /// Can be called from a UI button if desired

    public void ResetTransform()
    {
        if (targetObject != null)
        {
            targetObject.transform.localScale = Vector3.one * 0.2f; // Reset to default
            targetObject.transform.rotation = Quaternion.identity; // Reset rotation
            Debug.Log("Object transform reset");
        }
    }
}