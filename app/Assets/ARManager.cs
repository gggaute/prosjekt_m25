using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ARManager : MonoBehaviour
{
    public SceneUIController controller;
    public ARRaycastManager raycastManager;
    public Camera arCamera;

    private ContentItem itemToPlace;
    private string currentMarkerId;
    private GameObject previewInstance;

    private bool isPlacing = false;

    private List<GameObject> placedObjects = new List<GameObject>();


    public void BeginPlacingSymbol(ContentItem item, string markerId)
    {
        itemToPlace = item;
        currentMarkerId = markerId;
        isPlacing = true;

        // Show a preview object that follows detected planes
        if (itemToPlace.prefab != null)
        {
            previewInstance = Instantiate(itemToPlace.prefab);
            previewInstance.SetActive(false); // Don't show until we get a hit
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
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            if (previewInstance != null)
            {
                previewInstance.SetActive(true);
                previewInstance.transform.position = hitPose.position;
                previewInstance.transform.rotation = hitPose.rotation;
            }

#if UNITY_EDITOR
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
#else
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
#endif
            {
                Debug.Log("Placing symbol...");
                PlaceSymbolAt(hitPose.position, hitPose.rotation);
            }
        }
    }

    private void PlaceSymbolAt(Vector3 position, Quaternion rotation)
    {

        if (itemToPlace == null)
        {
            Debug.LogWarning("Trying to place a symbol, but itemToPlace is null!");
            return;
        }
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }

        GameObject placedObject = Instantiate(itemToPlace.prefab, position, rotation);
        placedObject.name = $"{currentMarkerId}_{itemToPlace.title}";
        placedObjects.Add(placedObject);

        ContentComponent cc = placedObject.AddComponent<ContentComponent>();
        cc.contentItem = itemToPlace;

        itemToPlace.position = position;

        // Save it to the content list
        if (!ButtonHandler.contentByLocation.ContainsKey(currentMarkerId))
        {
            ButtonHandler.contentByLocation[currentMarkerId] = new List<ContentItem>();
        }

        ButtonHandler.contentByLocation[currentMarkerId].Add(itemToPlace);

        Debug.Log($"Placed '{itemToPlace.title}' at position {position}");

        isPlacing = false;
        itemToPlace = null;
        currentMarkerId = null;
    }

    public void LoadContentIntoARScene(string markerId)
    {
        // Clear previously loaded symbols
        foreach (var obj in placedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        placedObjects.Clear();

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

            Debug.Log($"Loaded '{contentItem.title}' into AR scene at position {contentItem.position}.");
        }
    }

    private void HandleTouchInteraction()
    {
#if UNITY_EDITOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
#else
    if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
#endif
        {
            Debug.Log("Screen tapped!");
            Vector2 touchPosition = new Vector2(Screen.width / 2f, Screen.height / 2f); // or use touch position

            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                Debug.Log($"Hit object: {hitObject.name}");

                // Check if it's a content object
                ContentComponent component = hitObject.GetComponent<ContentComponent>();
                if (component != null)
                {
                    Debug.Log($"Tapped on {component.contentItem.title}");
                    // Trigger the UI display
                    controller.ShowStoryOverlay(component.contentItem);
                }
            }
        }
    }
}
