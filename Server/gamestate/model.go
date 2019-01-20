package gamestate

import (
	"google.golang.org/appengine"
)

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID             string            `json:"id,omitempty" datastore:",omitempty"`
	BoardID        int               `json:"boardId,omitempty" datastore:",omitempty"`
	MaxUsers       int               `json:"maxUsers,omitempty" datastore:",omitempty"`
	SpotsAvailable int               `json:"spotsAvailable,omitempty" datastore:",omitempty"`
	IsPublic       bool              `json:"isPublic"`
	Users          []string          `json:"users,omitempty" datastore:",omitempty,noindex"`
	AcceptedUsers  []string          `json:"acceptedUsers,omitempty" datastore:",omitempty,noindex"`
	ReadyUsers     []string          `json:"readyUsers,omitempty" datastore:",omitempty,noindex"`
	AliveUsers     []string          `json:"aliveUsers,omitempty" datastore:",omitempty,noindex"`
	UsersTurn      string            `json:"usersTurn,omitempty" datastore:",omitempty,noindex"`
	Units          map[string][]Unit `json:"units,omitempty" datastore:",omitempty,noindex,flatten"`
	Cards          map[string]Cards  `json:"cards,omitempty" datastore:",omitempty,noindex,flatten"`
}

// Summary is the same as GameState but wont json marshal as much on the return
type Summary struct {
	ID             string            `json:"id,omitempty" datastore:",omitempty"`
	BoardID        int               `json:"boardId,omitempty" datastore:",omitempty"`
	MaxUsers       int               `json:"maxUsers,omitempty" datastore:",omitempty"`
	SpotsAvailable int               `json:"spotsAvailable,omitempty" datastore:",omitempty"`
	IsPublic       bool              `json:"isPublic"`
	Users          []string          `json:"users,omitempty" datastore:",omitempty,noindex"`
	AcceptedUsers  []string          `json:"acceptedUsers,omitempty" datastore:",omitempty,noindex"`
	ReadyUsers     []string          `json:"readyUsers,omitempty" datastore:",omitempty,noindex"`
	AliveUsers     []string          `json:"-" datastore:",omitempty,noindex"`
	UsersTurn      string            `json:"usersTurn,omitempty" datastore:",omitempty,noindex"`
	Units          map[string][]Unit `json:"-" datastore:",omitempty,noindex,flatten"`
	Cards          map[string]Cards  `json:"-" datastore:",omitempty,noindex,flatten"`
}

// Unit is a game piece on the board
type Unit struct {
	UnitType int                `json:"unitType,omitempty" datastore:",omitempty"`
	Health   float32            `json:"health,omitempty" datastore:",omitempty"`
	Coord    appengine.GeoPoint `json:"coord,omitempty" datastore:",omitempty"`
}

// Cards contains all the card information on a per user bases
type Cards struct {
	Hand []string `json:"hand,omitempty" datastore:",omitempty"`
	Deck []string `json:"deck,omitempty" datastore:",omitempty"`
}
