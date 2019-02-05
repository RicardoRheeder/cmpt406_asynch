package gamestate

import (
	"context"
	"encoding/json"
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

	gameState := &trueGameState{
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
		Units:           []byte{},
		Cards:           []byte{},
		Actions:         []byte{},
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

		var trueGS trueGameState

		key := datastore.NewKey(ctx, "GameState", ID, 0, nil)
		err := datastore.Get(ctx, key, &trueGS)
		if err != nil {
			log.Errorf(ctx, "Failed to Get gameState: %s", ID)
			return err
		}

		gameState, err := convertTrueGameStateToGameState(ctx, &trueGS)
		if err != nil {
			return err
		}

		err = updateGameStateFunc(ctx, gameState)
		if err != nil {
			return err
		}

		returnTrueGS, err := convertGameStateToTrueGameState(ctx, gameState)
		if err != nil {
			return err
		}

		_, err = datastore.Put(ctx, key, returnTrueGS)
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

	var trueGS trueGameState

	key := datastore.NewKey(ctx, "GameState", ID, 0, nil)

	err := datastore.Get(ctx, key, &trueGS)
	if err != nil {
		log.Errorf(ctx, "Failed to Get gameState: %s", ID)
		return nil, err
	}

	return convertTrueGameStateToGameState(ctx, &trueGS)
}

// GetGameStateMulti will get a gamestate for each ID from DataStore
// If any of the keys are invalid, the entire lookup could fail
func GetGameStateMulti(ctx context.Context, IDs []string) ([]*GameState, error) {

	keys := []*datastore.Key{}

	for i := 0; i < len(IDs); i++ {
		keys = append(keys, datastore.NewKey(ctx, "GameState", IDs[i], 0, nil))
	}

	trueGSs := make([]trueGameState, len(IDs))
	err := datastore.GetMulti(ctx, keys, trueGSs)
	if err != nil {
		log.Errorf(ctx, "Failed to Get gameStates: %s", err.Error())
		return nil, err
	}

	gameStates := make([]*GameState, len(IDs))
	/* convert each []btyte to their map version */
	for _, v := range trueGSs {
		gs, err := convertTrueGameStateToGameState(ctx, &v)
		if err != nil {
			return nil, err
		}
		gameStates = append(gameStates, gs)
	}

	return gameStates, nil
}

// GetPublicGamesSummary queries for public available games
func GetPublicGamesSummary(ctx context.Context, username string, limit int) ([]*GameState, error) {

	var gameStates = []*GameState{}

	q := datastore.NewQuery("GameState").
		Filter("IsPublic =", true).
		Filter("SpotsAvailable >", 0).
		Project("ID", "GameName", "BoardID", "MaxUsers", "SpotsAvailable").
		Limit(limit)
	t := q.Run(ctx)
	for {
		var tgs trueGameState
		_, err := t.Next(&tgs)
		if err == datastore.Done {
			break // No further entities match the query.
		}
		if err != nil {
			log.Errorf(ctx, "fetching next Summary: %v", err)
			break
		}
		gs, err := convertTrueGameStateToGameState(ctx, &tgs)
		if err != nil {
			return nil, err
		}
		gameStates = append(gameStates, gs)
	}

	return gameStates, nil
}

/* helper function to deal with []bytes */
func convertTrueGameStateToGameState(ctx context.Context, gs *trueGameState) (*GameState, error) {
	returnGS := &GameState{
		ID:              gs.ID,
		GameName:        gs.GameName,
		CreatedBy:       gs.CreatedBy,
		BoardID:         gs.BoardID,
		MaxUsers:        gs.MaxUsers,
		SpotsAvailable:  gs.SpotsAvailable,
		IsPublic:        gs.IsPublic,
		Users:           gs.Users,
		AcceptedUsers:   gs.AcceptedUsers,
		ReadyUsers:      gs.ReadyUsers,
		AliveUsers:      gs.AliveUsers,
		UsersTurn:       gs.UsersTurn,
		TurnTime:        gs.TurnTime,
		TimeToStateTurn: gs.TimeToStateTurn,
		Created:         gs.Created,
	}

	if len(gs.Units) > 0 {
		err := json.Unmarshal(gs.Units, &returnGS.Units)
		if err != nil {
			return nil, err
		}
	}
	if len(gs.Cards) > 0 {
		err := json.Unmarshal(gs.Cards, &returnGS.Cards)
		if err != nil {
			return nil, err
		}
	}
	if len(gs.Actions) > 0 {
		err := json.Unmarshal(gs.Actions, &returnGS.Actions)
		if err != nil {
			return nil, err
		}
	}

	return returnGS, nil
}

func convertGameStateToTrueGameState(ctx context.Context, gs *GameState) (*trueGameState, error) {
	returnGS := &trueGameState{
		ID:              gs.ID,
		GameName:        gs.GameName,
		CreatedBy:       gs.CreatedBy,
		BoardID:         gs.BoardID,
		MaxUsers:        gs.MaxUsers,
		SpotsAvailable:  gs.SpotsAvailable,
		IsPublic:        gs.IsPublic,
		Users:           gs.Users,
		AcceptedUsers:   gs.AcceptedUsers,
		ReadyUsers:      gs.ReadyUsers,
		AliveUsers:      gs.AliveUsers,
		UsersTurn:       gs.UsersTurn,
		TurnTime:        gs.TurnTime,
		TimeToStateTurn: gs.TimeToStateTurn,
		Created:         gs.Created,
	}

	var err error
	if len(gs.Units) > 0 {
		returnGS.Units, err = json.Marshal(gs.Units)
		if err != nil {
			return nil, err
		}
	}
	if len(gs.Cards) > 0 {
		returnGS.Cards, err = json.Marshal(gs.Cards)
		if err != nil {
			return nil, err
		}
	}
	if len(gs.Cards) > 0 {
		returnGS.Actions, err = json.Marshal(gs.Actions)
		if err != nil {
			return nil, err
		}
	}

	return returnGS, nil
}
