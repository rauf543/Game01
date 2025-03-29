Task Instructions: CS-01 - Initialize Unity Project & Version Control
Objective
Set up a new Unity project using the 2D URP template and initialize version control with Git on GitHub. Establish a standardized folder structure and adhere strictly to the provided commit, branching, pull request, and code review guidelines.

Detailed Instructions
Unity Project Setup
Template: Use the 2D URP template.

Unity Version: Use the default Unity version (no specific version is required).

Project Configuration: Ensure the project is configured for a 2D environment with the Universal Render Pipeline. Do not modify any settings beyond those required for this configuration.

Folder Structure
Standard Directories: Create a clear and minimal folder structure within the project. Include, at minimum:

Assets: Root folder for all Unity assets.

Scenes: For all scene files.

Scripts: For all source code.

Art: For sprites and art assets (if applicable).

Prefabs: For reusable game objects.

Documentation: For any related project documentation.

Restrictions: Do not add extra folders or files that are not explicitly required by this task.

Version Control Setup
Git Initialization:

Initialize a Git repository in the Unity project folder.

Connect this repository to GitHub.

Branching Strategy: Establish the following branches:

main: Contains production-ready code (only merge from develop after thorough testing).

develop: Contains the current integration state; feature branches merge here.

feature/CS-01-initialize-unity-project: Create this branch off develop for all changes related to this task.

Commit Guidelines:

Use atomic commits that capture one logical change at a time.

Follow the Conventional Commits standard. For example:

feat(core): Initialize Unity project with 2D URP template

Pull Request Guidelines:

When merging, create a pull request from the feature branch into develop.

The PR title and description must include the task ID and clearly explain what was changed, why, and how.

Keep PRs focused only on changes relevant to this task.

Agent-Specific Instructions: What to Do and What NOT to Do
Do:

Follow the steps exactly as described.

Keep changes minimal and strictly within the task scope.

Use straightforward and direct methods to implement changes.

Adhere exactly to the specified folder structure and Git branching/commit standards.

Provide a final summary detailing:

A list of all created or modified files.

Key code/configuration excerpts.

A brief explanation of each change to verify its relevance to this task.

Do NOT:

Add any extra features, components, or files beyond what is explicitly required.

Modify or refactor any default or unrelated Unity files.

Introduce unnecessary complexity or roundabout methods.

Bloat the codebase with redundant or unrelated code.

Make changes outside the defined scope of initializing the project and version control setup.

Final Check
Before finalizing, ensure:

The Unity project is created using the 2D URP template.

The standard folder structure is present and no extra files have been added.

Git is initialized, connected to GitHub, and the branching strategy is correctly implemented.

A clear, concise summary of all changes is provided for review.