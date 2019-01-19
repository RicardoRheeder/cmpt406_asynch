package main

import (
	"Projects/cmpt406_asynch/Server/request"
	"encoding/json"
	"net/http"

	"google.golang.org/appengine"
)

func main() {
	http.HandleFunc("/CreateUser", handleCreateUser)
	http.HandleFunc("/GetUser", handleGetUser)

	http.HandleFunc("/AddFriend", handleAddFriend)
	http.HandleFunc("/RemoveFriend", handleRemoveFriend)

	http.HandleFunc("/CreatePrivateGame", handleCreatePrivateGame)
	http.HandleFunc("/CreatePublicGame", handleCreatePublicGame)
	http.HandleFunc("/AcceptGame", handleAcceptGame)

	http.HandleFunc("/GetGameState", handleGetGameState)
	http.HandleFunc("/GetGameStateMulti", handleGetGameStateMulti)

	appengine.Main()
}

func handleCreateUser(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	decoder := json.NewDecoder(r.Body)
	var ur request.CreateUser
	err := decoder.Decode(&ur)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = CreateUser(ctx, ur.Username, ur.Password)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleGetUser(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	user, err := GetUser(ctx, username)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	/* remove redundant data */
	user.Username = ""
	user.Password = ""

	json.NewEncoder(w).Encode(user)

	w.WriteHeader(http.StatusOK)
	return
}

func handleAddFriend(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var f request.Friend
	err = decoder.Decode(&f)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = AddFriend(ctx, username, f.Username)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleRemoveFriend(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var f request.Friend
	err = decoder.Decode(&f)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = RemoveFriend(ctx, username, f.Username)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleCreatePrivateGame(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var pg request.CreatePrivateGame
	err = decoder.Decode(&pg)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = CreatePrivateGame(ctx, username, pg.OpponentUsernames, pg.Board)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleCreatePublicGame(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var pg request.CreatePublicGame
	err = decoder.Decode(&pg)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = CreatePublicGame(ctx, username, pg.Board, pg.MaxUsers)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleAcceptGame(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var ar request.AcceptGame
	err = decoder.Decode(&ar)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = AcceptGame(ctx, username, ar.GameID)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleGetGameState(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var ggs request.GetGameState
	err = decoder.Decode(&ggs)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	gameState, err := GetGameState(ctx, ggs.GameID, username)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	json.NewEncoder(w).Encode(gameState)

	w.WriteHeader(http.StatusOK)
	return
}

func handleGetGameStateMulti(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var ggsm request.GetGameStateMulti
	err = decoder.Decode(&ggsm)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	gameStates, err := GetGameStateMulti(ctx, ggsm.GameIDs, username)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	json.NewEncoder(w).Encode(gameStates)

	w.WriteHeader(http.StatusOK)
	return
}
