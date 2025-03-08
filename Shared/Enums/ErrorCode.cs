namespace Shared.Enums
{
    public enum ErrorCode
    {
        UNKNOWN_ERROR,

        // General validation errors
        NAME_ALREADY_EXISTS,
        EMAIL_ALREADY_EXISTS,
        USERNAME_ALREADY_EXISTS,
        PASSWORD_SAME_AS_OLD,

        // Authentication and authorization errors
        INVALID_USERNAME_AND_PASSWORD,
        INVALID_PASSWORD,
        INVALID_TOKEN,
        FORBIDDEN,
        ROLE_ASSIGNMENT_FAILED,

        // User-related errors
        USER_NOT_FOUND,
        USER_CREATION_FAILED,
        USER_UPDATE_FAILED,
        USER_DELETE_FAILED,

        // Entity-related errors
        CATEGORY_NOT_FOUND
    }
}
