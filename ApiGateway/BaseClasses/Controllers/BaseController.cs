using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Enums;

namespace ApiGateway.BaseClasses.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private static readonly Dictionary<ErrorCodeEnum, Func<ApiResponseDto, IActionResult>> ErrorCodeHandlers = new()
        {
            { ErrorCodeEnum.USERNAME_ALREADY_EXISTS, dto => new ConflictObjectResult(dto) },
            { ErrorCodeEnum.USERNAME_OR_PASSWORD_IS_WRONG, dto => new UnauthorizedObjectResult(dto) }
        };

        protected static IActionResult GetObjectResult(ApiResponseDto apiResponseDto)
        {
            apiResponseDto.ErrorCode ??= ErrorCodeEnum.UNKNOWN_ERROR;

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
