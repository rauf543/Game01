using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI interactions for the Guild Hall scene.
/// </summary>
public class GuildHallUIController : MonoBehaviour
{
    [SerializeField] private Button missionMapButton;
    private SceneLoader sceneLoader;

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
        
        // Set up button click event
        if (missionMapButton != null)
        {
            missionMapButton.onClick.AddListener(OnMissionMapButtonClicked);
        }
        else
        {
            Debug.LogError("Mission Map Button reference not assigned in the Inspector.");
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
        // Clean up listeners when the object is destroyed
        if (missionMapButton != null)
        {
            missionMapButton.onClick.RemoveListener(OnMissionMapButtonClicked);
        }
    }
}