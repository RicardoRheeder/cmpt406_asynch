package gamestate

import (
	"context"
	"time"

	"google.golang.org/appengine"
)

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID              string      `json:"id,omitempty"`
	GameName        string      `json:"gameName,omitempty"`
	CreatedBy       string      `json:"createdBy,omitempty"`
	BoardID         int         `json:"boardId,omitempty"`
	MaxUsers        int         `json:"maxUsers,omitempty"`
	SpotsAvailable  int         `json:"spotsAvailable,omitempty"`
	IsPublic        bool        `json:"isPublic"`
	Users           []string    `json:"users,omitempty"`
	AcceptedUsers   []string    `json:"acceptedUsers,omitempty"`
	ReadyUsers      []string    `json:"readyUsers,omitempty"`
	AliveUsers      []string    `json:"aliveUsers,omitempty"`
	UsersTurn       string      `json:"usersTurn,omitempty"`
	Units           interface{} `json:"units,omitempty" datastore:",noindex,flatten"`
	Cards           interface{} `json:"cards,omitempty" datastore:",noindex,flatten"`
	Actions         [][]Action  `json:"actions,omitempty" datastore:",noindex,flatten"`
	TurnTime        int         `json:"turnTime,omitempty"`
	TimeToStateTurn int         `json:"timeToStartTurn,omitempty"`
	Created         time.Time   `json:"created,omitempty"`
}

type units map[string][]Unit
type cards map[string]Cards

// Unit is a game piece on the board
type Unit struct {
	UnitType int                `json:"unitType,omitempty" datastore:",omitempty"`
	Health   float32            `json:"health,omitempty" datastore:",omitempty"`
	Coord    appengine.GeoPoint `json:"coord,omitempty" datastore:",omitempty"`
}

// Cards contains all the card information on a per user bases
type Cards struct {
	Hand    []string `json:"hand,omitempty" datastore:",omitempty"`
	Deck    []string `json:"deck,omitempty" datastore:",omitempty"`
	Discard []string `json:"discard,omitempty" datastore:",omitempty"`
}

// Action contains the info for a single action in the game
type Action struct {
	Username   string             `json:"username,omitempty" datastore:",omitempty"`
	ActionType ActionType         `json:"actionType,omitempty" datastore:",omitempty"`
	Origin     appengine.GeoPoint `json:"origin,omitempty" datastore:",omitempty"`
	Target     appengine.GeoPoint `json:"target,omitempty" datastore:",omitempty"`
	CardID     int                `json:"cardId,omitempty" datastore:",omitempty"`
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

// UpdateGameStateFunc is a mutator function that edits the game state in the desired way
type UpdateGameStateFunc func(context.Context, *GameState) error
