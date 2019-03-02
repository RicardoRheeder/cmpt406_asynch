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

	http.HandleFunc("/AddArmyPreset", handleAddArmyPreset)
	http.HandleFunc("/RemoveArmyPreset", handleRemoveArmyPreset)

	http.HandleFunc("/CreatePrivateGame", handleCreatePrivateGame)
	http.HandleFunc("/CreatePublicGame", handleCreatePublicGame)
	http.HandleFunc("/AcceptGame", handleAcceptGame)
	http.HandleFunc("/DeclineGame", handleDeclineGame)
	http.HandleFunc("/BackOutGame", handleBackOutGame)
	http.HandleFunc("/ForfeitGame", handleForfeitGame)

	http.HandleFunc("/GetGameState", handleGetGameState)
	http.HandleFunc("/GetGameStateMulti", handleGetGameStateMulti)
	http.HandleFunc("/GetPublicGamesSummary", handleGetPublicGamesSummary)
	http.HandleFunc("/GetCompletedGames", handleGetCompletedGames)

	http.HandleFunc("/ReadyUnits", handleReadyUnits)
	http.HandleFunc("/MakeMove", handleMakeMove)

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
		return
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

func handleAddArmyPreset(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var aap request.AddArmyPreset
	err = decoder.Decode(&aap)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = AddArmyPreset(ctx, username, aap.Name, aap.Units, aap.General)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleRemoveArmyPreset(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var rap request.RemoveArmyPreset
	err = decoder.Decode(&rap)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = RemoveArmyPreset(ctx, username, rap.ArmyPresetID)
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

	err = CreatePrivateGame(ctx, username, pg.OpponentUsernames, pg.BoardID, pg.GameName, pg.TurnTime, pg.ForfeitTime)
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

	err = CreatePublicGame(ctx, username, pg.BoardID, pg.MaxUsers, pg.GameName, pg.TurnTime, pg.ForfeitTime)
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
	var ar request.OnlyGameID
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

func handleDeclineGame(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var req request.OnlyGameID
	err = decoder.Decode(&req)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = DeclineGame(ctx, username, req.GameID)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleBackOutGame(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var req request.OnlyGameID
	err = decoder.Decode(&req)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = BackOutGame(ctx, username, req.GameID)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleForfeitGame(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var req request.OnlyGameID
	err = decoder.Decode(&req)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = ForfeitGame(ctx, username, req.GameID)
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
	var ggs request.OnlyGameID
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

	response := &request.MultiStatesResponse{States: gameStates}

	json.NewEncoder(w).Encode(response)

	w.WriteHeader(http.StatusOK)
	return
}

func handleGetPublicGamesSummary(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var ol request.OnlyLimit
	err = decoder.Decode(&ol)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	publicGameStates, err := GetPublicGamesSummary(ctx, username, ol.Limit)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	response := &request.MultiStatesResponse{States: publicGameStates}

	json.NewEncoder(w).Encode(response)

	w.WriteHeader(http.StatusOK)
	return
}

func handleGetCompletedGames(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var ol request.OnlyLimit
	err = decoder.Decode(&ol)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	completedGameStates, err := GetCompletedGames(ctx, username, ol.Limit)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	response := &request.MultiStatesResponse{States: completedGameStates}

	json.NewEncoder(w).Encode(response)

	w.WriteHeader(http.StatusOK)
	return
}

func handleReadyUnits(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var ru request.ReadyUnits
	err = decoder.Decode(&ru)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = ReadyUnits(ctx, username, ru.GameID, ru.Units, ru.General, ru.Cards)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleMakeMove(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusUnauthorized)
		return
	}

	decoder := json.NewDecoder(r.Body)
	var mm request.MakeMove
	err = decoder.Decode(&mm)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	err = MakeMove(ctx, username, mm.GameID, mm.Units, mm.Generals, mm.Cards, mm.Actions, mm.KilledUsers)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	return
}
