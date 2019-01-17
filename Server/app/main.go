// Copyright 2018 Google Inc. All rights reserved.
// Use of this source code is governed by the Apache 2.0
// license that can be found in the LICENSE file.

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
	http.HandleFunc("/RequestGame", handleRequestGame)

	appengine.Main()
}

func handleCreateUser(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	decoder := json.NewDecoder(r.Body)
	var ur request.CreateUserRequest
	err := decoder.Decode(&ur)
	if err != nil {
		http.Error(w, "Could not decode json body", http.StatusNotFound)
		return
	}

	err = CreateUser(ctx, ur.Username, ur.Password)
	if err != nil {
		http.Error(w, "Creating User Failed", http.StatusInternalServerError)
	}

	w.WriteHeader(http.StatusOK)
	return
}

func handleGetUser(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, "Not Authorized with Basic Auth", http.StatusUnauthorized)
		return
	}

	user, err := GetUser(ctx, username)
	if err != nil {
		http.Error(w, "Getting User Failed", http.StatusInternalServerError)
		return
	}
	json.NewEncoder(w).Encode(user)

	w.WriteHeader(http.StatusOK)
	return
}

func handleRequestGame(w http.ResponseWriter, r *http.Request) {
	ctx := appengine.NewContext(r)

	username, err := ValidateAuth(ctx, r)
	if err != nil {
		http.Error(w, "Not Authorized with Basic Auth", http.StatusUnauthorized)
		return
	}

	user, err := GetUser(ctx, username)
	if err != nil {
		http.Error(w, "Getting User Failed", http.StatusInternalServerError)
	}
	json.NewEncoder(w).Encode(user)

	w.WriteHeader(http.StatusOK)
	return
}
