using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


/// Manages color selection UI and applies colors to spawned object
/// Provides real-time color changes via button interface

public class ColorController : MonoBehaviour
{
    [Header("UI Button References")]
    [Tooltip("Button to change object to red")]
    [SerializeField] private Button redButton;

    [Tooltip("Button to change object to green")]
    [SerializeField] private Button greenButton;

    [Tooltip("Button to change object to blue")]
    [SerializeField] private Button blueButton;

    [Tooltip("Button to change object to yellow")]
    [SerializeField] private Button yellowButton;

    [Header("Object Spawner Reference")]
    [Tooltip("Reference to ObjectSpawner to get the spawned object")]
    [SerializeField] private ObjectSpawner objectSpawner;

    /// Sets up button click listeners
 
    private void Start()
    {
        // Try to auto-link missing reference
        if (objectSpawner == null)
        {
            objectSpawner = FindFirstObjectByType<ObjectSpawner>();
            if (objectSpawner != null) 
                Debug.Log("Auto-linked ObjectSpawner to ColorController");
            else
                Debug.LogError("Object Spawner not found in scene for ColorController!");
        }

        // Setup button listeners
        if (redButton != null)
            redButton.onClick.AddListener(() => ChangeColor(Color.red, "Red"));
        else
            Debug.LogError("Red Button not assigned!");

        if (greenButton != null)
            greenButton.onClick.AddListener(() => ChangeColor(Color.green, "Green"));
        else
            Debug.LogError("Green Button not assigned!");

        if (blueButton != null)
            blueButton.onClick.AddListener(() => ChangeColor(Color.blue, "Blue"));
        else
            Debug.LogError("Blue Button not assigned!");

        if (yellowButton != null)
            yellowButton.onClick.AddListener(() => ChangeColor(Color.yellow, "Yellow"));
        else
            Debug.LogError("Yellow Button not assigned!");

        Debug.Log("Color Controller initialized");
    }

   
    /// Changes the color of the spawned object's material
   
    /// <param name="newColor">The color to apply</param>
    /// <param name="colorName">Name of color for debug logging</param>
    private void ChangeColor(Color newColor, string colorName)
    {
        // 1. Change color of spawned object (existing logic)
        ApplyColorToSpawnedObject(newColor);

        // 2. Change color of AR planes
        ApplyColorToARPlanes(newColor);

        Debug.Log($"✓ Color changed to {colorName}.");
    }

    private void ApplyColorToSpawnedObject(Color newColor)
    {
        if (objectSpawner == null)
        {
            Debug.LogWarning("Cannot change color: Object Spawner is null");
            return;
        }

        if (!objectSpawner.HasSpawned())
        {
            Debug.Log("No object spawned yet. Place an object first!");
            return;
        }

        GameObject spawnedObject = objectSpawner.GetSpawnedObject();
        if (spawnedObject == null)
        {
            Debug.LogWarning("Spawned object reference is null");
            return;
        }

        Renderer[] renderers = spawnedObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("No Renderer found on spawned object or its children!");
            return;
        }

        int renderersChanged = 0;
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = newColor;
                renderersChanged++;
            }
        }
        Debug.Log($"Updated {renderersChanged} material(s) on spawned object.");
    }

    private void ApplyColorToARPlanes(Color newColor)
    {
        // Find all active ARPlane objects in the scene
        ARPlane[] planes = FindObjectsByType<ARPlane>(FindObjectsSortMode.None);
        
        foreach (ARPlane plane in planes)
        {
            Renderer[] renderers = plane.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = newColor;
                }
            }
        }
        
        Debug.Log($"Updated color for {planes.Length} planes.");
    }

    /// Cleanup when destroyed

    private void OnDestroy()
    {
        // Remove all listeners
        if (redButton != null)
            redButton.onClick.RemoveAllListeners();

        if (greenButton != null)
            greenButton.onClick.RemoveAllListeners();

        if (blueButton != null)
            blueButton.onClick.RemoveAllListeners();

        if (yellowButton != null)
            yellowButton.onClick.RemoveAllListeners();
    }
}