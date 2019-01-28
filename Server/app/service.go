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

// AddFriend adds a friend to their friends list
func AddFriend(ctx context.Context, username string, friendName string) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Getting user failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(friendName)
	if err != nil {
		log.Errorf(ctx, "Getting user failed: friendName is required")
		return errors.New("friendName is required")
	}
	if username == friendName {
		return errors.New("You can't be your own friend")
	}

	/* Make sure that user actually exists */
	_, err = user.GetUser(ctx, friendName)
	if err != nil {
		return err
	}

	u, err := user.GetUser(ctx, username)
	if err != nil {
		return err
	}
	if common.Contains(u.Friends, friendName) {
		return errors.New("You already have that friend... dumb ass")
	}

	u.Friends = append(u.Friends, friendName)
	err = user.UpdateUser(ctx, username, nil, nil, nil, nil, u.Friends)
	if err != nil {
		return err
	}

	return nil
}

// RemoveFriend removes a friend from their friends list
func RemoveFriend(ctx context.Context, username string, friendName string) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Getting user failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(friendName)
	if err != nil {
		log.Errorf(ctx, "Getting user failed: friendName is required")
		return errors.New("friendName is required")
	}
	if username == friendName {
		return errors.New("Your own name is not a valid option here")
	}

	/* Make sure that user actually exists */
	_, err = user.GetUser(ctx, friendName)
	if err != nil {
		return err
	}

	u, err := user.GetUser(ctx, username)
	if err != nil {
		return err
	}
	if !common.Remove(u.Friends, friendName) {
		return errors.New("That name wasn't in your list")
	}

	err = user.UpdateUser(ctx, username, nil, nil, nil, nil, u.Friends)
	if err != nil {
		return err
	}

	return nil
}

/* GameState / User Section */
/************************************/

// CreatePrivateGame will create a private game with the requested users
func CreatePrivateGame(ctx context.Context, username string, OpponentUsernames []string, boardID int) error {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Create Private Game failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringSliceGreaterThanLength(OpponentUsernames, 0)
	if err != nil {
		log.Errorf(ctx, "Create Private Game failed: more users are required")
		return errors.New("more users required")
	}
	if boardID < 0 {
		log.Errorf(ctx, "Create Private Game failed: invalid boardID number")
		return errors.New("boardID number required")
	}

	/* Ensure the requested opponents actually exist */
	var opponentUsers = make([]*user.User, len(OpponentUsernames))
	for i := 0; i < len(OpponentUsernames); i++ {
		u, err := user.GetUser(ctx, OpponentUsernames[i])
		if err != nil {
			log.Errorf(ctx, "Game Invite for unknown user: %s", OpponentUsernames[i])
			return err
		}
		opponentUsers[i] = u
	}

	/* Create shell private gamestate */
	allUsers := append(OpponentUsernames, username)
	gameStateID := common.GetRandomID()
	err = gamestate.CreateGameState(ctx, gameStateID, boardID, allUsers, []string{username}, len(allUsers), false)
	if err != nil {
		return err
	}

	/* stick gamestate id onto users models */
	u, err := user.GetUser(ctx, username)
	if err != nil {
		log.Criticalf(ctx, "We created a gamestate but failed to send private Invites1")
		return err
	}
	u.PendingPrivateGames = append(u.PendingPrivateGames, gameStateID)
	err = user.UpdateUser(ctx, u.Username, nil, u.PendingPrivateGames, nil, nil, nil)
	if err != nil {
		log.Criticalf(ctx, "We created a gamestate but failed to send private Invites2")
		return err
	}

	for i := 0; i < len(opponentUsers); i++ {
		opponentUsers[i].PendingPrivateGames = append(opponentUsers[i].PendingPrivateGames, gameStateID)
		err = user.UpdateUser(ctx, opponentUsers[i].Username, nil, opponentUsers[i].PendingPrivateGames, nil, nil, nil)
		if err != nil {
			log.Criticalf(ctx, "We created a gamestate but failed to send private Invites4")
			return err
		}
	}

	return nil
}

// CreatePublicGame will create a public game open to all users
func CreatePublicGame(ctx context.Context, username string, boardID int, maxUsers int) error {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Create Public Game failed: username is required")
		return errors.New("username is required")
	}
	if boardID < 0 {
		log.Errorf(ctx, "Create Public Game failed: invalid boardID number")
		return errors.New("boardID number required")
	}
	if maxUsers <= 1 {
		log.Errorf(ctx, "Create Public Game failed: invalid max users")
		return errors.New("Must have more than 1 maxUsers")
	}

	/* Create shell public gamestate */
	gameStateID := common.GetRandomID()
	usersSoFar := []string{username}
	err = gamestate.CreateGameState(ctx, gameStateID, boardID, usersSoFar, usersSoFar, maxUsers, true)
	if err != nil {
		return err
	}

	/* Update the user that created the public game to have a PendingPublicGame*/
	u, err := user.GetUser(ctx, username)
	if err != nil {
		log.Criticalf(ctx, "We created a public gamestate %s but failed to update user who created it", gameStateID)
		return err
	}
	u.PendingPublicGames = append(u.PendingPublicGames, gameStateID)
	err = user.UpdateUser(ctx, u.Username, nil, nil, u.PendingPublicGames, nil, nil)
	if err != nil {
		log.Criticalf(ctx, "We created a public gamestate %s but failed to update user who created it", gameStateID)
		return err
	}

	return nil
}

