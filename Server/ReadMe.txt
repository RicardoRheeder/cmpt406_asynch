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


Change Project:
-----------------------
    - gcloud config set project my-project
    (for if you're a part of multiple projects)


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

Add Army Preset:
-----------------------
This endpoint will add an army preset to the user model. Allowing them to save
army builds that they like for another time

 - Path: /AddArmyPreset
 - POST
 - Auth: Basic Auth
  - Request Body Example:
    {   
        "name": "Best Army Ever",
        "units": [1,5,6,9,9,9,9],
        "general": 4
    }
 - Return: Http Resonse Code

Remove Army Preset:
-----------------------
This endpoint will remove an army preset from the user

 - Path: /RemoveArmyPreset
 - POST
 - Auth: Basic Auth
  - Request Body Example:
    {   
        "armyPresetId": "1234-1234-2134"
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
        "forfeitTime": 172800,
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
        "forfeitTime": 172800,
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

Get Old Game State:
-----------------------
Gets some gamestate data from a past turn

 - Path: /GetOldGameState
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {
        "gameId":    "123-456"
        "turnCount": "3"
    }
 - Return: GameState Data
 - Example Return Data:
    {   
        "units": "[unit1, unit2]"
        "generals": "[general1, general2]"
        "actions": "[action1, action2]"
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
    

Get Complete Games:
-----------------------
Gets all of the Games that are completed and returns a list of all of their data

- Path: /GetCompletedGames
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {
        "limit": 100
    }
 - Return: Array of Completed GameState Data (includes all fields)
 - Example Return Data:
    [
        {   
            "id": "123-456",
            "boardId": 9,
            "initUnits": []Unit,
            "actions": []Action
        },
         {   
            "id": "123-459",
            "boardId": 4,
            "initUnits": []Unit,
            "actions": []Action
        },
    ]
    

Ready Units:
-----------------------
For when you are placing your army in a public or private game.

 - Path: /ReadyUnits
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameId": "55410202-af58-470f-a690-d01b41458655",
        "units":        // Note: Should just be the units that the user has placed. (No one elses).  
            [
                {
                    "owner": "ParkerReese1",
                    "unitType": 5,
                    "health": 10,
                    "xPos": 1,
                    "yPos": 2,
                },
                {
                    "owner": "ParkerReese1",
                    "unitType": 2,
                    "health": 5,
                    "xPos": 1,
                    "yPos": 2,
                }
            ],
        "general":
            {
                "owner": "ParkerReese1",
                "unitType": 101,
                "health": 10,
                "xPos": 1,
                "yPos": 2,
            }
    }
 - Return: Http Resonse Code


Make Move:
-----------------------
For when it is your turn in a game and you want to let the
server know what you did.

 - Path: /MakeMove
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "gameId": "55410202-af58-470f-a690-d01b41458655",
        "units":        // Note: Should be ALL of the units on the board, for all users.
            [
                {
                    "owner": "ParkerReese1",
                    "unitType": 5,
                    "health": 10,
                    "xPos": 1,
                    "yPos": 2,
                },
                {
                    "owner": "ParkerReese2",
                    "unitType": 2,
                    "health": 5,
                    "xPos": 1,
                    "yPos": 4,
                }
            ]
        },
        "generals":        // Note: Should be ALL of the generals on the board, for all users.
            [
                {
                    "owner": "ParkerReese1",
                    "unitType": 101,
                    "health": 10,
                    "xPos": 1,
                    "yPos": 2,
                },
                {
                    "owner": "ParkerReese2",
                    "unitType": 102,
                    "health": 5,
                    "xPos": 1,
                    "yPos": 4,
                }
            ]
        },
        "cards":        // Note: Should be ALL of the cards for all users. (or as many card structs the client knows about so far)
            [
                {
                    "id": "abc-123",
                    "owner": "ParkerReese1",
                    "hand": [1, 1, 3],
                    "deck": [3, 3, 4, 5],
                    "discard": []
                },
                {
                    "id": "abc-124",
                    "owner": "ParkerReese2",
                    "hand": [1, 1, 3],
                    "deck": [3, 3, 4, 5],
                    "discard": []
                },
            ],
        "actions":      // Note: Should be only the new actions for this users turn. (No one elses).
            [
                {
                    "username":   "ParkerReese1",            
                    "actionType": 1, 
                    "originXPos": 1,
                    "originYPos": 2,
                    "targetXPos": 2,
                    "targetYPos": 2,
                },
                {
                    "username":   "ParkerReese1",            
                    "actionType": 1,       
                    "originXPos": 1,
                    "originYPos": 4,
                    "targetXPos": 2,
                    "targetYPos": 2,
                }
            ],
        "killedUsers": ["ParkerReese3"]
    }
 - Return: Http Resonse Code


