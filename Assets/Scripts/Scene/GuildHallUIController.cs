using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; // Added for TextMeshPro
using Game01.Data; // Added for CharacterData
using Game.UI; // Added for CharacterRosterEntry
using Game.Systems; // Added for ProgressionSystem (Assuming namespace)
using System.Linq; // Added for potential LINQ operations like ToList()

/// <summary>
/// Handles UI interactions and displays the team roster in the Guild Hall scene.
/// </summary>
public class GuildHallUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button missionMapButton;
    [SerializeField] private Button characterSelectionButton;
    [SerializeField] private Button refreshRosterButton;
    public Button createCharacterButton;

    [Header("Roster Display")]
    [SerializeField] private Transform rosterContainer; // Parent object for roster entries
    [SerializeField] private GameObject characterEntryPrefab; // Prefab for each character row
    [SerializeField] private TextMeshProUGUI rosterStatusText; // Text to show loading/error messages

    [Header("Managers")]
    private TeamRosterManager teamRosterManager; // Found dynamically
    private ProgressionSystem progressionSystem; // Found dynamically (Assuming singleton or similar)
    [SerializeField] private LoadingUIManager loadingUIManager; // Assign in inspector

    private SceneLoader sceneLoader;
    private List<Button> interactableButtons = new List<Button>();
    private Dictionary<string, CharacterRosterEntry> characterEntryMap = new Dictionary<string, CharacterRosterEntry>();

    private void Start()
    {
        // Initialize Roster Map
        characterEntryMap = new Dictionary<string, CharacterRosterEntry>();

        // Find GameInitializer and get its SceneLoader reference
        GameInitializer gameInitializer = FindFirstObjectByType<GameInitializer>();
        if (gameInitializer != null)
        {
            sceneLoader = gameInitializer.SceneLoader;
            if (sceneLoader == null)
            {
                Debug.LogError("SceneLoader not found in GameInitializer.");
            }
        }
        else
        {
            Debug.LogError("GameInitializer not found in scene.");
        }

        // Find Progression System instance (assuming singleton or similar access)
        progressionSystem = FindFirstObjectByType<ProgressionSystem>(); // Or ProgressionSystem.Instance if static
        if (progressionSystem == null)
        {
             Debug.LogWarning("ProgressionSystem instance not found. Level-up UI updates will not function.");
        }

        // Validate required UI references
        if (rosterContainer == null) Debug.LogError("Roster Container reference not assigned in the Inspector.", this);
        if (characterEntryPrefab == null) Debug.LogError("Character Entry Prefab reference not assigned in the Inspector.", this);
        if (rosterStatusText == null) Debug.LogError("Roster Status Text reference not assigned in the Inspector.", this);
        if (loadingUIManager == null) Debug.LogError("Loading UI Manager reference not assigned in the Inspector.", this);


        // Set up button click events
        if (missionMapButton != null)
        {
            missionMapButton.onClick.AddListener(OnMissionMapButtonClicked);
            interactableButtons.Add(missionMapButton);
        }
        else
        {
            Debug.LogError("Mission Map Button reference not assigned in the Inspector.");
        }
        
        if (characterSelectionButton != null)
        {
            interactableButtons.Add(characterSelectionButton);
        }
        
        if (refreshRosterButton != null)
        {
            refreshRosterButton.onClick.AddListener(OnRefreshRosterClicked);
            interactableButtons.Add(refreshRosterButton);
        }
if (createCharacterButton != null)
{
    createCharacterButton.onClick.AddListener(NavigateToCreateCharacter);
    interactableButtons.Add(createCharacterButton);
}
else
{
     Debug.LogWarning("Create Character Button reference not assigned in the Inspector."); // Use Warning as it might be optional
}

// Find the TeamRosterManager instance dynamically
        // Find the TeamRosterManager instance dynamically
        teamRosterManager = FindFirstObjectByType<TeamRosterManager>();
        
        // Subscribe to TeamRosterManager events
        if (teamRosterManager != null)
        {
            teamRosterManager.OnRosterLoadingStarted += HandleRosterLoadStart; // Correct event name
            teamRosterManager.OnRosterLoadingFinished += HandleRosterLoadingFinished; // Use combined finished event
            // Removed separate LoadComplete and LoadFailed subscriptions

            // Subscribe to ProgressionSystem events if found
            if (progressionSystem != null)
            {
                ProgressionSystem.OnCharacterLevelUp += HandleCharacterLevelUp; // Use static event access
            }

            // Trigger the initial roster fetch
            // Check if roster data is already available or if loading is in progress
            // Use correct property 'TeamRoster' and check count
            if (teamRosterManager.TeamRoster != null && teamRosterManager.TeamRoster.Count > 0 && !teamRosterManager.IsLoadingRoster)
            {
                // Roster already loaded, populate immediately using the combined handler
                HandleRosterLoadingFinished(true, string.Empty); // Simulate successful load
            }
            else if (teamRosterManager.IsLoadingRoster)
            {
                 // Already loading, show loading state
                HandleRosterLoadStart();
            }
            else
            {
                // Not loaded and not loading, start fetch
                teamRosterManager.FetchRosterAsync();
            }
        }
        else
        {
            Debug.LogError("TeamRosterManager instance could not be found. Roster functionality disabled.", this);
            if(rosterStatusText != null) rosterStatusText.text = "Error: Could not connect to Roster Service.";
            SetButtonsInteractable(false); // Disable buttons if manager is missing
            if(refreshRosterButton != null) refreshRosterButton.interactable = false;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (teamRosterManager != null)
        {
            teamRosterManager.OnRosterLoadingStarted -= HandleRosterLoadStart; // Correct event name
            teamRosterManager.OnRosterLoadingFinished -= HandleRosterLoadingFinished; // Use combined finished event
            // Removed separate LoadComplete and LoadFailed unsubscriptions
        }

        if (progressionSystem != null)
        {
            ProgressionSystem.OnCharacterLevelUp -= HandleCharacterLevelUp; // Use static event access
        }

        // Clean up button listeners
        if (missionMapButton != null) missionMapButton.onClick.RemoveListener(OnMissionMapButtonClicked);
        if (refreshRosterButton != null) refreshRosterButton.onClick.RemoveListener(OnRefreshRosterClicked);
        if (createCharacterButton != null) createCharacterButton.onClick.RemoveListener(NavigateToCreateCharacter);
        // Note: CharacterSelectionButton listener wasn't added, so no removal needed unless added elsewhere.
    }

    #region Roster Event Handlers

    /// <summary>
    /// Called when the roster loading process begins.
    /// </summary>
    private void HandleRosterLoadStart()
    {
        if (loadingUIManager != null) loadingUIManager.ShowLoading("Loading Roster...");
        if (rosterStatusText != null) rosterStatusText.text = "Loading...";
        SetButtonsInteractable(false); // Disable buttons during load
        ClearRosterDisplay();
    }

    /// <summary>
    /// Called when the roster loading process finishes (successfully or not).
    /// </summary>
    /// <param name="success">Indicates if the loading was successful.</param>
    /// <param name="message">Error message if loading failed, otherwise empty.</param>
    private void HandleRosterLoadingFinished(bool success, string message)
    {
        if (loadingUIManager != null) loadingUIManager.HideLoading();
        SetButtonsInteractable(true); // Re-enable buttons

        if (success)
        {
            if (rosterStatusText != null) rosterStatusText.text = ""; // Clear status
            // Populate roster using the correct property and converting to List
            // Add logging to check the received roster data
            if (teamRosterManager != null)
            {
                if (teamRosterManager.TeamRoster == null)
                {
                    Debug.Log("GuildHallUIController: Received null TeamRoster from TeamRosterManager.");
                }
                else
                {
                    Debug.Log($"GuildHallUIController: Received TeamRoster with {teamRosterManager.TeamRoster.Count} characters from TeamRosterManager.");
                    // Optional: Log character names/IDs if needed for deeper debugging
                    // foreach(var character in teamRosterManager.TeamRoster) { Debug.Log($" - Character ID: {character?.characterId}, Name: {character?.name}"); }

                    // Populate roster using the correct property and converting to List
                    // Need using System.Linq; for ToList() - Ensure this using directive exists at the top
                    PopulateRoster(new List<CharacterData>(teamRosterManager.TeamRoster)); // Convert IReadOnlyCollection to List
                }
            }
            else
            {
                 // This case might be covered by the null check above, but keep for safety
                 Debug.LogError("TeamRosterManager is null after successful load notification.", this);
                 if (rosterStatusText != null) rosterStatusText.text = "Error: Roster data missing.";
                 ClearRosterDisplay();
            }
        }
        else
        {
            // Handle failure
            if (loadingUIManager != null) loadingUIManager.ShowError($"Roster Load Failed: {message}");
            if (rosterStatusText != null) rosterStatusText.text = $"Error: {message}";
            ClearRosterDisplay(); // Clear potentially stale data
        }
    }

    /// <summary>
    /// Called when a character levels up (after successful save). Updates the specific character's entry.
    /// </summary>
    /// <param name="updatedCharacter">The character data with updated level and stats.</param>
    private void HandleCharacterLevelUp(CharacterData updatedCharacter)
    {
        if (updatedCharacter == null)
        {
             Debug.LogWarning("HandleCharacterLevelUp received null character data.");
             return;
        }

        if (characterEntryMap.TryGetValue(updatedCharacter.characterId, out CharacterRosterEntry entry))
        {
            // Update UI elements using data from the event argument
            entry.UpdateLevel(updatedCharacter.level);
            entry.UpdateHP(updatedCharacter.hp, updatedCharacter.maxHp); // Update HP as well
            entry.UpdateEnergy(updatedCharacter.energy, updatedCharacter.maxEnergy); // Update Energy as well
        }
        else
        {
             Debug.LogWarning($"Received level up for character ID {updatedCharacter.characterId}, but no corresponding entry found in the UI map.");
        }
    }


    #endregion

    #region Roster UI Management

    /// <summary>
    /// Clears the current roster display and the tracking dictionary.
    /// </summary>
    private void ClearRosterDisplay()
    {
        if (rosterContainer == null) return;

        // Destroy existing entries
        foreach (Transform child in rosterContainer)
        {
            Destroy(child.gameObject);
        }
        characterEntryMap.Clear();
    }

    /// <summary>
    /// Instantiates and sets up roster entries based on the provided data.
    /// </summary>
    private void PopulateRoster(List<CharacterData> roster)
    {
        ClearRosterDisplay(); // Ensure clean slate

        if (rosterContainer == null || characterEntryPrefab == null)
        {
            Debug.LogError("Cannot populate roster - container or prefab is missing.", this);
            if (rosterStatusText != null) rosterStatusText.text = "UI Error: Cannot display roster.";
            return;
        }

        if (roster == null || roster.Count == 0)
        {
            if (rosterStatusText != null) rosterStatusText.text = "No characters in roster.";
            return; // Nothing to display
        }


        foreach (CharacterData character in roster)
        {
            if (character == null) continue; // Skip null entries

            GameObject entryGO = Instantiate(characterEntryPrefab, rosterContainer);
            CharacterRosterEntry entryScript = entryGO.GetComponent<CharacterRosterEntry>();

            if (entryScript != null)
            {
                entryScript.Setup(character);
                // Add to dictionary, handle potential duplicate IDs if necessary
                if (!characterEntryMap.ContainsKey(character.characterId))
                {
                    characterEntryMap.Add(character.characterId, entryScript);
                }
                else
                {
                     Debug.LogWarning($"Duplicate character ID '{character.characterId}' found while populating roster. Overwriting UI entry reference.", this);
                     // Decide how to handle duplicates - overwrite or ignore? Overwriting for now.
                     characterEntryMap[character.characterId] = entryScript;
                }
            }
            else
            {
                Debug.LogError("Instantiated Character Entry Prefab does not contain a CharacterRosterEntry script!", entryGO);
                Destroy(entryGO); // Clean up invalid prefab instance
            }
        }
    }

    #endregion


    /// <summary>
    /// Enables or disables common interactive buttons.
    /// </summary>
    /// <param name="interactable">Whether the buttons should be interactable</param>
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in interactableButtons)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }
    
    /// <summary>
    /// Handler for Refresh Roster button click
    /// </summary>
    private void OnRefreshRosterClicked()
    {
        if (teamRosterManager != null && !teamRosterManager.IsLoadingRoster)
        {
            teamRosterManager.FetchRosterAsync();
        }
    }

    /// <summary>
    /// Handler for Mission Map button click.
    /// Loads the Mission Map scene.
    /// </summary>
    private void OnMissionMapButtonClicked()
    {
        if (sceneLoader != null)
        {
            sceneLoader.LoadMissionMap();
        }
    }

    /// <summary>
    /// Loads the Create Character scene.
    /// This method should be linked to a UI button in the Unity Editor.
    /// </summary>
    public void NavigateToCreateCharacter()
    {
        SceneManager.LoadScene("CreateCharacter");
    }

    // OnDestroy method moved up and updated earlier in the diff
}