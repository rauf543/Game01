const express = require('express');
const router = express.Router();
const { userDb, characterDb } = require('../db/database');

// Simple authentication middleware for simulation purposes
const authenticateUser = async (req, res, next) => {
  // Check for user ID in request header
  const UserID = req.header('X-User-ID');
  
  if (!UserID) {
    return res.status(401).json({
      success: false,
      message: "Authentication required"
    });
  }
  
  try {
    // Check if user exists
    const user = await userDb.getUserByID(UserID);
    
    if (!user) {
      return res.status(401).json({
        success: false,
        message: "Invalid user ID"
      });
    }
    
    // Add user to request object
    req.user = user;
    next();
  } catch (error) {
    console.error('Authentication error:', error.message);
    return res.status(500).json({
      success: false,
      message: "Error during authentication"
    });
  }
};

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

/**
 * Get Character by ID endpoint (READ)
 * GET /api/player/character/:characterID
 */
router.get('/character/:characterID', authenticateUser, async (req, res) => {
  const { characterID } = req.params;
  
  try {
    // Get character by ID
    const character = await characterDb.getCharacterByID(characterID);
    
    if (!character) {
      return res.status(404).json({
        success: false,
        message: "Character not found"
      });
    }
    
    // Check if the character belongs to the authenticated user
    if (character.OwnerUserID !== req.user.UserID) {
      return res.status(403).json({
        success: false,
        message: "You do not have permission to access this character"
      });
    }
    
    // Return character data
    return res.json({
      success: true,
      character: {
        CharacterID: character.CharacterID,
        Level: character.Level,
        XP: character.XP,
        MaxHP: character.MaxHP,
        MaxEnergy: character.MaxEnergy
      }
    });
  } catch (error) {
    console.error('Error fetching character:', error.message);
    
    return res.status(500).json({
      success: false,
      message: "Error fetching character"
    });
  }
});

/**
 * Create Character endpoint (CREATE)
 * POST /api/player/character
 */
router.post('/character', authenticateUser, async (req, res) => {
  console.log('Create character request received');
  
  try {
    // Extract character data from request body
    const { name, level, xp, maxHp, maxEnergy } = req.body;
    
    // Check if all required fields are present
    if (!name || level === undefined || xp === undefined || maxHp === undefined || maxEnergy === undefined) {
      return res.status(400).json({
        success: false,
        message: "Missing required character data. Name, level, xp, maxHp, and maxEnergy are required."
      });
    }
    
    // Extract owner user ID from the authenticated user
    const ownerUserId = req.user.UserID;
    
    // Create the character
    const characterData = { name, level, xp, maxHp, maxEnergy };
    const newCharacter = await characterDb.createCharacter(characterData, ownerUserId);
    
    console.log('Character created:', newCharacter);
    
    // Return success response with new character data
    return res.status(201).json({
      success: true,
      character: newCharacter
    });
  } catch (error) {
    console.error('Error creating character:', error.message);
    
    // Return appropriate error status based on the error
    let status = 500;
    if (error.message.includes('level') ||
        error.message.includes('xp') ||
        error.message.includes('MaxHP') ||
        error.message.includes('MaxEnergy') ||
        error.message.includes('name')) {
      status = 400; // Bad request for validation errors
    }
    
    // Return failure response
    return res.status(status).json({
      success: false,
      message: error.message
    });
  }
});

/**
 * Update Character endpoint (UPDATE)
 * PUT /api/player/character/:characterId
 */
router.put('/character/:characterId', authenticateUser, async (req, res) => {
  const { characterId } = req.params;
  const { level, xp, maxHp, maxEnergy } = req.body;
  const ownerUserId = req.user.UserID;
  
  try {
    // Check if all required fields are present
    if (level === undefined || xp === undefined || maxHp === undefined || maxEnergy === undefined) {
      return res.status(400).json({
        success: false,
        message: "Missing required character data. Level, xp, maxHp, and maxEnergy are required."
      });
    }
    
    // Update the character with validation
    const updatedCharacter = await characterDb.updateCharacter(
      characterId,
      { level, xp, maxHp, maxEnergy },
      ownerUserId
    );
    
    // Check if character was found and belongs to the user
    if (!updatedCharacter) {
      return res.status(404).json({
        success: false,
        message: "Character not found or you don't have permission to update it"
      });
    }
    
    console.log('Character updated:', updatedCharacter);
    
    // Return success response with updated character data
    return res.status(200).json({
      success: true,
      character: updatedCharacter
    });
  } catch (error) {
    console.error('Error updating character:', error.message);
    
    // Return appropriate error status based on the error
    let status = 500;
    if (error.message.includes('Level') ||
        error.message.includes('XP') ||
        error.message.includes('MaxHP') ||
        error.message.includes('MaxEnergy')) {
      status = 400; // Bad request for validation errors
    }
    
    // Return failure response
    return res.status(status).json({
      success: false,
      message: error.message
    });
  }
});

/**
 * Delete Character endpoint (DELETE)
 * DELETE /api/player/character/:characterID
 */
router.delete('/character/:characterID', authenticateUser, async (req, res) => {
  const { characterID } = req.params;
  
  try {
    // Get the character to make sure it exists and belongs to the user
    const character = await characterDb.getCharacterByID(characterID);
    
    if (!character) {
      return res.status(404).json({
        success: false,
        message: "Character not found"
      });
    }
    
    // Check if the character belongs to the authenticated user
    if (character.OwnerUserID !== req.user.UserID) {
      return res.status(403).json({
        success: false,
        message: "You do not have permission to delete this character"
      });
    }
    
    // Delete the character
    const result = await characterDb.deleteCharacter(characterID);
    
    if (result.deleted) {
      return res.json({
        success: true,
        message: "Character deleted successfully"
      });
    } else {
      return res.status(500).json({
        success: false,
        message: "Failed to delete character"
      });
    }
  } catch (error) {
    console.error('Error deleting character:', error.message);
    
    return res.status(500).json({
      success: false,
      message: "Error deleting character"
    });
  }
});

module.exports = router;