package gamestate

import (
	"context"
	"errors"
	"time"

	"google.golang.org/appengine/datastore"
	"google.golang.org/appengine/log"
)

// CreateGameState will create a game state in DataStore
func CreateGameState(ctx context.Context, ID string, boardID int, users, acceptedUsers []string, maxUsers int, isPublic bool, gameName string, turnTime, timeToStartTurn int) error {

	_, err := GetGameState(ctx, ID)
	if err == nil {
		log.Errorf(ctx, "Attempted to create GameState: %s, that already exists", ID)
		return errors.New("GameState Already Exists")
	}

	var spotsAvailable int
	if isPublic {
		spotsAvailable = maxUsers - len(acceptedUsers)
	} else {
		spotsAvailable = 0
	}

	gameState := &GameState{
		ID:              ID,
		GameName:        gameName,
		CreatedBy:       acceptedUsers[0],
		BoardID:         boardID,
		IsPublic:        isPublic,
		MaxUsers:        maxUsers,
		SpotsAvailable:  spotsAvailable,
		UsersTurn:       "",
		Users:           users,
		AcceptedUsers:   acceptedUsers,
		ReadyUsers:      []string{},
		AliveUsers:      users,
		Units:           []Unit{},
		Cards:           []Cards{},
		Actions:         []Action{},
		TurnTime:        turnTime,
		TimeToStateTurn: timeToStartTurn,
		Created:         time.Now().UTC(),
	}

	key := datastore.NewKey(ctx, "GameState", ID, 0, nil)
	_, err = datastore.Put(ctx, key, gameState)
	if err != nil {
		log.Errorf(ctx, "Failed to Put (create) gameState: %s", ID)
		return err
	}

	return nil

}

// UpdateGameState will update the GameState with new values
func UpdateGameState(ctx context.Context, ID string, updateGameStateFunc UpdateGameStateFunc) error {

	// Transaction to ensure race conditions wont break things
	err := datastore.RunInTransaction(ctx, func(ctx context.Context) error {

		var gameState GameState

		key := datastore.NewKey(ctx, "GameState", ID, 0, nil)
		err := datastore.Get(ctx, key, &gameState)
		if err != nil {
			log.Errorf(ctx, "Failed to Get gameState: %s", ID)
			return err
		}

		err = updateGameStateFunc(ctx, &gameState)
		if err != nil {
			return err
		}

		_, err = datastore.Put(ctx, key, gameState)
		if err != nil {
			log.Errorf(ctx, "Failed to Put (update) gameState: %s", ID)
			return err
		}

		return nil
	}, &datastore.TransactionOptions{XG: true})
	if err != nil {
		log.Errorf(ctx, "Update GameState Transaction failed: %v", err)
		return err
	}

	return nil
}

// GetGameState will get a gamestate via its key ID from DataStore
func GetGameState(ctx context.Context, ID string) (*GameState, error) {

	var gameState GameState

	key := datastore.NewKey(ctx, "GameState", ID, 0, nil)

	err := datastore.Get(ctx, key, &gameState)
	if err != nil {
		log.Errorf(ctx, "Failed to Get gameState: %s", ID)
		return nil, err
	}

	return &gameState, nil
}

// GetGameStateMulti will get a gamestate for each ID from DataStore
// If any of the keys are invalid, the entire lookup could fail
func GetGameStateMulti(ctx context.Context, IDs []string) ([]GameState, error) {

	keys := []*datastore.Key{}

	for i := 0; i < len(IDs); i++ {
		keys = append(keys, datastore.NewKey(ctx, "GameState", IDs[i], 0, nil))
	}

	gameStates := make([]GameState, len(IDs))
	err := datastore.GetMulti(ctx, keys, gameStates)
	if err != nil {
		log.Errorf(ctx, "Failed to Get gameStates: %s", err.Error())
		return nil, err
	}

	return gameStates, nil
}

// GetPublicGamesSummary queries for public available games
func GetPublicGamesSummary(ctx context.Context, username string, limit int) ([]GameState, error) {

	var gameStates = []GameState{}

	q := datastore.NewQuery("GameState").
		Filter("IsPublic =", true).
		Filter("SpotsAvailable >", 0).
		Project("ID", "GameName", "BoardID", "MaxUsers", "SpotsAvailable").
		Limit(limit)
	t := q.Run(ctx)
	for {
		var gs GameState
		_, err := t.Next(&gs)
		if err == datastore.Done {
			break // No further entities match the query.
		}
		if err != nil {
			log.Errorf(ctx, "fetching next Summary: %v", err)
			break
		}
		gameStates = append(gameStates, gs)
	}

	return gameStates, nil
}
