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

    private Queue<ContentItem> dummyQueue = new Queue<ContentItem>();
    private bool dummyPlaced = false;
    public static List<GameObject> spawnedARObjects = new List<GameObject>();

    public void Awake()
    {
        raycastManager = GetComponentInParent<ARRaycastManager>();
        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found on XR Origin or its children!");
        }
    }

    void Start()
    {
        // Clear any previously spawned AR objects
        foreach (GameObject obj in spawnedARObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedARObjects.Clear();

        // Queue up dummy content to be placed once plane is detected
        if (ButtonHandler.contentByLocation.ContainsKey(currentMarkerId))
        {
            foreach (ContentItem content in ButtonHandler.contentByLocation[currentMarkerId])
            {
                // We skip currentContentItem because that one is for user placement
                if (content != currentContentItem)
                {
                    dummyQueue.Enqueue(content);
                }
            }
        }

        // Instantiate all dummy content immediately
        if (dummyQueue.Count > 0)
        {
            Vector3 basePos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f; // Spawn near the camera
            int index = 0;

            while (dummyQueue.Count > 0)
            {
                ContentItem content = dummyQueue.Dequeue();

                // Offset each dummy a bit so they don't overlap
                Vector3 offset = new Vector3(index * 0.3f, 0, 0);
                Vector3 spawnPos = basePos + offset;

                GameObject obj = Instantiate(content.prefab, spawnPos, Quaternion.identity);
                spawnedARObjects.Add(obj);
                obj.SetActive(true); // Ensure the object is active

                // Optional: save this position back to the content (for future saving)
                content.position = spawnPos;

                ClickableObject clickable = obj.GetComponent<ClickableObject>();
                if (clickable != null)
                {
                    clickable.associatedContent = content;
                }

                index++;
            }

            dummyPlaced = true; // Mark dummy content as placed
        }

        // Prepare the symbol to be placed by user
        if (currentContentItem != null && currentContentItem.prefab != null)
        {
            symbolInstance = Instantiate(currentContentItem.prefab);
            symbolInstance.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f; // Spawn near the camera
            symbolInstance.SetActive(true); // Ensure the object is active
        }
        else
        {
            Debug.Log("No user-placed content to load");
        }
    }

    void Update()
    {
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // If user is placing a symbol
            if (symbolInstance != null)
            {
                // Make the symbol follow the touch or mouse position
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Vector3 adjustedPosition = hitPose.position;
                        adjustedPosition.y = Mathf.Max(hitPose.position.y, Camera.main.transform.position.y);

                        symbolInstance.transform.position = adjustedPosition;
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        PlaceSymbol(symbolInstance.transform.position);
                    }
                }
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
