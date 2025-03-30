# Loading UI Setup Instructions

This document provides detailed, step-by-step instructions for setting up the loading UI components required for UI-01-Refactor.

## Table of Contents
1. [Main Menu Scene Setup](#1-main-menu-scene-setup)
2. [Guild Hall Scene Setup](#2-guild-hall-scene-setup)
3. [Loading Animation Setup](#3-loading-animation-setup)
4. [Testing The Implementation](#4-testing-the-implementation)
5. [Troubleshooting](#5-troubleshooting)

---

## 1. Main Menu Scene Setup

### Step 1.1: Open the Main Menu Scene
1. In the Unity Editor, navigate to Assets/Scenes
2. Double-click on MainMenu.unity to open the scene

### Step 1.2: Create Loading UI Canvas
1. Right-click in the Hierarchy panel and select UI > Canvas
2. Rename the created Canvas to "LoadingUICanvas"
3. Set Canvas properties:
   - Render Mode: Screen Space - Overlay
   - Pixel Perfect: Checked
   - Sort Order: 10 (to ensure it appears on top of other UI)

### Step 1.3: Add Canvas Scaler
1. With LoadingUICanvas selected, in the Inspector, check if Canvas Scaler is present
2. If not, click Add Component > UI > Canvas Scaler
3. Set Canvas Scaler properties:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: X: 1920, Y: 1080
   - Screen Match Mode: Match Width Or Height
   - Match: 0.5 (balanced)

### Step 1.4: Create Loading Panel
1. Right-click on LoadingUICanvas in hierarchy and select UI > Panel
2. Rename to "LoadingPanel"
3. Set RectTransform properties:
   - Anchor Presets: Stretch to fill entire parent (Alt+Shift+Click middle button)
   - Left, Right, Top, Bottom: 0
4. Set Image component properties:
   - Color: #000000 with Alpha set to 0.7 (semi-transparent)
5. Set GameObject state to inactive (uncheck the checkbox next to the name)

### Step 1.5: Add Loading Text
1. Right-click on LoadingPanel and select UI > Text - TextMeshPro
   - If prompted to import TMP Essentials, click Import
2. Rename to "LoadingText"
3. Set RectTransform properties:
   - Anchor Presets: Middle-Center
   - Position: X: 0, Y: 50
   - Width: 600, Height: 80
4. Set TextMeshPro component properties:
   - Text: "Loading..."
   - Font Size: 48
   - Color: White
   - Alignment: Center
   - Wrapping: Enabled
   - Overflow: Truncate

### Step 1.6: Create Loading Spinner
1. Right-click on LoadingPanel and select UI > Image
2. Rename to "LoadingSpinner"
3. Set RectTransform properties:
   - Anchor Presets: Middle-Center
   - Position: X: 0, Y: -50
   - Width: 80, Height: 80
4. Set Image component properties:
   - Source Image: Any circular image (you can use a default Unity circle sprite)
   - Color: White
   - Material: Default-UI
   - Raycast Target: Unchecked (to prevent blocking clicks)

### Step 1.7: Create Error Panel
1. Right-click on LoadingUICanvas in hierarchy and select UI > Panel
2. Rename to "ErrorPanel"
3. Set RectTransform properties:
   - Anchor Presets: Middle-Center
   - Position: X: 0, Y: 0
   - Width: 600, Height: 300
4. Set Image component properties:
   - Color: #202020 with Alpha set to 0.9
5. Set GameObject state to inactive (uncheck the checkbox next to the name)

### Step 1.8: Add Error Text
1. Right-click on ErrorPanel and select UI > Text - TextMeshPro
2. Rename to "ErrorText"
3. Set RectTransform properties:
   - Anchor Presets: Stretch-Stretch
   - Left: 20, Right: 20, Top: 20, Bottom: 80
4. Set TextMeshPro component properties:
   - Text: "Error message will appear here."
   - Font Size: 32
   - Color: White
   - Alignment: Middle-Center
   - Wrapping: Enabled
   - Overflow: Truncate

### Step 1.9: Add Close Button
1. Right-click on ErrorPanel and select UI > Button - TextMeshPro
2. Rename to "CloseButton"
3. Set RectTransform properties:
   - Anchor Presets: Bottom-Center
   - Position: X: 0, Y: 30
   - Width: 200, Height: 60
4. Set Button component properties:
   - Transition: Color Tint
   - Normal Color: #4A4A4A
   - Highlighted Color: #7A7A7A
   - Pressed Color: #2A2A2A
5. Select the Text (TMP) child, and set its TextMeshPro properties:
   - Text: "OK"
   - Font Size: 30
   - Alignment: Middle-Center

### Step 1.10: Add LoadingUIManager Component
1. Select LoadingUICanvas in the hierarchy
2. Click Add Component in the Inspector
3. Type "LoadingUIManager" and select it
4. Assign references in the Inspector:
   - Loading Panel: Drag the LoadingPanel from hierarchy
   - Loading Text: Drag the LoadingText from hierarchy
   - Loading Spinner: Drag the LoadingSpinner from hierarchy
   - Error Panel: Drag the ErrorPanel from hierarchy
   - Error Text: Drag the ErrorText from hierarchy
   - Error Close Button: Drag the CloseButton from hierarchy

### Step 1.11: Update MainMenuUIController
1. Find and select the GameObject that has the MainMenuUIController script
2. In the Inspector, note that additional serialized fields have been added:
   - Username Input: Assign your scene's username input field
   - Password Input: Assign your scene's password input field
   - Login Button: Assign your scene's login button
   - Network Manager: Assign the NetworkManager object from the scene
   - Loading UI Manager: Drag the LoadingUICanvas from hierarchy

### Step 1.12: Set Initial Button States
1. Find and select the Start Button referenced in MainMenuUIController
2. Uncheck the Interactable property in the Button component to ensure it starts disabled
3. Users will need to successfully login before this button becomes active

---

## 2. Guild Hall Scene Setup

### Step 2.1: Open the Guild Hall Scene
1. In the Unity Editor, navigate to Assets/Scenes
2. Double-click on GuildHall.unity to open the scene

### Step 2.2 - 2.10: Create Loading UI Components
- Follow the same steps as 1.2 through 1.10 from the Main Menu scene setup to create:
  - LoadingUICanvas
  - LoadingPanel with LoadingText and LoadingSpinner
  - ErrorPanel with ErrorText and CloseButton
- The structure and properties should be identical

### Step 2.11: Update GuildHallUIController
1. Find and select the GameObject that has the GuildHallUIController script
2. In the Inspector, note that additional serialized fields have been added:
   - Character Selection Button: Assign any button used for character selection
   - Refresh Roster Button: Assign or create a button for refreshing the roster
   - Team Roster Manager: Assign the TeamRosterManager object from the scene
   - Loading UI Manager: Drag the LoadingUICanvas from hierarchy

---

## 3. Loading Animation Setup

### Step 3.1: Create Animation for Spinner
1. Select the LoadingSpinner GameObject in the hierarchy
2. In the menu, select Window > Animation > Animation
3. In the Animation window, click Create
4. Save the animation as "SpinnerRotation" in an appropriate location (e.g., Assets/Animations)

### Step 3.2: Record Rotation Animation
1. Make sure the Animation window is in Record mode (red circle button)
2. In the timeline, place your cursor at 0:00
3. In the Inspector, note the current rotation (should be 0, 0, 0)
4. Create a keyframe by clicking the Add Property button in the Animation window
5. Select Transform > Rotation
6. Move the timeline cursor to 1:00 (one second)
7. In the Inspector, change the Z-rotation to 360
8. This will automatically create another keyframe

### Step 3.3: Configure Animation Settings
1. Select the Animation clip in the Project window
2. In the Inspector, set:
   - Loop Time: Checked
   - Loop Pose: Checked
3. Select the LoadingSpinner GameObject again
4. In the Animator component (automatically added), set:
   - Update Mode: Unscaled Time (so animation continues even if game is paused)
   - Culling Mode: Always Animate

---

## 4. Testing The Implementation

### Step 4.1: Test Login Screen
1. Play the scene with the MainMenu
2. Verify the LoadingPanel is initially inactive
3. Enter login credentials and click Login
4. Verify:
   - "Logging in..." text appears
   - Login button becomes disabled
   - When login succeeds, the Start button becomes enabled
   - When login fails, an error message appears

### Step 4.2: Test Guild Hall Roster Loading
1. Navigate to the Guild Hall scene
2. Verify the loading indicator appears during roster loading
3. Verify buttons are disabled during loading
4. Test the Refresh Roster button to see if loading indicators reappear
5. Test error handling by temporarily disabling network connectivity

---

## 5. Troubleshooting

### Common Issues and Solutions

#### "References not set" errors
- Double-check all references in the Inspector for LoadingUIManager, MainMenuUIController, and GuildHallUIController
- Ensure all GameObjects are correctly named and assigned

#### Loading indicator not appearing
- Verify that LoadingPanel's GameObject is being correctly activated by the LoadingUIManager
- Check Console for any errors related to the activation

#### Button interactivity issues
- Ensure all buttons are correctly assigned in the GuildHallUIController's interactable buttons list
- Verify button state changes are occurring at the correct times

#### Spinner not animating
- Check the Animation window to ensure keyframes are set correctly
- Verify the Animator component settings on the LoadingSpinner

#### Login or roster loading not triggering UI updates
- Verify that event connections are working in the TeamRosterManager
- Check that the NetworkManager reference is assigned in MainMenuUIController

If issues persist, check the Console window for error messages and address the specific components referenced in the errors.