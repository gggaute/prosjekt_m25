using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ObjectPlacer : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private bool placed = false;

    void Start()
    {
        raycastManager = Object.FindFirstObjectByType<ARRaycastManager>();
    }

    void Update()
    {
        if (placed) return;

#if UNITY_EDITOR
        // For testing in the editor  
        if (Input.GetMouseButtonDown(0))
        {
            TryPlace(Input.mousePosition);
        }
#else
       // For mobile/touch  
       if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)  
       {  
           TryPlace(Input.GetTouch(0).position);  
       }  
#endif
    }

    void TryPlace(Vector2 screenPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            transform.position = hitPose.position;
            transform.rotation = hitPose.rotation;
            placed = true;

            // Optional: disable this script to stop further updates  
            this.enabled = false;

            // Optional: add visuals or feedback  
            Debug.Log("Object placed!");
        }
    }
}
