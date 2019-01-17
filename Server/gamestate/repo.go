package gamestate

import (
	"context"
	"errors"

	"google.golang.org/appengine/datastore"
	"google.golang.org/appengine/log"
)

// CreateGameState will create a game state in DataStore
func CreateGameState(ctx context.Context, ID string, board int, users []string) error {

	_, err := GetGameState(ctx, ID)
	if err == nil {
		log.Errorf(ctx, "Attempted to create GameState: %s, that already exists", ID)
		return errors.New("GameState Already Exists")
	}

	gameState := &GameState{
		ID:         ID,
		Board:      board,
		Users:      users,
		AliveUsers: users,
	}

	key := datastore.NewKey(ctx, "GameState", ID, 0, nil)

	_, err = datastore.Put(ctx, key, gameState)
	if err != nil {
		log.Errorf(ctx, "Failed to Put (create) gameState: %s", ID)
		return err
	}

	return nil
}

/* TODO: fill in updating the game state when we have a better understanding of what a game state is
// UpdateUser will update a currently existing user
func UpdateGameState(ctx context.Context, username string, password string) error {

	_, err := GetUser(ctx, username)
	if err != nil {
		log.Errorf(ctx, "Attempted to update user: %s, that doesn't exists", username)
		return errors.New("User Doesn't Exists")
	}

	user := &User{
		Username: username,
		Password: password,
	}

	key := datastore.NewKey(ctx, "User", username, 0, nil)

	_, err = datastore.Put(ctx, key, user)
	if err != nil {
		log.Errorf(ctx, "Failed to Put (update) user: %s", username)
		return err
	}

	return nil
}
*/

// GetGameState will get a gamestate via its key ID from DataStore
func GetGameState(ctx context.Context, id string) (*GameState, error) {

	var gameState GameState

	key := datastore.NewKey(ctx, "GameState", id, 0, nil)

	err := datastore.Get(ctx, key, &gameState)
	if err != nil {
		log.Errorf(ctx, "Failed to Get gameState: %s", id)
		return nil, err
	}

	return &gameState, nil
}
