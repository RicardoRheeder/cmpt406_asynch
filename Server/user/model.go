package user

// User is a human player of the game
type User struct {
	Username            string   `json:"username,omitempty" datastore:",omitempty"`
	Password            string   `json:"password,omitempty" datastore:",omitempty"`
	ActiveGames         []string `json:"activeGames,omitempty" datastore:",omitempty"`
	PendingPrivateGames []string `json:"pendingPrivateGames,omitempty" datastore:",omitempty"`
	PendingPublicGames  []string `json:"pendingPublicGames,omitempty" datastore:",omitempty"`
	CompletedGames      []string `json:"completedGames,omitempty" datastore:",omitempty"`
}
