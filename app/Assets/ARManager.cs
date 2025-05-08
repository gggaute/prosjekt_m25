using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

public class ARManager : MonoBehaviour
{
    public SceneUIController controller;
    public ARRaycastManager raycastManager;
    public Camera arCamera;
    [SerializeField] private ARAnchorManager anchorManager;

    private ContentItem itemToPlace;
    private string currentMarkerId;

    private bool isPlacing = false;

    private Dictionary<string, List<GameObject>> spawnedObjectsByMarker = new Dictionary<string, List<GameObject>>();


    public void BeginPlacingSymbol(ContentItem item, string markerId)
    {
        itemToPlace = item;
        currentMarkerId = markerId;
        isPlacing = true;

        controller.ShowCancelPlacement();

        // show placement message
        if (itemToPlace.prefab != null)
        {
            controller.ShowPlacementOverlay(itemToPlace); // Don't show until we get a hit
        }
    }

    private void Update()
    {
        if (raycastManager == null || arCamera == null)
            return;

        if (isPlacing)
        {
            HandlePlacement();
        }
        else
        {
            HandleTouchInteraction(); // Only runs when NOT placing
        }
    }

    private void HandlePlacement()
    {
#if UNITY_EDITOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 touchPosition = Mouse.current.position.ReadValue();
#else
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
#endif
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
            {
                Pose hitPose = hits[0].pose;
                Vector3 forwardOffset = arCamera.transform.forward * 0.05f;
                Quaternion uprightRotation = Quaternion.Euler(0, arCamera.transform.eulerAngles.y, 0);
                PlaceSymbolAt(hitPose.position + forwardOffset, uprightRotation, hits[0]);
                controller.HidePlacementOverlay(); // Hide placement overlay
            }
        }
    }

    private void PlaceSymbolAt(Vector3 position, Quaternion rotation, ARRaycastHit hit)
    {

        if (itemToPlace == null)
        {
            Debug.LogWarning("Trying to place a symbol, but itemToPlace is null!");
            return;
        }
        ARPlane plane = hit.trackable as ARPlane;
        if (plane == null)
        {
            Debug.LogError("No ARPlane found for anchor attachment.");
            return;
        }

        ARAnchor anchor = anchorManager.AttachAnchor(plane, new Pose(position, rotation));

        if (anchor == null)
        {
            Debug.LogError("Failed to create anchor!");
            return;
        }

        // Instantiate symbol
        GameObject placedObject = Instantiate(itemToPlace.prefab, anchor.transform);
        placedObject.name = $"{currentMarkerId}_{itemToPlace.title}";

        if (!spawnedObjectsByMarker.ContainsKey(currentMarkerId))
        {
            spawnedObjectsByMarker[currentMarkerId] = new List<GameObject>();
        }
        spawnedObjectsByMarker[currentMarkerId].Add(placedObject);

        ContentComponent cc = placedObject.AddComponent<ContentComponent>();
        cc.contentItem = itemToPlace;

        itemToPlace.position = position;

        // Save to  content list
        if (!ButtonHandler.contentByLocation.ContainsKey(currentMarkerId))
        {
            ButtonHandler.contentByLocation[currentMarkerId] = new List<ContentItem>();
        }

        ButtonHandler.contentByLocation[currentMarkerId].Add(itemToPlace);

        Debug.Log($"Placed '{itemToPlace.title}' at position {position}");

        // Show confirmation
        controller.ShowPlacementConfirmation();
        controller.HideCancelPlacement();

        isPlacing = false;
        itemToPlace = null;
        currentMarkerId = null;

    }

    public void LoadContentIntoARScene(string markerId)
    {
        // Clear previously loaded symbols
        ClearAllSpawnedObjects();

        if (ButtonHandler.contentByLocation == null)
        {
            Debug.LogError("ButtonHandler's contentByLocation is not initialized.");
            return;
        }

        if (!ButtonHandler.contentByLocation.ContainsKey(markerId)) return;

        List<ContentItem> contentItems = ButtonHandler.contentByLocation[markerId];

        foreach (var contentItem in contentItems)
        {
            if (contentItem.prefab == null)
            {
                Debug.LogWarning($"ContentItem '{contentItem.title}' has no prefab assigned.");
                continue;
            }

            GameObject instantiatedObject = Instantiate(contentItem.prefab, contentItem.position, Quaternion.identity);
            instantiatedObject.name = $"{markerId}_{contentItem.title}";
            ContentComponent cc = instantiatedObject.AddComponent<ContentComponent>();
            cc.contentItem = contentItem;

            if (!spawnedObjectsByMarker.ContainsKey(markerId))
            {
                spawnedObjectsByMarker[markerId] = new List<GameObject>();
            }
            spawnedObjectsByMarker[markerId].Add(instantiatedObject);

            Debug.Log($"Loaded '{contentItem.title}' into AR scene at position {contentItem.position}.");
        }
    }

    private void ClearAllSpawnedObjects()
    {
        foreach (var kvp in spawnedObjectsByMarker)
        {
            foreach (var obj in kvp.Value)
            {
                if (obj != null) Destroy(obj);
            }
        }
        spawnedObjectsByMarker.Clear();
    }

    private void HandleTouchInteraction()
    {
        if (controller.storyOverlayPanel.activeSelf ||
        controller.hubCanvas.activeSelf ||
        controller.symbolPanel.activeSelf)
        {
            return;
        }


#if UNITY_EDITOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 touchPosition = Mouse.current.position.ReadValue();
#else
    if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
    {
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
#endif
            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                Debug.Log($"Hit object: {hitObject.name}");

                ContentComponent component = hitObject.GetComponent<ContentComponent>();
                if (component != null)
                {
                    Debug.Log($"Tapped on {component.contentItem.title}");
                    controller.ShowStoryOverlay(component.contentItem);
                }
            }
        }
    }


    public void CancelPlacement()
        {
            isPlacing = false;
            itemToPlace = null;

            controller.HidePlacementOverlay();
            controller.HideCancelPlacement();

            controller.ShowHub();
            controller.ShowCreateContentMenu();
        }

    public void ExitARViewAndReset()
    {
        isPlacing = false;
        itemToPlace = null;
        currentMarkerId = null;

        controller.HidePlacementOverlay();
        controller.HideCancelPlacement();

        // clear input fields
        controller.ResetCreateStoryFields();

        controller.ShowHub();
    }


}
