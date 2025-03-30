# Task CS-04-Refactor: Online Team Roster Management - Implementation Summary

## Overview
This implementation fulfills the requirements of task CS-04-Refactor by refactoring the TeamRosterManager to fetch player roster data from the backend server and implement save functionality for permanent character updates. The refactoring eliminates local in-memory storage in favor of server-based persistence, ensuring that character data is synchronized with the backend whenever critical changes occur.

## Files Modified

1. **Assets/Scripts/Data/CharacterData.cs**
   - Added a `characterId` field to uniquely identify characters
   - Added `[Serializable]` attribute to ensure proper JSON serialization

2. **Assets/Scripts/Managers/TeamRosterManager.cs**
   - Refactored to use a Dictionary with characterId as key
   - Removed local in-memory storage and mock character creation
   - Implemented server data fetching on startup
   - Added save logic for character creation, level-up, and passive skill acquisition
   - Implemented retry mechanism and error handling for network operations

## Key Code Excerpts

### CharacterData Updates
```csharp
[Serializable]
public class CharacterData {
    public string characterId;  // Unique ID for the character
    public string CharacterName;
    public int CurrentLevel;
    public int CurrentXP;
    public int CurrentHP;
    public int CurrentEnergy;
    public int MaxHP;
    public int MaxEnergy;
    public List<PassiveSkill_SO> LearnedPassives;
    public List<CardDefinition_SO> CurrentRunDeck;
}
```

### TeamRosterManager Fetch Implementation
```csharp
// Dictionary to store all characters in the player's roster using characterId as key
private Dictionary<string, CharacterData> teamRoster = new Dictionary<string, CharacterData>();

[SerializeField] private NetworkManager networkManager;

private void Start()
{
    // Fetch roster from server on startup
    FetchRoster();
}

/// <summary>
/// Fetches the player's roster from the server
/// </summary>
private void FetchRoster()
{
    StartCoroutine(networkManager.WorkspaceRoster(OnRosterFetched));
}

/// <summary>
/// Callback for when the roster is fetched from the server
/// </summary>
/// <param name="characters">List of character data fetched from the server</param>
private void OnRosterFetched(List<CharacterData> characters)
{
    if (characters == null)
    {
        Debug.LogError("Failed to fetch character roster from server");
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
}
```

### Save Logic with Retry
```csharp
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
            Debug.LogError("Failed to sync progress with server. Please check connection.");
        }
    }
}
```

## How Requirements Were Met

### 1. Remove Local In-Memory Storage Logic
- Eliminated the `CreateDefaultCharacters()` method that created placeholder characters
- Removed code that populated the roster with mock characters locally
- Changed from using a List to a Dictionary with characterId as key

### 2. Fetch Roster via NetworkManager
- Implemented `FetchRoster()` method that calls `NetworkManager.WorkspaceRoster()`
- Added `OnRosterFetched()` callback to handle server response
- Stored fetched CharacterData in a Dictionary using characterId as key
- Roster is fetched automatically on startup in the `Start()` method

### 3. Implement Save Logic
- **Trigger Points**
  - Added save calls in `AddCharacter()` for character creation
  - Created `LevelUpCharacter()` method with save functionality
  - Created `AddPassiveSkill()` method with save functionality
- **Partial Updates**
  - Each save operation only sends data for the single modified character
  - Saves are performed individually at the time of change
- **Network Error Handling**
  - Implemented `SaveCharacterDataWithRetry()` with one immediate retry attempt
  - Added error logging and user notification for failed save attempts
  - No rollback logic as per requirements

## Design Decisions

### Dictionary Data Structure
Used a Dictionary with characterId as key instead of a List for efficient character lookup by ID and to better align with server-side data retrieval patterns. This provides O(1) access time when looking up characters by ID.

### TeamRoster Property Changed to IReadOnlyCollection
Changed the public property `TeamRoster` to return an `IReadOnlyCollection<CharacterData>` instead of direct access to the list. This provides abstraction over the underlying data structure while still allowing iteration through characters.

### Specialized Methods for Each Update Type
Created separate methods for different update types (AddCharacter, LevelUpCharacter, AddPassiveSkill) instead of a generic update method. This approach:
1. Makes the API more intuitive for other developers
2. Ensures save operations are properly triggered for each specific update type
3. Provides clear entry points for future expansion of functionality

### Coroutine-Based Network Handling
Used coroutines for network operations to handle the asynchronous nature of network requests without blocking the main thread. This ensures the game remains responsive during network operations.

## Verification
The refactored TeamRosterManager now fetches all character data from the server at startup, eliminating reliance on local placeholders. Each character update (creation, level-up, passive skill acquisition) triggers an immediate save to the server with appropriate retry logic and error handling. The implementation follows the specified requirements while maintaining the original functionality expected from the TeamRosterManager.