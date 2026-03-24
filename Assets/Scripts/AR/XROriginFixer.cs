using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;
using System.Reflection;

/// <summary>
/// Diagnostic script to help resolve XR Origin camera reference issues.
/// Uses Reflection to avoid compilation errors if CoreUtils is missing.
/// </summary>
public class XROriginFixer : MonoBehaviour
{
    private void Awake()
    {
        FixXROrigin();
    }

    [ContextMenu("Fix XR Origin")]
    public void FixXROrigin()
    {
        // Find any component with "XROrigin" in its name
        Component origin = FindOriginComponent();
        
        if (origin == null)
        {
            Debug.LogError("[XROriginFixer] No XR Origin or AR Session Origin found in scene!");
            return;
        }

        Debug.Log($"[XROriginFixer] Found origin component: {origin.GetType().Name} on {origin.gameObject.name}");

        // Try to find Camera property via Reflection
        PropertyInfo cameraProp = origin.GetType().GetProperty("Camera");
        if (cameraProp == null) cameraProp = origin.GetType().GetProperty("camera"); // Try lowercase

        if (cameraProp != null)
        {
            Camera currentCam = cameraProp.GetValue(origin) as Camera;
            if (currentCam == null)
            {
                Debug.LogWarning("[XROriginFixer] Origin has no camera assigned. Searching in children...");
                Camera childCam = origin.GetComponentInChildren<Camera>();
                if (childCam != null)
                {
                    cameraProp.SetValue(origin, childCam);
                    Debug.Log($"[XROriginFixer] ✓ Automatically assigned camera '{childCam.name}' to origin.");
                    currentCam = childCam;
                }
            }

            // Setup AR components if we have a camera
            if (currentCam != null)
            {
                SetupARComponents(currentCam);
            }
        }
        else
        {
            Debug.LogError("[XROriginFixer] Could not find 'Camera' property on the origin component.");
        }
    }

    private Component FindOriginComponent()
    {
        // Try to find by type name to avoid direct dependency
        foreach (var component in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            string typeName = component.GetType().Name;
            if (typeName == "XROrigin" || typeName == "ARSessionOrigin")
            {
                return component;
            }
        }
        return null;
    }

    private void SetupARComponents(Camera cam)
    {
        if (cam.GetComponent<ARCameraManager>() == null)
        {
            Debug.Log("[XROriginFixer] Adding ARCameraManager to " + cam.name);
            cam.gameObject.AddComponent<ARCameraManager>();
        }
        
        if (cam.GetComponent<ARCameraBackground>() == null)
        {
            Debug.Log("[XROriginFixer] Adding ARCameraBackground to " + cam.name);
            cam.gameObject.AddComponent<ARCameraBackground>();
        }

        if (!cam.CompareTag("MainCamera"))
        {
            Debug.LogWarning($"[XROriginFixer] Camera '{cam.name}' is NOT tagged as 'MainCamera'. Clicks might fail.");
        }
    }
}
