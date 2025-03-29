const express = require('express');
const router = express.Router();
const { userDb, characterDb } = require('../db/database');

// Simple authentication middleware for simulation purposes
const authenticateUser = async (req, res, next) => {
  // Check for user ID in request header
  const userId = req.header('X-User-ID');
  
  if (!userId) {
    return res.status(401).json({
      success: false,
      message: "Authentication required"
    });
  }
  
  try {
    // Check if user exists
    const user = await userDb.findUserById(userId);
    
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
 * Get Player Roster endpoint
 * GET /api/player/roster
 */
router.get('/roster', authenticateUser, async (req, res) => {
  console.log('Roster request received for user:', req.user.userId);
  
  try {
    // Get characters for the authenticated user
    const characters = await characterDb.getCharactersByUserId(req.user.userId);
    
    console.log(`Retrieved ${characters.length} characters for user:`, req.user.userId);
    
    // Return success response with roster
    return res.json({
      success: true,
      roster: characters.map(char => ({
        characterId: char.characterId,
        level: char.level,
        xp: char.xp,
        maxHp: char.maxHp,
        maxEnergy: char.maxEnergy
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
 * Save Player Character endpoint
 * POST /api/player/character
 */
router.post('/character', authenticateUser, async (req, res) => {
  console.log('Save character request received:', req.body);
  
  try {
    const characterData = req.body;
    
    // Validate character data
    if (!characterData || !characterData.characterId) {
      return res.status(400).json({
        success: false,
        message: "Character ID is required"
      });
    }
    
    // Save character to database
    const savedCharacter = await characterDb.saveCharacter(characterData);
    
    console.log('Character saved:', savedCharacter);
    
    // Return success response with updated character data
    return res.json({
      success: true,
      message: "Character saved",
      character: {
        characterId: savedCharacter.characterId,
        level: savedCharacter.level,
        xp: savedCharacter.xp,
        maxHp: savedCharacter.maxHp,
        maxEnergy: savedCharacter.maxEnergy
      }
    });
  } catch (error) {
    console.error('Error saving character:', error.message);
    
    // Return failure response
    return res.status(500).json({
      success: false,
      message: "Error saving character"
    });
  }
});

module.exports = router;