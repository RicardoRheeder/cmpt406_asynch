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
    - gcloud app deploy ./app/app.yaml
    - Should only be done on the clean MASTER BRANCH

Indexes:
-----------------------
- To remove indexes no longer in the index.yaml file:
    gcloud datastore indexes cleanup index.yaml
- To update new indexes:
    gcloud datastore indexes create index.yaml

---------------------------------------------------------------------
API Interface
---------------------------------------------------------------------

Root url: https://async-406.appspot.com


Create User:
-----------------------
This endpoint will create a user. A created user is needed to be authed
for all other endpoints.

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
Given a username this endpoint will return all the data for a user. Can 
also be used to validate an inputed username and password is correct

 - Path: /GetUser
 - GET
 - Auth: Basic Auth
 - Body: None
 - Return: User Data
 - Example Return Data:
    {   
        "activeGames": ["GameID1", "GameID2"]
        "PendingPrivateGames": ["GameId3"]
        "PendingPublicGames": ["GameId6"]
    }


Add Friend:
-----------------------
This endpoint will add a friend to the friends list of the user. Provided
freinds name must be an existing user.

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
This endpoint will remove a friend to the friends list of the user. Provided
freinds must actually be on their freinds list.

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
By creating a Private Game you are inviting x number of users to play against you. 
Only the provided usernames will be able to join the game 

 - Path: /CreatePrivateGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameName": "FightMeYouLilBitch",
        "turnTime": 3600,
        "timeToStartTurn": 172800,
        "opponentUsernames": ["username1", "username2"],
        "boardId": 6
    }
 - Return: Http Resonse Code


Create Public Game:
-----------------------
By creating a Public Game you are openly inviting any user to play against you.
Once the total spots fill up and are "Ready", the game begins.

 - Path: /CreatePublicGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameName": "WhoWantsToPlay",
        "turnTime": 3600,
        "timeToStartTurn": 172800,
        "maxUsers": 3,
        "boardId": 6
    }
 - Return: Http Resonse Code


Accept Game: 
-----------------------
This will accept both a Private Game you've been invited to or a Public Game.

 - Path: /AcceptGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameId": "123-456"
    }
 - Return: Http Resonse Code


Decline Game:
-----------------------
Used to decline the invite to a Private Game. If you were not the only person
invited to this game, the game can continued to be played without you 

 - Path: /DeclineGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameId": "123-456"
    }
 - Return: Http Resonse Code


BackOut Game: 
-----------------------
For when you've accepted a public game and then changed your mind. Can only be
done BEFORE you've placed your army.

 - Path: /BackOutGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameId": "123-456"
    }
 - Return: Http Resonse Code


Forfeit Game: 
-----------------------
For any time after you've placed your army. This will remove you from the game
as if you've been killed.

 - Path: /ForfeitGame
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
Gets all of the data for a provided GameStateID

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
Gets all of the data for multiple provided GameStateIDs

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
            "id": "123-456",
            "boardId": 9,
            "users": ["user1", "user2", "user3"],
            "aliveUsers": ["user1", "user2"]
        },
        {
            "id": "111-222",
            "boardId": 5,
            "users": ["user4", "user5"],
            "aliveUsers": ["user4", "user5"]
        }
    ]


Get Public Games Summary:
-----------------------
Gets a subset of the fields for all Public GameStates that have open spots

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
            "id": "123-456",
            "boardId": 9,
            "spotsAvailable": 1,
            "maxUsers": 6,
        },
        {
            "id": "111-222",
            "boardId": 5,            
            "spotsAvailable": 3,
            "maxUsers": 4,
        }
    ]
    

Update Game State:
-----------------------
For when you are placing your army or making a turn in a game.

 - Path: /UpdateGameState
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameId": "123-456"
        "readyUsers": ["User1", "User2", "User3"]
        "units": {
            "User3": [
                {
                    "unitType": 5,
                    "health": 10,
                    "coord": {1, 2}
                },
                {
                    "unitType": 2,
                    "health": 5,
                    "coord": {2, 2} 
                }
            ]
        },
        "cards": {
            "User3": {
                "hand": ["cardId1", "cardId2", "cardId1"],
                "deck": ["cardId2", "cardId3", "cardId4", "cardId3"],
                "discard": []
            }
        }
    }
 - Return: Http Resonse Code
