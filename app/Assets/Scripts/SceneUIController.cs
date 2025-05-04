using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections;

public class SceneUIController : MonoBehaviour
{

    public ContentMenuManager contentMenuManager;
    public GameObject storyOverlayPanel;
    public TMP_Text storyTitleText;
    public TMP_Text storyDescText;
    public Image storySymbolImage;
    public GameObject placementOverlay;
    public TMP_Text placementMessage;
    public Image placementImage;
    public GameObject placementConfirmationPanel;
    public GameObject cancelPlacementButton;

    public GameObject symbolPanel;

    [Header("Canvas References")]
    public GameObject hubCanvas; // Hub ui 
    public GameObject arCanvas;  // AR ui Canvas
    public GameObject contentMenuContainer;
    public GameObject createContentPanel;

    public ButtonHandler buttonHandler;


    [Header("Camera References")]
    public Camera arCamera;      // ar camera inside XR Origin



    private void Start()
    {

        hubCanvas.SetActive(true);
        arCanvas.SetActive(false);
        contentMenuContainer.SetActive(false);
        createContentPanel.SetActive(false);
        // Start in hub mode
    }

    public void ShowHub()
    {
        // Enable the hub canvas and root camera
        hubCanvas.SetActive(true);
        contentMenuContainer.SetActive(true);
        contentMenuManager.OpenMenu(contentMenuManager.markerId);

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

    public void ShowStoryOverlay(ContentItem item)
    {
        // Show the story overlay
        Debug.Log("Showing story overlay...");
        storyOverlayPanel.SetActive(true);
        storyTitleText.text = item.title;
        storyDescText.text = item.description;
        storySymbolImage.sprite = item.symbol.sprite;
    }

    public void HideStoryOverlay()
    {         // Hide the story overlay
        Debug.Log("Hiding story overlay...");
        storyOverlayPanel.SetActive(false);
    }

    public void ShowPlacementOverlay(ContentItem item)
    {
        if (placementOverlay != null)
            placementOverlay.SetActive(true);
            placementMessage.text = "Tap where you want to place your story";
            placementImage.sprite = item.symbol.sprite;
    }

    public void HidePlacementOverlay()
    {
        if (placementOverlay != null)
            placementOverlay.SetActive(false);
    }

    public void ShowPlacementConfirmation()
    {
        StopAllCoroutines(); // In case it's already fading
        placementConfirmationPanel.SetActive(true);
        StartCoroutine(HideAfterDelay(2f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        placementConfirmationPanel.SetActive(false);
    }

    public void ShowCancelPlacement() 
    {
        cancelPlacementButton.SetActive(true);
    }
    public void HideCancelPlacement()
    {
        cancelPlacementButton.SetActive(false);
    }

    public void ResetCreateStoryFields()
    {
        // Clear text fields and selected symbol
        buttonHandler.titleInputField.text = "";
        buttonHandler.descriptionInputField.text = "";
        buttonHandler.ResetSelectedSymbol();
    }
    public void ShowCreateContentMenu()
    {
        createContentPanel.SetActive(true);
        contentMenuContainer.SetActive(false);
    }
    public void ShowSymbols()
    {
        symbolPanel.SetActive(true);
    }
    public void SymbolCancel()
    {
        symbolPanel.SetActive(false);
        ShowCreateContentMenu();
    }
}
