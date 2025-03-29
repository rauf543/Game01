using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI interactions for the Main Menu scene.
/// </summary>
public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private Button startButton;
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
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogError("Start Button reference not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Handler for Start button click.
    /// Loads the Guild Hall scene.
    /// </summary>
    private void OnStartButtonClicked()
    {
        if (sceneLoader != null)
        {
            sceneLoader.LoadGuildHall();
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners when the object is destroyed
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }
    }
}