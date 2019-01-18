package gamestate

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID            string   `json:"id,omitempty" datastore:",omitempty"`
	Board         int      `json:"board,omitempty" datastore:",omitempty"`
	Users         []string `json:"users,omitempty" datastore:",omitempty"`
	AcceptedUsers []string `json:"acceptedUsers,omitempty" datastore:",omitempty"`
	ReadyUsers    []string `json:"readtyUsers,omitempty" datastore:",omitempty"`
	AliveUsers    []string `json:"aliveUsers,omitempty" datastore:",omitempty"`
	UsersTurn     string   `json:"usersTurn,omitempty" datastore:",omitempty"`
}

// TODO: Fill this model in more
