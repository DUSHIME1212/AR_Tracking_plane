using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Handles tap-to-place functionality for spawning objects on AR planes
/// CRITICAL REQUIREMENT: Only ONE object can be spawned per session
/// After spawning, plane detection is automatically disabled

/// Assignment: AR Plane Tracking - Object Spawning
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [Tooltip("The prefab to instantiate when user taps on a detected plane")]
    [SerializeField] private GameObject objectToSpawn;

    [Header("AR Manager References")]
    [Tooltip("AR Raycast Manager for detecting tap positions on planes")]
    [SerializeField] private ARRaycastManager raycastManager;

    [Tooltip("AR Plane Manager to disable after spawning")]
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Spawn Status (Read Only)")]
    [Tooltip("Reference to the spawned object instance")]
    [SerializeField] private GameObject spawnedObjectInstance;

    [Tooltip("Has an object been spawned yet?")]
    [SerializeField] private bool hasSpawned = false;

    // List to store raycast hit results
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

    /// <summary>
    /// Validates required references on start
    /// </summary>
    private void Start()
    {
        // Validate references
        if (objectToSpawn == null)
        {
            Debug.LogError("Object To Spawn is not assigned! Please assign a prefab in the Inspector.");
        }

        if (raycastManager == null)
        {
            Debug.LogError("AR Raycast Manager is not assigned!");
        }

        if (planeManager == null)
        {
            Debug.LogError("AR Plane Manager is not assigned!");
        }

        Debug.Log("Object Spawner initialized. Tap on a detected plane to place object.");
    }

    /// <summary>
    /// Checks for touch input every frame
    /// Only processes input if object hasn't been spawned yet
    /// </summary>
    private void Update()
    {
        // Don't process input if object already spawned
        if (hasSpawned)
        {
            return;
        }

        // Check for touch input (mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Only spawn on initial touch (not while dragging)
            if (touch.phase == TouchPhase.Began)
            {
                TrySpawnObject(touch.position);
            }
        }

        // Editor testing with mouse (only works in Unity Editor)
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            TrySpawnObject(Input.mousePosition);
        }
#endif
    }

    /// <summary>
    /// Attempts to spawn object at the touch/click position
    /// Uses AR raycasting to find position on detected planes
    /// </summary>
    /// <param name="screenPosition">Screen position of touch/click</param>
    private void TrySpawnObject(Vector2 screenPosition)
    {
        // Validate raycast manager
        if (raycastManager == null)
        {
            Debug.LogError("Cannot spawn: Raycast Manager is null");
            return;
        }

        // Perform AR raycast from screen position
        // TrackableType.PlaneWithinPolygon ensures we hit detected planes
        bool hitDetected = raycastManager.Raycast(
            screenPosition,
            raycastHits,
            TrackableType.PlaneWithinPolygon
        );

        if (hitDetected && raycastHits.Count > 0)
        {
            // Get the first (closest) hit point
            ARRaycastHit hit = raycastHits[0];

            // Get position and rotation from hit
            Pose hitPose = hit.pose;

            // Spawn the object
            SpawnObjectAtPose(hitPose);
        }
        else
        {
            Debug.Log("No plane detected at tap position. Try tapping on a detected surface.");
        }
    }

    /// <summary>
    /// Spawns the object at the specified pose and disables further spawning
    /// </summary>
    /// <param name="pose">Position and rotation where object should be spawned</param>
    private void SpawnObjectAtPose(Pose pose)
    {
        // Validate prefab
        if (objectToSpawn == null)
        {
            Debug.LogError("Cannot spawn: Object To Spawn prefab is not assigned!");
            return;
        }

        // Instantiate the object
        spawnedObjectInstance = Instantiate(
            objectToSpawn,
            pose.position,
            pose.rotation
        );

        // Mark as spawned
        hasSpawned = true;

        Debug.Log($"✓ Object spawned at position: {pose.position}");
        Debug.Log("✓ Object spawning disabled - only one object allowed per session");

        // Disable plane detection
        DisablePlaneDetection();
    }

    /// <summary>
    /// Disables AR plane detection and hides all existing planes
    /// Called after object is successfully spawned
    /// Saves battery and improves performance
    /// </summary>
    private void DisablePlaneDetection()
    {
        if (planeManager == null)
        {
            Debug.LogWarning("Cannot disable plane detection: Plane Manager is null");
            return;
        }

        // Disable the plane manager (stops detecting new planes)
        planeManager.enabled = false;

        // Hide all currently detected planes
        int planesHidden = 0;
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
            planesHidden++;
        }

        Debug.Log($"✓ Plane detection disabled. {planesHidden} planes hidden.");
    }

    /// <summary>
    /// Public method to check if object has been spawned
    /// Used by other scripts (like ColorController)
    /// </summary>
    /// <returns>True if object has been spawned, false otherwise</returns>
    public bool HasSpawned()
    {
        return hasSpawned;
    }

    /// <summary>
    /// Public method to get reference to spawned object
    /// Used by other scripts to manipulate the object
    /// </summary>
    /// <returns>The spawned GameObject, or null if nothing spawned yet</returns>
    public GameObject GetSpawnedObject()
    {
        return spawnedObjectInstance;
    }

    /// <summary>
    /// Optional: Reset spawning (useful for testing)
    /// Remove this method in final build if not needed
    /// </summary>
    public void ResetSpawning()
    {
        if (spawnedObjectInstance != null)
        {
            Destroy(spawnedObjectInstance);
            spawnedObjectInstance = null;
        }

        hasSpawned = false;

        if (planeManager != null)
        {
            planeManager.enabled = true;

            foreach (ARPlane plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(true);
            }
        }

        Debug.Log("Spawning reset - ready to spawn again");
    }
}