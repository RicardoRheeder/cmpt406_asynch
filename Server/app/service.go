package main

import (
	"Projects/cmpt406_asynch/Server/common"
	"Projects/cmpt406_asynch/Server/user"
	"context"
	"encoding/base64"
	"errors"
	"net/http"
	"strings"

	"google.golang.org/appengine/log"
)

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

// RequestGame will update both users to have
func RequestGame(ctx context.Context, username string) (*user.User, error) {
	var err error

	err = common.StringNotEmpty(username)
	if err != nil {
		log.Errorf(ctx, "Getting user failed: username is required")
		return nil, errors.New("username is required")
	}

	return user.GetUser(ctx, username)
}