// AcceptGame will either add user to a public game, or accept a private game invite
func AcceptGame(ctx context.Context, username string, gameStateID string) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Accept Game Invite failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(gameStateID)
	if err != nil {
		log.Errorf(ctx, "Accept Game Invite failed: gameStateID is required")
		return errors.New("gameStateID is required")
	}

	/* Update GameState list of Accepted Users */
	gs, err := gamestate.GetGameState(ctx, gameStateID)
	if err != nil {
		return err
	}

	if common.Contains(gs.AcceptedUsers, username) {
		log.Errorf(ctx, "User %s attempted to accept game %s when already accepted", username, gameStateID)
		return errors.New("Can't accept game twice")
	}

	if gs.IsPublic {
		if len(gs.AcceptedUsers) == gs.MaxUsers {
			log.Errorf(ctx, "User %s attempted to accept game %s that is full", username, gameStateID)
			return errors.New("Game is full")
		}
		/* for public game also add user to list of users */
		gs.Users = append(gs.Users, username)

	} else if !common.Contains(gs.Users, username) {
		log.Errorf(ctx, "Attempted to accept private game %s that user %s is not a part of", gameStateID, username)
		return errors.New("User was not invited to that game")
	}

	gs.AcceptedUsers = append(gs.AcceptedUsers, username)
	err = gamestate.UpdateGameState(ctx, gs.ID, "", gs.Users, gs.AcceptedUsers, nil, nil, nil, nil)
	if err != nil {
		return err
	}

	if gs.IsPublic && len(gs.AcceptedUsers) == gs.MaxUsers {
		/* If the game is public and it's now been filled */
		for i := 0; i < len(gs.AcceptedUsers); i++ {
			u, err := user.GetUser(ctx, gs.Users[i])
			if err != nil {
				log.Criticalf(ctx, "We failed to get a user %s to assign it an active game", u.Username)
				return err
			}

			u.ActiveGames = append(u.ActiveGames, gs.ID)
			_ = common.Remove(u.PendingPublicGames, gs.ID)
			err = user.UpdateUser(ctx, u.Username, u.ActiveGames, nil, u.PendingPublicGames, nil, nil)
			if err != nil {
				log.Criticalf(ctx, "We failed to update a user %s to have an active game", u.Username)
				return err
			}
		}

	} else if !gs.IsPublic && len(gs.Users) == len(gs.AcceptedUsers) {
		log.Infof(ctx, "All Accepted Users: %v, length %d", gs.Users, len(gs.Users))
		/* If the game is private and has been accepted by all people invited */
		for i := 0; i < len(gs.Users); i++ {
			u, err := user.GetUser(ctx, gs.Users[i])
			if err != nil {
				log.Criticalf(ctx, "We failed to get a user %s to assign it an active game", u.Username)
				return err
			}
			log.Infof(ctx, "Operating on user: %v", u)

			u.ActiveGames = append(u.ActiveGames, gs.ID)
			if !common.Remove(u.PendingPrivateGames, gs.ID) {
				log.Criticalf(ctx, "Somehow user %s accepted private game %s without having it on their model", u.Username, gs.ID)
			}
			log.Infof(ctx, "New Values: %v", u)
			err = user.UpdateUser(ctx, u.Username, u.ActiveGames, u.PendingPrivateGames, nil, nil, nil)
			if err != nil {
				log.Criticalf(ctx, "We failed to update a user %s to have an active game", u.Username)
				return err
			}
		}
	} else if gs.IsPublic {
		/* Accepting of public games need to update user if the game isn't full yet */

		u, err := user.GetUser(ctx, username)
		if err != nil {
			log.Criticalf(ctx, "We failed to get a user %s to assign it an active game", username)
			return err
		}
		u.PendingPublicGames = append(u.PendingPublicGames, gs.ID)

		err = user.UpdateUser(ctx, u.Username, nil, nil, u.PendingPublicGames, nil, nil)
		if err != nil {
			log.Criticalf(ctx, "We failed to update a user %s to have an pending game", username)
			return err
		}
	}

	return nil
}

/* GameState Section */
/************************************/

// GetGameState will get the GameState for a particular gameID and user
func GetGameState(ctx context.Context, gameStateID string, username string) (*gamestate.GameState, error) {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Get Game failed: username is required")
		return nil, errors.New("username is required")
	}
	err = common.StringNotEmpty(gameStateID)
	if err != nil {
		log.Errorf(ctx, "Get Game failed: gameStateID is required")
		return nil, errors.New("gameStateID is required")
	}

	gs, err := gamestate.GetGameState(ctx, gameStateID)
	if err != nil {
		return nil, err
	}
	if !gs.IsPublic && !common.Contains(gs.Users, username) {
		log.Errorf(ctx, "Attempted to get game %s that user %s is not a part of", gameStateID, username)
		return nil, errors.New("User is not a part of that game")
	}

	return gs, nil
}

