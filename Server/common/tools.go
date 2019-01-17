package common

import (
	"errors"

	"github.com/google/uuid"
)

// StringNotEmpty validates that the string is not empty
func StringNotEmpty(input string) error {
	if input == "" {
		return errors.New("String Empty")
	}
	return nil
}

// StringSliceGreaterThanLength validates that the slice is greater than requested
func StringSliceGreaterThanLength(input []string, size int) error {
	if len(input) <= size {
		return errors.New("Slice Too Small")
	}
	return nil
}

// GetRandomID will return a uuid as a string
func GetRandomID() string {
	return uuid.New().String()
}
