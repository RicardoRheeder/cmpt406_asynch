package user

// User is a human player of the game
type User struct {
	Username        string   `json:"username" datastore:", omitempty"`
	Password        string   `json:"password" datastore:", omitempty"`
	ActiveGames     []string `json:"activeGames" datastore:", omitempty"`
	SentInvites     []string `json:"sentInvites" datastore:", omitempty"`
	RecievedInvites []string `json:"recievedInvites" datastore:", omitempty"`
	CompletedGames  []string `json:"completedGames" datastore:", omitempty"`
}
