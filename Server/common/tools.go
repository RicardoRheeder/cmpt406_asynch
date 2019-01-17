package common

import (
	"errors"
)

// StringNotEmpty validates that the string is not empty
func StringNotEmpty(input string) error {
	if input == "" {
		return errors.New("String Empty")
	}
	return nil
}

// StringSliceNotEmpty validates that the slize is not empty
func StringSliceNotEmpty(input []string) error {
	if len(input) <= 0 {
		return errors.New("Slice Empty")
	}
	return nil
}
