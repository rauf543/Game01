const sqlite3 = require('sqlite3').verbose();
const path = require('path');
const bcrypt = require('bcrypt');

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
async function initializeDatabase() {
  try {
    // Enable foreign keys
    await runQuery('PRAGMA foreign_keys = ON');
    
    // Drop existing tables if they exist to avoid conflicts
    await runQuery('DROP TABLE IF EXISTS Characters');
    await runQuery('DROP TABLE IF EXISTS Users');
    
    // 1. Create Users table first
    await runQuery(`
      CREATE TABLE IF NOT EXISTS Users (
        UserID TEXT PRIMARY KEY,
        username TEXT NOT NULL UNIQUE,
        hashed_password TEXT NOT NULL
      )
    `);
    console.log('Users table initialized');
    
    // 2. Insert sample users
    await insertSampleUsers();
    
    // 3. Create Characters table after users are created
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
    console.log('Characters table initialized');
    
    // 4. Insert sample characters
    const count = await getCount('Characters');
    if (count === 0) {
      await insertSampleCharacters();
    }
  } catch (error) {
    console.error('Error initializing database:', error.message);
  }
}

// Helper function to run a query and return a promise
function runQuery(sql, params = []) {
  return new Promise((resolve, reject) => {
    db.run(sql, params, function(err) {
      if (err) {
        reject(err);
      } else {
        resolve(this);
      }
    });
  });
}

// Helper function to get count from a table
function getCount(tableName) {
  return new Promise((resolve, reject) => {
    db.get(`SELECT COUNT(*) as count FROM ${tableName}`, (err, row) => {
      if (err) {
        reject(err);
      } else {
        resolve(row.count);
      }
    });
  });
}

// Helper function to hash passwords
async function hashPassword(password) {
  const saltRounds = 10;
  return await bcrypt.hash(password, saltRounds);
}

// Insert sample users for testing
async function insertSampleUsers() {
  return new Promise(async (resolve, reject) => {
    try {
      const users = [
        { UserID: 'user1', username: 'testuser', password: 'password123' },
        { UserID: 'user2', username: 'admin', password: 'admin123' }
      ];
      
      // Using Promise.all to insert all users in parallel
      const insertPromises = users.map(async user => {
        // Hash the password before inserting
        const hashedPassword = await hashPassword(user.password);
        
        return new Promise((resolve, reject) => {
          db.run(
            'INSERT INTO Users (UserID, username, hashed_password) VALUES (?, ?, ?)',
            [user.UserID, user.username, hashedPassword],
            function(err) {
              if (err) {
                console.error('Error inserting user:', err.message);
                reject(err);
              } else {
                resolve();
              }
            }
          );
        });
      });
      
      await Promise.all(insertPromises);
      console.log('Sample users inserted');
      resolve();
    } catch (error) {
      console.error('Error inserting sample users:', error.message);
      reject(error);
    }
  });
}

// Insert sample characters for testing
function insertSampleCharacters() {
  return new Promise((resolve, reject) => {
    const characters = [
      { CharacterID: 'char1', OwnerUserID: 'user1', Level: 1, XP: 0, MaxHP: 100, MaxEnergy: 50, Name: 'Warrior' },
      { CharacterID: 'char2', OwnerUserID: 'user1', Level: 2, XP: 150, MaxHP: 120, MaxEnergy: 60, Name: 'Mage' },
      { CharacterID: 'char3', OwnerUserID: 'user2', Level: 3, XP: 350, MaxHP: 150, MaxEnergy: 75, Name: 'Rogue' }
    ];
    
    // Use Promise.all to handle all insertions
    const insertPromises = characters.map(char => {
      return new Promise((resolve, reject) => {
        db.run(
          'INSERT INTO Characters (CharacterID, OwnerUserID, Level, XP, MaxHP, MaxEnergy, Name) VALUES (?, ?, ?, ?, ?, ?, ?)',
          [char.CharacterID, char.OwnerUserID, char.Level, char.XP, char.MaxHP, char.MaxEnergy, char.Name],
          function(err) {
            if (err) {
              console.error('Error inserting sample character:', err.message);
              // We'll still resolve so one failed character doesn't fail the whole process
              resolve();
            } else {
              resolve();
            }
          }
        );
      });
    });
    
    Promise.all(insertPromises)
      .then(() => {
        console.log('Sample characters inserted');
        resolve();
      })
      .catch(err => {
        console.error('Error during character insertion:', err.message);
        reject(err);
      });
  });
}

