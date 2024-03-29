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
		log.Errorf(ctx, "AddFriend failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(friendName)
	if err != nil {
		log.Errorf(ctx, "AddFriend failed: friendName is required")
		return errors.New("friendName is required")
	}
	if username == friendName {
		return errors.New("You can't be your own friend")
	}

	/* Make sure that the friend actually exists */
	_, err = user.GetUser(ctx, friendName)
	if err != nil {
		return err
	}

	err = user.UpdateUserWithT(ctx, username, addFriend(ctx, friendName))
	if err != nil {
		return err
	}

	return nil
}

// Helper function to editing the User model
func addFriend(ctx context.Context, friendName string) user.UpdateUserFunc {

	return func(ctx context.Context, u *user.User) error {
		if common.Contains(u.Friends, friendName) {
			return errors.New("You already have that friend... dumb ass")
		}

		u.Friends = append(u.Friends, friendName)

		return nil
	}
}

// RemoveFriend removes a friend from their friends list
func RemoveFriend(ctx context.Context, username string, friendName string) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "RemoveFriend failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(friendName)
	if err != nil {
		log.Errorf(ctx, "RemoveFriend failed: friendName is required")
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

	err = user.UpdateUserWithT(ctx, username, removeFriend(ctx, friendName))
	if err != nil {
		return err
	}

	return nil
}

// Helper function to editing the User model
func removeFriend(ctx context.Context, friendName string) user.UpdateUserFunc {

	return func(ctx context.Context, u *user.User) error {

		if !common.Remove(&u.Friends, friendName) {
			return errors.New("That name wasn't in your list")
		}
		return nil
	}
}

// AddArmyPreset will add an army preset to the users model
func AddArmyPreset(ctx context.Context, username string, presetName string, units []int, general int) error {
	if general <= 0 {
		log.Errorf(ctx, "AddArmyPreset Failed: General int must be greated than 0")
		return errors.New("General int must be greated than 0")
	}
	if len(units) <= 0 {
		log.Errorf(ctx, "AddArmyPreset Failed: There must be at least 1 unit provided")
		return errors.New("There must be at least 1 unit provided")
	}
	err := common.StringNotEmpty(presetName)
	if err != nil {
		log.Errorf(ctx, "AddArmyPreset Failed: name is required")
		return errors.New("name is required")
	}

	err = user.UpdateUserWithT(ctx, username, addArmyPreset(ctx, presetName, units, general))
	if err != nil {
		return err
	}

	return nil
}

// Helper function to editing the User model
func addArmyPreset(ctx context.Context, presetName string, units []int, general int) user.UpdateUserFunc {

	return func(ctx context.Context, u *user.User) error {

		armyPresetID := common.GetRandomID()
		armyPreset := user.ArmyPreset{ID: armyPresetID, Name: presetName, Units: units, General: general}

		u.ArmyPresetIDs = append(u.ArmyPresetIDs, armyPresetID)
		u.ArmyPresets = append(u.ArmyPresets, armyPreset)
		return nil
	}
}

// RemoveArmyPreset will remove an army preset from the users model
func RemoveArmyPreset(ctx context.Context, username string, armyPresetID string) error {
	err := common.StringNotEmpty(armyPresetID)
	if err != nil {
		log.Errorf(ctx, "RemoveArmyPreset Failed: armyPresetId required")
		return errors.New("armyPresetId required")
	}

	err = user.UpdateUserWithT(ctx, username, removeArmyPreset(ctx, armyPresetID))
	if err != nil {
		return err
	}

	return nil
}

// Helper function to editing the User model
func removeArmyPreset(ctx context.Context, armyPresetID string) user.UpdateUserFunc {

	return func(ctx context.Context, u *user.User) error {

		if !common.Remove(&u.ArmyPresetIDs, armyPresetID) {
			return errors.New("Provided Army Preset ID doesn't exist on user ")
		}
		/* TODO: this just orphans the army preset, it doesn't actually delete it */
		return nil
	}
}

/* GameState / User Section */
/************************************/

