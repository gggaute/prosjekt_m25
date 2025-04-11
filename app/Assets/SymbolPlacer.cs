using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class SymbolPlacer : MonoBehaviour
{
    public static string currentMarkerId; // The marker/place we in
    public static ContentItem currentContentItem; // The content being placed

    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private GameObject symbolInstance; // Instance of the prefab being placed

    public void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Start()
    {
        if (ButtonHandler.contentByLocation.ContainsKey(currentMarkerId))
        {
            foreach (ContentItem content in ButtonHandler.contentByLocation[currentMarkerId])
            {
                if (content.prefab != null)
                {
                    // Instantiate the prefab at the saved position
                    GameObject placedObject = Instantiate(content.prefab, content.position, Quaternion.identity);

                    // Assign the associated content to the ClickableObject script
                    ClickableObject clickable = placedObject.GetComponent<ClickableObject>();
                    if (clickable != null)
                    {
                        clickable.associatedContent = content;
                    }
                }
            }
        }
        // Check if there is a content item to place
        if (currentContentItem != null && currentContentItem.prefab != null)
        {
            // Instantiate the symbol prefab
            symbolInstance = Instantiate(currentContentItem.prefab);
            symbolInstance.SetActive(false); // Hide it until placement starts
        }
        else
        {
            Debug.LogError("No content item to place or prefab is missing!");
        }
    }

    public void Update()
    {
        if (symbolInstance == null || Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Adjust the position to be at camera level
            Vector3 adjustedPosition = hitPose.position;
            adjustedPosition.y = Mathf.Max(hitPose.position.y, Camera.main.transform.position.y);

            // Move the symbol to the adjusted position
            symbolInstance.transform.position = adjustedPosition;
            symbolInstance.SetActive(true);

            if (touch.phase == TouchPhase.Ended)
            {
                // Finalize placement
                PlaceSymbol(adjustedPosition);
            }
        }
    }

    public void PlaceSymbol(Vector3 position)
    {
        // Ensure the ARAnchorManager is available  
        ARAnchorManager anchorManager = Object.FindFirstObjectByType<ARAnchorManager>();
        if (anchorManager == null)
        {
            Debug.LogError("ARAnchorManager not found in the scene!");
            return;
        }

        // Create a new GameObject to act as the anchor  
        GameObject anchorObject = new GameObject("ARAnchor");
        anchorObject.transform.position = position;
        anchorObject.transform.rotation = Quaternion.identity;

        // Add an ARAnchor component to the GameObject  
        ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();
        if (anchor == null)
        {
            Debug.LogError("Failed to create ARAnchor!");
            return;
        }

        // Parent the symbol instance to the anchor  
        symbolInstance.transform.SetParent(anchorObject.transform, worldPositionStays: true);

        // Update the ContentItem's position  
        currentContentItem.position = position;

        // Add the content to the marker's list  
        if (!ButtonHandler.contentByLocation.ContainsKey(currentMarkerId))
        {
            ButtonHandler.contentByLocation[currentMarkerId] = new List<ContentItem>();
        }
        ButtonHandler.contentByLocation[currentMarkerId].Add(currentContentItem);

        Debug.Log($"Placed symbol at {position} for marker {currentMarkerId}");

        // Clear the current content item  
        currentContentItem = null;
        symbolInstance = null;
    }
}
