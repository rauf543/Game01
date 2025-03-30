Task Instructions: UI-01-Refactor - Handle Async Loading
Overview
We need to implement simple, clear, and strictly scoped asynchronous loading indicators in our Unity-based UI. The aim is to ensure the user sees a "Loading..." or spinner when network calls are in progress, and that clickable UI elements are disabled to prevent unwanted user actions.

Primary Objectives
Login Screen (UI-AUTH-01)  

When NetworkManager.Login() is called, display a loading indicator or text message such as "Logging in..." (or similar) while the call is in progress.

Disable the "Login" button and any other relevant UI elements that should not be clicked while logging in.

If the call fails, hide the loading indicator and show a simple error message such as "Login Failed. Please check your connection."

Ensure the login button is re-enabled if the user can retry.

Guild Hall (UI-03-Refactor related, referencing CS-04-Refactor)  

When fetching the player roster via TeamRosterManager (for example, WorkspaceRosterAsync()), display a loading indicator or spinner.

Disable relevant Guild Hall buttons (e.g., navigation or character selection buttons) while loading.

If the data fetch fails, hide the loading indicator and display an error message. Keep interactive elements disabled until the user performs a retry or navigates away.

Requirements
Loading Indicator Implementation  

Simple text label or a basic rotating spinner icon.  

Must appear only while async operations are in progress.  

The style/design can be minimal; no fancy progress bars needed.

UI Interaction Lock/Disable  

While loading, disable critical buttons and inputs (e.g., login button, navigation buttons in Guild Hall).  

Re-enable those elements once loading completes or fails, as appropriate.

Error Handling UI  

On async operation failure, hide loading indicators and present an error message.  

Keep it simple: A TextMeshPro label or a basic panel with a short message.  

If applicable, re-enable or keep disabled certain UI elements until the user takes a valid action (like “Retry” or close the error message).

Scope and Limitations  

Focus only on the Login screen and Guild Hall’s initial roster load.  

Do not implement or refactor anything outside these areas.  

Do not add extra features not requested (e.g., no advanced caching or rewriting unrelated UI scripts).  

Keep the code additions minimal and relevant.

Code Structure and Patterns  

Maintain existing project folder structure and naming conventions.  

If new scripts or UI elements are needed, place them logically (e.g., Scripts/UI/LoadingIndicators, or inside existing UI script files if it’s minimal).  

Keep code changes restricted to only what’s necessary for handling async operations and showing/hiding loading states.

Testing  

Perform manual tests:  

Login sequence: Confirm “Loading…”/spinner appears, the button is disabled, success re-enables the button, failure shows an error message.  

Guild Hall roster load: Confirm “Loading…”/spinner appears, navigation/selection buttons are disabled, and success re-enables them.  

No automated tests are required at this time.

Definition of Done  

The code compiles and runs in the Unity Editor without errors.  

Logging into the game shows a loading state, then transitions to the game on success, or displays an error on failure.  

Entering the Guild Hall fetches and displays the roster with a loading state, and an error message on failure.  

No extraneous or unrelated changes are made to the codebase.

Important Notes
This task is time-bound to Sprint 1.5. Only implement exactly what is specified.  

Do not add new, unrelated features or large-scale refactors.  

The user experience must remain smooth, with minimal text/spinner usage.  

After implementing, you must provide a concise but thorough summary of all changes made, including filenames and brief rationale for each change. This summary is required for our code review process.