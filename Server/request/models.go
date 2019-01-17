package request

// CreateUser is a struct to handle the request of creating a user
type CreateUser struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

// GameRequest is a struct to handle the request of inviting a user to a g
type GameRequest struct {
	OpponentUsernames []string `json:"opponentUsernames"`
	Board             int      `json:"board"`
}
