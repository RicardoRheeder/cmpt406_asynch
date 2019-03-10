package gamestate

import (
	"context"
	"time"

	"google.golang.org/appengine/datastore"
	"google.golang.org/appengine/log"
)

// CreateGameState will create a game state in DataStore
func CreateGameState(ctx context.Context, ID string, boardID int, users, acceptedUsers []string, maxUsers int, isPublic bool, gameName string, forfeitTime int) error {

	var spotsAvailable int
	if isPublic {
		spotsAvailable = maxUsers - len(acceptedUsers)
	} else {
		spotsAvailable = 0
	}

	gameState := &GameState{
		ID:             ID,
		GameName:       gameName,
		CreatedBy:      acceptedUsers[0],
		BoardID:        boardID,
		IsPublic:       isPublic,
		IsComplete:     false,
		MaxUsers:       maxUsers,
		SpotsAvailable: spotsAvailable,
		UsersTurn:      "",
		Users:          users,
		AcceptedUsers:  acceptedUsers,
		ReadyUsers:     []string{},
		AliveUsers:     users,
		Units:          []Unit{},
		InitUnits:      []Unit{},
		Generals:       []Unit{},
		CardIDs:        []string{},
		Actions:        []Action{},
		ForfeitTime:    forfeitTime,
		LoseReasons:    []Lose{},
		Created:        time.Now().UTC(),
	}

	key := datastore.NewKey(ctx, "GameState", ID, 0, nil)
	_, err := datastore.Put(ctx, key, gameState)
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

		/* Put the changes to cards */
		cardKeys := []*datastore.Key{}
		for i := 0; i < len(gameState.Cards); i++ {
			cardKeys = append(cardKeys, datastore.NewKey(ctx, "Cards", gameState.Cards[i].ID, 0, nil))
		}
		_, err = datastore.PutMulti(ctx, cardKeys, gameState.Cards)
		if err != nil {
			log.Errorf(ctx, "Failed to Put (update) the Cards: %s", ID)
			return err
		}

		/* Put the changes to gamestate */
		_, err = datastore.Put(ctx, key, &gameState)
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

	/* Now get the cards for the GameState */
	cardKeys := []*datastore.Key{}

	for i := 0; i < len(gameState.CardIDs); i++ {
		cardKeys = append(cardKeys, datastore.NewKey(ctx, "Cards", gameState.CardIDs[i], 0, nil))
	}

	cards := make([]Cards, len(gameState.CardIDs))
	err = datastore.GetMulti(ctx, cardKeys, cards)
	if err != nil {
		log.Errorf(ctx, "Failed to Get GameState Cards: %s", err.Error())
		return nil, err
	}
	gameState.Cards = cards

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
func GetPublicGamesSummary(ctx context.Context, limit int) ([]GameState, error) {

	var gameStates = []GameState{}

	q := datastore.NewQuery("GameState").
		Filter("IsPublic =", true).
		Filter("SpotsAvailable >", 0).
		Project("ID", "GameName", "BoardID", "MaxUsers", "SpotsAvailable", "CreatedBy").
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

// GetCompletedGames queries for finished games
func GetCompletedGames(ctx context.Context, limit int) ([]GameState, error) {

	var gameStates = []GameState{}

	q := datastore.NewQuery("GameState").
		Filter("IsComplete =", true).
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
