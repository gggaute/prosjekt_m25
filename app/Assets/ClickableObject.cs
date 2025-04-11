using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public ContentItem associatedContent; // The story this object represents

    private void OnMouseDown()
    {
        // This method is called when the object is tapped in the AR scene
        Debug.Log($"Clicked on: {associatedContent.title}");

        // TODO: Add functionality to open the story UI or perform other actions
    }
}
