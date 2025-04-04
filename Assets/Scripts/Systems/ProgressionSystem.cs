using UnityEngine;
using System; // Required for System.Action

// Placeholder for CharacterData structure - Assume it exists elsewhere
public class CharacterData
{
    public string CharacterID; // Assuming an ID for logging
    public int CurrentLevel;
    public int CurrentXP;
    public int MaxHP;
    public int MaxEnergy;
    // Other character properties...
}

// Placeholder for LevelXPData_SO structure - Assume it exists elsewhere
public class LevelXPData_SO : ScriptableObject
{
    // Assuming a method or data structure to get XP required for a level.
    // Returns -1 or throws an exception if level is invalid/max level exceeded.
    public int GetXPRequired(int level)
    {
        // Example implementation: Replace with actual logic
        if (level <= 0) return 0;
        if (level > 50) return -1; // Example max level
        return level * 100; // Simple formula for demonstration
    }
}

// Placeholder for TeamRosterManager - Assume it exists elsewhere
public class TeamRosterManager : MonoBehaviour
{
    public static TeamRosterManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Only if needed
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Assume this method exists and handles saving data, then calls the callback.
    public void SaveCharacterData(CharacterData character, Action<bool, CharacterData> onComplete)
    {
        Debug.Log($"Attempting to save data for Character ID: {character.CharacterID}");
        // Simulate async save operation
        // In a real scenario, this would involve network calls etc.
        bool success = UnityEngine.Random.value > 0.1f; // Simulate 90% success rate
        onComplete?.Invoke(success, character);
    }
}


namespace Game.Systems // Assuming a namespace, adjust if needed
{
    public class ProgressionSystem : MonoBehaviour
    {
        #region Singleton
        public static ProgressionSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject); // Not strictly needed unless coroutines span scene loads
            }
            else
            {
                Debug.LogWarning("Duplicate ProgressionSystem instance found. Destroying duplicate.");
                Destroy(gameObject);
            }
        }
        #endregion

        // Reference to the Scriptable Object defining XP per level.
        // Assign this in the Unity Inspector.
        [Tooltip("Assign the LevelXPData Scriptable Object here.")]
        public LevelXPData_SO levelXPData;

        // Event raised when a character levels up and the data is successfully saved.
        public static event System.Action<CharacterData> OnCharacterLevelUp;

        /// <summary>
        /// Grants a specified amount of XP to a character and checks for level ups.
        /// </summary>
        /// <param name="character">The character receiving XP.</param>
        /// <param name="xpAmount">The amount of XP to grant.</param>
        public void GrantXP(CharacterData character, int xpAmount)
        {
            if (character == null)
            {
                Debug.LogError("ProgressionSystem: Cannot grant XP to a null character.");
                return;
            }
            if (xpAmount <= 0)
            {
                Debug.LogWarning($"ProgressionSystem: Attempted to grant non-positive XP ({xpAmount}) to character {character.CharacterID}. Ignoring.");
                return;
            }

            character.CurrentXP += xpAmount;
            Debug.Log($"Character {character.CharacterID} granted {xpAmount} XP. Current XP: {character.CurrentXP}");

            CheckLevelUp(character);
        }

        /// <summary>
        /// Checks if the character has enough XP to level up, handles multi-level ups,
        /// calculates new stats, and initiates the save process.
        /// </summary>
        /// <param name="character">The character to check.</param>
        private void CheckLevelUp(CharacterData character)
        {
            if (character == null)
            {
                Debug.LogError("ProgressionSystem: Cannot check level up for a null character.");
                return;
            }

            if (levelXPData == null)
            {
                Debug.LogError("ProgressionSystem: LevelXPData_SO reference not set in the Inspector!");
                return;
            }

            bool leveledUpThisCheck = false; // Flag to ensure save is only called once if multiple levels gained

            // Loop to handle multiple level ups from a single XP grant
            while (true)
            {
                int nextLevel = character.CurrentLevel + 1;
                int xpRequiredForNextLevel = levelXPData.GetXPRequired(nextLevel);

                // Check if max level reached or invalid level data
                if (xpRequiredForNextLevel <= 0)
                {
                    // Debug.Log($"Character {character.CharacterID} has reached max level or level data is invalid for level {nextLevel}.");
                    break; // Exit loop if max level or no data for next level
                }

                if (character.CurrentXP >= xpRequiredForNextLevel)
                {
                    // Level Up!
                    int xpOverflow = character.CurrentXP - xpRequiredForNextLevel;
                    character.CurrentLevel++;
                    character.CurrentXP = xpOverflow;

                    // --- Stat Calculation ---
                    // Simple formula: +10 MaxHP, +5 MaxEnergy per level gained.
                    character.MaxHP += 10;
                    character.MaxEnergy += 5;
                    // --- End Stat Calculation ---

                    Debug.Log($"Character {character.CharacterID} leveled up to Level {character.CurrentLevel}! New Stats - MaxHP: {character.MaxHP}, MaxEnergy: {character.MaxEnergy}. Overflow XP: {character.CurrentXP}");
                    leveledUpThisCheck = true; // Mark that a level up occurred in this check cycle
                }
                else
                {
                    // Not enough XP for the next level
                    break; // Exit the loop
                }
            }

            // If any level up occurred during this check, save the updated character data.
            if (leveledUpThisCheck)
            {
                Debug.Log($"Initiating save for Character {character.CharacterID} after level up(s).");
                TeamRosterManager.Instance.SaveCharacterData(character, OnSaveComplete);
            }
        }

        /// <summary>
        /// Callback executed after attempting to save character data via TeamRosterManager.
        /// </summary>
        /// <param name="success">Whether the save operation was successful.</param>
        /// <param name="savedCharacter">The character data that was attempted to be saved.</param>
        private void OnSaveComplete(bool success, CharacterData savedCharacter)
        {
            if (success)
            {
                Debug.Log($"Character {savedCharacter.CharacterID} data saved successfully after level up. Raising OnCharacterLevelUp event.");
                // Invoke the event only on successful save
                OnCharacterLevelUp?.Invoke(savedCharacter);
            }
            else
            {
                // Log an error if the save failed. Do not raise the event.
                Debug.LogError($"ProgressionSystem: Failed to save character data for Character ID: {savedCharacter?.CharacterID} after level up attempt.");
            }
        }
    }
}