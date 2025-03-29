const express = require('express');
const router = express.Router();
const { userDb } = require('../db/database');

/**
 * User Login endpoint
 * POST /api/auth/login
 */
router.post('/login', async (req, res) => {
  console.log('Login request received:', req.body);
  
  try {
    const { username, password } = req.body;
    
    // Validate request payload
    if (!username || !password) {
      return res.status(400).json({
        success: false,
        message: "Username and password are required"
      });
    }
    
    // Find user in database
    const user = await userDb.findUserByUsername(username);
    
    // Check if user exists and password matches
    if (user && user.password === password) {
      console.log('Login successful for user:', username);
      
      // Return success response with userId
      return res.json({
        success: true,
        message: "Login successful",
        userId: user.userId
      });
    } else {
      console.log('Login failed for user:', username);
      
      // Return failure response
      return res.status(401).json({
        success: false,
        message: "Invalid credentials"
      });
    }
  } catch (error) {
    console.error('Error during login:', error.message);
    
    // Return error response
    return res.status(500).json({
      success: false,
      message: "Error during login"
    });
  }
});

module.exports = router;