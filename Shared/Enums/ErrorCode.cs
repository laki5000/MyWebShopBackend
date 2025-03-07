namespace Shared.Enums
{
    public enum ErrorCode
    {
        // General Errors
        UNKNOWN_ERROR,

        // User-related Errors
        USER_CREATION_FAILED,
        USER_UPDATE_FAILED,
        USER_DELETE_FAILED,
        USER_NOT_FOUND,
        USERNAME_ALREADY_EXISTS,
        EMAIL_ALREADY_EXISTS,
        PASSWORD_SAME_AS_OLD,

        // Authentication Errors
        USERNAME_OR_PASSWORD_IS_WRONG,
        PASSWORD_IS_WRONG,
        INVALID_TOKEN,

        // Authorization Errors
        ROLE_ASSIGNMENT_FAILED,
        FORBIDDEN
    }
}
