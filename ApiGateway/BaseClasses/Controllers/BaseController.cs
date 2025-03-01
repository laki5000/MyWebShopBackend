using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Enums;

namespace ApiGateway.BaseClasses.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private static readonly Dictionary<ErrorCode, Func<ApiResponseDto, IActionResult>> ErrorCodeHandlers = new()
        {
            { ErrorCode.USERNAME_ALREADY_EXISTS, dto => new ConflictObjectResult(dto) },
            { ErrorCode.EMAIL_ALREADY_EXISTS, dto => new ConflictObjectResult(dto) },
            { ErrorCode.USERNAME_OR_PASSWORD_IS_WRONG, dto => new UnauthorizedObjectResult(dto) }
        };

        protected static IActionResult GetObjectResult(ApiResponseDto apiResponseDto)
        {
            apiResponseDto.ErrorCode ??= ErrorCode.UNKNOWN_ERROR;

            if (ErrorCodeHandlers.TryGetValue(apiResponseDto.ErrorCode.Value, out var handler))
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
