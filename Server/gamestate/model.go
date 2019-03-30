package gamestate

import (
	"context"
	"time"
)

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID             string    `json:"id,omitempty"`
	GameName       string    `json:"gameName,omitempty"`
	CreatedBy      string    `json:"createdBy,omitempty"`
	BoardID        int       `json:"boardId,omitempty"`
	MaxUsers       int       `json:"maxUsers,omitempty"`
	SpotsAvailable int       `json:"spotsAvailable,omitempty"`
	IsPublic       bool      `json:"isPublic"`
	TurnCount      int       `json:"turnCount"`
	IsComplete     bool      `json:"isComplete"`
	Users          []string  `json:"users,omitempty" datastore:",noindex,omitempty"`
	AcceptedUsers  []string  `json:"acceptedUsers,omitempty" datastore:",noindex,omitempty"`
	ReadyUsers     []string  `json:"readyUsers,omitempty" datastore:",noindex,omitempty"`
	AliveUsers     []string  `json:"aliveUsers,omitempty" datastore:",noindex,omitempty"`
	UsersTurn      string    `json:"usersTurn,omitempty" datastore:",noindex"`
	InitUnits      []Unit    `json:"initUnits,omitempty" datastore:",noindex,omitempty"`
	Units          []Unit    `json:"units,omitempty" datastore:",noindex,omitempty"`
	Generals       []Unit    `json:"generals,omitempty" datastore:",noindex,omitempty"`
	InitGenerals   []Unit    `json:"initGenerals,omitempty" datastore:",noindex,omitempty"`
	Cards          []Cards   `json:"cards,omitempty" datastore:"-"`
	CardIDs        []string  `json:"-" datastore:",noindex,omitempty"`
	ActiveEffects  []Effect  `json:"activeEffects,omitempty" datastore:",noindex,omitempty"`
	Actions        []Action  `json:"actions,omitempty" datastore:",noindex,omitempty"`
	ForfeitTime    int       `json:"forfeitTime,omitempty" datastore:",noindex"`
	LoseReasons    []Lose    `json:"loseReasons,omitempty" datastore:",noindex,omitempty"`
	Created        time.Time `json:"created,omitempty" datastore:",noindex,omitempty"`
}

// GSAncestor is an old version of a gamestate
type GSAncestor struct {
	ID            string   `json:"id,omitempty"`
	TurnCount     int      `json:"turnCount" datastore:",noindex"`
	Units         []Unit   `json:"units,omitempty" datastore:",noindex,omitempty"`
	Generals      []Unit   `json:"generals,omitempty" datastore:",noindex,omitempty"`
	ActiveEffects []Effect `json:"activeEffects,omitempty" datastore:",noindex,omitempty"`
	Actions       []Action `json:"actions,omitempty" datastore:",noindex,omitempty"`
}

// Unit is a game piece on the board
type Unit struct {
	Owner            string  `json:"owner,omitempty" datastore:",omitempty"`
	UnitType         int     `json:"unitType,omitempty" datastore:",omitempty"`
	Health           float32 `json:"health,omitempty" datastore:",omitempty"`
	XPos             int     `json:"xPos" datastore:""`
	YPos             int     `json:"yPos" datastore:""`
	Ability1CoolDown int     `json:"ability1CoolDown" datastore:""`
	Ability2CoolDown int     `json:"ability2CoolDown" datastore:""`
	Ability1Duration int     `json:"ability1Duration" datastore:""`
	Ability2Duration int     `json:"ability2Duration" datastore:""`
	Direction        int     `json:"direction" datastore:""`
}

// Cards contains all the card information on a per user bases
type Cards struct {
	ID      string `json:"id,omitempty" datastore:",omitempty"`
	Owner   string `json:"owner,omitempty" datastore:",omitempty"`
	Hand    []int  `json:"hand,omitempty" datastore:",omitempty"`
	Deck    []int  `json:"deck,omitempty" datastore:",omitempty"`
	Discard []int  `json:"discard,omitempty" datastore:",omitempty"`
}

// Action contains the info for a single action in the game
type Action struct {
	Username      string     `json:"username,omitempty" datastore:",omitempty"`
	ActionType    ActionType `json:"actionType" datastore:""`
	OriginXPos    int        `json:"originXPos" datastore:""`
	OriginYPos    int        `json:"originYPos" datastore:""`
	TargetXPos    int        `json:"targetXPos" datastore:""`
	TargetYPos    int        `json:"targetYPos" datastore:""`
	AbilityNumber int        `json:"abilityNumber" datastore:""`
	CardID        int        `json:"cardId,omitempty" datastore:",omitempty"`
}

// ActionType Enum for use in the Action struct
type ActionType int

const (
	// Move : unit moved from one spot to another
	Move ActionType = iota
	// Attack : unit attacked a position
	Attack
	// Card : user used a card
	Card
	// Forfeit : user has quit the game, counting as a loss
	Forfeit
)

// Lose is a struct to say who lost and why
type Lose struct {
	Username string     `json:"username,omitempty" datastore:",omitempty"`
	Reason   LoseReason `json:"reason" datastore:""`
}

// LoseReason Enum to say why a person lost
type LoseReason int

const (
	// Died : all players units are dead
	Died LoseReason = iota
	// PlayerForfeit : player forfeited
	PlayerForfeit
	// ForcedForfeit : player was forced to forfeit by the server
	ForcedForfeit
)

// Effect is a struct used to say what tiles have additional effects present on them
type Effect struct {
	Owner        string `json:"owner,omitempty" datastore:",omitempty"`
	Type         int    `json:"type" datastore:""`
	XPos         int    `json:"xPos" datastore:""`
	YPos         int    `json:"yPos" datastore:""`
	DurationLeft int    `json:"durationLeft" datastore:""`
}

// UpdateGameStateFunc is a mutator function that edits the game state in the desired way
type UpdateGameStateFunc func(context.Context, *GameState) error
