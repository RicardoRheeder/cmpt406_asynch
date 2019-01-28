package gamestate

import (
	"Projects/cmpt406_asynch/Server/common"
	"context"
	"errors"

	"google.golang.org/appengine/datastore"
	"google.golang.org/appengine/log"
)

// CreateGameState will create a game state in DataStore
func CreateGameState(ctx context.Context, ID string, boardID int, users, acceptedUsers []string, maxUsers int, isPublic bool) error {

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
		ID:             ID,
		BoardID:        boardID,
		IsPublic:       isPublic,
		MaxUsers:       maxUsers,
		SpotsAvailable: spotsAvailable,
		Users:          users,
		AcceptedUsers:  acceptedUsers,
		AliveUsers:     users,
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
func UpdateGameState(ctx context.Context, ID, usersTurn string, users, acceptedUsers, readyUsers, aliveUsers []string, units map[string][]Unit, cards map[string]Cards) error {

	gs, err := GetGameState(ctx, ID)
	if err != nil {
		log.Errorf(ctx, "Attempted to update GameState: %s, that doesn't exists", ID)
		return errors.New("GameState Doesn't Exists")
	}
	var spotsAvailable int
	if gs.IsPublic {
		spotsAvailable = gs.MaxUsers - len(acceptedUsers)
	} else {
		spotsAvailable = 0
	}

	/* Input 0 values to make the value not change */
	if users != nil {
		gs.Users = users
	}
	if acceptedUsers != nil {
		gs.AcceptedUsers = acceptedUsers
	}
	if readyUsers != nil {
		gs.ReadyUsers = readyUsers
	}
	if aliveUsers != nil {
		gs.AliveUsers = aliveUsers
	}
	if usersTurn != "" {
		gs.UsersTurn = usersTurn
	}
	if units != nil {
		gs.Units = units
	}
	if cards != nil {
		gs.Cards = cards
	}

	gameState := &GameState{
		ID:             gs.ID,
		BoardID:        gs.BoardID,
		MaxUsers:       gs.MaxUsers,
		SpotsAvailable: spotsAvailable,
		IsPublic:       gs.IsPublic,
		Users:          gs.Users,
		AcceptedUsers:  gs.AcceptedUsers,
		ReadyUsers:     gs.ReadyUsers,
		AliveUsers:     gs.AliveUsers,
		UsersTurn:      gs.UsersTurn,
		Units:          gs.Units,
		Cards:          gs.Cards,
	}

	key := datastore.NewKey(ctx, "GameState", gs.ID, 0, nil)

	_, err = datastore.Put(ctx, key, gameState)
	if err != nil {
		log.Errorf(ctx, "Failed to Put (update) GameState: %s", gs.ID)
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

	q := datastore.NewQuery("GameState")

	for i := 0; i < len(IDs); i++ {
		key := datastore.NewKey(ctx, "GameState", IDs[i], 0, nil)
		q = q.Filter("__key__ =", key)
	}

	q = q.
		Project("ID", "BoardID", "MaxUsers", "SpotsAvailable", "IsPublic", "Users", "AcceptedUsers", "ReadyUsers", "AliveUsers", "UsersTurn")

	var gameStates = []GameState{}

	t := q.Run(ctx)
	for {
		var s GameState
		_, err := t.Next(&s)
		if err == datastore.Done {
			break // No further entities match the query.
		}
		if err != nil {
			log.Errorf(ctx, "fetching next multi GameState: %v", err)
			break
		}
	}

	return gameStates, nil
}

// GetPublicGamesSummary queries for public available games
func GetPublicGamesSummary(ctx context.Context, username string, limit int) ([]GameState, error) {

	var gameStates = []GameState{}

	q := datastore.NewQuery("GameState").
		Filter("IsPublic =", true).
		Filter("SpotsAvailable >", 0).
		Project("ID", "BoardID", "MaxUsers", "SpotsAvailable").
		Limit(limit)
	t := q.Run(ctx)
	for {
		var s GameState
		_, err := t.Next(&s)
		if err == datastore.Done {
			break // No further entities match the query.
		}
		if err != nil {
			log.Errorf(ctx, "fetching next GameState: %v", err)
			break
		}
		/* No point including games they're already a part of */
		if !common.Contains(s.AcceptedUsers, username) {
			gameStates = append(gameStates, s)
		}
	}

	return gameStates, nil
}