// CreatePrivateGame will create a private game with the requested users
func CreatePrivateGame(ctx context.Context, username string, opponentUsernames []string, boardID int, gameName string, forfeitTime int) error {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Create Private Game failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(gameName)
	if err != nil {
		log.Errorf(ctx, "Create Private Game failed: gameName is required")
		return errors.New("gameName is required")
	}
	err = common.StringSliceGreaterThanLength(opponentUsernames, 0)
	if err != nil {
		log.Errorf(ctx, "Create Private Game failed: more users are required")
		return errors.New("more users required")
	}
	if boardID <= 0 {
		log.Errorf(ctx, "Create Private Game failed: invalid boardID number")
		return errors.New("boardId number required")
	}
	if forfeitTime == 0 || forfeitTime < -1 {
		log.Errorf(ctx, "Create Private Game failed: forfeitTime turnTime number")
		return errors.New("forfeitTime number invalid")
	}

	/* Ensure the requested opponents actually exist */
	/* TODO: not a fan of all this extra datastore lookups, But also not sure how to avoid it */
	for i := 0; i < len(opponentUsernames); i++ {
		_, err := user.GetUser(ctx, opponentUsernames[i])
		if err != nil {
			log.Errorf(ctx, "Game Invite for unknown user: %s", opponentUsernames[i])
			return err
		}
	}

	/* Update the user, all other invite and create a shell game state*/
	err = user.UpdateUserWithT(ctx, username, createPrivateGame(ctx, username, opponentUsernames, boardID, gameName, forfeitTime))
	if err != nil {
		return err
	}
	return nil
}

func createPrivateGame(ctx context.Context, username string, opponentUsernames []string, boardID int, gameName string, forfeitTime int) user.UpdateUserFunc {

	return func(ctx context.Context, u *user.User) error {
		/* Create shell private gamestate */
		allUsers := append(opponentUsernames, username)
		gameStateID := common.GetRandomID()
		err := gamestate.CreateGameState(ctx, gameStateID, boardID, allUsers, []string{username}, len(allUsers), false, gameName, forfeitTime)
		if err != nil {
			return err
		}

		/* Update user creating the private game and all invitees */
		u.PendingPrivateGames = append(u.PendingPrivateGames, gameStateID)
		for i := 0; i < len(opponentUsernames); i++ {
			err = user.UpdateUser(ctx, opponentUsernames[i], appendToPrivateGames(ctx, gameStateID))
			if err != nil {
				log.Criticalf(ctx, "We created a gamestate but failed to send private Invites4")
				return err
			}
		}
		return nil
	}
}

// CreatePublicGame will create a public game open to all users
func CreatePublicGame(ctx context.Context, username string, boardID int, maxUsers int, gameName string, forfeitTime int) error {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Create Public Game failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(gameName)
	if err != nil {
		log.Errorf(ctx, "Create Private Game failed: gameName is required")
		return errors.New("gameName is required")
	}
	if boardID <= 0 {
		log.Errorf(ctx, "Create Public Game failed: invalid boardID number")
		return errors.New("boardId number required")
	}
	if maxUsers <= 1 {
		log.Errorf(ctx, "Create Public Game failed: invalid max users")
		return errors.New("Must have more than 1 maxUsers")
	}
	if forfeitTime == 0 || forfeitTime < -1 {
		log.Errorf(ctx, "Create Private Game failed: forfeitTime turnTime number")
		return errors.New("forfeitTime number invalid")
	}

	/* Update the user that created the public game to have a PendingPublicGame */
	err = user.UpdateUserWithT(ctx, username, createPublicGame(ctx, username, boardID, maxUsers, gameName, forfeitTime))
	if err != nil {
		return err
	}

	return nil
}

func createPublicGame(ctx context.Context, username string, boardID int, maxUsers int, gameName string, forfeitTime int) user.UpdateUserFunc {

	return func(ctx context.Context, u *user.User) error {
		/* Create shell public gamestate */
		gameStateID := common.GetRandomID()
		usersSoFar := []string{username}
		err := gamestate.CreateGameState(ctx, gameStateID, boardID, usersSoFar, usersSoFar, maxUsers, true, gameName, forfeitTime)
		if err != nil {
			return err
		}
		/* Update the user who created the public game */
		u.PendingPublicGames = append(u.PendingPublicGames, gameStateID)

		return nil
	}
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

	err = gamestate.UpdateGameState(ctx, gameStateID, acceptGame(ctx, username, gameStateID))
	if err != nil {
		return err
	}

	return nil
}

