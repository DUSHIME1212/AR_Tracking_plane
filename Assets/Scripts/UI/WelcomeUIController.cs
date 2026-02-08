using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;


/// Controls the welcome screen UI with interactive slider and transition to AR
/// Demonstrates non-button interactive UI element (instruction slider)

public class WelcomeUIController : MonoBehaviour
{
    [Header("UI Panel References")]
    [Tooltip("The main welcome panel that will be hidden when AR starts")]
    [SerializeField] private GameObject welcomePanel;

    [Tooltip("The game canvas with color selector - will be shown after start")]
    [SerializeField] private GameObject gameCanvas;

    [Header("Button References")]
    [Tooltip("Button to start the AR experience")]
    [SerializeField] private Button startButton;

    [Header("Interactive Slider (Non-Button Element)")]
    [Tooltip("Interactive slider to browse through instructions")]
    [SerializeField] private Slider instructionSlider;

    [Header("Text Display References")]
    [Tooltip("Text that displays current instruction based on slider value")]
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("AR Component References")]
    [Tooltip("AR Plane Manager - will be disabled until user starts")]
    [SerializeField] private ARPlaneManager arPlaneManager;

    [Tooltip("AR Raycast Manager - will be disabled until user starts")]
    [SerializeField] private ARRaycastManager arRaycastManager;

    // Array of instructions that user can browse through with slider
    private string[] instructions = new string[]
    {
        "📱 Point your camera at a flat surface like a table or floor",
        "👆 Tap the screen once to place your object on the detected plane",
        "🤏 Use pinch gesture (two fingers) to scale the object bigger or smaller",
        "🔄 Use twist gesture (two fingers rotating) to rotate the object",
        "🎨 Use the color buttons to change the object's appearance"
    };

    /// Initializes the welcome UI and sets up listeners
  
    private void Start()
    {
        // Disable AR tracking until user is ready
        DisableARTracking();

        // Ensure welcome panel is visible
        if (welcomePanel != null)
        {
            welcomePanel.SetActive(true);
        }

        // Game canvas can stay visible (color buttons won't work until object spawns anyway)
        // Or you can hide it initially:
        if (gameCanvas != null)
        {
            gameCanvas.SetActive(true); // Keep visible or set to false to hide initially
        }

        // Setup button click listener
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogError("Start Button not assigned in WelcomeUIController!");
        }

        // Setup slider listener for interactive instruction browsing
        if (instructionSlider != null)
        {
            instructionSlider.minValue = 0;
            instructionSlider.maxValue = instructions.Length - 1;
            instructionSlider.wholeNumbers = true;
            instructionSlider.value = 0;
            instructionSlider.onValueChanged.AddListener(OnSliderValueChanged);

            // Display first instruction
            UpdateInstructionDisplay(0);
        }
        else
        {
            Debug.LogError("Instruction Slider not assigned in WelcomeUIController!");
        }

        Debug.Log("Welcome UI initialized successfully");
    }

    
    /// Disables AR components to prevent tracking before user starts
    /// Saves battery and prevents unwanted plane detection
   
    private void DisableARTracking()
    {
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = false;
            Debug.Log("AR Plane Manager disabled");
        }

        if (arRaycastManager != null)
        {
            arRaycastManager.enabled = false;
            Debug.Log("AR Raycast Manager disabled");
        }
    }

    
    /// Enables AR components when user starts the experience
   
    private void EnableARTracking()
    {
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = true;
            Debug.Log("AR Plane Manager enabled - starting plane detection");
        }

        if (arRaycastManager != null)
        {
            arRaycastManager.enabled = true;
            Debug.Log("AR Raycast Manager enabled - ready for tap input");
        }
    }

    
    /// Called when slider value changes
    /// Updates the instruction text to match selected instruction
    /// This demonstrates the required "non-button interactive UI element"
   
    /// <param name="sliderValue">Current value of the slider (0 to instructions.Length-1)</param>
    private void OnSliderValueChanged(float sliderValue)
    {
        int instructionIndex = Mathf.RoundToInt(sliderValue);
        UpdateInstructionDisplay(instructionIndex);

        Debug.Log($"Slider moved to instruction {instructionIndex + 1} of {instructions.Length}");
    }

    
    /// Updates the instruction text display based on current index
   
    /// <param name="index">Index of instruction to display (0-based)</param>
    private void UpdateInstructionDisplay(int index)
    {
        if (instructionText != null && index >= 0 && index < instructions.Length)
        {
            instructionText.text = instructions[index];
        }
        else
        {
            Debug.LogWarning($"Invalid instruction index: {index}");
        }
    }

    
    /// Called when Start Button is clicked
    /// Hides welcome UI and starts AR experience
   
    private void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked - transitioning to AR experience");

        // Hide the welcome panel
        if (welcomePanel != null)
        {
            welcomePanel.SetActive(false);
            Debug.Log("Welcome panel hidden");
        }

        // Make sure game canvas is visible (if you hid it initially)
        if (gameCanvas != null)
        {
            gameCanvas.SetActive(true);
            Debug.Log("Game canvas shown");
        }

        // Enable AR tracking
        EnableARTracking();
    }


    /// Cleanup when script is destroyed
    /// Removes all event listeners to prevent memory leaks
 
    private void OnDestroy()
    {
        // Remove button listener
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        // Remove slider listener
        if (instructionSlider != null)
        {
            instructionSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        Debug.Log("Welcome UI Controller cleaned up");
    }
}