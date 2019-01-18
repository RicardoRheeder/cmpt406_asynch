package gamestate

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID            string   `json:"id" datastore:", omitempty"`
	Board         int      `json:"board" datastore:", omitempty"`
	Users         []string `json:"users" datastore:", omitempty"`
	AcceptedUsers []string `json:"acceptedUsers" datastore:", omitempty"`
	ReadyUsers    []string `json:"readtyUsers" datastore:", omitempty"`
	AliveUsers    []string `json:"aliveUsers" datastore:", omitempty"`
	UsersTurn     string   `json:"usersTurn" datastore:", omitempty"`
}

// TODO: Fill this model in more
