using UnityEngine;
using UnityEngine.UI; // Added for UI elements
using UnityEngine.SceneManagement; // Added for scene loading
using System.Collections.Generic; // Added for Lists
using System; // Added for Action
public class CharacterCreationController : MonoBehaviour
{
    // --- Serialized Fields ---
    [SerializeField] private TMPro.TMP_InputField characterNameInputField; // Assuming TMP_InputField, adjust if using standard InputField
    [SerializeField] private Button createCharacterButton;
    [SerializeField] private LoadingUIManager loadingUIManagerRef; // Assuming this is assigned in the Inspector

    // --- Constants ---
    private const string GUILD_HALL_SCENE_NAME = "GuildHallScene"; // Replace with actual scene name if different

    // --- Dependencies ---
    // Assuming TeamRosterManager is a Singleton or accessible via static Instance
    // Assuming LoadingUIManager is a Singleton or accessible via static Instance (or assigned via loadingUIManagerRef)
    public void InitializeUI()
    {
    }

    /// &lt;summary&gt;
    /// Called when the Create Character button is pressed.
    /// Validates input, creates CharacterData, and initiates the save process.
    /// &lt;/summary&gt;
    public void AttemptCharacterCreation()
    {
        if (characterNameInputField == null || createCharacterButton == null || loadingUIManagerRef == null)
        {
            Debug.LogError("CharacterCreationController: UI elements or LoadingUIManager not assigned in Inspector.");
            return;
        }

        string characterName = characterNameInputField.text;

        // Client-Side Validation
        if (string.IsNullOrWhiteSpace(characterName))
        {
            // Use the assigned reference or Singleton instance
            LoadingUIManager uiManager = loadingUIManagerRef ?? LoadingUIManager.Instance;
            if (uiManager != null)
            {
                uiManager.ShowError("Character name cannot be empty.");
            }
            else
            {
                Debug.LogError("LoadingUIManager reference not set and Singleton instance not found.");
            }
            return;
        }

        // Disable UI
        createCharacterButton.interactable = false;

        // Show loading indicator (Optional, but good practice)
        LoadingUIManager loadingManager = loadingUIManagerRef ?? LoadingUIManager.Instance;
        if (loadingManager != null)
        {
            loadingManager.ShowLoading("Creating Character..."); // Assuming a ShowLoading method exists
        }


        // Create CharacterData
        CharacterData newCharacterData = new CharacterData
        {
            CharacterName = characterName,
            CurrentLevel = 1,
            CurrentXP = 0,
            MaxHP = 50,
            MaxEnergy = 10,
            CurrentHP = 50, // Start with full HP
            CurrentEnergy = 10, // Start with full Energy
            LearnedPassives = new List&lt;PassiveAbilityType&gt;(), // Assuming PassiveAbilityType enum/class exists
            CurrentRunDeck = new List&lt;CardData&gt;() // Assuming CardData class/struct exists
        };

        // Call Manager
        // Assuming TeamRosterManager exists and has a static Instance property
        if (TeamRosterManager.Instance != null)
        {
            TeamRosterManager.Instance.AddCharacter(newCharacterData, HandleCharacterCreationResponse);
        }
        else
        {
            Debug.LogError("TeamRosterManager Singleton instance not found.");
            HandleCharacterCreationResponse(false, "Internal error: Roster Manager not available."); // Handle error locally
        }
    }

    /// &lt;summary&gt;
    /// Callback handler for the response from TeamRosterManager.AddCharacter.
    /// &lt;/summary&gt;
    /// &lt;param name="success"&gt;True if the character was saved successfully, false otherwise.&lt;/param&gt;
    /// &lt;param name="message"&gt;Feedback or error message from the operation.&lt;/param&gt;
    private void HandleCharacterCreationResponse(bool success, string message)
    {
        // Ensure LoadingUIManager is available
        LoadingUIManager uiManager = loadingUIManagerRef ?? LoadingUIManager.Instance;
        if (uiManager == null)
        {
             Debug.LogError("LoadingUIManager reference not set and Singleton instance not found. Cannot show feedback.");
             // Still attempt to re-enable button on failure
             if (!success &amp;&amp; createCharacterButton != null)
             {
                 createCharacterButton.interactable = true;
             }
             return; // Cannot proceed without UI manager
        }

        // Hide loading indicator (assuming a HideLoading method exists)
        uiManager.HideLoading();

        if (success)
        {
            uiManager.ShowMessage("Character Created Successfully!"); // Or ShowSuccess if available
            // Transition to Guild Hall
            SceneManager.LoadScene(GUILD_HALL_SCENE_NAME);
        }
        else
        {
            // Re-enable UI on failure
            if (createCharacterButton != null)
            {
                createCharacterButton.interactable = true;
            }
            // Display Error
            uiManager.ShowError(message);
        }
    }

    // Removed placeholder methods ShowLoadingState and ShowFeedback as they are superseded by LoadingUIManager usage.
}