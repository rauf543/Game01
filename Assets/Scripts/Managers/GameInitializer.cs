using UnityEngine;

/// <summary>
/// Helper class to initialize game managers in scenes.
/// Place this in each scene to ensure proper initialization.
/// </summary>
public class GameInitializer : MonoBehaviour
{
    // Prefab references
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject sceneLoaderPrefab;
    
    // Reference to SceneLoader component for easy access
    private SceneLoader _sceneLoader;
    public SceneLoader SceneLoader => _sceneLoader;

    private void Awake()
    {
        // Create GameManager if it doesn't exist
        if (GameManager.Instance == null)
        {
            if (gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab);
            }
            else
            {
                Debug.LogWarning("GameManager prefab reference not set in GameInitializer.");
                GameObject gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent<GameManager>();
            }
        }

        // Create SceneLoader
        if (sceneLoaderPrefab != null)
        {
            GameObject sceneLoaderObj = Instantiate(sceneLoaderPrefab);
            _sceneLoader = sceneLoaderObj.GetComponent<SceneLoader>();
        }
        else
        {
            Debug.LogWarning("SceneLoader prefab reference not set in GameInitializer.");
            GameObject sceneLoaderObj = new GameObject("SceneLoader");
            _sceneLoader = sceneLoaderObj.AddComponent<SceneLoader>();
        }
    }
}