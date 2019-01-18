package main

import (
	"Projects/cmpt406_asynch/Server/common"
	"Projects/cmpt406_asynch/Server/gamestate"
	"Projects/cmpt406_asynch/Server/user"
	"context"
	"encoding/base64"
	"errors"
	"net/http"
	"strings"

	"google.golang.org/appengine/log"
)

/* Auth Section */
/************************************/

// ValidateAuth checks that the person says who he says he is
func ValidateAuth(ctx context.Context, r *http.Request) (string, error) {

	auth := strings.SplitN(r.Header.Get("Authorization"), " ", 2)

	if len(auth) != 2 || auth[0] != "Basic" {
		log.Errorf(ctx, "Basic Auth is required")
		return "", errors.New("Un-Authed")
	}

	payload, _ := base64.StdEncoding.DecodeString(auth[1])
	pair := strings.SplitN(string(payload), ":", 2)

	if len(pair) != 2 {
		log.Errorf(ctx, "Name/Pass Basic Auth is required")
		return "", errors.New("Un-Authed")
	}

	return pair[0], validateUserNamePassword(ctx, pair[0], pair[1])
}

// validateUserNamePassword ensure that a given username matches the given password
func validateUserNamePassword(ctx context.Context, username string, password string) error {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Validate user failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(password)
	if err != nil {
		log.Errorf(ctx, "validate user failed: password is required")
		return errors.New("password is required")
	}

	user, err := user.GetUser(ctx, username)
	if err != nil {
		return err
	}

	if user.Password != password {
		log.Errorf(ctx, "validate user failed: password does not match")
		return errors.New("Wrong Password")
	}

	return nil
}

/* User Section */
/************************************/

// CreateUser will create a user
func CreateUser(ctx context.Context, username string, password string) error {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Create user failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(password)
	if err != nil {
		log.Errorf(ctx, "Create user failed: password is required")
		return errors.New("password is required")
	}

	return user.CreateUser(ctx, username, password)
}

// GetUser will get a user with the given name
func GetUser(ctx context.Context, username string) (*user.User, error) {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Getting user failed: username is required")
		return nil, errors.New("username is required")
	}

	return user.GetUser(ctx, username)
}

/* GameState / User Section */
/************************************/

// RequestGame will update all users involved in the game request
func RequestGame(ctx context.Context, username string, OpponentUsernames []string, board int) error {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Request Game failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringSliceGreaterThanLength(OpponentUsernames, 0)
	if err != nil {
		log.Errorf(ctx, "Request Game failed: more users are required")
		return errors.New("more users required")
	}
	if board <= 0 {
		log.Errorf(ctx, "Request Game failed: invalid board number")
		return errors.New("board number required")
	}

	/* Ensure the requested opponents actually exist */
	for i := 0; i < len(OpponentUsernames); i++ {
		_, err = user.GetUser(ctx, OpponentUsernames[i])
		if err != nil {
			log.Errorf(ctx, "Request Game for unknoewn user: %s", OpponentUsernames[i])
			return err
		}
	}

	/* Create shell gamestate */
	allUsers := append(OpponentUsernames, username)
	gameStateID := common.GetRandomID()
	err = gamestate.CreateGameState(ctx, gameStateID, board, allUsers)
	if err != nil {
		return err
	}

	/* stick gamestate id onto users models */
	u, err := user.GetUser(ctx, username)
	if err != nil {
		log.Criticalf(ctx, "We created a gamestate but failed to send Invites1")
		return err
	}
	u.SentInvites = append(u.SentInvites, gameStateID)
	err = user.UpdateUser(ctx, u.Username, nil, u.SentInvites, nil, nil)
	if err != nil {
		log.Criticalf(ctx, "We created a gamestate but failed to send Invites2")
		return err
	}

	for i := 0; i < len(OpponentUsernames); i++ {
		u, err := user.GetUser(ctx, OpponentUsernames[i])
		if err != nil {
			log.Criticalf(ctx, "We created a gamestate but failed to send Invites3")
			return err
		}
		u.RecievedInvites = append(u.RecievedInvites, gameStateID)
		err = user.UpdateUser(ctx, u.Username, nil, nil, u.RecievedInvites, nil)
		if err != nil {
			log.Criticalf(ctx, "We created a gamestate but failed to send Invites4")
			return err
		}
	}

	return nil
}
