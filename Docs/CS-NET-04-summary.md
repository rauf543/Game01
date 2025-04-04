# Task CS-NET-04: Implement Backend Roster/Character Logic - Implementation Summary

## Overview
This implementation fulfills the requirements of task CS-NET-04 by enhancing the backend API logic for creating, updating, and retrieving character data associated with users. The implementation includes proper validation logic, secure ownership verification, and reliable data persistence while adhering to the specified database schema and API contract.

## Files Modified

1. **backend/db/database.js**
   - Added `Name TEXT` column to the Characters table schema
   - Implemented validation rules for character creation and updates
   - Created methods to handle character roster management with ownership security

2. **backend/routes/player.js**
   - Enhanced existing API routes with proper validation and error handling
   - Implemented secure ownership verification for character operations
   - Standardized consistent JSON response formats

## Key Code Excerpts

### Characters Table Schema Update
```javascript
// Create Characters table with Name column
await runQuery(`
  CREATE TABLE IF NOT EXISTS Characters (
    CharacterID TEXT PRIMARY KEY,
    OwnerUserID TEXT,
    Level INTEGER DEFAULT 1,
    XP INTEGER DEFAULT 0,
    MaxHP INTEGER DEFAULT 100,
    MaxEnergy INTEGER DEFAULT 50,
    Name TEXT,
    FOREIGN KEY (OwnerUserID) REFERENCES Users(UserID)
  )
`);
```

### Create Character Implementation
```javascript
// CREATE - Create a new character with validation
createCharacter(characterData, ownerUserId) {
  return new Promise((resolve, reject) => {
    // Validate inputs
    if (!ownerUserId) {
      reject(new Error('Owner user ID is required'));
      return;
    }
    
    if (characterData.level !== 1) {
      reject(new Error('New characters must have exactly level 1'));
      return;
    }
    
    if (characterData.xp !== 0) {
      reject(new Error('New characters must have exactly 0 XP'));
      return;
    }
    
    if (!characterData.maxHp || characterData.maxHp <= 0) {
      reject(new Error('MaxHP must be greater than 0'));
      return;
    }
    
    if (!characterData.maxEnergy || characterData.maxEnergy <= 0) {
      reject(new Error('MaxEnergy must be greater than 0'));
      return;
    }
    
    if (!characterData.name || characterData.name.trim() === '') {
      reject(new Error('Character name is required'));
      return;
    }
    
    // Generate a unique CharacterID
    const CharacterID = 'char_' + Date.now() + '_' + Math.floor(Math.random() * 1000);
    
    // Insert the new character
    db.run(
      'INSERT INTO Characters (CharacterID, OwnerUserID, Level, XP, MaxHP, MaxEnergy, Name) VALUES (?, ?, ?, ?, ?, ?, ?)',
      [
        CharacterID,
        ownerUserId,
        characterData.level,
        characterData.xp,
        characterData.maxHp,
        characterData.maxEnergy,
        characterData.name
      ],
      function(err) {
        if (err) {
          reject(err);
          return;
        }
        
        // Get the newly created character
        characterDb.getCharacterByID(CharacterID)
          .then(character => resolve(character))
          .catch(err => reject(err));
      }
    );
  });
}
```

### Update Character with Ownership Verification
```javascript
// UPDATE - Update a character with validation and ownership check
updateCharacter(characterId, characterData, ownerUserId) {
  return new Promise((resolve, reject) => {
    // Validate inputs
    if (!characterId || !ownerUserId) {
      reject(new Error('Character ID and owner user ID are required'));
      return;
    }
    
    if (characterData.level === undefined || characterData.level < 1) {
      reject(new Error('Level must be greater than or equal to 1'));
      return;
    }
    
    if (characterData.xp === undefined || characterData.xp < 0) {
      reject(new Error('XP must be greater than or equal to 0'));
      return;
    }
    
    if (characterData.maxHp === undefined || characterData.maxHp <= 0) {
      reject(new Error('MaxHP must be greater than 0'));
      return;
    }
    
    if (characterData.maxEnergy === undefined || characterData.maxEnergy <= 0) {
      reject(new Error('MaxEnergy must be greater than 0'));
      return;
    }
    
    // Update the character, checking both CharacterID and OwnerUserID
    db.run(
      'UPDATE Characters SET Level = ?, XP = ?, MaxHP = ?, MaxEnergy = ? WHERE CharacterID = ? AND OwnerUserID = ?',
      [
        characterData.level,
        characterData.xp,
        characterData.maxHp,
        characterData.maxEnergy,
        characterId,
        ownerUserId
      ],
      function(err) {
        if (err) {
          reject(err);
          return;
        }
        
        // Check if the update affected exactly one row
        if (this.changes !== 1) {
          resolve(null); // Character not found or not owned by user
          return;
        }
        
        // Get the updated character data
        characterDb.getCharacterByID(characterId)
          .then(updated => resolve(updated))
          .catch(err => reject(err));
      }
    );
  });
}
```

