using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Enums;

namespace ApiGateway.BaseClasses.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private static readonly Dictionary<ErrorCode, Func<ApiResponseDto, IActionResult>> ErrorCodeHandlers = new()
        {
            { ErrorCode.UNKNOWN_ERROR, dto => new ObjectResult(dto) { StatusCode = StatusCodes.Status500InternalServerError } },

            // General validation errors
            { ErrorCode.NAME_ALREADY_EXISTS, dto => new ConflictObjectResult(dto) },
            { ErrorCode.EMAIL_ALREADY_EXISTS, dto => new ConflictObjectResult(dto) },
            { ErrorCode.USERNAME_ALREADY_EXISTS, dto => new ConflictObjectResult(dto) },
            { ErrorCode.PASSWORD_SAME_AS_OLD, dto => new ConflictObjectResult(dto) },
            { ErrorCode.TITLE_ALREADY_EXISTS, dto => new ConflictObjectResult(dto) },
            { ErrorCode.NOT_MODIFIED, dto => new BadRequestObjectResult(dto) },

            // Authentication and authorization errors
            { ErrorCode.INVALID_USERNAME_OR_PASSWORD, dto => new UnauthorizedObjectResult(dto) },
            { ErrorCode.INVALID_PASSWORD, dto => new UnauthorizedObjectResult(dto) },
            { ErrorCode.INVALID_TOKEN, dto => new UnauthorizedObjectResult(dto) },
            { ErrorCode.FORBIDDEN, dto => new ObjectResult(dto) { StatusCode = StatusCodes.Status403Forbidden } },
            { ErrorCode.ROLE_ASSIGNMENT_FAILED, dto => new ObjectResult(dto) { StatusCode = StatusCodes.Status500InternalServerError } },

            // User-related errors
            { ErrorCode.USER_NOT_FOUND, dto => new NotFoundObjectResult(dto) },
            { ErrorCode.USER_CREATION_FAILED, dto => new ObjectResult(dto) { StatusCode = StatusCodes.Status500InternalServerError } },
            { ErrorCode.USER_UPDATE_FAILED, dto => new ObjectResult(dto) { StatusCode = StatusCodes.Status500InternalServerError } },
            { ErrorCode.USER_DELETE_FAILED, dto => new ObjectResult(dto) { StatusCode = StatusCodes.Status500InternalServerError } },

            // Category-related errors
            { ErrorCode.CATEGORY_NOT_FOUND, dto => new NotFoundObjectResult(dto) },

            // Product-related errors
            { ErrorCode.PRODUCT_NOT_FOUND, dto => new NotFoundObjectResult(dto) },
            { ErrorCode.NOT_OWNER_OF_PRODUCT, dto => new ObjectResult(dto) { StatusCode = StatusCodes.Status403Forbidden } }
        };

        protected static IActionResult GetObjectResult(ApiResponseDto apiResponseDto)
        {
            if (ErrorCodeHandlers.TryGetValue(apiResponseDto.ErrorCode!.Value, out var handler))
            {
                return handler(apiResponseDto);
            }

            return new ObjectResult(apiResponseDto)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
