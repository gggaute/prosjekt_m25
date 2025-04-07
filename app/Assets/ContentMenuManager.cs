using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ContentMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject contentMenuContainer; // ContentMenuContainer panel
    public Transform verticalContainer;     // VerticalContainer (has VerticalLayoutGroup)
    public GameObject contentButtonPrefab;  // Button prefab (assigned in Inspector)

    public void OpenMenu(string markerId)
    {
        contentMenuContainer.SetActive(true);

        // Clear existing buttons
        foreach (Transform child in verticalContainer)
        {
            Destroy(child.gameObject);
        }

        // Load content list
        if (!ButtonHandler.contentByLocation.ContainsKey(markerId)) return;
        List<ContentItem> contentItems = ButtonHandler.contentByLocation[markerId];

        // Instantiate buttons for each content piece
        foreach (var item in contentItems)
        {
            GameObject buttonGO = Instantiate(contentButtonPrefab, verticalContainer);

            // Assume the prefab has a TMP child named TitleText and TypeText
            buttonGO.transform.Find("TitleText").GetComponent<TextMeshProUGUI>().text = item.title;
            buttonGO.transform.Find("TypeText").GetComponent<TextMeshProUGUI>().text = $"{item.type} - {item.source}";

            ContentItem capturedItem = item;
            buttonGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                ContentContainer.currentContent = capturedItem;
                SceneManager.LoadScene("AR");
            });
        }
    }

    public void CloseMenu()
    {
        contentMenuContainer.SetActive(false);
    }
}
