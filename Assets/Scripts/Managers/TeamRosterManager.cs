using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game01.Data;

/// <summary>
/// TeamRosterManager manages the player's team of characters.
/// It relies on the GameManager's singleton pattern and persistence mechanism.
/// </summary>
public class TeamRosterManager : MonoBehaviour
{
    // Dictionary to store all characters in the player's roster using characterId as key
    private Dictionary<string, CharacterData> teamRoster = new Dictionary<string, CharacterData>();
    
    // NetworkManager reference obtained from GameManager
    private NetworkManager networkManager;

    // Public property to access the team roster
    public IReadOnlyCollection<CharacterData> TeamRoster => teamRoster.Values;
    
    // Events for loading state changes
    public event Action OnRosterLoadingStarted;
    public event Action<bool, string> OnRosterLoadingFinished; // bool: success, string: error message if any
    
    // Loading state flag
    private bool isLoadingRoster = false;
    public bool IsLoadingRoster => isLoadingRoster;

    private void Awake()
    {
        // Get NetworkManager reference directly from its singleton instance
        networkManager = NetworkManager.Instance;
        if (networkManager == null)
        {
            // This might happen if TeamRosterManager's Awake runs before NetworkManager's Awake.
            // Consider using Start() or ensuring script execution order if this becomes an issue.
            Debug.LogError("NetworkManager.Instance is null in TeamRosterManager.Awake. Check script execution order or initialization timing.");
        }
    }

    private void Start()
    {
        // Fetch roster from server on startup if NetworkManager is available
        // FetchRosterAsync(); // Removed: Roster should be fetched explicitly when needed (e.g., entering Guild Hall)
    }

    /// <summary>
    /// Fetches the player's roster from the server asynchronously
    /// </summary>
    public void FetchRosterAsync()
    {
        if (isLoadingRoster || networkManager == null)
            return;
            
        isLoadingRoster = true;
        OnRosterLoadingStarted?.Invoke();
        
        StartCoroutine(networkManager.WorkspaceRoster(OnRosterFetched));
    }

    /// <summary>
    /// Callback for when the roster is fetched from the server
    /// </summary>
    /// <param name="characters">List of character data fetched from the server</param>
    private void OnRosterFetched(List<CharacterData> characters)
    {
        isLoadingRoster = false;
        
        if (characters == null)
        {
            Debug.LogError("Failed to fetch character roster from server");
            OnRosterLoadingFinished?.Invoke(false, "Failed to fetch team roster. Please check your connection.");
            return;
        }

        // Clear existing roster and populate with fetched data
        teamRoster.Clear();
        
        foreach (CharacterData character in characters)
        {
            if (!string.IsNullOrEmpty(character.characterId))
            {
                teamRoster[character.characterId] = character;
                Debug.Log($"Added character: {character.CharacterName} (ID: {character.characterId}) to the team roster");
            }
            else
            {
                Debug.LogWarning($"Character {character.CharacterName} has no ID and was not added to roster");
            }
        }
        
        // Notify that loading has finished successfully
        OnRosterLoadingFinished?.Invoke(true, string.Empty);
    }

    /// <summary>
    /// Adds a new character to the team roster and saves it to the server
    /// </summary>
    /// <param name="character">The CharacterData instance to add</param>
    public void AddCharacter(CharacterData character)
    {
        if (character == null)
        {
            Debug.LogWarning("Attempted to add a null character to the team roster");
            return;
        }

        if (string.IsNullOrEmpty(character.characterId))
        {
            Debug.LogWarning("Attempted to add a character without an ID to the team roster");
            return;
        }

        // Add to local dictionary
        teamRoster[character.characterId] = character;
        Debug.Log($"Added character: {character.CharacterName} (ID: {character.characterId}) to the team roster");

        // Save to server
        SaveCharacterData(character);
    }

    /// <summary>
    /// Handles character level-up and saves the updated data to the server
    /// </summary>
    /// <param name="characterId">ID of the character to level up</param>
    /// <param name="newLevel">New level value</param>
    /// <param name="newXP">New XP value</param>
    /// <param name="newMaxHP">New MaxHP value</param>
    /// <param name="newMaxEnergy">New MaxEnergy value</param>
    public void LevelUpCharacter(string characterId, int newLevel, int newXP, int newMaxHP, int newMaxEnergy)
    {
        if (!teamRoster.TryGetValue(characterId, out CharacterData character))
        {
            Debug.LogWarning($"Character with ID {characterId} not found for level-up");
            return;
        }

        // Update character data
        character.CurrentLevel = newLevel;
        character.CurrentXP = newXP;
        character.MaxHP = newMaxHP;
        character.MaxEnergy = newMaxEnergy;

        Debug.Log($"Leveled up character: {character.CharacterName} (ID: {characterId}) to level {newLevel}");

        // Save to server
        SaveCharacterData(character);
    }

