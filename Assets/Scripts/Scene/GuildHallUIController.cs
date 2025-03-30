using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Handles UI interactions for the Guild Hall scene.
/// </summary>
public class GuildHallUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button missionMapButton;
    [SerializeField] private Button characterSelectionButton;
    [SerializeField] private Button refreshRosterButton;
    
    [Header("Managers")]
    // TeamRosterManager is found dynamically as it persists across scenes
    private TeamRosterManager teamRosterManager;
    [SerializeField] private LoadingUIManager loadingUIManager;
    
    private SceneLoader sceneLoader;
    private List<Button> interactableButtons = new List<Button>();

    private void Start()
    {
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

        // Find the TeamRosterManager instance dynamically
        teamRosterManager = FindFirstObjectByType<TeamRosterManager>();
        
        // Subscribe to team roster loading events if found
        if (teamRosterManager != null)
        {
            teamRosterManager.OnRosterLoadingStarted += OnRosterLoadingStarted;
            teamRosterManager.OnRosterLoadingFinished += OnRosterLoadingFinished;

            // Trigger the initial roster fetch when entering the Guild Hall
            teamRosterManager.FetchRosterAsync();
            
            // If roster is already loading when we initialize, update UI accordingly (this handles re-entry while loading)
            if (teamRosterManager.IsLoadingRoster)
            {
                OnRosterLoadingStarted();
            }
        }
        else
        {
            Debug.LogError("TeamRosterManager instance could not be found in the scene. Ensure it is correctly initialized and persists.");
        }
    }
    
    /// <summary>
    /// Handler for when the roster loading starts
    /// </summary>
    private void OnRosterLoadingStarted()
    {
        // Show loading indicator
        if (loadingUIManager != null)
        {
            loadingUIManager.ShowLoading("Loading team roster...");
        }
        
        // Disable interactive buttons
        SetButtonsInteractable(false);
    }
    
    /// <summary>
    /// Handler for when the roster loading finishes
    /// </summary>
    /// <param name="success">Whether the loading was successful</param>
    /// <param name="errorMessage">Error message if loading failed</param>
    private void OnRosterLoadingFinished(bool success, string errorMessage)
    {
        // Hide loading indicator
        if (loadingUIManager != null)
        {
            loadingUIManager.HideLoading();
        }
        
        if (success)
        {
            // Re-enable interactive buttons
            SetButtonsInteractable(true);
        }
        else
        {
            // Show error message
            if (loadingUIManager != null && !string.IsNullOrEmpty(errorMessage))
            {
                loadingUIManager.ShowError(errorMessage);
            }
            
            // Keep buttons disabled until retry or user dismisses error
        }
    }
    
    /// <summary>
    /// Enables or disables all interactive buttons
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

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (teamRosterManager != null)
        {
            teamRosterManager.OnRosterLoadingStarted -= OnRosterLoadingStarted;
            teamRosterManager.OnRosterLoadingFinished -= OnRosterLoadingFinished;
        }
        
        // Clean up listeners when the object is destroyed
        if (missionMapButton != null)
        {
            missionMapButton.onClick.RemoveListener(OnMissionMapButtonClicked);
        }
        
        if (refreshRosterButton != null)
        {
            refreshRosterButton.onClick.RemoveListener(OnRefreshRosterClicked);
        }
    }
}