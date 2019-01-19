package request

// CreateUser is a struct to handle the request of creating a user
type CreateUser struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

// CreatePrivateGame is a struct to handle the request of inviting users to a private game
type CreatePrivateGame struct {
	OpponentUsernames []string `json:"opponentUsernames"`
	Board             int      `json:"board"`
}

// CreatePublicGame is a struct to handle the request of creating a public game
type CreatePublicGame struct {
	MaxUsers int `json:"maxUsers"`
	Board    int `json:"board"`
}

// AcceptGame is a struct to handle the /AcceptGame payload
type AcceptGame struct {
	GameID string `json:"gameId"`
}

// GetGameState is a struct to handle the /GetGameState paylaod
type GetGameState struct {
	GameID string `json:"gameId"`
}

// GetGameStateMulti is a struct to handle the /GetGameStateMulti paylaod
type GetGameStateMulti struct {
	GameIDs []string `json:"gameIds"`
}
