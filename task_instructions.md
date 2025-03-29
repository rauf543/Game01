# Task Instructions for AI Agent: CS-NET-01 - Basic Backend Setup & API Stub  ## Overview This task involves creating a minimal backend service using Node.js and Express with a SQLite database connection. You must implement stub API endpoints for user login, retrieving a player roster, and saving player character data. Your solution should strictly adhere to the following specifications without adding any extra functionality or unnecessary complexity.  ## Requirements  ### Tech Stack - **Primary Framework:** Node.js with Express   - **Database:** SQLite (file-based) using a Node.js library such as `sqlite3` - **Logging:** Use `console.log()` for basic logging of incoming requests, errors, and key actions. - **Error Handling:** Use minimal `try...catch` blocks; every endpoint must return a JSON object with a consistent structure.  ### API Endpoints  #### 1. User Login (POST `/api/auth/login`) - **Request Payload:**     ```json   {"username": "string", "password": "string"} 

Response Payload (Success):



json
Copy

{"success": true, "message": "Login successful", "userId": "some_unique_user_id"} 

Response Payload (Failure):



json
Copy

{"success": false, "message": "Invalid credentials"} 

Note:

Do not implement full authentication logic; use hardcoded or simple value checks for stub behavior.

2. Get Player Roster/Data (GET /api/player/roster)
Authentication:

Assume an authenticated user (authentication mechanism can be simulated via a header or session; full implementation is not required).

Response Payload (Success):



json
Copy

{   "success": true,   "roster": [     {"characterId": "char1", "level": 1, "xp": 0, "maxHp": 100, "maxEnergy": 50},     {"characterId": "char2", ...}   ] } 

Return an empty array if no characters exist.

Response Payload (Failure):



json
Copy

{"success": false, "message": "Error fetching roster"} 

3. Save Player Roster/Data (POST or PUT /api/player/character or /api/player/character/{characterId})
Request Payload:



json
Copy

{"characterId": "char1", "level": 2, "xp": 150, "maxHp": 110, "maxEnergy": 55} 

Response Payload (Success):



json
Copy

{"success": true, "message": "Character saved", "character": {updated_character_data}} 

Response Payload (Failure):



json
Copy

{"success": false, "message": "Error saving character"} 

Note:

For the stub, simply log the received data and return a success response without complex validation or storage logic.

Database Connection
Setup:

Establish a basic SQLite connection.

Do not design a complex schema; only include the minimal code required to demonstrate connectivity.

Documentation
README.md:

Create a simple README in the backend project directory.

Document each endpoint’s path, expected request payload, and sample responses.

Do not include any additional documentation or extra features beyond what is specified.

What to Do
Project Initialization:

Set up a Node.js project using npm init.

Install required dependencies such as Express and sqlite3.

API Implementation:

Implement the following endpoints exactly as specified:

POST /api/auth/login

GET /api/player/roster

POST/PUT /api/player/character (choose one method consistently)

Ensure each endpoint returns the precise JSON structure detailed above.

Database Integration:

Configure a simple SQLite connection for local development.

Only implement the minimal functionality needed to support the stub endpoints.

Logging & Error Handling:

Add basic logging (console.log()) for each incoming request and error.

Use simple try...catch blocks to catch errors and respond with the specified JSON error messages.

Documentation:

Create a README.md file that documents:

A brief project overview.

All API endpoints with sample requests/responses.

Do not include any extra documentation or details not required.

What NOT to Do
Avoid Unnecessary Tasks:

Do not implement any features or endpoints that are not explicitly specified.

Do not add extra middleware, libraries, or abstractions beyond the minimal requirements.

Avoid Code Bloat:

Do not introduce extra logging, error handling, or complex architectural patterns.

Do not over-engineer or add any extra functionality like full authentication, token management, or additional endpoints.

Stick to the Spec:

Do not modify endpoint paths, payload structures, or response formats.

Do not implement any changes that deviate from the given instructions.

Final Deliverable
At the end of the task, before submission, provide a comprehensive summary that includes:

An overview of all changes made.

A list of modified or added files.

Relevant code snippets highlighting key changes.

A clear explanation of how each requirement was met.

This summary is critical for the code review process and must be included as the final output.