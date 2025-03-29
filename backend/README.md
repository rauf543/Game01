# Game Backend Service

A minimal backend service using Node.js with Express and SQLite for game data management.

## Setup

1. Install dependencies:
   ```
   npm install
   ```

2. Start the server:
   ```
   npm start
   ```

The server will run on port 3000 by default.

## API Endpoints

### User Login

Authenticates a user with username and password.

- **URL:** `/api/auth/login`
- **Method:** `POST`
- **Request Body:**
  ```json
  {
    "username": "string",
    "password": "string"
  }
  ```
- **Success Response:**
  ```json
  {
    "success": true,
    "message": "Login successful",
    "userId": "some_unique_user_id"
  }
  ```
- **Error Response:**
  ```json
  {
    "success": false,
    "message": "Invalid credentials"
  }
  ```

### Get Player Roster

Retrieves the character roster for the authenticated user.

- **URL:** `/api/player/roster`
- **Method:** `GET`
- **Headers:**
  ```
  X-User-ID: user1
  ```
- **Success Response:**
  ```json
  {
    "success": true,
    "roster": [
      {
        "characterId": "char1",
        "level": 1,
        "xp": 0,
        "maxHp": 100,
        "maxEnergy": 50
      },
      {
        "characterId": "char2",
        "level": 2,
        "xp": 150,
        "maxHp": 120,
        "maxEnergy": 60
      }
    ]
  }
  ```
- **Empty Roster Response:**
  ```json
  {
    "success": true,
    "roster": []
  }
  ```
- **Error Response:**
  ```json
  {
    "success": false,
    "message": "Error fetching roster"
  }
  ```

### Save Character

Saves or updates a character's data.

- **URL:** `/api/player/character`
- **Method:** `POST`
- **Headers:**
  ```
  X-User-ID: user1
  ```
- **Request Body:**
  ```json
  {
    "characterId": "char1",
    "level": 2,
    "xp": 150,
    "maxHp": 110,
    "maxEnergy": 55
  }
  ```
- **Success Response:**
  ```json
  {
    "success": true,
    "message": "Character saved",
    "character": {
      "characterId": "char1",
      "level": 2,
      "xp": 150,
      "maxHp": 110,
      "maxEnergy": 55
    }
  }
  ```
- **Error Response:**
  ```json
  {
    "success": false,
    "message": "Error saving character"
  }
  ```

## Sample Data

The backend comes pre-configured with sample data for testing:

- **Users:**
  - Username: `testuser`, Password: `password123`
  - Username: `admin`, Password: `admin123`

- **Characters:**
  - Two characters for the first user
  - One character for the second user