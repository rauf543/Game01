# Known Issues

## Issue 1: Guild Hall Roster Displays 0 Characters Despite Server Reporting 5

**Date Reported:** 2025-04-05

**Component(s) Affected:** `GuildHallUIController`, `TeamRosterManager`, potentially `NetworkManager`

**Symptoms:**
- The Guild Hall UI (`GuildHallUIController`) displays the message "No characters in roster."
- Diagnostic logging added to `GuildHallUIController.HandleRosterLoadingFinished` confirms it receives a roster count of 0 from `TeamRosterManager.TeamRoster`.
  ```
  GuildHallUIController: Received TeamRoster with 0 characters from TeamRosterManager.
  ```
- Server logs indicate that 5 characters were successfully retrieved for the logged-in user (`user1`) during the roster request initiated by the client.
  ```
  Roster request received for user: user1
  Retrieved 5 characters for user: user1
  ```
- Creating a new character does not resolve the issue; the roster remains empty in the UI.

**Expected Behavior:**
- The Guild Hall UI should display the 5 characters retrieved by the server for the user.
- `GuildHallUIController` should receive a collection with 5 `CharacterData` objects from `TeamRosterManager.TeamRoster`.

**Actual Behavior:**
- The Guild Hall UI displays "No characters in roster."
- `GuildHallUIController` receives a collection with 0 characters from `TeamRosterManager.TeamRoster`.

**Potential Causes / Areas to Investigate:**
1.  **`NetworkManager.WorkspaceRoster` Deserialization:** Verify that the `NetworkManager` coroutine responsible for fetching the roster (`WorkspaceRoster`) is correctly deserializing the JSON response from the server into a `List<CharacterData>`. Check for potential deserialization errors or mismatches between the JSON structure and the `CharacterData` class definition.
2.  **`TeamRosterManager.OnRosterFetched` Logic:** Examine the `OnRosterFetched` method within `TeamRosterManager`. Ensure that the `List<CharacterData>` received from the `NetworkManager` callback is correctly processed and used to populate the internal `teamRoster` dictionary. Check for any logic errors that might cause the dictionary to remain empty despite receiving data.
3.  **Data Integrity:** Although less likely given the server logs, double-check the structure and content of the `CharacterData` being sent by the server and expected by the client to ensure consistency.

**Status:** Open - Requires investigation into `NetworkManager` and/or `TeamRosterManager`.