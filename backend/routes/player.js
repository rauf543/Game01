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
    const characters = await characterDb.getCharactersByOwnerUserID(req.user.UserID);
    
    console.log(`Retrieved ${characters.length} characters for user:`, req.user.UserID);
    
    // Return success response with roster
    return res.json({
      success: true,
      roster: characters.map(char => ({
        CharacterID: char.CharacterID,
        Level: char.Level,
        XP: char.XP,
        MaxHP: char.MaxHP,
        MaxEnergy: char.MaxEnergy
      }))
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
    const characterData = req.body;
    
    // Ensure the character belongs to the authenticated user
    characterData.OwnerUserID = req.user.UserID;
    
    // Create the character
    const newCharacter = await characterDb.createCharacter(characterData);
    
    console.log('Character created:', newCharacter);
    
    // Return success response with new character data
    return res.status(201).json({
      success: true,
      message: "Character created",
      character: {
        CharacterID: newCharacter.CharacterID,
        Level: newCharacter.Level,
        XP: newCharacter.XP,
        MaxHP: newCharacter.MaxHP,
        MaxEnergy: newCharacter.MaxEnergy
      }
    });
  } catch (error) {
    console.error('Error creating character:', error.message);
    
    // Return failure response
    return res.status(500).json({
      success: false,
      message: "Error creating character"
    });
  }
});

/**
 * Update Character endpoint (UPDATE)
 * PUT /api/player/character/:characterID
 */
router.put('/character/:characterID', authenticateUser, async (req, res) => {
  const { characterID } = req.params;
  const updateData = req.body;
  
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
        message: "You do not have permission to update this character"
      });
    }
    
    // Update the character
    const updatedCharacter = await characterDb.updateCharacter(characterID, updateData);
    
    console.log('Character updated:', updatedCharacter);
    
    // Return success response with updated character data
    return res.json({
      success: true,
      message: "Character updated",
      character: {
        CharacterID: updatedCharacter.CharacterID,
        Level: updatedCharacter.Level,
        XP: updatedCharacter.XP,
        MaxHP: updatedCharacter.MaxHP,
        MaxEnergy: updatedCharacter.MaxEnergy
      }
    });
  } catch (error) {
    console.error('Error updating character:', error.message);
    
    // Return failure response
    return res.status(500).json({
      success: false,
      message: "Error updating character"
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