    /// <summary>
    /// Adds a passive skill to a character and saves the updated data to the server
    /// </summary>
    /// <param name="characterId">ID of the character to add the passive skill to</param>
    /// <param name="passiveSkill">The passive skill to add</param>
    public void AddPassiveSkill(string characterId, PassiveSkill_SO passiveSkill)
    {
        if (!teamRoster.TryGetValue(characterId, out CharacterData character))
        {
            Debug.LogWarning($"Character with ID {characterId} not found for adding passive skill");
            return;
        }

        if (passiveSkill == null)
        {
            Debug.LogWarning("Attempted to add a null passive skill");
            return;
        }

        // Add the passive skill
        character.LearnedPassives.Add(passiveSkill);
        Debug.Log($"Added passive skill to character: {character.CharacterName} (ID: {characterId})");

        // Save to server
        SaveCharacterData(character);
    }

    /// <summary>
    /// Saves character data to the server with retry logic
    /// </summary>
    /// <param name="character">Character data to save</param>
    private void SaveCharacterData(CharacterData character)
    {
        StartCoroutine(SaveCharacterDataWithRetry(character));
    }

    /// <summary>
    /// Helper method for saving character data with one retry attempt
    /// </summary>
    /// <param name="character">Character data to save</param>
    private IEnumerator SaveCharacterDataWithRetry(CharacterData character)
    {
        bool saveSuccess = false;
        
        // First attempt
        yield return networkManager.SaveCharacterData(character, success => saveSuccess = success);
        
        // If first attempt failed, retry once
        if (!saveSuccess)
        {
            Debug.LogWarning($"First attempt to save character data failed for {character.CharacterName} (ID: {character.characterId}). Retrying...");
            yield return networkManager.SaveCharacterData(character, success => saveSuccess = success);
            
            // If retry also failed, notify user
            if (!saveSuccess)
            {
                Debug.LogError($"Failed to sync progress with server for character {character.CharacterName} (ID: {character.characterId}) after retry");
                // Show notification to user (in a real implementation, this would use a UI manager or similar)
                Debug.LogError("Failed to sync progress with server. Please check connection.");
            }
        }
    }

    /// <summary>
    /// Gets a character from the team roster by ID
    /// </summary>
    /// <param name="characterId">The ID of the character to find</param>
    /// <returns>The CharacterData with matching ID, or null if not found</returns>
    public CharacterData GetCharacter(string characterId)
    {
        if (teamRoster.TryGetValue(characterId, out CharacterData character))
        {
            return character;
        }
        
        Debug.LogWarning($"Character with ID {characterId} not found");
        return null;
    }

    /// <summary>
    /// Gets a character from the team roster by name
    /// </summary>
    /// <param name="name">The name of the character to find</param>
    /// <returns>The first CharacterData with matching name, or null if not found</returns>
    public CharacterData GetCharacterByName(string name)
    {
        foreach (CharacterData character in teamRoster.Values)
        {
            if (character.CharacterName == name)
            {
                return character;
            }
        }
        
        return null;
    }

    /// <summary>
    /// Gets the current size of the team roster
    /// </summary>
    /// <returns>The number of characters in the team roster</returns>
    public int GetRosterSize()
    {
        return teamRoster.Count;
    }

    /// <summary>
    /// Removes a character from the team roster by ID
    /// </summary>
    /// <param name="characterId">The ID of the character to remove</param>
    /// <returns>True if removal was successful, false otherwise</returns>
    public bool RemoveCharacter(string characterId)
    {
        if (teamRoster.TryGetValue(characterId, out CharacterData character))
        {
            string name = character.CharacterName;
            teamRoster.Remove(characterId);
            Debug.Log($"Removed character: {name} (ID: {characterId}) from the team roster");
            return true;
        }
        
        Debug.LogWarning($"Failed to remove character. ID {characterId} not found");
        return false;
    }
}