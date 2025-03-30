using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    // Singleton instance
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            // Logic to handle instance creation if needed, though typically initialized by GameInitializer
            if (_instance == null)
            {
                Debug.LogError("NetworkManager instance is null. Ensure it's initialized properly, likely via GameInitializer loading the prefab.");
            }
            return _instance;
        }
    }

    // BackendConfig should be assigned in the prefab Inspector
    [SerializeField] private BackendConfig backendConfig;

    // Store the authenticated user ID after successful login
    private string authenticatedUserId;

    private void Awake()
    {
        // Singleton pattern: Ensure only one instance exists
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Make this instance persistent across scenes

            // Optional: Validate BackendConfig assignment
            if (backendConfig == null)
            {
                 Debug.LogError("BackendConfig is not assigned to the NetworkManager prefab in the Inspector!");
            }
        }
        else if (_instance != this)
        {
            // If another instance already exists, destroy this one
            Debug.LogWarning("Duplicate NetworkManager instance found. Destroying the new one.");
            Destroy(gameObject);
        }
    }

    // Login response structure
    [Serializable]
    private class LoginResponse
    {
        public bool success;
        public string UserID; // Changed field name to match backend JSON key
    }

    // Login result structure
    public class LoginResult
    {
        public bool success;
        public string userId;
        public string errorMessage;
    }

    // WorkspaceRoster response wrapper for JsonUtility
    [Serializable]
    private class CharacterDataListWrapper
    {
        public List<CharacterData> characters;
    }

    // SetBackendConfig removed as config is now set via prefab

    /// <summary>
    /// Login to the backend service
    /// </summary>
    /// <param name="username">User's username</param>
    /// <param name="password">User's password</param>
    /// <returns>Coroutine that returns a LoginResult</returns>
    public IEnumerator Login(string username, string password, Action<LoginResult> callback)
    {
        // Create form data
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        // Ensure trailing slash on baseUrl if not present, then append path
        string baseUrl = backendConfig.baseUrl.EndsWith("/") ? backendConfig.baseUrl : backendConfig.baseUrl + "/";
        string url = $"{baseUrl}api/auth/login";
        
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            LoginResult result = new LoginResult();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Login error: {request.error} - URL: {url} - Response: {request.downloadHandler.text}");
                result.success = false;
                result.errorMessage = request.error;
            }
            else
            {
                try
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                    result.success = response.success;
                    result.userId = response.UserID; // Use the corrected field name

                    // Store the user ID if login was successful
                    if (result.success && !string.IsNullOrEmpty(result.userId))
                    {
                        authenticatedUserId = result.userId;
                        Debug.Log($"[NetworkManager] Login Success: Stored authenticatedUserId = {authenticatedUserId}"); // DEBUG LOG
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON parsing error: {e.Message} - Response: {request.downloadHandler.text}");
                    result.success = false;
                    result.errorMessage = "Failed to parse response";
                }
            }

            callback(result);
        }
    }

    /// <summary>
    /// Retrieve the workspace roster from the backend
    /// </summary>
    /// <returns>Coroutine that returns a List of CharacterData or null on failure</returns>
    public IEnumerator WorkspaceRoster(Action<List<CharacterData>> callback)
    {
        // Ensure trailing slash on baseUrl if not present, then append path
        string baseUrl = backendConfig.baseUrl.EndsWith("/") ? backendConfig.baseUrl : backendConfig.baseUrl + "/";
        string url = $"{baseUrl}api/player/roster"; // Corrected endpoint path
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Add authentication header if user is logged in
            Debug.Log($"[NetworkManager] WorkspaceRoster: Checking authenticatedUserId = {authenticatedUserId}"); // DEBUG LOG
            if (!string.IsNullOrEmpty(authenticatedUserId))
            {
                Debug.Log($"[NetworkManager] WorkspaceRoster: Setting X-User-ID header to {authenticatedUserId}"); // DEBUG LOG
                request.SetRequestHeader("X-User-ID", authenticatedUserId);
            }
            else
            {
                Debug.LogWarning("WorkspaceRoster called but no user is authenticated. Request might fail.");
                // Optionally, you could prevent the request here or handle the unauthenticated state
            }

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"WorkspaceRoster error: {request.error} - URL: {url} - Response: {request.downloadHandler.text}");
                callback(null);
            }
            else
            {
                try
                {
                    // Since JsonUtility can't deserialize a JSON array directly, we need to wrap it
                    // Assuming the response is in the format: {"characters":[...]}
                    CharacterDataListWrapper wrapper = JsonUtility.FromJson<CharacterDataListWrapper>(request.downloadHandler.text);
                    callback(wrapper.characters);
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON parsing error: {e.Message} - Response: {request.downloadHandler.text}");
                    callback(new List<CharacterData>());
                }
            }
        }
    }

    /// <summary>
    /// Save character data to the backend
    /// </summary>
    /// <param name="characterData">The character data to save</param>
    /// <returns>Coroutine that returns true on success, false on failure</returns>
    public IEnumerator SaveCharacterData(CharacterData characterData, Action<bool> callback)
    {
        // Ensure trailing slash on baseUrl if not present, then append path
        string baseUrl = backendConfig.baseUrl.EndsWith("/") ? backendConfig.baseUrl : backendConfig.baseUrl + "/";
        string url = $"{baseUrl}api/player/save_character_data";
        
        // Convert CharacterData to JSON
        string jsonData = JsonUtility.ToJson(characterData);
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"SaveCharacterData error: {request.error} - URL: {url} - Response: {request.downloadHandler.text}");
                callback(false);
            }
            else
            {
                callback(true);
            }
        }
    }
}