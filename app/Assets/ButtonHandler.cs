using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class ButtonHandler : MonoBehaviour
{
    public static Dictionary<string, List<ContentItem>> contentByLocation;

    [Header("UI References")]
    public ContentMenuManager contentMenuManager; // Reference to ContentMenuManager
    public GameObject createContentPanel; // Reference to the CreateContentPanel
    public RectTransform markerContainer; // Reference to the marker container (where markers are placed)
    public GameObject markerPrefab; // Prefab for the marker (button)

    [Header("User Input References")]
    public TMP_InputField titleInputField; // Reference to the title input field
    public TMP_InputField descriptionInputField; // Reference to the description input field
    public TMP_Dropdown contentTypeDropdown; // Reference to the dropdown for selecting content type

    void Awake()
    {
        contentByLocation = new Dictionary<string, List<ContentItem>>
        {
            {
                "marker1", new List<ContentItem>
                {
                    new ContentItem
                    {
                        title = "Marker 1 - Content 1",
                        type = ContentType.Story,
                        source = ContentSource.System,
                        steps = new List<ContentStep>
                        {
                            new ContentStep
                            {
                                text = "Marker 1, content 1 - Step 1",
                                image = Resources.Load<Texture>("Overlay1/liv"),
                                video = null
                            },
                            new ContentStep
                            {
                                text = "Marker 1, content 1 - Step 2",
                                image = null,
                                video = Resources.Load<VideoClip>("Overlay1/video")
                            }
                        }
                    },
                    new ContentItem
                    {
                        title = "Content 2",
                        type = ContentType.Story,
                        source = ContentSource.User,
                        steps = new List<ContentStep>
                        {
                            new ContentStep
                            {
                                text = "Content 2 - step 1",
                                image = Resources.Load<Texture>("Overlay2/bird"),
                                video = null
                            },
                            new ContentStep
                            {
                                text = "Content 2 - step 2",
                                image = null,
                                video = Resources.Load<VideoClip>("Overlay2/dota")
                            }
                        }
                    }
                }
            },
            {
                "marker2", new List<ContentItem>
                {
                    new ContentItem
                    {
                        title = "Marker 2 - Content 1",
                        type = ContentType.Story,
                        source = ContentSource.System,
                        steps = new List<ContentStep>
                        {
                            new ContentStep
                            {
                                text = "Marker 2, content 1 - Step 1",
                                image = Resources.Load<Texture>("Overlay1/liv"),
                                video = null
                            },
                            new ContentStep
                            {
                                text = "Marker 2, content 1 - Step 2",
                                image = null,
                                video = Resources.Load<VideoClip>("Overlay1/video")
                            }
                        }
                    },
                    new ContentItem
                    {
                        title = "Content 2",
                        type = ContentType.Story,
                        source = ContentSource.User,
                        steps = new List<ContentStep>
                        {
                            new ContentStep
                            {
                                text = "Content 2 - step 1",
                                image = Resources.Load<Texture>("Overlay2/bird"),
                                video = null
                            },
                            new ContentStep
                            {
                                text = "Content 2 - step 2",
                                image = null,
                                video = Resources.Load<VideoClip>("Overlay2/dota")
                            }
                        }
                    }
                }
            }
        };
    }

    public void OpenMenu(Button button)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        if (buttonText != null) {
            string markerId = buttonText.text;
            contentMenuManager.OpenMenu(markerId);
        }
    }

    public void Contribute()
    {
        // Show the CreateContentPanel
        createContentPanel.SetActive(true);
    }
    public void CloseContentMenu()
    {
        contentMenuManager.CloseMenu();
    }

    public void CloseCreateContent() {
        createContentPanel.SetActive(false);
    }

    public void AddContent(string markerId)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(titleInputField.text))
        {
            Debug.LogError("Title is required!");
            return;
        }

        // Create a new ContentItem
        ContentItem newContent = new ContentItem
        {
            title = titleInputField.text,
            type = (ContentType)contentTypeDropdown.value, // Convert dropdown value to ContentType enum
            source = ContentSource.User, // Assume user-created content
            steps = new List<ContentStep>
            {
                new ContentStep
                {
                    text = descriptionInputField.text,
                    image = null, // Placeholder for now
                    video = null  // Placeholder for now
                }
            }
        };

        // Add the new content to the appropriate marker
        if (!contentByLocation.ContainsKey(markerId))
        {
            contentByLocation[markerId] = new List<ContentItem>();
        }
        contentByLocation[markerId].Add(newContent);

        Debug.Log($"Added new content to marker {markerId}: {newContent.title}");

        // Clear the input fields
        titleInputField.text = string.Empty;
        descriptionInputField.text = string.Empty;
        contentTypeDropdown.value = 0;

        contentMenuManager.OpenMenu(markerId);

        // Close the CreateContentPanel
        createContentPanel.SetActive(false);
    }
}



