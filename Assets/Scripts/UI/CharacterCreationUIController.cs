using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming TextMeshPro is used as preferred
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System; // Required for Action<>
using Game01.Data; // Added to resolve CharacterData type

// Assuming these namespaces exist based on the instructions
// using YourGame.Networking; // Replace with actual namespace if NetworkManager/CallbackResponse are namespaced


public class CharacterCreationUIController : MonoBehaviour
{
    // --- UI Element References ---
    // Assign these in the Unity Editor Inspector
    [Header("UI Elements")]
    [Tooltip("Input field for the character's name.")]
    public TMP_InputField nameInputField; // Use TMP_InputField as preferred

    [Tooltip("Button to initiate character creation.")]
    public Button createButton;

    [Tooltip("Button to go back to the previous screen (e.g., Main Menu).")]
    public Button backButton;

    [Tooltip("Reference to the Loading UI Manager.")]
    [SerializeField] private LoadingUIManager loadingUIManager;

    // --- Unity Lifecycle Methods ---

    void Start()
    {
        // --- Input Validation ---
        // Ensure fields are assigned in the Unity Editor's Inspector
        if (nameInputField == null)
        {
            Debug.LogError("CharacterCreationUIController: Name Input Field (TMP_InputField) not assigned in the inspector!");
        }
        if (createButton == null)
        {
            Debug.LogError("CharacterCreationUIController: Create Button not assigned in the inspector!");
        }
        if (backButton == null)
        {
            Debug.LogError("CharacterCreationUIController: Back Button not assigned in the inspector!");
        }

        // --- Add Listeners ---
        // Check if buttons are assigned before adding listeners to prevent NullReferenceException
        if (createButton != null)
        {
            createButton.onClick.AddListener(OnCreateButtonClicked);
            createButton.interactable = true; // Ensure interactable at start
        }
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    void OnDestroy()
    {
        // --- Clean Up Listeners ---
        // Check if buttons are assigned before removing listeners
        if (createButton != null)
        {
            createButton.onClick.RemoveListener(OnCreateButtonClicked);
        }
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }

    // --- Button Click Handlers ---

    private void OnCreateButtonClicked()
    {
        // Ensure input field is assigned before accessing its text
        if (nameInputField == null)
        {
            Debug.LogError("Cannot create character: Name Input Field is not assigned.");
            // Optionally show an error via LoadingUIManager if it's configured early enough
            // if (loadingUIManager != null) loadingUIManager.ShowError("Internal UI Error: Input field missing."); // Updated access pattern
            return;
        }

        string characterName = nameInputField.text;

        if (!IsNameValid(characterName))
        {
            return; // Stop processing, validation method already showed error via LoadingUIManager
        }

        // Disable button to prevent multiple clicks during processing
        if (createButton != null)
        {
            createButton.interactable = false;
        }

        // Show loading indicator
        // Show loading indicator using the assigned reference
        if (loadingUIManager == null) {
            Debug.LogError("LoadingUIManager reference not assigned in the inspector!");
            // Re-enable button if loading cannot be shown
            if (createButton != null) createButton.interactable = true;
            return;
        }
        loadingUIManager.ShowLoading();

        // Prepare character data
        // Ensure CharacterData and its namespace are correct
        CharacterData newCharData = new CharacterData
        {
            name = characterName.Trim(), // Use trimmed name
            level = 1,                     // Use new field name
            xp = 0,                        // Use new field name
            hp = 50,                       // Use new field name
            energy = 10,                   // Use new field name
            maxHp = 50,                    // Use new field name
            maxEnergy = 10,                // Use new field name
            LearnedPassives = new List<PassiveSkill_SO>(), // Field name unchanged
            CurrentRunDeck = new List<CardDefinition_SO>() // Field name unchanged
            // characterId is intentionally omitted as per instructions
        };

        // Call the backend service via NetworkManager
        // Adapt access pattern if NetworkManager is not a singleton Instance
        // Ensure NetworkManager.CreateCharacter signature and CallbackResponse type match
        StartCoroutine(NetworkManager.Instance.SaveCharacterData(newCharData, OnCharacterSaveResponse));
    }

    private void OnBackButtonClicked()
    {
        // Add safety check if needed (e.g., prevent backing out while creation is in progress)
        // if (createButton != null &amp;&amp; !createButton.interactable) { /* Maybe show warning? */ return; }

        // Load the main menu scene
        // Confirm "MainMenuScene" is the correct scene name
        SceneManager.LoadScene("GuildHall");
    }

    // --- Validation Logic ---

    private bool IsNameValid(string name)
    {
        string trimmedName = name.Trim(); // Remove leading/trailing whitespace

        if (string.IsNullOrEmpty(trimmedName))
        {
            // Show error using the assigned reference
            if (loadingUIManager == null) {
                 Debug.LogError("LoadingUIManager reference not assigned, cannot show error: Name cannot be empty.");
            } else {
                loadingUIManager.ShowError("Name cannot be empty.");
            }
            return false;
        }

        if (trimmedName.Length < 3 || trimmedName.Length > 16)
        {
            // Show error using the assigned reference
            if (loadingUIManager == null) {
                 Debug.LogError("LoadingUIManager reference not assigned, cannot show error: Name must be 3-16 characters long.");
            } else {
                loadingUIManager.ShowError("Name must be 3-16 characters long.");
            }
            return false;
        }

        // Add other validation rules here if needed in the future (but not for this task)
        // e.g., check for invalid characters

        return true; // Name is valid
    }

    // --- Network Callback Handler ---

    private void OnCharacterSaveResponse(bool success)
    {
        // Hide loading indicator
        // Hide loading indicator using the assigned reference
        if (loadingUIManager == null) {
             Debug.LogError("LoadingUIManager reference not assigned, cannot hide loading indicator.");
        } else {
            loadingUIManager.HideLoading();
        }

        if (success)
        {
            // Show success message (using ShowError as per instructions for now)
            // Show success message using the assigned reference
            if (loadingUIManager == null) {
                 Debug.LogError("LoadingUIManager reference not assigned, cannot show success message.");
            } else {
                loadingUIManager.ShowError("Character Saved Successfully!"); // Use generic success message
            }

            // Navigate to the next scene (e.g., Guild Hall)
            // Confirm "GuildHall" is the correct scene name
            SceneManager.LoadScene("GuildHall");
        }
        else
        {
            // Show generic error message
            // Show error message using the assigned reference
            if (loadingUIManager == null) {
                 Debug.LogError("LoadingUIManager reference not assigned, cannot show error: Character saving failed.");
            } else {
                loadingUIManager.ShowError("Character saving failed. Please try again.");
            }

            // Re-enable the create button ONLY on failure, so the user can retry
            if (createButton != null)
            {
                createButton.interactable = true;
            }
        }
    }
}