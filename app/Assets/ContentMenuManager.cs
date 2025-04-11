using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ContentMenuManager : MonoBehaviour
{
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
        if (!ButtonHandler.contentByLocation.ContainsKey(markerId)) return;
        List<ContentItem> contentItems = ButtonHandler.contentByLocation[markerId];

        // Instantiate buttons for each content piece
        foreach (var item in contentItems)
        {
            GameObject buttonGO = Instantiate(contentButtonPrefab, content);

            var titleText = buttonGO.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = $"Title: {item.title}";
            }

            // Find and set the DescriptionText
            var descriptionText = buttonGO.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
            if (descriptionText != null)
            {
                // Truncate the description to the first 10 characters
                string truncatedDescription = item.description.Length > 20
                    ? item.description.Substring(0, 20) + "..."
                    : item.description;

                descriptionText.text = truncatedDescription;
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
            buttonGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                ContentContainer.currentContent = capturedItem;
                SceneManager.LoadScene("AR");
            });
        }
        contentMenuContainer.SetActive(true);
    }
    
    public void CloseMenu()
    {
        contentMenuContainer.SetActive(false);
    }
}
