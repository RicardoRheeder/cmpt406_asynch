package request

// CreateUserRequest is a struct to handle the request of creating a user
type CreateUserRequest struct {
	Username string `json:"username"`
	Password string `json:"password"`
}
