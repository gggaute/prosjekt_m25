using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;

public class ARSceneManager : MonoBehaviour
{
    private Camera arCamera;
    private ARSession arSession;

    private void Start()
    {
        arSession = Object.FindFirstObjectByType<ARSession>();
        arCamera = Camera.main; // Update the camera reference

        if (arCamera == null)
        {
            Debug.LogError("AR Camera is missing or not assigned!");
        }

        StartCoroutine(ResetARSessionCoroutine());
        ResetCamera();
    }
    private IEnumerator ResetARSessionCoroutine()
    {
        if (arSession != null)
        {
            arSession.enabled = false;
            yield return null; // wait one frame
            arSession.enabled = true;
        }
    }
    private void ResetCamera()
    {
        if (arCamera != null)
        {
            arCamera.transform.position = Vector3.zero;
            arCamera.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("AR Camera is missing or destroyed!");
        }
    }

    public void ExitToHub()
    {
        // Save the current content item (if any)
        if (SymbolPlacer.currentContentItem != null)
        {
            if (!ButtonHandler.contentByLocation.ContainsKey(SymbolPlacer.currentMarkerId))
            {
                ButtonHandler.contentByLocation[SymbolPlacer.currentMarkerId] = new List<ContentItem>();
            }
            ButtonHandler.contentByLocation[SymbolPlacer.currentMarkerId].Add(SymbolPlacer.currentContentItem);

            Debug.Log($"Saved story for marker {SymbolPlacer.currentMarkerId}: {SymbolPlacer.currentContentItem.title}");
        }
        // Cleanup AR objects
        foreach (GameObject obj in SymbolPlacer.spawnedARObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        SymbolPlacer.spawnedARObjects.Clear();

        // Reset AR session
        if (arSession != null)
        {
            arSession.Reset();
        }

        // Return to the hub scene
        SceneManager.LoadScene("Hub");
    }

}