// Helper function to editing the User model
func acceptGame(ctx context.Context, username, gameStateID string) gamestate.UpdateGameStateFunc {
	return func(ctx context.Context, gs *gamestate.GameState) error {

		/* Make sure user isn't being dumb */
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
			gs.AliveUsers = append(gs.AliveUsers, username)
			gs.SpotsAvailable = gs.MaxUsers - len(gs.Users)

			/* Accepting of public games need to update user */
			err := user.UpdateUser(ctx, username, appendToPublicGames(ctx, gameStateID))
			if err != nil {
				log.Errorf(ctx, "We failed to update a user %s to have an pending game", username)
				return err
			}

		} else if !common.Contains(gs.Users, username) {
			log.Errorf(ctx, "Attempted to accept private game %s that user %s is not a part of", gameStateID, username)
			return errors.New("User was not invited to that game")
		}
		/* add user to list of accepted users */
		gs.AcceptedUsers = append(gs.AcceptedUsers, username)

		return nil
	}
}

// DeclineGame will reject a private game invite
func DeclineGame(ctx context.Context, username string, gameStateID string) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Decline Game failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(gameStateID)
	if err != nil {
		log.Errorf(ctx, "Decline Game failed: gameStateID is required")
		return errors.New("gameStateID is required")
	}

	err = gamestate.UpdateGameState(ctx, gameStateID, declineGame(ctx, username, gameStateID))
	if err != nil {
		return err
	}

	return nil
}

func declineGame(ctx context.Context, username, gameStateID string) gamestate.UpdateGameStateFunc {

	return func(ctx context.Context, gs *gamestate.GameState) error {
		if gs.CreatedBy == username {
			log.Errorf(ctx, "user: %s, tried to Decline a game they created", username)
			return errors.New("Cannot Decline a game you created")
		}
		if gs.IsPublic {
			log.Errorf(ctx, "user: %s, tried to Decline a public game", username)
			return errors.New("Cannot BackOut of private game once 'Accepted'")
		}
		if !common.Contains(gs.Users, username) {
			log.Errorf(ctx, "user: %s, tried to Decline a game they weren't invited to", username)
			return errors.New("Cannot Decline game you were'nt invite to")
		}
		if common.Contains(gs.AcceptedUsers, username) {
			log.Errorf(ctx, "user: %s, tried to Decline a game they already accepted", username)
			return errors.New("Cannot Decline a game you already accepted. Try BackOut endpoint")
		}
		common.Remove(&gs.Users, username)
		common.Remove(&gs.AliveUsers, username)
		gs.MaxUsers = len(gs.Users)

		err := user.UpdateUser(ctx, username, removePendingGame(ctx, gameStateID))
		if err != nil {
			log.Errorf(ctx, "We failed to update a user %s to remove pending game", username)
			return err
		}

		/* If the game now only has 1 user, remove the game from that last user */
		if gs.MaxUsers == 1 {
			err := user.UpdateUser(ctx, gs.Users[0], removePendingGame(ctx, gameStateID))
			if err != nil {
				log.Errorf(ctx, "We failed to update a user %s to remove pending game", gs.Users[0])
				return err
			}
			/*
				TODO: Not sure if we also want to delete the GameState entity
				Could be used for looking back at a history of sent invites :shrug:
			*/
		} else if gs.MaxUsers == len(gs.ReadyUsers) {
			/* oppositely, the user declining could kickoff the game */
			/* If all the users are now considered ready */
			gs.UsersTurn = gs.AliveUsers[0]

			err = CreateTask(ctx, gs.ID, gs.UsersTurn, 0, gs.ForfeitTime)
			if err != nil {
				log.Errorf(ctx, "Failed to create task: %v", err)
				// not a huge deal so just continue on
			}

			if gs.IsPublic {
				for i := 0; i < len(gs.ReadyUsers); i++ {
					err := user.UpdateUser(ctx, gs.Users[i], updatePublicGameToActive(ctx, gs.ID))
					if err != nil {
						log.Errorf(ctx, "We failed to update a user %s to have an active game", gs.Users[i])
						return err
					}
				}
			} else {
				for i := 0; i < len(gs.ReadyUsers); i++ {
					err := user.UpdateUser(ctx, gs.Users[i], updatePrivateGameToActive(ctx, gs.ID))
					if err != nil {
						log.Errorf(ctx, "We failed to update a user %s to have an active game", gs.Users[i])
						return err
					}
				}
			}
		}

		return nil
	}
}

