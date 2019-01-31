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

// Contains checks if a string exists in a slice of strings
func Contains(sa []string, s string) bool {
	for _, a := range sa {
		if a == s {
			return true
		}
	}
	return false
}

// Remove will delete the given string from the array
// returns true if it found it, false otherwise
func Remove(s *[]string, r string) bool {
	slice := *s
	for i, v := range slice {
		if v == r {
			*s = append(slice[:i], slice[i+1:]...)
			return true
		}
	}
	return false
}
