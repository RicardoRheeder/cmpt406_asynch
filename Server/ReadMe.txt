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
        "username": "usernameTest2",
        "activeGames": ["GameID1", "GameID2"]
    }

Request Game:
-----------------------
 - Path: /RequestGame
 - POST
 - Auth: Basic Auth
 - Body: JSON
 - Request Body Example:
    {   
        "opponentUsernames": ["username1", "username2"],
        "board": 6
    }
 - Return: Http Resonse Code