---------------------------------------------------------------------
Structures
---------------------------------------------------------------------

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID             string    `json:"id,omitempty"`
	GameName       string    `json:"gameName,omitempty"`
	CreatedBy      string    `json:"createdBy,omitempty"`
	BoardID        int       `json:"boardId,omitempty"`
	MaxUsers       int       `json:"maxUsers,omitempty"`
	SpotsAvailable int       `json:"spotsAvailable,omitempty"`
	IsPublic       bool      `json:"isPublic"`
	IsComplete     bool      `json:"isComplete"`
	Users          []string  `json:"users,omitempty" datastore:",noindex,omitempty"`
	AcceptedUsers  []string  `json:"acceptedUsers,omitempty" datastore:",noindex,omitempty"`
	ReadyUsers     []string  `json:"readyUsers,omitempty" datastore:",noindex,omitempty"`
	AliveUsers     []string  `json:"aliveUsers,omitempty" datastore:",noindex,omitempty"`
	UsersTurn      string    `json:"usersTurn,omitempty" datastore:",noindex"`
	InitUnits      []Unit    `json:"initUnits,omitempty" datastore:",noindex,omitempty"`
	Units          []Unit    `json:"units,omitempty" datastore:",noindex,omitempty"`
	Generals       []Unit    `json:"generals,omitempty" datastore:",noindex,omitempty"`
	InitGenerals   []Unit    `json:"initGenerals,omitempty" datastore:",noindex,omitempty"`
	Cards          []Cards   `json:"cards,omitempty" datastore:"-"`
	CardIDs        []string  `json:"-" datastore:",noindex,omitempty"`
	ActiveEffects  []Effect  `json:"activeEffects,omitempty" datastore:",noindex,omitempty"`
	Actions        []Action  `json:"actions,omitempty" datastore:",noindex,omitempty"`
	ForfeitTime    int       `json:"forfeitTime,omitempty" datastore:",noindex"`
	LoseReasons    []Lose    `json:"loseReasons,omitempty" datastore:",noindex,omitempty"`
	Created        time.Time `json:"created,omitempty" datastore:",noindex,omitempty"`
}

// Unit is a game piece on the board
type Unit struct {
	Owner            string  `json:"owner,omitempty" datastore:",omitempty"`
	UnitType         int     `json:"unitType,omitempty" datastore:",omitempty"`
	Health           float32 `json:"health,omitempty" datastore:",omitempty"`
	XPos             int     `json:"xPos" datastore:""`
	YPos             int     `json:"yPos" datastore:""`
	Ability1CoolDown int     `json:"ability1CoolDown" datastore:""`
	Ability2CoolDown int     `json:"ability2CoolDown" datastore:""`
	Ability1Duration int     `json:"ability1Duration" datastore:""`
	Ability2Duration int     `json:"ability2Duration" datastore:""`
	Direction        int     `json:"direction" datastore:""`
}

// Cards contains all the card information on a per user bases
type Cards struct {
	ID      string `json:"id,omitempty" datastore:",omitempty"`
	Owner   string `json:"owner,omitempty" datastore:",omitempty"`
	Hand    []int  `json:"hand,omitempty" datastore:",omitempty"`
	Deck    []int  `json:"deck,omitempty" datastore:",omitempty"`
	Discard []int  `json:"discard,omitempty" datastore:",omitempty"`
}

// Action contains the info for a single action in the game
type Action struct {
	Username      string     `json:"username,omitempty" datastore:",omitempty"`
	ActionType    ActionType `json:"actionType" datastore:""`
	OriginXPos    int        `json:"originXPos" datastore:""`
	OriginYPos    int        `json:"originYPos" datastore:""`
	TargetXPos    int        `json:"targetXPos" datastore:""`
	TargetYPos    int        `json:"targetYPos" datastore:""`
	AbilityNumber int        `json:"abilityNumber" datastore:""`
	CardID        int        `json:"cardId,omitempty" datastore:",omitempty"`
}

// Lose is a struct to say who lost and why
type Lose struct {
	Username string     `json:"username,omitempty" datastore:",omitempty"`
	Reason   LoseReason `json:"reason" datastore:""`
}

// Effect is a struct used to say what tiles have additional effects present on them
type Effect struct {
	Owner        string `json:"owner,omitempty" datastore:",omitempty"`
	Type         int    `json:"type" datastore:""`
	XPos         int    `json:"xPos" datastore:""`
	YPos         int    `json:"yPos" datastore:""`
	DurationLeft int    `json:"durationLeft" datastore:""`
}

// User is a human player of the game
type User struct {
	Username            string       `json:"username,omitempty" datastore:",omitempty"`
	Password            string       `json:"password,omitempty" datastore:",omitempty"`
	Friends             []string     `json:"friends,omitempty" datastore:",omitempty,noindex"`
	ActiveGames         []string     `json:"activeGames,omitempty" datastore:",omitempty,noindex"`
	PendingPrivateGames []string     `json:"pendingPrivateGames,omitempty" datastore:",omitempty,noindex"`
	PendingPublicGames  []string     `json:"pendingPublicGames,omitempty" datastore:",omitempty,noindex"`
	CompletedGames      []string     `json:"completedGames,omitempty" datastore:",omitempty,noindex"`
	ArmyPresets         []ArmyPreset `json:"armyPresets,omitempty" datastore:"-"`
	ArmyPresetIDs       []string     `json:"-" datastore:",omitempty,noindex"`
}

// ArmyPreset is an army created and saved by the user for future use
type ArmyPreset struct {
	ID      string `json:"id,omitempty" datastore:",omitempty,noindex"`
	Name    string `json:"name,omitempty" datastore:",omitempty,noindex"`
	Units   []int  `json:"units,omitempty" datastore:",omitempty,noindex"`
	General int    `json:"general,omitempty" datastore:",omitempty,noindex"`
}
