using UnityEngine;

/// <summary>
/// GameManager implemented as a Singleton pattern to persist across scene loads.
/// Manages high-level game state and references to core managers.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager _instance;
    
    // Public property to access the singleton instance
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager instance not found. Ensure it exists in the scene.");
            }
            return _instance;
        }
    }

    // Reference to the TeamRosterManager
    [SerializeField] private TeamRosterManager teamRosterManager;
    public TeamRosterManager TeamRoster => teamRosterManager;

    // NetworkManager prefab reference removed, using Resources.Load now

    // Flag to track if player is currently in a mission
    private bool _isInMission = false;
    public bool IsInMission 
    { 
        get => _isInMission; 
        set => _isInMission = value; 
    }

    private void Awake()
    {
        // Ensure singleton pattern - only one GameManager exists
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeComponents();
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initialize and find required components if not set
    /// </summary>
    private void InitializeComponents()
    {
        // Ensure NetworkManager singleton instance exists by loading and instantiating the prefab if needed
        if (NetworkManager.Instance == null)
        {
            // Load the prefab from "Assets/Resources/Prefabs/NetworkManager.prefab"
            GameObject networkManagerPrefab = Resources.Load<GameObject>("Prefabs/NetworkManager");
            if (networkManagerPrefab != null)
            {
                // Instantiate the prefab. The NetworkManager's Awake method will handle setting the instance and DontDestroyOnLoad.
                Instantiate(networkManagerPrefab);
                Debug.Log("NetworkManager prefab instantiated.");
            }
            else
            {
                Debug.LogError("Failed to load NetworkManager prefab from Resources/Prefabs/Managers. Ensure the prefab exists at the correct path.");
            }
        }
        // NetworkManager.Instance should now be available for other managers

        // Find TeamRosterManager if not already assigned
        if (teamRosterManager == null)
        {
            teamRosterManager = GetComponentInChildren<TeamRosterManager>();
            
            // If still not found, try to find in scene
            if (teamRosterManager == null)
            {
                teamRosterManager = FindFirstObjectByType<TeamRosterManager>();
                
                // If not found in scene, create a new instance
                if (teamRosterManager == null)
                {
                    GameObject teamRosterObj = new GameObject("TeamRosterManager");
                    teamRosterObj.transform.SetParent(transform);
                    // TeamRosterManager's Awake will now correctly find the initialized NetworkManager
                    teamRosterManager = teamRosterObj.AddComponent<TeamRosterManager>();
                }
            }
        }
    }

    /// <summary>
    /// Set the mission state to indicate player has entered a mission
    /// </summary>
    public void EnterMission()
    {
        _isInMission = true;
    }

    /// <summary>
    /// Set the mission state to indicate player has exited a mission
    /// </summary>
    public void ExitMission()
    {
        _isInMission = false;
    }
}