// BackOutGame will back a user out of a game that they accepted but not readied to
func BackOutGame(ctx context.Context, username string, gameStateID string) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Backout Game failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(gameStateID)
	if err != nil {
		log.Errorf(ctx, "Backout Game failed: gameStateID is required")
		return errors.New("gameStateID is required")
	}

	err = gamestate.UpdateGameState(ctx, gameStateID, backOutGame(ctx, username, gameStateID))
	if err != nil {
		return err
	}

	return nil
}

func backOutGame(ctx context.Context, username, gameStateID string) gamestate.UpdateGameStateFunc {

	return func(ctx context.Context, gs *gamestate.GameState) error {
		if !gs.IsPublic {
			log.Errorf(ctx, "user: %s, tried to BackOut of a private game", username)
			return errors.New("Cannot BackOut of private game. (did you mean Decline?)")
		}
		if gs.CreatedBy == username {
			log.Errorf(ctx, "user: %s, tried to BackOut of game they created", username)
			return errors.New("Cannot BackOut of game you created")
		}
		if common.Contains(gs.ReadyUsers, username) {
			log.Errorf(ctx, "user: %s, tried to BackOut of game they are readied in", username)
			return errors.New("Cannot BackOut of game you are 'Ready' in")
		}
		if !common.Remove(&gs.AcceptedUsers, username) {
			log.Errorf(ctx, "user: %s, tried to BackOut of game they not 'Accepted' in", username)
			return errors.New("Cannot BackOut of game you are not 'Accepted' in")
		}
		common.Remove(&gs.AliveUsers, username)
		common.Remove(&gs.Users, username)
		gs.SpotsAvailable = gs.MaxUsers - len(gs.AcceptedUsers)

		err := user.UpdateUser(ctx, username, removePendingGame(ctx, gameStateID))
		if err != nil {
			log.Errorf(ctx, "We failed to update a user %s to remove pending game", username)
			return err
		}

		return nil
	}
}

// ForfeitGame will remove the user from the game counting as a loss
func ForfeitGame(ctx context.Context, username string, gameStateID string, reason gamestate.LoseReason) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Forfeit failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(gameStateID)
	if err != nil {
		log.Errorf(ctx, "Forfeit failed: gameStateID is required")
		return errors.New("gameStateID is required")
	}

	err = gamestate.UpdateGameState(ctx, gameStateID, forfeitGame(ctx, username, gameStateID, reason))
	if err != nil {
		return err
	}

	return nil
}

