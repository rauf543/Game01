using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles UI interactions for the Main Menu scene.
/// </summary>
public class MainMenuUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    
    [Header("Managers")]
    // NetworkManager is now obtained from the persistent GameManager
    private NetworkManager networkManager;
    [SerializeField] private LoadingUIManager loadingUIManager;
    
    private SceneLoader sceneLoader;
    private bool isLoggingIn = false;

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

        // Get NetworkManager instance
        networkManager = NetworkManager.Instance;
        if (networkManager == null)
        {
             Debug.LogError("NetworkManager instance not found. Ensure the prefab is loaded correctly.");
        }
        
        // Set up button click events
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogError("Start Button reference not assigned in the Inspector.");
        }
        
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }
        else
        {
            Debug.LogError("Login Button reference not assigned in the Inspector.");
        }
    }
    
    /// <summary>
    /// Handler for Login button click.
    /// </summary>
    private void OnLoginButtonClicked()
    {
        if (isLoggingIn || networkManager == null || loadingUIManager == null)
            return;
            
        string username = usernameInput != null ? usernameInput.text : "";
        string password = passwordInput != null ? passwordInput.text : "";
        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            loadingUIManager.ShowError("Please enter username and password.");
            return;
        }
        
        // Set logging in state
        isLoggingIn = true;
        
        // Show loading indicator
        loadingUIManager.ShowLoading("Logging in...");
        
        // Disable login button
        if (loginButton != null)
            loginButton.interactable = false;
            
        // Start login process
        StartCoroutine(networkManager.Login(username, password, OnLoginComplete));
    }
    
    /// <summary>
    /// Callback for login completion
    /// </summary>
    private void OnLoginComplete(NetworkManager.LoginResult result)
    {
        isLoggingIn = false;
        
        // Re-enable login button
        if (loginButton != null)
            loginButton.interactable = true;
            
        // Hide loading indicator
        loadingUIManager.HideLoading();
        
        if (result.success)
        {
            // Login successful
            Debug.Log($"Login successful for user ID: {result.userId}");
            
            // Enable start button after successful login
            if (startButton != null) {
                startButton.interactable = true;
                Debug.Log("Start button enabled - interactable set to true");
            } else {
                Debug.LogError("Start button reference is null! Please assign it in the Inspector.");
            }
        }
        else
        {
            // Login failed - show error message
            string errorMsg = string.IsNullOrEmpty(result.errorMessage)
                ? "Login Failed. Please check your connection."
                : $"Login Failed: {result.errorMessage}";
                
            loadingUIManager.ShowError(errorMsg);
            
            Debug.LogWarning($"Login failed: {errorMsg}");
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
        
        if (loginButton != null)
        {
            loginButton.onClick.RemoveListener(OnLoginButtonClicked);
        }
    }
}