using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private BackendConfig backendConfig;

    // Login response structure
    [Serializable]
    private class LoginResponse
    {
        public bool success;
        public string userId;
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

        string url = $"{backendConfig.baseUrl}/login";
        
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
                    result.userId = response.userId;
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
        string url = $"{backendConfig.baseUrl}/workspace_roster";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
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
        string url = $"{backendConfig.baseUrl}/save_character_data";
        
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