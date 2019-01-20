---------------------------------------------------------------------
Server Documentation
---------------------------------------------------------------------

- Hosting: 
    - Google Cloud Platform using App Engine
    https://console.cloud.google.com/home/dashboard?organizationId=0&project=async-406

- Storage:
    - Datastore

- Logging:
    - Logging statments go to the Google Cloud Platform where you can search through them


Required Setup:
-----------------------
- GCP has it's own cloud SDK you need to download
    - https://cloud.google.com/sdk/

- This project uses Go (required installing of language)
    - https://golang.org/doc/install

Note: Git Project needs to be placed inside the go path.
    - ex: user/go/src/Projects


Running Locally:
-----------------------
    - dev_appserver.py ./app/app.yaml
    - defaults to http://localhost:8080
    - Note: datastore puts and gets while running locally do not propogate to the deployed Datastore


Testing:
-----------------------
    - When running locally, use Postman or another tool to send http Posts and Gets to the endpoint
    - Ask Parker for the Postman Collection


Deploy Project:
-----------------------
    - gcloud app deployed ./app/app.yaml
    - Should only be done on the clean MASTER BRANCH


---------------------------------------------------------------------
API Interface
---------------------------------------------------------------------

Root url: https://async-406.appspot.com


Create User:
-----------------------
 - Path: /CreateUser
 - POST
 - Auth: None
 - Body: JSON
 - Request Body Example:
    {   
        "username": "usernameTest2",
        "password": "thisIsPassword"
    }
 - Return: Http Resonse Code


Get User:
-----------------------
 - Path: /GetUser
 - GET
 - Auth: Basic Auth
 - Body: None
 - Return: User Data
 - Example Return Data:
    {   
        "activeGames": ["GameID1", "GameID2"]
        "sentInvites": ["GameId3"]
    }


Add Friend:
-----------------------
 - Path: /AddFriend
 - POST
 - Auth: Basic Auth
  - Request Body Example:
    {   
        "username": "myNewFriend1"
    }
 - Return: Http Resonse Code


Remove Friend:
-----------------------
 - Path: /RemoveFriend
 - POST
 - Auth: Basic Auth
  - Request Body Example:
    {   
        "username": "fuckThisFriendHeOut420"
    }
 - Return: Http Resonse Code


Create Private Game:
-----------------------
 - Path: /CreatePrivateGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "opponentUsernames": ["username1", "username2"],
        "boardId": 6
    }
 - Return: Http Resonse Code


Create Public Game:
-----------------------
 - Path: /CreatePublicGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "maxUsers": 3,
        "boardId": 6
    }
 - Return: Http Resonse Code


Accept Game: (used for both private and public)
-----------------------
 - Path: /AcceptGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameId": "123-456"
    }
 - Return: Http Resonse Code


Get Game State:
-----------------------
 - Path: /GetGameState
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {
        "gameId": "123-456"
    }
 - Return: GameState Data
 - Example Return Data:
    {   
        "boardId": 2,
        "users": ["user1", "user2", "user3"],
        "aliveUsers": ["user1", "user2"]
    }


Get Game State Multi:
-----------------------
 - Path: /GetGameStateMulti
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {
        "gameIds": ["123-456", "111-222"]
    }
 - Return: Array of GameState Data
 - Example Return Data:
    [
        {   
            "Id": "123-456",
            "boardId": 9,
            "users": ["user1", "user2", "user3"],
            "aliveUsers": ["user1", "user2"]
        },
        {
            "Id": "111-222",
            "boardId": 5,
            "users": ["user4", "user5"],
            "aliveUsers": ["user4", "user5"]
        }
    ]


Get Public Games Summary:
-----------------------
- Path: /GetPublicGamesSummary
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {
        "limit": 100
    }
 - Return: Array of Public GameState Data (with more limited fields)
 - Example Return Data:
    [
        {   
            "Id": "123-456",
            "boardId": 9,
            "spotsAvailable": 1,
            "isPublic": true,
            "users": ["user1", "user2", "user3"]
        },
        {
            "Id": "111-222",
            "boardId": 5,            
            "spotsAvailable": 3,
            "isPublic": true,
            "users": ["user4", "user5"]
        }
    ]
