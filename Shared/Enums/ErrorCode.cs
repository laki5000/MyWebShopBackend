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
        TITLE_ALREADY_EXISTS, //new

        // Authentication and authorization errors
        INVALID_USERNAME_OR_PASSWORD,
        INVALID_PASSWORD,
        INVALID_TOKEN,
        FORBIDDEN,
        ROLE_ASSIGNMENT_FAILED,

        // User-related errors
        USER_NOT_FOUND,
        USER_CREATION_FAILED,
        USER_UPDATE_FAILED,
        USER_DELETE_FAILED,

        // Category-related errors
        CATEGORY_NOT_FOUND,

        // Product-related errors
        PRODUCT_NOT_FOUND, //new

    }
}
