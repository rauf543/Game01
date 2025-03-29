const express = require('express');
const router = express.Router();
const { userDb } = require('../db/database');

/**
 * User Registration endpoint
 * POST /api/auth/register
 */
router.post('/register', async (req, res) => {
  console.log('Registration request received');
  
  try {
    const { username, password } = req.body;
    
    // Validate request payload
    if (!username || !password) {
      return res.status(400).json({
        success: false,
        message: "Username and password are required"
      });
    }
    
    // Check if username already exists
    const existingUser = await userDb.getUserByUsername(username);
    if (existingUser) {
      return res.status(409).json({
        success: false,
        message: "Username already exists"
      });
    }
    
    // Create new user with hashed password
    const user = await userDb.createUser(username, password);
    
    console.log('User registered successfully:', username);
    
    // Return success response
    return res.status(201).json({
      success: true,
      message: "User registered successfully",
      user: {
        UserID: user.UserID,
        username: user.username
      }
    });
  } catch (error) {
    console.error('Error during registration:', error.message);
    
    // Return error response
    return res.status(500).json({
      success: false,
      message: "Error during registration"
    });
  }
});

/**
 * User Login endpoint
 * POST /api/auth/login
 */
router.post('/login', async (req, res) => {
  console.log('Login request received');
  
  try {
    const { username, password } = req.body;
    
    // Validate request payload
    if (!username || !password) {
      return res.status(400).json({
        success: false,
        message: "Username and password are required"
      });
    }
    
    // Verify user credentials (this checks the password with bcrypt)
    const user = await userDb.verifyPassword(username, password);
    
    // Check if verification was successful
    if (user) {
      console.log('Login successful for user:', username);
      
      // Return success response with UserID
      return res.json({
        success: true,
        message: "Login successful",
        UserID: user.UserID
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

/**
 * Update User endpoint
 * PUT /api/auth/user/:userID
 */
router.put('/user/:userID', async (req, res) => {
  try {
    const { userID } = req.params;
    const updateData = req.body;
    
    // Update the user
    const updatedUser = await userDb.updateUser(userID, updateData);
    
    // Return success response
    return res.json({
      success: true,
      message: "User updated successfully",
      user: updatedUser
    });
  } catch (error) {
    console.error('Error updating user:', error.message);
    
    // Return error response
    return res.status(500).json({
      success: false,
      message: "Error updating user"
    });
  }
});

/**
 * Delete User endpoint
 * DELETE /api/auth/user/:userID
 */
router.delete('/user/:userID', async (req, res) => {
  try {
    const { userID } = req.params;
    
    // Delete the user
    const result = await userDb.deleteUser(userID);
    
    if (result.deleted) {
      // Return success response
      return res.json({
        success: true,
        message: "User deleted successfully"
      });
    } else {
      // User not found
      return res.status(404).json({
        success: false,
        message: "User not found"
      });
    }
  } catch (error) {
    console.error('Error deleting user:', error.message);
    
    // Return error response
    return res.status(500).json({
      success: false,
      message: "Error deleting user"
    });
  }
});

module.exports = router;