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
	http.HandleFunc("/SendGameInvite", handleSendGameInvite)
	http.HandleFunc("/AcceptGameInvite", handleAcceptGameInvite)
	http.HandleFunc("/GetGameState", handleGetGameState)
	http.HandleFunc("/GetGameStateMulti", handleGetGameState)

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

func handleSendGameInvite(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var gr request.GameRequest
	err = decoder.Decode(&gr)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = GameInvite(ctx, username, gr.OpponentUsernames, gr.Board)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleAcceptGameInvite(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var ar request.AcceptRequest
	err = decoder.Decode(&ar)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = AcceptGameInvite(ctx, username, ar.GameID)
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