func forfeitGame(ctx context.Context, username, gameStateID string, reason gamestate.LoseReason) gamestate.UpdateGameStateFunc {

	return func(ctx context.Context, gs *gamestate.GameState) error {
		if !common.Contains(gs.ReadyUsers, username) {
			log.Errorf(ctx, "user: %s, tried to Forfeit a game they arent readied in", username)
			return errors.New("Cannot Forfeit a game you are not 'Ready' in")
		}

		if gs.UsersTurn == username {
			gs.UsersTurn = getNextUsersTurn(gs.UsersTurn, gs.AliveUsers, []string{})
		}
		if !common.Remove(&gs.AliveUsers, username) {
			log.Errorf(ctx, "user: %s, tried to Forfeit a game they are not alive in", username)
			return errors.New("Cannot Forfeit a game you are not alive in")
		}

		/* remove all cards and units that belonged to that user */
		common.RemoveAllUnits(&gs.Units, username)
		common.RemoveAllUnits(&gs.Generals, username)
		common.RemoveCards(&gs.Cards, username)

		/* Add the action that says they forfeited */
		gs.Actions = append(gs.Actions, gamestate.Action{Username: username, ActionType: gamestate.Forfeit})
		gs.LoseReasons = append(gs.LoseReasons, gamestate.Lose{Username: username, Reason: reason})
		gs.TurnCount++

		/* Store a record of this turn */
		err := gamestate.SaveGSAncestor(ctx, gs.ID, gs.TurnCount, gs.Units, gs.Generals, gs.ActiveEffects, gs.Actions)
		if err != nil {
			/* We failed to save this turn but I don't think it should block anything... */
		}

		err = user.UpdateUser(ctx, username, updateActiveGameToComplete(ctx, gameStateID))
		if err != nil {
			log.Errorf(ctx, "We failed to update a user %s to remove active game", username)
			return err
		}
		/* If there is now only one other person left */
		if len(gs.AliveUsers) == 1 {
			/* Update that user to have the game as a completed game */
			err := user.UpdateUser(ctx, gs.AcceptedUsers[0], updateActiveGameToComplete(ctx, gameStateID))
			if err != nil {
				log.Errorf(ctx, "We failed to update a user %s to remove active game", username)
				return err
			}
			gs.UsersTurn = ""
			gs.IsComplete = true
		} else {
			err := CreateTask(ctx, gs.ID, gs.UsersTurn, len(gs.Actions), gs.ForfeitTime)
			if err != nil {
				log.Errorf(ctx, "Failed to create task: %v", err)
				// not a huge deal so just continue on
			}
		}

		return nil
	}
}

// Helper function to editing the User model
func removePendingGame(ctx context.Context, gameID string) user.UpdateUserFunc {
	return func(ctx context.Context, u *user.User) error {
		if !common.Remove(&u.PendingPublicGames, gameID) {
			if !common.Remove(&u.PendingPrivateGames, gameID) {
				log.Criticalf(ctx, "user: %s, managed to remove themselves as accepted from a gameState without that id on their model", u.Username)
			}
		}
		return nil
	}
}

// Helper function to editing the User model
func updateActiveGameToComplete(ctx context.Context, gameID string) user.UpdateUserFunc {
	return func(ctx context.Context, u *user.User) error {
		common.Remove(&u.ActiveGames, gameID)
		u.CompletedGames = append(u.CompletedGames, gameID)
		return nil
	}
}

// Helper function to editing the User model
func updatePublicGameToActive(ctx context.Context, gameID string) user.UpdateUserFunc {
	return func(ctx context.Context, u *user.User) error {
		common.Remove(&u.PendingPublicGames, gameID)
		u.ActiveGames = append(u.ActiveGames, gameID)
		return nil
	}
}

// Helper function to editing the User model
func updatePrivateGameToActive(ctx context.Context, gameID string) user.UpdateUserFunc {
	return func(ctx context.Context, u *user.User) error {
		if !common.Remove(&u.PendingPrivateGames, gameID) {
			log.Criticalf(ctx, "Somehow user %s accepted private game %s without having it on their model", u.Username, gameID)
		}
		u.ActiveGames = append(u.ActiveGames, gameID)
		return nil
	}
}

// Helper function to editing the User model
func appendToPrivateGames(ctx context.Context, gameID string) user.UpdateUserFunc {
	return func(ctx context.Context, u *user.User) error {
		u.PendingPrivateGames = append(u.PendingPrivateGames, gameID)
		return nil
	}
}

