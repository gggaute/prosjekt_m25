using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public static Dictionary<string, List<ContentItem>> contentByLocation;

    public SceneUIController controller;

    [Header("Symbol Data")]
    public List<Symbol> symbols; // List of available symbols
    public GameObject symbolButtonPrefab; // Prefab for the symbol button
    public GameObject symbolPanelContainer; // The SymbolPanel container (to show/hide)
    private Symbol selectedSymbol; // The selected 3D prefab
    public ContentMenuManager contentMenuManager; // Reference to ContentMenuManager
    public GameObject createContentPanel; // Reference to the CreateContentPanel
    public RectTransform markerContainer; // Reference to the marker container (where markers are placed)

    [Header("UI References")]
    public TMP_InputField titleInputField; // Reference to the title input field
    public TMP_InputField descriptionInputField; // Reference to the description input field
    public string markerId; // which marker are we at

    public void Awake()
    {
        symbols = new List<Symbol>
        {
            new Symbol { name = "Heart", sprite = Resources.Load<Sprite>("Sprites/heart"), prefab = Resources.Load<GameObject>("Models/HeartModel") },
            new Symbol { name = "Balloon", sprite = Resources.Load<Sprite>("Sprites/balloon"), prefab = Resources.Load<GameObject>("Models/BalloonModel") },
            new Symbol { name = "Happy", sprite = Resources.Load<Sprite>("Sprites/happy"), prefab = Resources.Load<GameObject>("Models/HappyModel") },
            new Symbol { name = "Laughing", sprite = Resources.Load<Sprite>("Sprites/laughing"), prefab = Resources.Load<GameObject>("Models/LaughingModel") },
            new Symbol { name = "Sunglasses", sprite = Resources.Load<Sprite>("Sprites/sunglasses"), prefab = Resources.Load<GameObject>("Models/SunglassesModel") },
            new Symbol { name = "Angry", sprite = Resources.Load<Sprite>("Sprites/angry"), prefab = Resources.Load<GameObject>("Models/LaughingModel") },

        };
        contentByLocation = new Dictionary<string, List<ContentItem>>();
        contentByLocation["marker1"] = new List<ContentItem>
        {
            new ContentItem
            {
                title = "A Day at the Park",
                description = "This is a story about a relaxing day spent at the park.",
                symbol = symbols[0],
                prefab = symbols[0].prefab,
                position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f + new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f)) // Placeholder position
            },
            new ContentItem
            {
                title = "Evening by the Lake",
                description = "A beautiful evening spent watching the sunset by the lake.",
                symbol = symbols[1],
                prefab = symbols[1].prefab,
                position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f + new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f)) // Placeholder position
            }
        };
        contentByLocation["marker2"] = new List<ContentItem>
        {
            new ContentItem
            {
                title = "My first solo trip",
                description = "A long time ago i went to the park to go fetch some shoes",
                symbol = symbols[2],
                prefab = symbols[2].prefab,
                position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f + new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f)) // Placeholder position
            },
            new ContentItem
            {
                title = "Last meal with my grandma",
                description = "At this spot i had my last lucnh with my grandmother before she passed away",
                symbol = symbols[3],
                prefab = symbols[3].prefab,
                position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f + new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f)) // Placeholder position
            }
        };
    }

    public void OpenMenu(Button button)
    {
        if (button == null)
        {
            Debug.LogError("Button is null!");
            return;
        }

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText == null)
        {
            Debug.LogError("Button does not have a TMP_Text component!");
            return;
        }

        markerId = buttonText.text;
        contentMenuManager.OpenMenu(markerId);
    }
    public void Contribute()
    {
        // Show the CreateContentPanel
        createContentPanel.SetActive(true);
    }
    public void CloseCreateContentMenu()
    {
        createContentPanel.SetActive(false);
    }

    public void ViewSpace()
    {
        Debug.Log("Entering AR space...");
        controller.ShowAR();

        // Debug the state of the AR canvas
        Debug.Log($"ARCanvas active: {controller.arCanvas.activeSelf}");
        foreach (Transform child in controller.arCanvas.transform)
        {
            Debug.Log($"Child: {child.name}, Active: {child.gameObject.activeSelf}");
        }
    }

    public void CreateStory()
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(titleInputField.text))
        {
            Debug.LogError("Title is required!");
            return;
        }

        if (selectedSymbol == null)
        {
            Debug.LogError("No symbol selected!");
            return;
        }

        // Create a new ContentItem
        ContentItem newContent = new ContentItem
        {
            title = titleInputField.text,
            description = descriptionInputField.text,
            symbol = selectedSymbol,
            prefab = selectedSymbol.prefab,// Assign the selected prefab
        };

        // Add the new content to the current marker
        if (!contentByLocation.ContainsKey(markerId))
        {
            contentByLocation[markerId] = new List<ContentItem>();
        }
        contentByLocation[markerId].Add(newContent);

        Debug.Log($"Created new story for marker {markerId}: {newContent.title}");

        contentMenuManager.OpenMenu(markerId); // Refresh the content menu to show the new item

        // CloseCreateContentMenu();

        // Transition to AR space
        controller.ShowAR();
    }

    public void ShowSymbolGrid()
    {
        // Hide the CreateContentPanel and show the SymbolPanel
        createContentPanel.SetActive(false);
        symbolPanelContainer.SetActive(true);

        // Populate the SymbolPanel with buttons
        PopulateSymbolGrid();
    }

    public void PopulateSymbolGrid()
    {
        // Clear existing buttons in the SymbolPanel
        foreach (Transform child in symbolPanelContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Dynamically create buttons for each symbol
        foreach (Symbol symbol in symbols)
        {
            // Instantiate a button for the symbol
            GameObject button = Instantiate(symbolButtonPrefab, symbolPanelContainer.transform);

            // Set the button's image to the symbol's sprite
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = symbol.sprite;

            // Add a click listener to select the symbol
            Button buttonComponent = button.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => SelectSymbol(symbol));
        }
    }

    public void SelectSymbol(Symbol symbol)
    {
        // Store the selected 3D prefab
        selectedSymbol = symbol;

        // Log the selected symbol for debugging
        Debug.Log($"Selected symbol: {symbol.name}");

        // Hide the SymbolPanel and return to the CreateContentPanel
        symbolPanelContainer.SetActive(false);
        createContentPanel.SetActive(true);

    }

    
}
