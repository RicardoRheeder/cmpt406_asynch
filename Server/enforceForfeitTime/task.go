package enforceforfeittime

import (
	"Projects/cmpt406_asynch/Server/common"
	"Projects/cmpt406_asynch/Server/gamestate"
	"context"
	"encoding/json"
	"errors"
	"io"
	"net/http"
	"time"

	"google.golang.org/appengine/log"
	"google.golang.org/appengine/taskqueue"
)

type enforceTimeTaskPayload struct {
	GameID           string `json:"GameId"`
	UsernamesTurn    string `json:"usernamesTurn"`
	ActionListLength int    `json:"actionListLength"`
}

// CreateTask will create a task to enforce time limits
func CreateTask(ctx context.Context, gameID string, usernamesTurn string, actionListLength int, forfeitTime int) error {
	if gameID == "" {
		log.Errorf(ctx, "A gameID is required to create a enforcing task")
		return errors.New("gameID required")
	}
	if usernamesTurn == "" {
		log.Errorf(ctx, "A username is required to create a enforcing task")
		return errors.New("username required")
	}
	if actionListLength < 0 {
		log.Errorf(ctx, "Action list length is required to create a enforcing task")
		return errors.New("action list length required")
	}
	if forfeitTime <= 0 {
		// if its infinite just return
		return nil
	}

	payload, err := json.Marshal(enforceTimeTaskPayload{GameID: gameID, UsernamesTurn: usernamesTurn, ActionListLength: actionListLength})
	if err != nil {
		log.Errorf(ctx, "%v", err)
		return err
	}

	h := make(http.Header)
	h.Set("Content-Type", "application/json")
	t := &taskqueue.Task{
		Path:    "/EnforceForfeitTime",
		Payload: payload,
		Header:  h,
		Method:  "POST",
		Delay:   time.Second * time.Duration(forfeitTime),
	}
	// Add task to the default queue
	if _, err := taskqueue.Add(ctx, t, ""); err != nil {
		log.Errorf(ctx, "%v", err)
		return err
	}

	return nil
}

// EnforceTask will work with the task that was created for enforcing
func EnforceTask(ctx context.Context, body io.ReadCloser) error {

	decoder := json.NewDecoder(body)
	var ettp enforceTimeTaskPayload
	err := decoder.Decode(&ettp)
	if err != nil {
		log.Errorf(ctx, "%v", err)
		return err
	}

	err = gamestate.UpdateGameState(ctx, ettp.GameID, enforceTask(ettp.UsernamesTurn, ettp.ActionListLength))
	if err != nil {
		return err
	}

	return nil
}

// Helper function to editing the gamestate
func enforceTask(usernamesTurn string, actionListLength int) gamestate.UpdateGameStateFunc {

	return func(ctx context.Context, gs *gamestate.GameState) error {
		if usernamesTurn == gs.UsersTurn && actionListLength == len(gs.Actions) {
			// player has failed to do their turn and will now forfeit
			common.Remove(&gs.AliveUsers, usernamesTurn)
			common.RemoveAllUnits(&gs.Units, usernamesTurn)
			common.RemoveAllUnits(&gs.Generals, usernamesTurn)
			common.RemoveCards(&gs.Cards, usernamesTurn)

			gs.LoseReasons = append(gs.LoseReasons, gamestate.Lose{Username: usernamesTurn, Reason: gamestate.ForcedForfeit})
		}

		return nil
	}
}
