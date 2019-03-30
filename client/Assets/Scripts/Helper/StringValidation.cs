using System.Text.RegularExpressions;

public static class StringValidation {
    private static readonly string CREDENTIALS_PATTERN = "^[a-zA-Z0-9_-]*$";
    public static readonly int CREDENTIALS_LOWER_LIMIT = 4;
    public static readonly int CREDENTIALS_UPPER_LIMIT = 20;

    private static readonly string GAME_NAME_PATTERN = "^[a-zA-Z0-9 _-]*$";
    public static readonly int GAME_NAME_LOWER_LIMIT = 4;
    public static readonly int GAME_NAME_UPPER_LIMIT = 20;

    private static readonly string ARMY_NAME_PATTERN = "^[a-zA-Z0-9 _-]*$";
    public static readonly int ARMY_NAME_LOWER_LIMIT = 4;
    public static readonly int ARMY_NAME_UPPER_LIMIT = 20;

    public static bool ValidateUsername(string username) {
        return ValidateString(username, CREDENTIALS_LOWER_LIMIT, CREDENTIALS_UPPER_LIMIT, CREDENTIALS_PATTERN);
    }

    public static bool ValidateUsernamePassword(string username, string password) {
        return ValidateString(username, CREDENTIALS_LOWER_LIMIT, CREDENTIALS_UPPER_LIMIT, CREDENTIALS_PATTERN) &&
            ValidateString(password, CREDENTIALS_LOWER_LIMIT, CREDENTIALS_UPPER_LIMIT, CREDENTIALS_PATTERN);
    }

    public static bool ValidateGameName(string gameName) {
        return ValidateString(gameName, GAME_NAME_LOWER_LIMIT, GAME_NAME_UPPER_LIMIT, GAME_NAME_PATTERN);
    }

    public static bool ValidateArmyName(string armyName) {
        return ValidateString(armyName, ARMY_NAME_LOWER_LIMIT, ARMY_NAME_UPPER_LIMIT, ARMY_NAME_PATTERN);
    }

    private static bool ValidateString(string s, int lowerLimit, int upperLimit, string validationPattern) {
        if (s.Length >= lowerLimit && s.Length <= upperLimit) {
            return Regex.Match(s, validationPattern).Success;
        }
        return false;
    }
}
