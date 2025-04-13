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
    public Camera rootCamera;    // The camera for the hub ui
    public Camera arCamera;      // ar camera inside XR Origin

    private AudioListener rootAudioListener;
    private AudioListener arAudioListener;

    private void Start()
    {
        
        rootAudioListener = rootCamera.GetComponent<AudioListener>();
        arAudioListener = arCamera.GetComponent<AudioListener>();

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

        rootCamera.enabled = true; // Enable the root camera
        arCamera.enabled = false; // Disable the AR camera

        // Manage AudioListeners
        if (rootAudioListener != null) rootAudioListener.enabled = true;
        if (arAudioListener != null) arAudioListener.enabled = false;
    }

    public void ShowAR()
    {
        // Enable the AR canvas and AR camera
        hubCanvas.SetActive(false);
        arCanvas.SetActive(true);

        rootCamera.enabled = false; // Disable the root camera
        arCamera.enabled = true; // Enable the AR camera

        // Manage AudioListeners
        if (rootAudioListener != null) rootAudioListener.enabled = false;
        if (arAudioListener != null) arAudioListener.enabled = true;
    }
}