// Helper function to editing the User model
func appendToPublicGames(ctx context.Context, gameID string) user.UpdateUserFunc {
	return func(ctx context.Context, u *user.User) error {
		u.PendingPublicGames = append(u.PendingPublicGames, gameID)
		return nil
	}
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

// GetGSAncestor will get the GameState for a particular gameID at the particular turnNumber
func GetGSAncestor(ctx context.Context, gameStateID string, turnCount int) (*gamestate.GSAncestor, error) {

	err := common.StringNotEmpty(gameStateID)
	if err != nil {
		log.Errorf(ctx, "Get Old Game failed: gameStateID is required")
		return nil, errors.New("gameStateID is required")
	}
	if turnCount <= 0 {
		log.Errorf(ctx, "Get Old Game failed: turnCount must be > 0")
		return nil, errors.New("turnCount invalid (<= 0)")
	}

	return gamestate.GetGSAncestor(ctx, gameStateID, turnCount)
}

// GetGameStateMulti will get all the GameStates for the gameIDs and user
func GetGameStateMulti(ctx context.Context, gameStateIDs []string, username string) ([]gamestate.GameState, error) {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Get Game Multi failed: username is required")
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
		log.Errorf(ctx, "Get Public Games failed: limit invalid")
		return nil, errors.New("Invalid limit given")
	} else if limit == 0 {
		limit = 100
	}

	return gamestate.GetPublicGamesSummary(ctx, limit)
}

// GetCompletedGames will query and return all finished public games
func GetCompletedGames(ctx context.Context, username string, limit int) ([]gamestate.GameState, error) {
	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Get Completed Games failed: username is required")
		return nil, errors.New("username is required")
	}
	if limit < 0 {
		log.Errorf(ctx, "Get Completed Games failed: limit invalid")
		return nil, errors.New("Invalid limit given")
	} else if limit == 0 {
		limit = 100
	}

	return gamestate.GetCompletedGames(ctx, limit)
}

// ReadyUnits is used to place your units on the field and be provided cards
func ReadyUnits(ctx context.Context, username, gameID string, units []gamestate.Unit, general gamestate.Unit) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Ready Units failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(gameID)
	if err != nil {
		log.Errorf(ctx, "Ready Units failed: gameID is required")
		return errors.New("gameID is required")
	}
	if len(units) <= 0 {
		log.Errorf(ctx, "Ready Units failed: units are required")
		return errors.New("units are required")
	}
	if len(general.Owner) <= 0 {
		log.Errorf(ctx, "Ready Units failed: general is required")
		return errors.New("general is required")
	}

	return gamestate.UpdateGameState(ctx, gameID, readyUnits(username, units, general))
}

// readyUnits is a helper function to assigning a player units and making them considered "Ready"
func readyUnits(username string, units []gamestate.Unit, general gamestate.Unit) gamestate.UpdateGameStateFunc {

	return func(ctx context.Context, gs *gamestate.GameState) error {
		if !common.Contains(gs.AcceptedUsers, username) {
			log.Errorf(ctx, "User %s tried to ready up in game %s that they arn't accepted in", username, gs.ID)
			return errors.New("Cannot ReadyUnits in a game you haven't accepted")
		}
		if common.Contains(gs.ReadyUsers, username) {
			log.Errorf(ctx, "User %s tried to ready up in game %s that they've already readied up in", username, gs.ID)
			return errors.New("Cannot ReadyUnits more than once")
		}
		/* make user considered "Ready"*/
		gs.ReadyUsers = append(gs.ReadyUsers, username)
		/* Add Provided Units */
		gs.Units = append(gs.Units, units...)
		gs.InitUnits = append(gs.InitUnits, units...)
		/* add the provided general */
		gs.Generals = append(gs.Generals, general)
		gs.InitGenerals = append(gs.Generals, general)

		/* If it's the last person that needed to ready up */
		if gs.MaxUsers == len(gs.ReadyUsers) {
			gs.UsersTurn = gs.AliveUsers[0]

			err := CreateTask(ctx, gs.ID, gs.UsersTurn, 0, gs.ForfeitTime)
			if err != nil {
				log.Errorf(ctx, "Failed to create task: %v", err)
				// not a huge deal so just continue on
			}

			if gs.IsPublic {
				for i := 0; i < len(gs.ReadyUsers); i++ {
					err := user.UpdateUser(ctx, gs.Users[i], updatePublicGameToActive(ctx, gs.ID))
					if err != nil {
						log.Errorf(ctx, "We failed to update a user %s to have an active game", gs.Users[i])
						return err
					}
				}
			} else {
				for i := 0; i < len(gs.ReadyUsers); i++ {
					err := user.UpdateUser(ctx, gs.Users[i], updatePrivateGameToActive(ctx, gs.ID))
					if err != nil {
						log.Errorf(ctx, "We failed to update a user %s to have an active game", gs.Users[i])
						return err
					}
				}
			}
		}

		return nil
	}

}