### Character Roster API Endpoint
```javascript
/**
 * Get Player Roster endpoint (READ)
 * GET /api/player/roster
 */
router.get('/roster', authenticateUser, async (req, res) => {
  console.log('Roster request received for user:', req.user.UserID);
  
  try {
    // Get characters for the authenticated user
    const characters = await characterDb.getCharactersByUserId(req.user.UserID);
    
    console.log(`Retrieved ${characters.length} characters for user:`, req.user.UserID);
    
    // Return success response with roster
    return res.status(200).json({
      success: true,
      roster: characters
    });
  } catch (error) {
    console.error('Error fetching roster:', error.message);
    
    // Return failure response
    return res.status(500).json({
      success: false,
      message: "Error fetching roster"
    });
  }
});
```

## How Requirements Were Met

### 1. Database Logic (`backend/db/database.js`)

- **Characters Table Schema Update**:
  - Added `Name TEXT` column to the Characters table definition
  - Maintained all required columns: `CharacterID`, `OwnerUserID`, `Level`, `XP`, `MaxHP`, `MaxEnergy`

- **createCharacter Implementation**:
  - Accepts complete character data and owner user ID parameters
  - Generates unique CharacterID
  - Performs comprehensive input validation
  - Executes parameterized INSERT queries to prevent SQL injection
  - Returns the complete newly created character object

- **updateCharacter Implementation**:
  - Validates all input parameters: level ≥ 1, xp ≥ 0, maxHp > 0, maxEnergy > 0
  - Uses WHERE clause to match both CharacterID AND OwnerUserID for security
  - Checks if update affected exactly one row to verify ownership
  - Returns the updated character or null if not found/owned

- **getCharactersByUserId Implementation**:
  - Accepts ownerUserId parameter
  - Executes a secure, parameterized SELECT query
  - Returns an array of character objects (empty array if none found)

### 2. API Routes (`backend/routes/player.js`)

- **POST /api/player/character (Create)**:
  - Uses existing Express router with authentication middleware
  - Extracts required character data from request body
  - Ensures character ownership by using the authenticated user's ID
  - Calls the database createCharacter function with proper parameters
  - Handles errors with appropriate status codes and descriptive messages
  - Returns HTTP 201 (Created) with JSON success response on success

- **PUT /api/player/character/:characterId (Update)**:
  - Properly defined with authentication middleware
  - Extracts characterId from URL parameters
  - Extracts update data from request body
  - Passes ownerUserId from authenticated user
  - Calls the database updateCharacter function
  - Handles various error cases with appropriate status codes
  - Returns HTTP 200 (OK) with JSON success response on success

- **GET /api/player/roster (Read)**:
  - Uses existing router with authentication middleware
  - Extracts ownerUserId from authenticated user
  - Calls the database getCharactersByUserId function
  - Returns HTTP 200 (OK) with JSON success response including character array

## Design Decisions

### Character Name Addition
Added the `Name` column to the Characters table to properly identify characters beyond just their ID. This enhances the user experience by allowing meaningful character identification in the roster.

### Security-First Approach
Implemented a dual-layer security approach for character updates:
1. Database-level verification using a WHERE clause that matches both CharacterID and OwnerUserID
2. API-level verification that checks if any rows were affected by the update

This ensures that users can only modify characters they own, protecting against potential privilege escalation vulnerabilities.

### Input Validation Strategy
Implemented comprehensive input validation at both the database and API levels to ensure data integrity:
1. Database-level validation enforces core business rules (level = 1 for new characters, stats > 0)
2. API-level validation ensures required fields are present before database operations

This dual-validation approach provides defense in depth against invalid or malicious inputs.

### Backward Compatibility
Maintained backward compatibility by keeping the existing `getCharactersByOwnerUserID` function as a wrapper around the new `getCharactersByUserId` function. This ensures that existing code using the old function name continues to work while moving forward with the new implementation.

## Verification
The implemented character management system allows authenticated users to create, update, and retrieve their characters with proper validation and security. All operations are performed with parameterized queries to prevent SQL injection vulnerabilities. The implementation strictly adheres to the task requirements, focusing solely on data persistence and basic validation, leaving complex game logic to reside on the client.