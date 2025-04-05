using UnityEngine;
using System; // Required for System.Action
using Game01.Data; // For CharacterData
using System.Collections.Generic;
using System.Linq; // For LINQ operations

// Extension method for LevelXPData_SO to add the GetXPRequired functionality
public static class LevelXPData_SOExtensions
{
    public static int GetXPRequired(this LevelXPData_SO levelData, int level)
    {
        if (levelData == null || levelData.LevelRequirements == null)
            return -1;
            
        var levelReq = levelData.LevelRequirements.FirstOrDefault(req => req.Level == level);
        return levelReq != null ? levelReq.XPRequired : -1;
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
                Debug.LogWarning($"ProgressionSystem: Attempted to grant non-positive XP ({xpAmount}) to character {character.characterId}. Ignoring.");
                return;
            }
character.xp += xpAmount;
Debug.Log($"Character {character.characterId} granted {xpAmount} XP. Current XP: {character.xp}");


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
                int nextLevel = character.level + 1;
                int xpRequiredForNextLevel = levelXPData.GetXPRequired(nextLevel);

                // Check if max level reached or invalid level data
                if (xpRequiredForNextLevel <= 0)
                {
                    // Debug.Log($"Character {character.characterId} has reached max level or level data is invalid for level {nextLevel}.");
                    break; // Exit loop if max level or no data for next level
                }

                if (character.xp >= xpRequiredForNextLevel)
                {
                    // Level Up!
                    int xpOverflow = character.xp - xpRequiredForNextLevel;
                    character.level++;
                    character.xp = xpOverflow;

                    // --- Stat Calculation ---
                    // Simple formula: +10 maxHp, +5 maxEnergy per level gained.
                    character.maxHp += 10;
                    character.maxEnergy += 5;
                    // --- End Stat Calculation ---

                    Debug.Log($"Character {character.characterId} leveled up to Level {character.level}! New Stats - MaxHP: {character.maxHp}, MaxEnergy: {character.maxEnergy}. Overflow XP: {character.xp}");
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
                Debug.Log($"Initiating save for Character {character.characterId} after level up(s).");
                // Use the TeamRosterManager directly without callback
                TeamRosterManager teamRoster = FindObjectOfType<TeamRosterManager>();
                if (teamRoster != null)
                {
                    teamRoster.AddCharacter(character, (success, message) => OnSaveComplete(success, character));
                }
                else
                {
                    Debug.LogError("TeamRosterManager not found in scene. Character data not saved.");
                    OnSaveComplete(false, character);
                }
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
                Debug.Log($"Character {savedCharacter.characterId} data saved successfully after level up. Raising OnCharacterLevelUp event.");
                // Invoke the event only on successful save
                OnCharacterLevelUp?.Invoke(savedCharacter);
            }
            else
            {
                // Log an error if the save failed. Do not raise the event.
                Debug.LogError($"ProgressionSystem: Failed to save character data for Character ID: {savedCharacter?.characterId} after level up attempt.");
            }
        }
    }
}