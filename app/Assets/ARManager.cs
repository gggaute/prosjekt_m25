using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARManager : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public Camera arCamera;

    private ContentItem itemToPlace;
    private string currentMarkerId;
    private GameObject previewInstance;

    private bool isPlacing = false;

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
        if (!isPlacing || raycastManager == null || itemToPlace == null)
            return;

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

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceSymbolAt(hitPose.position, hitPose.rotation);
            }
        }
    }

    private void PlaceSymbolAt(Vector3 position, Quaternion rotation)
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }

        GameObject placedObject = Instantiate(itemToPlace.prefab, position, rotation);
        placedObject.name = $"{currentMarkerId}_{itemToPlace.title}";

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

            Debug.Log($"Loaded '{contentItem.title}' into AR scene at position {contentItem.position}.");
        }
    }
}
