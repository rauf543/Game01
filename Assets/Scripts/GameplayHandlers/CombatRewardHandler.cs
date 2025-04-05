using UnityEngine;
using System.Collections.Generic;

// Assuming CombatManager and ProgressionSystem exist and are accessible via Singleton pattern
// Assuming CharacterData class exists with a public string characterId field
// Assuming CombatManager has a public static event System.Action<bool> OnCombatEnd
// Assuming CombatManager has public List<CharacterData> GetCurrentPlayerTeam() method
// Assuming ProgressionSystem has public void GrantXP(string characterId, int xpAmount) method

public class CombatRewardHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to the combat end event
        // Assuming CombatManager exists and has the event
        CombatManager.OnCombatEnd += HandleCombatEnd;
    }

    private void OnDisable()
    {
        // Unsubscribe from the combat end event to prevent memory leaks
        // Assuming CombatManager exists and has the event
        CombatManager.OnCombatEnd -= HandleCombatEnd;
    }

    private void HandleCombatEnd(bool combatWon)
    {
        // Only grant rewards if the combat was won
        if (!combatWon)
        {
            return;
        }

        const int xpToGrant = 10; // Fixed XP amount as per requirement

        // Get singleton instances with null checks
        CombatManager combatManager = CombatManager.Instance;
        if (combatManager == null)
        {
            Debug.LogError("CombatManager instance is null. Cannot grant rewards.");
            return;
        }

        ProgressionSystem progressionSystem = ProgressionSystem.Instance;
        if (progressionSystem == null)
        {
            Debug.LogError("ProgressionSystem instance is null. Cannot grant rewards.");
            return;
        }

        // Get the list of participating player characters
        List<CharacterData> participants = combatManager.GetCurrentPlayerTeam();

        // Check if the participant list is valid
        if (participants == null || participants.Count == 0)
        {
            // No participants or list is null, nothing to do.
            // Could log a warning if desired, but keeping it minimal as requested.
            return;
        }

        // Grant XP to each participant
        foreach (CharacterData character in participants)
        {
            // Null check for the character data itself
            if (character == null)
            {
                Debug.LogWarning("Found a null character in the participants list.");
                continue; // Skip this entry
            }

            // Null or empty check for the character ID
            if (string.IsNullOrEmpty(character.characterId))
            {
                Debug.LogWarning("Character found with null or empty characterId. Cannot grant XP.");
                continue; // Skip this character
            }

            // Grant the XP using the ProgressionSystem
            progressionSystem.GrantXP(character.characterId, xpToGrant);
            // Optional: Log XP grant for debugging
            // Debug.Log($"Granted {xpToGrant} XP to character {character.characterId}.");
        }
    }
}