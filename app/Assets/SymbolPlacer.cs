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
        // Reload previously placed models
        foreach (var markerContent in ButtonHandler.contentByLocation)
        {
            foreach (ContentItem content in markerContent.Value)
            {
                if (content.prefab != null)
                {
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
    }

    public void Update()
    {
        if (symbolInstance == null || Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Move the symbol to the touch position
            symbolInstance.transform.position = hitPose.position;

            if (touch.phase == TouchPhase.Ended)
            {
                // Finalize placement
                PlaceSymbol(hitPose.position);
            }
        }
    }

    public void PlaceSymbol(Vector3 position)
    {
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
