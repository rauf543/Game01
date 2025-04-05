using UnityEngine;
using System.Collections.Generic;
using Game01.Data; // Added for CharacterData
using Game.Systems; // Added for ProgressionSystem
using Game01.Combat; // Added for CharacterCombat

// Assuming ProgressionSystem is accessible via Singleton pattern
// Assuming CharacterData class exists with a public string characterId field
// Assuming CombatManager is assigned via Inspector and has instance event System.Action<bool> OnCombatEnd
// Assuming CombatManager has public List<CharacterCombat> GetPlayerCombatants() method
// Assuming CharacterCombat has a public CharacterData characterData field/property
// Assuming ProgressionSystem has public void GrantXP(CharacterData character, int xpAmount) method

public class CombatRewardHandler : MonoBehaviour
{
    [Tooltip("Assign the CombatManager instance from the scene.")]
    [SerializeField] private CombatManager combatManager;
    private void OnEnable()
    {
        // Subscribe to the combat end event via the assigned instance
        if (combatManager != null)
        {
            CombatManager.OnCombatEnd += HandleCombatEnd; // Keep static subscription if event is static
            // If OnCombatEnd is NOT static on CombatManager, use:
            // combatManager.OnCombatEnd += HandleCombatEnd;
        }
        else
        {
            Debug.LogError("CombatManager reference not set in CombatRewardHandler. Cannot subscribe to OnCombatEnd.", this);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the combat end event
        // No need to check combatManager null here, as static event doesn't require instance
        CombatManager.OnCombatEnd -= HandleCombatEnd;
        // If OnCombatEnd is NOT static on CombatManager, use:
        // if (combatManager != null)
        // {
        //     combatManager.OnCombatEnd -= HandleCombatEnd;
        // }
    }

    private void HandleCombatEnd(bool combatWon)
    {
        // Only grant rewards if the combat was won
        if (!combatWon)
        {
            return;
        }

        const int xpToGrant = 10; // Fixed XP amount as per requirement

        // Ensure CombatManager reference is set
        if (combatManager == null)
        {
            Debug.LogError("CombatManager reference not set in CombatRewardHandler. Cannot grant rewards.", this);
            return;
        }

        // Get ProgressionSystem singleton instance
        ProgressionSystem progressionSystem = ProgressionSystem.Instance;
        if (progressionSystem == null)
        {
            // ProgressionSystem uses Awake for singleton setup, so this check is still valid.
            Debug.LogError("ProgressionSystem instance is null. Cannot grant rewards.");
            return;
        }

        // Get the list of participating player characters
        List<CharacterCombat> combatants = combatManager.GetPlayerCombatants();

        // Check if the participant list is valid
        if (combatants == null || combatants.Count == 0)
        {
            // No combatants or list is null, nothing to do.
            Debug.LogWarning("No player combatants found to grant rewards to.");
            return;
        }

        // Grant XP to each participant
        foreach (CharacterCombat combatant in combatants)
        {
            // Null check for the combatant component itself
            if (combatant == null)
            {
                Debug.LogWarning("Found a null combatant component in the list.");
                continue; // Skip this entry
            }

            // Access the CharacterData from the CharacterCombat component
            // *** This uses the new 'Data' property on CharacterCombat ***
            CharacterData characterData = combatant.Data;

            // Null check for the character data
            if (characterData == null)
            {
                Debug.LogWarning($"Combatant {combatant.gameObject.name} has null CharacterData.");
                continue; // Skip this entry
            }

            // Null or empty check for the character ID
            if (string.IsNullOrEmpty(characterData.characterId))
            {
                Debug.LogWarning($"Character found on {combatant.gameObject.name} with null or empty characterId. Cannot grant XP.");
                continue; // Skip this character
            }

            // Grant the XP using the ProgressionSystem, passing the CharacterData object
            progressionSystem.GrantXP(characterData, xpToGrant);
            // Optional: Log XP grant for debugging
            // Debug.Log($"Granted {xpToGrant} XP to character {characterData.characterId}.");
        }
    }
}