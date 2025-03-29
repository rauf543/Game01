const express = require('express');
const path = require('path');

// Import routes
const authRoutes = require('./routes/auth');
const playerRoutes = require('./routes/player');

// Initialize database
require('./db/database');

// Create Express app
const app = express();
const PORT = process.env.PORT || 3000;

// Log all incoming requests
app.use((req, res, next) => {
  console.log(`${new Date().toISOString()} - ${req.method} ${req.url}`);
  next();
});

// Middleware to parse JSON requests
app.use(express.json());

// Register routes
app.use('/api/auth', authRoutes);
app.use('/api/player', playerRoutes);

// Basic error handling
app.use((err, req, res, next) => {
  console.error('Unhandled error:', err);
  res.status(500).json({
    success: false,
    message: "Internal server error"
  });
});

// Handle 404 for any other routes
app.use((req, res) => {
  res.status(404).json({
    success: false,
    message: "Endpoint not found"
  });
});

// Start the server
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
  console.log(`API endpoints:`);
  console.log(`  POST /api/auth/login`);
  console.log(`  GET /api/player/roster`);
  console.log(`  POST /api/player/character`);
});