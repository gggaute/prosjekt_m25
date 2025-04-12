using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    public ContentItem associatedContent;

    void Start()
    {
        mainCamera = Camera.main; // Cache the main camera
    }

    void OnMouseDown()
    {
        // Start dragging the object
        isDragging = true;
    }

    void OnMouseUp()
    {
        // Stop dragging the object
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            // Cast a ray from the camera to the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Move the object to the hit point
                transform.position = hit.point;
            }
        }
    }
}
