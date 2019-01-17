package user

// User is a human player of the game
type User struct {
	Username        string   `json:"username"`
	Password        string   `json:"password"`
	ActiveGames     []string `json:"activeGames"`
	SentInvites     []string `json:"sentInvites"`
	RecievedInvites []string `json:"recievedInvites"`
}