// User-related database operations
const userDb = {
  // CREATE - Create a new user
  createUser(username, password) {
    return new Promise(async (resolve, reject) => {
      try {
        // Hash the password
        const hashed_password = await hashPassword(password);
        
        // Generate a random UserID
        const UserID = 'user_' + Date.now();
        
        // Insert the new user
        db.run(
          'INSERT INTO Users (UserID, username, hashed_password) VALUES (?, ?, ?)',
          [UserID, username, hashed_password],
          function(err) {
            if (err) {
              reject(err);
              return;
            }
            
            // Return the new user (without password)
            resolve({
              UserID,
              username
            });
          }
        );
      } catch (error) {
        reject(error);
      }
    });
  },
  
  // READ - Get user by username
  getUserByUsername(username) {
    return new Promise((resolve, reject) => {
      db.get('SELECT UserID, username, hashed_password FROM Users WHERE username = ?', [username], (err, row) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(row);
      });
    });
  },
  
  // READ - Get user by ID
  getUserByID(UserID) {
    return new Promise((resolve, reject) => {
      db.get('SELECT UserID, username, hashed_password FROM Users WHERE UserID = ?', [UserID], (err, row) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(row);
      });
    });
  },
  
  // UPDATE - Update user information
  updateUser(UserID, updateData) {
    return new Promise(async (resolve, reject) => {
      try {
        // Start with the current user data
        const currentUser = await this.getUserByID(UserID);
        
        if (!currentUser) {
          reject(new Error('User not found'));
          return;
        }
        
        // Prepare update variables
        let username = updateData.username || currentUser.username;
        let hashed_password = currentUser.hashed_password;
        
        // Hash the new password if provided
        if (updateData.password) {
          hashed_password = await hashPassword(updateData.password);
        }
        
        // Update the user
        db.run(
          'UPDATE Users SET username = ?, hashed_password = ? WHERE UserID = ?',
          [username, hashed_password, UserID],
          function(err) {
            if (err) {
              reject(err);
              return;
            }
            
            // Return the updated user (without password)
            resolve({
              UserID,
              username
            });
          }
        );
      } catch (error) {
        reject(error);
      }
    });
  },
  
  // DELETE - Delete a user
  deleteUser(UserID) {
    return new Promise((resolve, reject) => {
      // First delete any characters owned by this user
      db.run('DELETE FROM Characters WHERE OwnerUserID = ?', [UserID], (err) => {
        if (err) {
          reject(err);
          return;
        }
        
        // Then delete the user
        db.run('DELETE FROM Users WHERE UserID = ?', [UserID], function(err) {
          if (err) {
            reject(err);
            return;
          }
          
          // Return the number of affected rows
          resolve({ deleted: this.changes > 0 });
        });
      });
    });
  },
  
  // Authentication - Verify password
  verifyPassword(username, password) {
    return new Promise(async (resolve, reject) => {
      try {
        // Get the user with the hashed password
        const user = await this.getUserByUsername(username);
        
        if (!user) {
          resolve(false);
          return;
        }
        
        // Compare the provided password with the stored hash
        const match = await bcrypt.compare(password, user.hashed_password);
        
        if (match) {
          // Return the user without the password
          const { hashed_password, ...userWithoutPassword } = user;
          resolve(userWithoutPassword);
        } else {
          resolve(false);
        }
      } catch (error) {
        reject(error);
      }
    });
  }
};

// Character-related database operations
const characterDb = {
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
  },
  
  // READ - Get all characters for a user (roster)
  getCharactersByUserId(ownerUserId) {
    return new Promise((resolve, reject) => {
      db.all(
        'SELECT CharacterID, OwnerUserID, Level, XP, MaxHP, MaxEnergy, Name FROM Characters WHERE OwnerUserID = ?',
        [ownerUserId],
        (err, rows) => {
          if (err) {
            reject(err);
            return;
          }
          resolve(rows || []);
        }
      );
    });
  },
  
  // READ - Get all characters for a user (keeping this for backward compatibility)
  getCharactersByOwnerUserID(OwnerUserID) {
    return this.getCharactersByUserId(OwnerUserID);
  },
  
  // READ - Get a specific character by ID (keeping this existing function)
  getCharacterByID(CharacterID) {
    return new Promise((resolve, reject) => {
      db.get('SELECT * FROM Characters WHERE CharacterID = ?', [CharacterID], (err, row) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(row);
      });
    });
  },
  
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
  },
  
  // DELETE - Delete a character
  deleteCharacter(CharacterID) {
    return new Promise((resolve, reject) => {
      db.run('DELETE FROM Characters WHERE CharacterID = ?', [CharacterID], function(err) {
        if (err) {
          reject(err);
          return;
        }
        
        // Return the number of affected rows
        resolve({ deleted: this.changes > 0 });
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