// GetGameStateMulti will get all the GameStates for the gameIDs and user
func GetGameStateMulti(ctx context.Context, gameStateIDs []string, username string) ([]gamestate.GameState, error) {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Get Game failed: username is required")
		return nil, errors.New("username is required")
	}
	err = common.StringSliceGreaterThanLength(gameStateIDs, 0)
	if err != nil {
		log.Errorf(ctx, "Get Game Multi failed: gameStateIDs is required")
		return nil, errors.New("gameStateIDs is required")
	}

	gs, err := gamestate.GetGameStateMulti(ctx, gameStateIDs)
	if err != nil {
		return nil, err
	}

	/* This kinda sucks but I'm not sure how to make it better. Could just not check this... */
	for i := 0; i < len(gs); i++ {
		if !gs[i].IsPublic && !common.Contains(gs[i].Users, username) {
			log.Errorf(ctx, "Attempted to get game %s that user %s is not a part of", gs[i].ID, username)
			return nil, errors.New("User is not a part of that game")
		}
	}

	return gs, nil
}

// GetPublicGamesSummary will query and return all available public games for the user to join
func GetPublicGamesSummary(ctx context.Context, username string, limit int) ([]gamestate.GameState, error) {
	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Get Public Games failed: username is required")
		return nil, errors.New("username is required")
	}
	if limit < 0 {
		log.Errorf(ctx, "Get Game Multi failed: limit invalid")
		return nil, errors.New("Invalid limit given")
	} else if limit == 0 {
		limit = 100
	}

	return gamestate.GetPublicGamesSummary(ctx, username, limit)
}

// UpdateGameState is used to update the game state because a user acted upon the game
func UpdateGameState(ctx context.Context, username, gameID string, readyUsers, aliveUsers []string, units map[string][]gamestate.Unit, cards map[string]gamestate.Cards) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Update GameState failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Update GameState failed: gameID is required")
		return errors.New("gameID is required")
	}

	gs, err := gamestate.GetGameState(ctx, gameID)
	if err != nil {
		return err
	}

	var usersTurn string
	if gs.UsersTurn == "" {
		/* The game hasn't started yet, people are just readying in their units */
		if gs.MaxUsers == len(readyUsers) {
			/* This is the last person readying up right now. Choose a person for their turn */
			usersTurn = gs.AliveUsers[0]
		}
		if len(gs.ReadyUsers) <= len(readyUsers) {
			/* readyUsers should have been increased from before...*/
			log.Errorf(ctx, "Update GameState failed: readied users should have been increased")
			return errors.New("Readied Users should have been increased")
		}
		if units == nil {
			log.Errorf(ctx, "Update GameState failed: user %s readied up but didn't get passed in units", username)
			return errors.New("New readied user should be assigned units")
		}
		if cards == nil {
			log.Errorf(ctx, "Update GameState failed: user %s readied up but didn't get passed in cards", username)
			return errors.New("New readied user should be assigned cards")
		}
		/* Update the game state, adding in units and cards to current gamestate */
		/* Union Units */
		for k, v := range units {
			gs.Units[k] = v
		}
		/* Union Cards */
		for k, v := range cards {
			gs.Cards[k] = v
		}

		return gamestate.UpdateGameState(ctx, gameID, usersTurn, nil, nil, readyUsers, nil, gs.Units, gs.Cards)
	}

	/* This is a person who is making their turn */
	if gs.UsersTurn != username {
		/* Player is trying to cheat? */
		log.Errorf(ctx, "Update GameState failed: username %s does not match players turn %s", username, gs.UsersTurn)
		return errors.New("gameID is required")
	}
	/* If there is only one person left alive then the game is over we need to update all users */
	if aliveUsers != nil {
		if len(aliveUsers) == 1 {
			// TODO: update users to have a `Completed Game`. Still need to update the game state tho so users can see that last move?
		}
		gs.AliveUsers = aliveUsers
	}

	/* Choose the next user that should go */
	var found = false
	for i, v := range gs.AliveUsers {
		if v == gs.UsersTurn {
			if i+1 >= len(gs.AliveUsers) {
				usersTurn = gs.AliveUsers[0]
			} else {
				usersTurn = gs.AliveUsers[i+1]
			}
			found = true
			break
		}
	}
	/* Will happen if the current user died in his turn */
	if !found {
		/* TODO: I'm not sure if this is possible so I'll fix it later if it is */
		log.Criticalf(ctx, "The user %s died in his turn and now no next user has been selected", username)
	}

	if units != nil {
		/* Overwrite unit keys with new values */
		for k, v := range units {
			gs.Units[k] = v
		}
	}
	if cards != nil {
		/* Overwrite card keys with new values */
		for k, v := range cards {
			gs.Cards[k] = v
		}
	}

	return gamestate.UpdateGameState(ctx, gameID, usersTurn, nil, nil, nil, gs.AliveUsers, gs.Units, gs.Cards)
}
