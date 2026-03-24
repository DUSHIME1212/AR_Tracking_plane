using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Diagnostic script to identify why UI buttons are not clickable.
/// Attach this to a GameObject in your scene.
/// </summary>
public class UIDiagnostic : MonoBehaviour
{
    private void Update()
    {
        // Check for click/touch
        if (Input.GetMouseButtonDown(0))
        {
            CheckWhatWasClicked();
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("[UIDiagnostic] Persistence enabled. I will survive scene changes.");
    }

    private void CheckWhatWasClicked()
    {
        Debug.Log($"[UIDiagnostic] Screen Click detected at: {Input.mousePosition}");

        // 1. Check if EventSystem exists
        if (EventSystem.current == null)
        {
            Debug.LogError("[UIDiagnostic] ❌ NO EventSystem! UI WILL NOT WORK.");
            return;
        }

        // 2. Perform Raycast
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            Debug.Log($"[UIDiagnostic] HIT {results.Count} UI elements:");
            for (int i = 0; i < results.Count; i++)
            {
                GameObject obj = results[i].gameObject;
                string blockReason = GetBlockReason(obj);
                Debug.Log($"   [{i}] {obj.name} - {blockReason}");
            }
        }
        else
        {
            Debug.Log("[UIDiagnostic] ⚠️ Click hit NOTHING in the UI. Is there a GraphicRaycaster on your Canvas?");
        }
    }

    private string GetBlockReason(GameObject obj)
    {
        Button btn = obj.GetComponentInParent<Button>();
        if (btn != null)
        {
            if (!btn.interactable) return "Button is NOT interactable.";
            return $"Part of button '{btn.name}'. Should be clickable.";
        }

        Graphic g = obj.GetComponent<Graphic>();
        if (g != null && g.raycastTarget)
        {
            return "This graphic is a 'Raycast Target' and might be BLOCKING elements behind it.";
        }

        return "Unknown blocking element.";
    }

    private void Start()
    {
        Debug.Log("[UIDiagnostic] UI Diagnostic Tool Started. Click anywhere to see what UI elements are hit.");
        
        // Final sanity check on hierarchies
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in canvases)
        {
            if (c.GetComponent<GraphicRaycaster>() == null)
            {
                Debug.LogError($"[UIDiagnostic] ❌ Canvas '{c.name}' is missing a GraphicRaycaster! Buttons on this canvas will not work.");
            }
        }
    }
}
