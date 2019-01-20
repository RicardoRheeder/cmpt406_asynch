package gamestate

import (
	"google.golang.org/appengine"
)

// GameState is everything the client needs to know to construct the state of the game
type GameState struct {
	ID             string   `json:"id,omitempty" datastore:",omitempty"`
	Board          int      `json:"board,omitempty" datastore:",omitempty"`
	MaxUsers       int      `json:"maxUsers,omitempty" datastore:",omitempty"`
	SpotsAvailable int      `json:"spotsAvailable,omitempty" datastore:",omitempty"`
	Public         bool     `json:"public"`
	Users          []string `json:"users,omitempty" datastore:",omitempty,noindex"`
	AcceptedUsers  []string `json:"acceptedUsers,omitempty" datastore:",omitempty,noindex"`
	ReadyUsers     []string `json:"readtyUsers,omitempty" datastore:",omitempty,noindex"`
	AliveUsers     []string `json:"aliveUsers,omitempty" datastore:",omitempty,noindex"`
	UsersTurn      string   `json:"usersTurn,omitempty" datastore:",omitempty,noindex"`
	Units          []Unit   `json:"units,omitempty" datastore:",omitempty,noindex,flatten"`
	//Cards          []Cards  `json:"cards,omitempty" datastore:",omitempty,noindex,flatten"`
}

// Summary is the same as GameState but wont json marshal as much on the return
type Summary struct {
	ID             string   `json:"id,omitempty" datastore:",omitempty"`
	Board          int      `json:"board,omitempty" datastore:",omitempty"`
	MaxUsers       int      `json:"maxUsers,omitempty" datastore:",omitempty"`
	SpotsAvailable int      `json:"spotsAvailable,omitempty" datastore:",omitempty"`
	Public         bool     `json:"public"`
	Users          []string `json:"users,omitempty" datastore:",omitempty,noindex"`
	AcceptedUsers  []string `json:"acceptedUsers,omitempty" datastore:",omitempty,noindex"`
	ReadyUsers     []string `json:"readtyUsers,omitempty" datastore:",omitempty,noindex"`
	AliveUsers     []string `json:"-" datastore:",omitempty,noindex"`
	UsersTurn      string   `json:"usersTurn,omitempty" datastore:",omitempty,noindex"`
	Units          []Unit   `json:"-" datastore:",omitempty,noindex,flatten"`
	//Cards          []Cards  `json:"-" datastore:",omitempty,noindex,flatten"`
}

// Unit is a game piece on the board
type Unit struct {
	Type   int                `json:"type,omitempty" datastore:",omitempty"`
	Health float32            `json:"health,omitempty" datastore:",omitempty"`
	Coord  appengine.GeoPoint `json:"coord,omitempty" datastore:",omitempty"`
	Owner  string             `json:"owner,omitempty" datastore:",omitempty"`
}

// Cards contains all the card information on a per user bases
type Cards struct {
	Owner string   `json:"owner,omitempty" datastore:",omitempty"`
	Hand  []string `json:"hand,omitempty" datastore:",omitempty"`
	Deck  []string `json:"deck,omitempty" datastore:",omitempty"`
}
