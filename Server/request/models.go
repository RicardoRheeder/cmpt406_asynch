package request

// CreateUser is a struct to handle the request of creating a user
type CreateUser struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

// SendInvite is a struct to handle the request of inviting a user to a game
type SendInvite struct {
	OpponentUsernames []string `json:"opponentUsernames"`
	Board             int      `json:"board"`
}

// AcceptInvite is a struct to handle the /AcceptRequest payload
type AcceptInvite struct {
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
