const sqlite3 = require('sqlite3').verbose();
const path = require('path');

// Create path to database file
const dbPath = path.join(__dirname, '../data/game.db');

// Connect to SQLite database
const db = new sqlite3.Database(dbPath, (err) => {
  if (err) {
    console.error('Error connecting to database:', err.message);
    return;
  }
  console.log('Connected to SQLite database');
  
  // Initialize the database tables
  initializeDatabase();
});

// Set up database tables
function initializeDatabase() {
  // Enable foreign keys
  db.run('PRAGMA foreign_keys = ON');
  
  // Create users table if it doesn't exist
  db.run(`
    CREATE TABLE IF NOT EXISTS users (
      userId TEXT PRIMARY KEY,
      username TEXT NOT NULL UNIQUE,
      password TEXT NOT NULL
    )
  `, (err) => {
    if (err) {
      console.error('Error creating users table:', err.message);
      return;
    }
    console.log('Users table initialized');
    
    // Insert sample user data if none exists
    db.get('SELECT COUNT(*) as count FROM users', (err, row) => {
      if (err) {
        console.error('Error checking users count:', err.message);
        return;
      }
      
      if (row.count === 0) {
        insertSampleUsers();
      }
    });
  });
  
  // Create characters table if it doesn't exist
  db.run(`
    CREATE TABLE IF NOT EXISTS characters (
      characterId TEXT PRIMARY KEY,
      userId TEXT,
      level INTEGER DEFAULT 1,
      xp INTEGER DEFAULT 0,
      maxHp INTEGER DEFAULT 100,
      maxEnergy INTEGER DEFAULT 50,
      FOREIGN KEY (userId) REFERENCES users(userId)
    )
  `, (err) => {
    if (err) {
      console.error('Error creating characters table:', err.message);
      return;
    }
    console.log('Characters table initialized');
    
    // Insert sample character data if none exists
    db.get('SELECT COUNT(*) as count FROM characters', (err, row) => {
      if (err) {
        console.error('Error checking characters count:', err.message);
        return;
      }
      
      if (row.count === 0) {
        insertSampleCharacters();
      }
    });
  });
}

// Insert sample users for testing
function insertSampleUsers() {
  const users = [
    { userId: 'user1', username: 'testuser', password: 'password123' },
    { userId: 'user2', username: 'admin', password: 'admin123' }
  ];
  
  const stmt = db.prepare('INSERT INTO users (userId, username, password) VALUES (?, ?, ?)');
  users.forEach(user => {
    stmt.run(user.userId, user.username, user.password, (err) => {
      if (err) {
        console.error('Error inserting sample user:', err.message);
      }
    });
  });
  stmt.finalize();
  console.log('Sample users inserted');
}

// Insert sample characters for testing
function insertSampleCharacters() {
  const characters = [
    { characterId: 'char1', userId: 'user1', level: 1, xp: 0, maxHp: 100, maxEnergy: 50 },
    { characterId: 'char2', userId: 'user1', level: 2, xp: 150, maxHp: 120, maxEnergy: 60 },
    { characterId: 'char3', userId: 'user2', level: 3, xp: 350, maxHp: 150, maxEnergy: 75 }
  ];
  
  const stmt = db.prepare(
    'INSERT INTO characters (characterId, userId, level, xp, maxHp, maxEnergy) VALUES (?, ?, ?, ?, ?, ?)'
  );
  characters.forEach(char => {
    stmt.run(char.characterId, char.userId, char.level, char.xp, char.maxHp, char.maxEnergy, (err) => {
      if (err) {
        console.error('Error inserting sample character:', err.message);
      }
    });
  });
  stmt.finalize();
  console.log('Sample characters inserted');
}

// User-related database operations
const userDb = {
  // Find user by username
  findUserByUsername(username) {
    return new Promise((resolve, reject) => {
      db.get('SELECT * FROM users WHERE username = ?', [username], (err, row) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(row);
      });
    });
  },
  
  // Find user by ID
  findUserById(userId) {
    return new Promise((resolve, reject) => {
      db.get('SELECT * FROM users WHERE userId = ?', [userId], (err, row) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(row);
      });
    });
  }
};

// Character-related database operations
const characterDb = {
  // Get all characters for a user
  getCharactersByUserId(userId) {
    return new Promise((resolve, reject) => {
      db.all('SELECT * FROM characters WHERE userId = ?', [userId], (err, rows) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(rows || []);
      });
    });
  },
  
  // Get a specific character by ID
  getCharacterById(characterId) {
    return new Promise((resolve, reject) => {
      db.get('SELECT * FROM characters WHERE characterId = ?', [characterId], (err, row) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(row);
      });
    });
  },
  
  // Save/update a character
  saveCharacter(character) {
    return new Promise((resolve, reject) => {
      db.get('SELECT * FROM characters WHERE characterId = ?', [character.characterId], (err, row) => {
        if (err) {
          reject(err);
          return;
        }
        
        // If character exists, update it
        if (row) {
          db.run(
            'UPDATE characters SET level = ?, xp = ?, maxHp = ?, maxEnergy = ? WHERE characterId = ?',
            [character.level, character.xp, character.maxHp, character.maxEnergy, character.characterId],
            function(err) {
              if (err) {
                reject(err);
                return;
              }
              
              // Get updated character data
              characterDb.getCharacterById(character.characterId)
                .then(updated => resolve(updated))
                .catch(err => reject(err));
            }
          );
        } 
        // If character doesn't exist, insert it
        else {
          // Let's assume we're assigning it to the first user for simplicity in this stub
          const userId = 'user1';
          
          db.run(
            'INSERT INTO characters (characterId, userId, level, xp, maxHp, maxEnergy) VALUES (?, ?, ?, ?, ?, ?)',
            [character.characterId, userId, character.level, character.xp, character.maxHp, character.maxEnergy],
            function(err) {
              if (err) {
                reject(err);
                return;
              }
              
              // Get the newly inserted character data
              characterDb.getCharacterById(character.characterId)
                .then(inserted => resolve(inserted))
                .catch(err => reject(err));
            }
          );
        }
      });
    });
  }
};

// Export the database and operations
module.exports = {
  db,
  userDb,
  characterDb
};