
using Shared.Enums;

namespace Shared.Dtos
{
    public class ApiResponseDto
    {
        public bool IsSuccess { get; set; }
        public ErrorCodeEnum? ErrorCode { get; set; }

        public ApiResponseDto(bool isSuccess, ErrorCodeEnum? errorCode = null)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
        }

        public static ApiResponseDto Success() => new ApiResponseDto(true);
        public static ApiResponseDto Fail(ErrorCodeEnum errorCode) => new ApiResponseDto(false, errorCode);
    }

    public class ApiResponseDto<T> : ApiResponseDto
    {
        public T? Data { get; set; }

        public ApiResponseDto(bool isSuccess, T? data = default, ErrorCodeEnum? errorCode = null)
            : base(isSuccess, errorCode)
        {
            Data = data;
        }

        public static ApiResponseDto<T> Success(T data) => new ApiResponseDto<T>(true, data);
        public static new ApiResponseDto<T> Fail(ErrorCodeEnum errorCode) => new ApiResponseDto<T>(false, default, errorCode);
    }
}
