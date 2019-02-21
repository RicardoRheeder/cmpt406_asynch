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

// AddArmyPreset has the info needed to add an army preset
type AddArmyPreset struct {
	Name    string `json:"name"`
	Units   []int  `json:"units"`
	General int    `json:"general"`
}

// RemoveArmyPreset will remove the given army preset
type RemoveArmyPreset struct {
	ArmyPresetID string `json:"armyPresetId"`
}

// CreatePrivateGame is a struct to handle the request of inviting users to a private game
type CreatePrivateGame struct {
	GameName          string   `json:"gameName"`
	TurnTime          int      `json:"turnTime"`
	ForfeitTime       int      `json:"forfeitTime"`
	OpponentUsernames []string `json:"opponentUsernames"`
	BoardID           int      `json:"boardId"`
}

// CreatePublicGame is a struct to handle the request of creating a public game
type CreatePublicGame struct {
	GameName    string `json:"gameName"`
	TurnTime    int    `json:"turnTime"`
	ForfeitTime int    `json:"forfeitTime"`
	MaxUsers    int    `json:"maxUsers"`
	BoardID     int    `json:"boardId"`
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

// ReadyUnits holds all the information to ready up in a game
type ReadyUnits struct {
	GameID  string           `json:"gameId"`
	Units   []gamestate.Unit `json:"units"`
	General gamestate.Unit   `json:"general"`
	Cards   gamestate.Cards  `json:"cards"`
}

// MakeMove holds all the information for doing a turn
type MakeMove struct {
	GameID      string             `json:"gameId"`
	KilledUsers []string           `json:"killedUsers"`
	Units       []gamestate.Unit   `json:"units"`
	Generals    []gamestate.Unit   `json:"generals"`
	Cards       []gamestate.Cards  `json:"cards"`
	Actions     []gamestate.Action `json:"actions"`
}

// MultiStatesResponse is here because client requires that states to be one field nested
type MultiStatesResponse struct {
	States []gamestate.GameState `json:"states"`
}
