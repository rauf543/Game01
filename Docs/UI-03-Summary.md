# Task Summary: Implement Online Guild Hall Roster Display

## Summary of Changes

This task involved implementing a UI system in the Guild Hall scene to display the player's character roster fetched from an online source. The implementation included creating a reusable prefab for individual character entries and modifying the main Guild Hall UI controller to handle data loading, display, updates, and error states. Initial implementation encountered issues with incorrect namespaces and field names, which were subsequently corrected. Event handling was updated to match the actual signatures in `TeamRosterManager` and `ProgressionSystem`. Diagnostic logging was added to help identify a discrepancy where the UI receives an empty roster despite server logs indicating available characters; this external issue was documented separately.

## New Files Created

*   `Assets/Scripts/UI/CharacterRosterEntry.cs`
*   `known-issues.md` (Created to document the roster data discrepancy)

*(Note: `Assets/Prefabs/UI/CharacterRosterEntry.prefab` was required but created manually in the Unity Editor as per instructions.)*

## Existing Files Modified

*   `Assets/Scripts/Scene/GuildHallUIController.cs`

## Core Logic Description

*   **`Assets/Scripts/UI/CharacterRosterEntry.cs`**:
    *   Holds references to TextMeshPro components for character name, level, HP, and energy.
    *   Provides a `Setup(CharacterData characterData)` method to populate the UI fields using data from a `CharacterData` object.
    *   Includes `UpdateLevel`, `UpdateHP`, and `UpdateEnergy` methods to allow individual stat updates (e.g., on level up).
    *   Corrected `using` directive to `Game01.Data` and updated field access (`characterData.name`, `characterData.level`, etc.) to match the `CharacterData` class definition.

*   **`Assets/Scripts/Scene/GuildHallUIController.cs`**:
    *   Added serialized fields for UI references: `rosterContainer` (Transform), `characterEntryPrefab` (GameObject), `rosterStatusText` (TextMeshProUGUI).
    *   Added reference to `LoadingUIManager`.
    *   Maintains a `Dictionary<string, CharacterRosterEntry>` (`characterEntryMap`) to track instantiated roster entries by character ID for efficient updates.
    *   Subscribes to `TeamRosterManager.OnRosterLoadingStarted` and `TeamRosterManager.OnRosterLoadingFinished` events.
    *   Subscribes to the static `ProgressionSystem.OnCharacterLevelUp` event.
    *   Implements `HandleRosterLoadStart` to show loading indicators and clear the display.
    *   Implements `HandleRosterLoadingFinished` to handle both success (populating the roster via `PopulateRoster` using data from `TeamRosterManager.TeamRoster`) and failure (displaying error messages). Added logging to show the count of characters received from `TeamRosterManager`.
    *   Implements `HandleCharacterLevelUp(CharacterData updatedCharacter)` to find the corresponding UI entry in `characterEntryMap` and update its level, HP, and energy using the data from the event argument.
    *   Implements `ClearRosterDisplay` to destroy old UI elements and clear the map.
    *   Implements `PopulateRoster` to instantiate `characterEntryPrefab` under `rosterContainer` for each character in the provided list, set them up using `CharacterRosterEntry.Setup`, and add them to `characterEntryMap`.
    *   Ensures event unsubscription in `OnDestroy`.
    *   Added `using System.Linq;`.

*   **`known-issues.md`**:
    *   Documents the observed issue where `GuildHallUIController` receives 0 characters from `TeamRosterManager`, despite server logs indicating 5 characters were retrieved. Outlines potential causes related to data deserialization or processing in `NetworkManager` or `TeamRosterManager`.