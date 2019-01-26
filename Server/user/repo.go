package user

import (
	"context"

	"google.golang.org/appengine/datastore"
	"google.golang.org/appengine/log"
)

// CreateUser will create a user in DataStore
func CreateUser(ctx context.Context, username string, password string) error {

	_, err := GetUser(ctx, username)
	if err == nil {
		log.Errorf(ctx, "Attempted to create user: %s, that already exists", username)
		return err
	}

	user := &User{
		Username: username,
		Password: password,
	}

	key := datastore.NewKey(ctx, "User", username, 0, nil)

	_, err = datastore.Put(ctx, key, user)
	if err != nil {
		log.Errorf(ctx, "Failed to Put (create) user: %s, err: %s", username, err.Error())
		return err
	}

	return nil
}

// UpdateUser will update a currently existing user
func UpdateUser(ctx context.Context, username string, activeGames, pendingPrivateGames, pendingPublicGames, completedGames, friends []string) error {

	u, err := GetUser(ctx, username)
	if err != nil {
		log.Errorf(ctx, "Attempted to update user: %s, that doesn't exists", username)
		return err
	}

	/* Pass nil values into the function if you want the value to remain the same */
	if activeGames != nil {
		u.ActiveGames = activeGames
	}
	if pendingPrivateGames != nil {
		u.PendingPrivateGames = pendingPrivateGames
	}
	if pendingPublicGames != nil {
		u.PendingPublicGames = pendingPublicGames
	}
	if completedGames != nil {
		u.CompletedGames = completedGames
	}
	if friends != nil {
		u.Friends = friends
	}

	user := &User{
		Username:            u.Username,
		Password:            u.Password,
		Friends:             u.Friends,
		ActiveGames:         u.ActiveGames,
		PendingPrivateGames: u.PendingPrivateGames,
		PendingPublicGames:  u.PendingPublicGames,
		CompletedGames:      u.CompletedGames,
	}

	key := datastore.NewKey(ctx, "User", username, 0, nil)

	_, err = datastore.Put(ctx, key, user)
	if err != nil {
		log.Errorf(ctx, "Failed to Put (update) user: %s, err: %s", username, err.Error())
		return err
	}

	return nil
}

// GetUser will get a user via its key from DataStore
func GetUser(ctx context.Context, username string) (*User, error) {

	var user User

	key := datastore.NewKey(ctx, "User", username, 0, nil)

	err := datastore.Get(ctx, key, &user)
	if err != nil {
		log.Errorf(ctx, "Failed to Get user: %s, err: %s", username, err.Error())
		return nil, err
	}

	return &user, nil
}
