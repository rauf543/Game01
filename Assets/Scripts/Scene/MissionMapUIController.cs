using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI interactions for the Mission Map scene.
/// </summary>
public class MissionMapUIController : MonoBehaviour
{
    [SerializeField] private Button startCombatButton;
    [SerializeField] private Button returnToGuildHallButton;
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
        
        // Set up button click events
        if (startCombatButton != null)
        {
            startCombatButton.onClick.AddListener(OnStartCombatButtonClicked);
        }
        else
        {
            Debug.LogError("Start Combat Button reference not assigned in the Inspector.");
        }

        if (returnToGuildHallButton != null)
        {
            returnToGuildHallButton.onClick.AddListener(OnReturnToGuildHallButtonClicked);
        }
        else
        {
            Debug.LogError("Return to Guild Hall Button reference not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Handler for Start Combat button click.
    /// Loads the Combat scene.
    /// </summary>
    private void OnStartCombatButtonClicked()
    {
        if (sceneLoader != null)
        {
            sceneLoader.LoadCombat();
        }
    }

    /// <summary>
    /// Handler for Return to Guild Hall button click.
    /// Loads the Guild Hall scene.
    /// </summary>
    private void OnReturnToGuildHallButtonClicked()
    {
        if (sceneLoader != null)
        {
            sceneLoader.LoadGuildHall();
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners when the object is destroyed
        if (startCombatButton != null)
        {
            startCombatButton.onClick.RemoveListener(OnStartCombatButtonClicked);
        }

        if (returnToGuildHallButton != null)
        {
            returnToGuildHallButton.onClick.RemoveListener(OnReturnToGuildHallButtonClicked);
        }
    }
}