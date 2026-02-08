using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Visualizes AR planes with custom FBX model and texture showing student name
/// Tracking Type: Horizontal and Vertical planes
/// Author: [YOUR FULL NAME HERE]
/// Date: February 2025
/// </summary>
[RequireComponent(typeof(ARPlane))]
public class CustomPlaneVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [Tooltip("The custom material to display on detected planes")]
    [SerializeField] private Material customPlaneMaterial;

    [Header("Optional FBX Model")]
    [Tooltip("Optional: Child FBX model for visual decoration")]
    [SerializeField] private GameObject fbxModel;

    private ARPlane arPlane;
    private MeshRenderer meshRenderer;

    /// <summary>
    /// Initializes components when script is loaded
    /// </summary>
    private void Awake()
    {
        arPlane = GetComponent<ARPlane>();
        meshRenderer = GetComponent<MeshRenderer>();

        // Apply material to main mesh renderer
        if (customPlaneMaterial != null && meshRenderer != null)
        {
            meshRenderer.material = customPlaneMaterial;
        }

        // Apply material to all child renderers (for FBX models)
        ApplyMaterialToChildren();

        Debug.Log($"Custom plane tracker initialized with FBX model");
    }

    /// <summary>
    /// Applies custom material to all child mesh renderers
    /// </summary>
    private void ApplyMaterialToChildren()
    {
        if (customPlaneMaterial == null) return;

        // Get all mesh renderers in children
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in childRenderers)
        {
            renderer.material = customPlaneMaterial;
        }

        Debug.Log($"Applied material to {childRenderers.Length} mesh renderers");
    }

    /// <summary>
    /// Subscribe to plane events when enabled
    /// </summary>
    private void OnEnable()
    {
        arPlane.boundaryChanged += OnPlaneBoundaryChanged;
    }

    /// <summary>
    /// Unsubscribe from events when disabled
    /// </summary>
    private void OnDisable()
    {
        arPlane.boundaryChanged -= OnPlaneBoundaryChanged;
    }

    /// <summary>
    /// Called when the AR plane boundary is updated
    /// </summary>
    private void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        // Optionally scale FBX model based on plane size
        if (fbxModel != null && arPlane != null)
        {
            // You can scale the FBX based on plane size if desired
            // Vector2 planeSize = arPlane.size;
            // fbxModel.transform.localScale = new Vector3(planeSize.x, 1, planeSize.y);
        }
    }
}