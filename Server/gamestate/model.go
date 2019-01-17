package gamestate

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID            string   `json:"id"`
	Board         int      `json:"board"`
	Users         []string `json:"users"`
	AcceptedUsers []string `json:"acceptedUsers"`
	ReadyUsers    []string `json:"readtyUsers"`
	AliveUsers    []string `json:"aliveUsers"`
	UsersTurn     string   `json:"usersTurn"`
}

// TODO: Fill this model in more
