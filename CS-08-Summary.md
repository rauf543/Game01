# CS-08 - Progression System Implementation Summary

## Summary of Changes

This task involved the creation and implementation of the `ProgressionSystem` to manage character experience points (XP), leveling up, stat increases, and saving progress online. The system adheres to the specified requirements, including handling multi-level ups, using a predefined stat increase formula (+10 MaxHP, +5 MaxEnergy per level), interacting with the existing `TeamRosterManager` for data persistence, and raising an `OnCharacterLevelUp` event upon successful data saving.

## New Files Created

*   `Assets/Scripts/Systems/ProgressionSystem.cs`

## Existing Files Modified

*   None.

## Core Logic Added

### `Assets/Scripts/Systems/ProgressionSystem.cs`

*   **Singleton Pattern:** Implemented standard `Instance` property and `Awake` logic for singleton access.
*   **`LevelXPData_SO` Reference:** Added a public `[SerializeField]` field (`levelXPData`) to assign the `LevelXPData_SO` Scriptable Object via the Unity Inspector. This object provides the XP required for each level.
*   **`GrantXP(CharacterData character, int xpAmount)`:** Public method to grant XP. Validates input, adds XP to the character's `CurrentXP`, and calls `CheckLevelUp`.
*   **`CheckLevelUp(CharacterData character)`:** Private method containing the core level-up logic.
    *   Uses a `while` loop to handle potential multi-level gains from a single XP grant.
    *   Retrieves `xpRequiredForNextLevel` from the assigned `levelXPData`.
    *   Handles the max level case gracefully (stops checking if `GetXPRequired` returns <= 0).
    *   If a level up occurs:
        *   Calculates XP overflow.
        *   Increments `character.CurrentLevel`.
        *   Assigns overflow XP to `character.CurrentXP`.
        *   Calculates new stats using the formula: `MaxHP += 10`, `MaxEnergy += 5`.
        *   Sets a flag (`leveledUpThisCheck`) to indicate a level up occurred.
    *   After the loop, if `leveledUpThisCheck` is true, it calls `TeamRosterManager.Instance.SaveCharacterData(character, OnSaveComplete)` to persist the changes.
*   **`OnSaveComplete(bool success, CharacterData savedCharacter)`:** Private callback method for `SaveCharacterData`.
    *   If `success` is true, it invokes the static `OnCharacterLevelUp` event, passing the `savedCharacter` data.
    *   If `success` is false, it logs an error using `Debug.LogError` and does *not* invoke the event.
*   **`OnCharacterLevelUp` Event:** Defined as `public static event System.Action<CharacterData> OnCharacterLevelUp;` for other systems to subscribe to level-up notifications.