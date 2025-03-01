﻿using ApiGateway.BaseClasses.Controllers;
using ApiGateway.Configurations;
using ApiGateway.Constants;
using ApiGateway.Interfaces.Grpc;
using AuthService.Shared.Dtos;
using AuthService.Shared.Interfaces.Communication.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Configurations;
using Shared.Dtos;
using Shared.Enums;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/v1/gw_auth/user")]
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly IAuthServiceUserClientAdapter _authServiceUserClientAdapter;
        private readonly IUserServiceUserClientAdapter _userServiceUserClientAdapter;
        private readonly IAuthServiceKafkaProducer _authServiceKafkaProducer;

        public AuthController(
            ILogger<AuthController> logger,
            IOptions<AppSettings> appSettings,
            IAuthServiceUserClientAdapter authServiceUserClientAdapter,
            IUserServiceUserClientAdapter userServiceUserClientAdapter,
            IAuthServiceKafkaProducer authServiceKafkaProducer)
        {
            _logger = logger;
            _jwtSettings = appSettings.Value.JwtSettings;
            _authServiceUserClientAdapter = authServiceUserClientAdapter;
            _userServiceUserClientAdapter = userServiceUserClientAdapter;
            _authServiceKafkaProducer = authServiceKafkaProducer;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateAspNetUserAndUserDto createAspNetUserAndUserDto)
        {
            var authServiceResult = await _authServiceUserClientAdapter.CreateAsync(createAspNetUserAndUserDto.AspNetUser);
            if (!authServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to create aspNetUser for {Username}. Error: {Error}", createAspNetUserAndUserDto.AspNetUser.UserName, authServiceResult.ErrorCode);
                var result = GetObjectResult(authServiceResult);
                return result;
            }

            var userId = authServiceResult.Data ?? throw new ArgumentNullException(nameof(authServiceResult.Data));
            try
            {
                createAspNetUserAndUserDto.User.Id = userId;
                var userServiceResult = await _userServiceUserClientAdapter.CreateAsync(createAspNetUserAndUserDto.User);

                if (!userServiceResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to create user data for user ID {UserId}. Error: {Error}", userId, userServiceResult.ErrorCode);
                    await _authServiceKafkaProducer.ForceDeleteAspNetUserAsync(userId);

                    var result = GetObjectResult(userServiceResult);
                    return result;
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An unexpected error occurred during user registration.");

                await _authServiceKafkaProducer.ForceDeleteAspNetUserAsync(userId);
                var response = ApiResponseDto.Fail(ErrorCodeEnum.UNKNOWN_ERROR);

                return new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAspNetUserDto loginAspNetUserDto)
        {
            var authServiceResult = await _authServiceUserClientAdapter.LoginAsync(loginAspNetUserDto);
            if (!authServiceResult.IsSuccess) {
                _logger.LogWarning("Login failed for user {UserName}. Error: {Error}", loginAspNetUserDto.UserName, authServiceResult.ErrorCode);
                var result = GetObjectResult(authServiceResult);
                return result;
            }

            var jwt = authServiceResult.Data ?? throw new ArgumentNullException(nameof(authServiceResult.Data));
            var expiryMinutes = _jwtSettings.ExpiryMinutes;
            Response.Cookies.Append(AuthConstants.ACCESS_TOKEN_COOKIE_NAME, jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
            });

            return Ok(authServiceResult);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(AuthConstants.ACCESS_TOKEN_COOKIE_NAME);

            return NoContent();
        }
    }
}
