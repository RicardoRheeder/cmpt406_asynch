package user

import (
	"context"
	"errors"

	"google.golang.org/appengine/datastore"
	"google.golang.org/appengine/log"
)

// CreateUser will create a user in DataStore
func CreateUser(ctx context.Context, username string, password string) error {

	// Transaction to ensure race conditions wont break things
	err := datastore.RunInTransaction(ctx, func(ctx context.Context) error {

		_, err := GetUser(ctx, username)
		if err == nil {
			log.Errorf(ctx, "Attempted to create user: %s, that already exists", username)
			return errors.New("User Already Exists")
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
	}, &datastore.TransactionOptions{XG: true})
	if err != nil {
		log.Errorf(ctx, "Create User Transaction failed: %v", err)
		return err
	}

	return nil
}

// UpdateUserWithT will update a currently existing user inside a transaction
func UpdateUserWithT(ctx context.Context, username string, updateFunc UpdateUserFunc) error {

	// Transaction to ensure race conditions wont break things
	err := datastore.RunInTransaction(ctx, func(ctx context.Context) error {

		return updateUserHelper(ctx, username, updateFunc)

	}, &datastore.TransactionOptions{XG: true})
	if err != nil {
		log.Errorf(ctx, "Update User Transaction failed: %v", err)
		return err
	}

	return nil
}

// UpdateUser will update a currently existing user without a transaction
func UpdateUser(ctx context.Context, username string, updateFunc UpdateUserFunc) error {
	return updateUserHelper(ctx, username, updateFunc)
}

func updateUserHelper(ctx context.Context, username string, updateFunc UpdateUserFunc) error {
	key := datastore.NewKey(ctx, "User", username, 0, nil)

	var u User

	err := datastore.Get(ctx, key, &u)
	if err != nil {
		log.Errorf(ctx, "Failed to Get user: %s, err: %s", username, err.Error())
		return err
	}

	err = updateFunc(ctx, &u)
	if err != nil {
		log.Errorf(ctx, "Update Func Failed: %v", err)
		return err
	}

	/* Put the changes to army presets */
	armyKeys := []*datastore.Key{}
	for i := 0; i < len(u.ArmyPresets); i++ {
		armyKeys = append(armyKeys, datastore.NewKey(ctx, "ArmyPreset", u.ArmyPresets[i].ID, 0, nil))
	}
	_, err = datastore.PutMulti(ctx, armyKeys, u.ArmyPresets)
	if err != nil {
		log.Errorf(ctx, "Failed to Put (update) the ArmyPresets: %v", err)
		return err
	}

	/* put the changes to the user */
	_, err = datastore.Put(ctx, key, &u)
	if err != nil {
		log.Errorf(ctx, "Failed to Put (update) user: %s, err: %s", username, err.Error())
		return err
	}

	return nil
}

// GetUser will get a user via its key from DataStore
func GetUser(ctx context.Context, username string) (*User, error) {

	var user User

	/* Get the user model */
	key := datastore.NewKey(ctx, "User", username, 0, nil)
	err := datastore.Get(ctx, key, &user)
	if err != nil {
		log.Errorf(ctx, "Failed to Get user: %s, err: %s", username, err.Error())
		return nil, err
	}

	/* Now get the Army Presets for the User */
	armyKeys := []*datastore.Key{}
	for i := 0; i < len(user.ArmyPresetIDs); i++ {
		armyKeys = append(armyKeys, datastore.NewKey(ctx, "ArmyPreset", user.ArmyPresetIDs[i], 0, nil))
	}

	armys := make([]ArmyPreset, len(user.ArmyPresetIDs))
	err = datastore.GetMulti(ctx, armyKeys, armys)
	if err != nil {
		log.Errorf(ctx, "Failed to Get User Army Presets: %s", err.Error())
		return nil, err
	}
	user.ArmyPresets = armys

	return &user, nil
}
