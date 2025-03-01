using AuthService.Interfaces.Services;
using AuthService.Models;
using AuthService.Shared.Dtos;
using AuthService.Shared.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Shared.Dtos;
using Shared.Enums;

namespace AuthService.Services
{
    public class AspNetUserServiceImpl : IAspNetUserService
    {
        private readonly ILogger<AspNetUserServiceImpl> _logger;
        private readonly IJwtService _jwtService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;

        public AspNetUserServiceImpl(
            ILogger<AspNetUserServiceImpl> logger,
            IJwtService jwtService,
            UserManager<AspNetUser> userManager,
            IMapper mapper
        )
        {
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<string>> CreateAsync(CreateAspNetUserDto createAspNetUserDto)
        {
            var existingByUserName = await _userManager.FindByNameAsync(createAspNetUserDto.UserName);
            if (existingByUserName is not null)
            {
                _logger.LogError("Username already exists: {UserName}", createAspNetUserDto.UserName);

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.USERNAME_ALREADY_EXISTS);
                return failDto;
            }

            var existingByEmail = await _userManager.FindByEmailAsync(createAspNetUserDto.Email);
            if (existingByEmail is not null)
            {
                _logger.LogError("Email already exists: {Email}", createAspNetUserDto.Email);

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.EMAIL_ALREADY_EXISTS);
                return failDto;
            }

            var entity = _mapper.Map<AspNetUser>(createAspNetUserDto);

            var createResult = await _userManager.CreateAsync(entity, createAspNetUserDto.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogError("User creation failed: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));

                return ApiResponseDto<string>.Fail(ErrorCode.USER_CREATION_FAILED);
            }

            var roleResult = await _userManager.AddToRoleAsync(entity, Role.CUSTOMER.ToString());
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Role assignment failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));

                return ApiResponseDto<string>.Fail(ErrorCode.ROLE_ASSIGNMENT_FAILED);
            }

            var successDto = ApiResponseDto<string>.Success(entity.Id);
            return successDto;
        }

        public async Task<ApiResponseDto<string>> LoginAsync(LoginAspNetUserDto loginAspNetUserDto)
        {
            var entity = await _userManager.FindByNameAsync(loginAspNetUserDto.UserName);
            if (entity is null || entity.DeletedAt is not null)
            {
                _logger.LogError("Login failed: Username {UserName} not found", loginAspNetUserDto.UserName);

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.USERNAME_OR_PASSWORD_IS_WRONG);
                return failDto;
            }

            var passwordCheckResult = await _userManager.CheckPasswordAsync(entity, loginAspNetUserDto.Password);
            if (!passwordCheckResult)
            {
                _logger.LogError("Login failed: Invalid password for user {UserName}", loginAspNetUserDto.UserName);

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.USERNAME_OR_PASSWORD_IS_WRONG);
                return failDto;
            }

            var roles = await _userManager.GetRolesAsync(entity);

            var token = _jwtService.GenerateToken(entity, roles);
            var successDto = ApiResponseDto<string>.Success(token);

            return successDto;
        }
    }
}
