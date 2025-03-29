using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Demo script for testing scene transitions.
/// Provides UI buttons to switch between different scenes.
/// </summary>
public class SceneTransitionDemo : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button guildHallButton;
    [SerializeField] private Button missionMapButton;
    [SerializeField] private Button combatButton;
    [SerializeField] private Toggle inMissionToggle;

    private SceneLoader sceneLoader;
    private GameManager gameManager;

    private void Start()
    {
        // Find references
        sceneLoader = FindFirstObjectByType<SceneLoader>();
        gameManager = GameManager.Instance;

        if (sceneLoader == null)
        {
            Debug.LogError("SceneLoader not found. Scene transitions won't work.");
        }

        // Set up button listeners
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(() => {
                if (sceneLoader != null) sceneLoader.LoadMainMenu();
            });
        }

        if (guildHallButton != null)
        {
            guildHallButton.onClick.AddListener(() => {
                if (sceneLoader != null) sceneLoader.LoadGuildHall();
            });
        }

        if (missionMapButton != null)
        {
            missionMapButton.onClick.AddListener(() => {
                if (sceneLoader != null) sceneLoader.LoadMissionMap();
            });
        }

        if (combatButton != null)
        {
            combatButton.onClick.AddListener(() => {
                if (sceneLoader != null) 
                {
                    // Set mission state when entering combat
                    if (gameManager != null) gameManager.EnterMission();
                    sceneLoader.LoadCombat();
                }
            });
        }

        // Set up mission toggle
        if (inMissionToggle != null && gameManager != null)
        {
            // Set initial state
            inMissionToggle.isOn = gameManager.IsInMission;
            
            // Set up toggle listener
            inMissionToggle.onValueChanged.AddListener((value) => {
                if (gameManager != null)
                {
                    if (value)
                    {
                        gameManager.EnterMission();
                    }
                    else
                    {
                        gameManager.ExitMission();
                    }
                }
            });
        }
    }

    private void OnEnable()
    {
        // Update toggle state if it exists
        if (inMissionToggle != null && gameManager != null)
        {
            inMissionToggle.isOn = gameManager.IsInMission;
        }
    }
}