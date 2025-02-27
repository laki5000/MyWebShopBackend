﻿using ApiGateway.BaseClasses.Controllers;
using ApiGateway.Configurations;
using ApiGateway.Constants;
using ApiGateway.Interfaces.Grpc;
using Auth;
using AuthService.Shared.Dtos;
using AuthService.Shared.Interfaces.Communication.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly JwtSettings _jwtSettings;

        private readonly IAuthServiceUserClientAdapter _authServiceUserClientAdapter;
        private readonly IUserServiceUserClientAdapter _userServiceUserClientAdapter;

        private readonly IAuthServiceKafkaProducer _authServiceKafkaProducer;

        public AuthController(
            IOptions<AppSettings> appSettings,
            IAuthServiceUserClientAdapter authServiceUserClientAdapter,
            IUserServiceUserClientAdapter userServiceUserClientAdapter,
            IAuthServiceKafkaProducer authServiceKafkaProducer)
        {            _jwtSettings = appSettings.Value.JwtSettings
                ?? throw new ArgumentNullException(nameof(appSettings.Value.JwtSettings));
            _authServiceUserClientAdapter = authServiceUserClientAdapter
                ?? throw new ArgumentException(nameof(authServiceUserClientAdapter));
            _userServiceUserClientAdapter = userServiceUserClientAdapter
                ?? throw new ArgumentException(nameof(userServiceUserClientAdapter));
            _authServiceKafkaProducer = authServiceKafkaProducer
                ?? throw new ArgumentException(nameof(authServiceKafkaProducer));
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateAspNetUserAndUserDto createAspNetUserAndUserDto)
        {
            var authServiceResult = await _authServiceUserClientAdapter.CreateAsync(createAspNetUserAndUserDto.AspNetUser);
            
            if (!authServiceResult.IsSuccess)
            {
                var result = GetObjectResult(authServiceResult);

                return result;
            }

            var userId = authServiceResult.Data?.Id ?? throw new ArgumentNullException(nameof(authServiceResult.Data.Id));

            try
            {
                createAspNetUserAndUserDto.User.Id = userId;

                var userServiceResult = await _userServiceUserClientAdapter.CreateAsync(createAspNetUserAndUserDto.User);

                if (!userServiceResult.IsSuccess)
                {
                    await _authServiceKafkaProducer.ForceDeleteAspNetUserAsync(userId);

                    var result = GetObjectResult(userServiceResult);

                    return result;
                }
            }
            catch (Exception ex) 
            {
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
