package user

import "context"

// User is a human player of the game
type User struct {
	Username            string   `json:"username,omitempty" datastore:",omitempty"`
	Password            string   `json:"password,omitempty" datastore:",omitempty"`
	Friends             []string `json:"freinds,omitempty" datastore:",omitempty,noindex"`
	ActiveGames         []string `json:"activeGames,omitempty" datastore:",omitempty,noindex"`
	PendingPrivateGames []string `json:"pendingPrivateGames,omitempty" datastore:",omitempty,noindex"`
	PendingPublicGames  []string `json:"pendingPublicGames,omitempty" datastore:",omitempty,noindex"`
	CompletedGames      []string `json:"completedGames,omitempty" datastore:",omitempty,noindex"`
}

// UpdateUserFunc is a mutator function that edits the user in the desired way
type UpdateUserFunc func(context.Context, *User) error
