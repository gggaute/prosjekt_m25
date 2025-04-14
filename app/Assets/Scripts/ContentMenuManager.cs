using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class ContentMenuManager : MonoBehaviour
{
    public SceneUIController controller;

    [Header("UI References")]
    public GameObject contentMenuContainer; // ContentMenuContainer panel
    public Transform content;     // VerticalContainer (has VerticalLayoutGroup)
    public GameObject contentButtonPrefab;  // Button prefab (assigned in Inspector)

    public void OpenMenu(string markerId)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Load content list
        if (!ButtonHandler.contentByLocation.ContainsKey(markerId))
        {
            Debug.LogError($"No content found for markerId: {markerId}");
            return;
        }

        List<ContentItem> contentItems = ButtonHandler.contentByLocation[markerId];

        // Instantiate buttons for each content piece
        foreach (var item in contentItems)
        {
            GameObject buttonGO = Instantiate(contentButtonPrefab, content);

            if (buttonGO == null)
            {
                Debug.LogError("contentButtonPrefab is null or failed to instantiate!");
                continue;
            }

            var titleText = buttonGO.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = $"Title: {item.title}";
            }
            else
            {
                Debug.LogError("TitleText is missing or not a TextMeshProUGUI component in contentButtonPrefab.");
            }

            var descriptionText = buttonGO.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
            if (descriptionText != null)
            {
                string truncatedDescription = item.description.Length > 20
                    ? item.description.Substring(0, 20) + "..."
                    : item.description;

                descriptionText.text = truncatedDescription;
            }
            else
            {
                Debug.LogError("DescriptionText is missing or not a TextMeshProUGUI component in contentButtonPrefab.");
            }

            var symbolImage = buttonGO.transform.Find("SymbolImage")?.GetComponent<Image>();
            if (symbolImage != null)
            {
                if (item.symbol != null && item.symbol.sprite != null)
                {
                    symbolImage.sprite = item.symbol.sprite;
                }
                else
                {
                    Debug.LogError($"Symbol or sprite is missing for content item: {item.title}");
                }
            }
            else
            {
                Debug.LogError("SymbolImage is missing or not an Image component in contentButtonPrefab.");
            }

            ContentItem capturedItem = item;
            var buttonComponent = buttonGO.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() =>
                {
                    ContentContainer.currentContent = capturedItem;
                    controller.ShowAR();
                });
            }
            else
            {
                Debug.LogError("contentButtonPrefab is missing a Button component.");
            }
        }

        contentMenuContainer.SetActive(true);
    }
    
    public void CloseMenu()
    {
        contentMenuContainer.SetActive(false);
    }
}
