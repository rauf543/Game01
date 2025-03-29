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
      { CharacterID: 'char1', OwnerUserID: 'user1', Level: 1, XP: 0, MaxHP: 100, MaxEnergy: 50 },
      { CharacterID: 'char2', OwnerUserID: 'user1', Level: 2, XP: 150, MaxHP: 120, MaxEnergy: 60 },
      { CharacterID: 'char3', OwnerUserID: 'user2', Level: 3, XP: 350, MaxHP: 150, MaxEnergy: 75 }
    ];
    
    // Use Promise.all to handle all insertions
    const insertPromises = characters.map(char => {
      return new Promise((resolve, reject) => {
        db.run(
          'INSERT INTO Characters (CharacterID, OwnerUserID, Level, XP, MaxHP, MaxEnergy) VALUES (?, ?, ?, ?, ?, ?)',
          [char.CharacterID, char.OwnerUserID, char.Level, char.XP, char.MaxHP, char.MaxEnergy],
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
  // CREATE - Create a new character
  createCharacter(characterData) {
    return new Promise((resolve, reject) => {
      // Generate a CharacterID if not provided
      const CharacterID = characterData.CharacterID || 'char_' + Date.now();
      
      // Insert the new character
      db.run(
        'INSERT INTO Characters (CharacterID, OwnerUserID, Level, XP, MaxHP, MaxEnergy) VALUES (?, ?, ?, ?, ?, ?)',
        [
          CharacterID,
          characterData.OwnerUserID,
          characterData.Level || 1,
          characterData.XP || 0,
          characterData.MaxHP || 100,
          characterData.MaxEnergy || 50
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
  
  // READ - Get all characters for a user
  getCharactersByOwnerUserID(OwnerUserID) {
    return new Promise((resolve, reject) => {
      db.all('SELECT * FROM Characters WHERE OwnerUserID = ?', [OwnerUserID], (err, rows) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(rows || []);
      });
    });
  },
  
  // READ - Get a specific character by ID
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
  
  // UPDATE - Update a character
  updateCharacter(CharacterID, updateData) {
    return new Promise(async (resolve, reject) => {
      try {
        // Get the current character data
        const currentCharacter = await this.getCharacterByID(CharacterID);
        
        if (!currentCharacter) {
          reject(new Error('Character not found'));
          return;
        }
        
        // Prepare update variables, using current values as defaults
        const Level = updateData.Level !== undefined ? updateData.Level : currentCharacter.Level;
        const XP = updateData.XP !== undefined ? updateData.XP : currentCharacter.XP;
        const MaxHP = updateData.MaxHP !== undefined ? updateData.MaxHP : currentCharacter.MaxHP;
        const MaxEnergy = updateData.MaxEnergy !== undefined ? updateData.MaxEnergy : currentCharacter.MaxEnergy;
        
        // Update the character
        db.run(
          'UPDATE Characters SET Level = ?, XP = ?, MaxHP = ?, MaxEnergy = ? WHERE CharacterID = ?',
          [Level, XP, MaxHP, MaxEnergy, CharacterID],
          function(err) {
            if (err) {
              reject(err);
              return;
            }
            
            // Get the updated character data
            characterDb.getCharacterByID(CharacterID)
              .then(updated => resolve(updated))
              .catch(err => reject(err));
          }
        );
      } catch (error) {
        reject(error);
      }
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