// MakeMove is used to initiate a turn in the game
func MakeMove(ctx context.Context, username, gameID string, units []gamestate.Unit, generals []gamestate.Unit, cards []gamestate.Cards, activeEffects []gamestate.Effect, actions []gamestate.Action, killedUsers []string) error {

	err := common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Make Move failed: username is required")
		return errors.New("username is required")
	}
	err = common.StringNotEmpty(gameID)
	if err != nil {
		log.Errorf(ctx, "Make Move failed: gameID is required")
		return errors.New("gameID is required")
	}

	return gamestate.UpdateGameState(ctx, gameID, makeMove(username, units, generals, cards, activeEffects, actions, killedUsers))
}

// makeMove is a helper function to update the GameState for doing a turn
func makeMove(username string, units []gamestate.Unit, generals []gamestate.Unit, cards []gamestate.Cards, activeEffects []gamestate.Effect, actions []gamestate.Action, killedUsers []string) gamestate.UpdateGameStateFunc {

	return func(ctx context.Context, gs *gamestate.GameState) error {

		/* This is a person who is making their turn */
		if gs.UsersTurn != username {
			log.Errorf(ctx, "Make Move failed: username %s does not match players turn %s", username, gs.UsersTurn)
			return errors.New("It's not your turn")
		}

		/* add to the thing of actions */
		if actions != nil {
			gs.Actions = append(gs.Actions, actions...)
		}

		gs.UsersTurn = getNextUsersTurn(gs.UsersTurn, gs.AliveUsers, killedUsers)

		/* remove killed users from list of alive users */
		if killedUsers != nil {
			for _, v := range killedUsers {
				common.Remove(&gs.AliveUsers, v)
			}
		}

		if len(gs.AliveUsers) == 1 {
			// A user has won!
			gs.IsComplete = true
			gs.UsersTurn = ""
		} else {
			err := CreateTask(ctx, gs.ID, gs.UsersTurn, len(gs.Actions), gs.ForfeitTime)
			if err != nil {
				log.Errorf(ctx, "Failed to create task: %v", err)
				// not a huge deal so just continue on
			}
		}

		/* For the first move of every player the cards will not have an id, must assign that */
		for i := 0; i < len(cards); i++ {
			if cards[i].ID == "" {
				cards[i].ID = common.GetRandomID()
				gs.CardIDs = append(gs.CardIDs, cards[i].ID)
			}
		}

		/* assign the new units, generals, and cards of the gamestate */
		gs.Units = units
		gs.Cards = cards // The client will always give the server ALL of the cards on MakeMove
		gs.Generals = generals
		gs.ActiveEffects = activeEffects
		gs.TurnCount++

		/* Store a record of this turn */
		err := gamestate.SaveGSAncestor(ctx, gs.ID, gs.TurnCount, gs.Units, gs.Generals, gs.ActiveEffects, gs.Actions)
		if err != nil {
			/* We failed to save this turn but I don't think it should block anything... */
		}

		return nil
	}
}

func getNextUsersTurn(usersTurn string, aliveUsers []string, killedUsers []string) string {
	/* Choose the next user that should go */
	tempAliveUsers := make([]string, len(aliveUsers))
	copy(tempAliveUsers, aliveUsers)
	if killedUsers != nil {
		for _, v := range killedUsers {
			if usersTurn != v {
				common.Remove(&tempAliveUsers, v)
			}
		}
	}
	if len(tempAliveUsers) <= 1 {
		return ""
	}

	for i, v := range tempAliveUsers {
		if v == usersTurn {
			if i+1 >= len(tempAliveUsers) {
				return tempAliveUsers[0]
			}
			return tempAliveUsers[i+1]
		}
	}
	return ""
}
