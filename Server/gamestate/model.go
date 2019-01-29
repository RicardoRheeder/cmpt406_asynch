package gamestate

import (
	"context"

	"google.golang.org/appengine"
)

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID             string            `json:"id,omitempty"`
	BoardID        int               `json:"boardId,omitempty"`
	MaxUsers       int               `json:"maxUsers,omitempty"`
	SpotsAvailable int               `json:"spotsAvailable,omitempty"`
	IsPublic       bool              `json:"isPublic"`
	Users          []string          `json:"users,omitempty"`
	AcceptedUsers  []string          `json:"acceptedUsers,omitempty"`
	ReadyUsers     []string          `json:"readyUsers,omitempty"`
	AliveUsers     []string          `json:"aliveUsers,omitempty"`
	UsersTurn      string            `json:"usersTurn,omitempty"`
	Units          map[string][]Unit `json:"units,omitempty" datastore:",omitempty,noindex,flatten"`
	Cards          map[string]Cards  `json:"cards,omitempty" datastore:",omitempty,noindex,flatten"`
}

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

// UpdateGameStateFunc is a mutator function that edits the game state in the desired way
type UpdateGameStateFunc func(context.Context, *GameState) error
