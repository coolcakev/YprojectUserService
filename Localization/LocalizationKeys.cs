namespace YprojectUserService.Localization;

public static class LocalizationKeys
{
    public static class User
    {
        public const string NotFound = "userNotFound";
        public const string InvalidPassword = "invalidPassword";
        public const string LoginEmailRegistered = "loginEmailRegistered";
    }

    public static class Recovery
    {
        public const string CodeNotFound = "recoveryCodeNotFound";
        public const string ProviderIncorrect = "recoveryCodeProviderIncorrect";
    }

    public static class Common
    {
        public const string IncorrectData = "incorrectData";
    }
}