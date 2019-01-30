package request

import "Projects/cmpt406_asynch/Server/gamestate"

// CreateUser is a struct to handle the request of creating a user
type CreateUser struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

// Friend is a struct to handle the request of adding or removing a user to a users friendslist
type Friend struct {
	Username string `json:"username"`
}

// CreatePrivateGame is a struct to handle the request of inviting users to a private game
type CreatePrivateGame struct {
	GameName          string   `json:"gameName"`
	TurnTime          int      `json:"turnTime"`
	TimeToStartTurn   int      `json:"timeToStartTurn"`
	OpponentUsernames []string `json:"opponentUsernames"`
	BoardID           int      `json:"boardId"`
}

// CreatePublicGame is a struct to handle the request of creating a public game
type CreatePublicGame struct {
	GameName        string `json:"gameName"`
	TurnTime        int    `json:"turnTime"`
	TimeToStartTurn int    `json:"timeToStartTurn"`
	MaxUsers        int    `json:"maxUsers"`
	BoardID         int    `json:"boardId"`
}

// OnlyGameID is a struct to handle payloads that consist of only the GameID
type OnlyGameID struct {
	GameID string `json:"gameId"`
}

// GetGameStateMulti is a struct to handle the /GetGameStateMulti payload
type GetGameStateMulti struct {
	GameIDs []string `json:"gameIds"`
}

// GetPublicGamesSummary is a struct to handle the /GetPublicGamesSummary payload
type GetPublicGamesSummary struct {
	Limit int `json:"limit"`
}

// UpdateGameState holds all the information that can be updated in the GameState
type UpdateGameState struct {
	GameID     string                      `json:"gameId"`
	ReadyUsers []string                    `json:"readyUsers"`
	AliveUsers []string                    `json:"aliveUsers"`
	Units      map[string][]gamestate.Unit `json:"units"`
	Cards      map[string]gamestate.Cards  `json:"cards"`
}
