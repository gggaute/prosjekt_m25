using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SceneUIController : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject hubCanvas; // Hub ui 
    public GameObject arCanvas;  // AR ui Canvas
    public GameObject contentMenuContainer;
    public GameObject createContentPanel;

    [Header("Camera References")]
    public Camera arCamera;      // ar camera inside XR Origin


    private void Start()
    {

        ShowHub();
        contentMenuContainer.SetActive(false);
        createContentPanel.SetActive(false);
        // Start in hub mode
    }

    public void ShowHub()
    {
        // Enable the hub canvas and root camera
        hubCanvas.SetActive(true);
        contentMenuContainer.SetActive(true);

        arCanvas.SetActive(false);
    }

    public void ShowAR()
    {
        Debug.Log("Switching to AR view...");

        // Enable the AR canvas and AR camera
        hubCanvas.SetActive(false);
        arCanvas.SetActive(true);

        // Debug the state of the AR canvas
        Debug.Log($"ARCanvas active: {arCanvas.activeSelf}");
        foreach (Transform child in arCanvas.transform)
        {
            Debug.Log($"Child: {child.name}, Active: {child.gameObject.activeSelf}");
        }
    }
}
