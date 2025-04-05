# Task Summary: GP-06 Connect Progression to Combat Events

## Summary of Changes
A new C# script was created to automatically grant experience points (XP) to participating player characters upon successful combat completion. This connects the combat system's outcome to the character progression system.

## New Files Created
*   `Assets/Scripts/GameplayHandlers/CombatRewardHandler.cs`

## Existing Files Modified
*   None

## Core Logic Description
*   **`Assets/Scripts/GameplayHandlers/CombatRewardHandler.cs`**:
    *   Defines the `CombatRewardHandler` class inheriting from `MonoBehaviour`.
    *   Subscribes the `HandleCombatEnd` method to the static `CombatManager.OnCombatEnd` event in `OnEnable` and unsubscribes in `OnDisable`.
    *   The `HandleCombatEnd` method checks if the combat was won (`combatWon == true`).
    *   If won, it retrieves the `CombatManager` and `ProgressionSystem` instances (with null checks).
    *   It gets the list of participating player characters using `CombatManager.Instance.GetCurrentPlayerTeam()` (with null/empty checks).
    *   It iterates through the participants, performing null checks on the character and their `characterId`.
    *   For each valid participant, it calls `ProgressionSystem.Instance.GrantXP(character.characterId, 10)` to grant a fixed 10 XP.