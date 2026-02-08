using UnityEngine;
using UnityEngine.UI;


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
        // Validate references
        if (objectSpawner == null)
        {
            Debug.LogError("Object Spawner not assigned in ColorController!");
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
        // Check if object has been spawned
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

        // Get the spawned object
        GameObject spawnedObject = objectSpawner.GetSpawnedObject();

        if (spawnedObject == null)
        {
            Debug.LogWarning("Spawned object reference is null");
            return;
        }

        // Find renderer - check object and all children
        Renderer[] renderers = spawnedObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogError("No Renderer found on spawned object or its children!");
            return;
        }

        // Apply color to all renderers
        int renderersChanged = 0;
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = newColor;
                renderersChanged++;
            }
        }

        Debug.Log($"✓ Color changed to {colorName}. Updated {renderersChanged} material(s).");
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