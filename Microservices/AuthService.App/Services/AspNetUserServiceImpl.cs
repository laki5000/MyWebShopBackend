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
                _logger.LogError("User creation failed: Username {Username} already exists", createAspNetUserDto.UserName);

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.USERNAME_ALREADY_EXISTS);
                return failDto;
            }

            var existingByEmail = await _userManager.FindByEmailAsync(createAspNetUserDto.Email);
            if (existingByEmail is not null)
            {
                _logger.LogError("User creation failed: Email {Email} already exists", createAspNetUserDto.Email);


                var failDto = ApiResponseDto<string>.Fail(ErrorCode.EMAIL_ALREADY_EXISTS);
                return failDto;
            }

            var entity = _mapper.Map<AspNetUser>(createAspNetUserDto);
            entity.Status = ObjectStatus.PENDING;

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
            if (entity is null || entity.Status is not ObjectStatus.CREATED)
            {
                _logger.LogError("Login failed: Username {UserName} not found", loginAspNetUserDto.UserName);

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.INVALID_USERNAME_AND_PASSWORD);
                return failDto;
            }

            var passwordCheckResult = await _userManager.CheckPasswordAsync(entity, loginAspNetUserDto.Password);
            if (!passwordCheckResult)
            {
                _logger.LogError("Login failed: Invalid password for user {UserName}", loginAspNetUserDto.UserName);

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.INVALID_USERNAME_AND_PASSWORD);
                return failDto;
            }

            var roles = await _userManager.GetRolesAsync(entity);

            var token = _jwtService.GenerateToken(entity, roles);
            var successDto = ApiResponseDto<string>.Success(token);

            return successDto;
        }

        public async Task<ApiResponseDto> DeleteAsync(string userId, bool forceDelete = false)
        {
            var entity = await FindByIdAsync(userId);
            if (entity is null)
            {
                _logger.LogError("Delete failed: User not found with {Id}", userId);
                var failDto = ApiResponseDto.Fail(ErrorCode.USER_NOT_FOUND);
                return failDto;
            }

            if (forceDelete)
            {
                var deleteResult = await _userManager.DeleteAsync(entity);
                if (!deleteResult.Succeeded)
                {
                    _logger.LogError("Delete failed: {Errors}", string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
                    var failDto = ApiResponseDto.Fail(ErrorCode.USER_DELETE_FAILED);
                    return failDto;
                }
            }
            else
            {
                entity.DeletedAt = DateTime.UtcNow;
                entity.Status = ObjectStatus.DELETED;
            }

            var successDto = ApiResponseDto.Success();
            return successDto;
        }

        public async Task CompleteCreationAsync(string userId)
        {
            var entity = await FindByIdAsync(userId);
            if (entity is null)
            {
                _logger.LogError("Complete creation failed: User not found with {Id}", userId);
                return;
            }

            entity.Status = ObjectStatus.CREATED;
            entity.UpdatedAt = DateTime.UtcNow;
            var updateResult = await _userManager.UpdateAsync(entity);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Complete creation failed: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }
        }

        public async Task<ApiResponseDto> ChangePasswordAsync(ChangeAspNetUserPasswordDto changeAspNetUserPasswordDto)
        {
            var entity = await FindByIdAsync(changeAspNetUserPasswordDto.UserId!);
            if (entity is null)
            {
                _logger.LogError("Password change failed: User not found with {Id}", changeAspNetUserPasswordDto.UserId);
                var failDto = ApiResponseDto.Fail(ErrorCode.USER_NOT_FOUND);
                return failDto;
            }

            var passwordCheckResult = await _userManager.CheckPasswordAsync(entity, changeAspNetUserPasswordDto.OldPassword);
            if (!passwordCheckResult)
            {
                _logger.LogError("Password change failed: Invalid password for user {UserName}", entity.UserName);

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.INVALID_PASSWORD);
                return failDto;
            }

            if (changeAspNetUserPasswordDto.OldPassword == changeAspNetUserPasswordDto.NewPassword)
            {
                _logger.LogError("Password change failed: The new password cannot be the same as the old password");

                var failDto = ApiResponseDto<string>.Fail(ErrorCode.PASSWORD_SAME_AS_OLD);
                return failDto;
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(entity, changeAspNetUserPasswordDto.OldPassword, changeAspNetUserPasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                _logger.LogError("Password change failed: {Errors}", string.Join(", ", changePasswordResult.Errors.Select(e => e.Description)));
                var failDto = ApiResponseDto.Fail(ErrorCode.USER_UPDATE_FAILED);
                return failDto;
            }

            entity.UpdatedAt = DateTime.UtcNow;
            var updateResult = await _userManager.UpdateAsync(entity);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Password change failed: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                var failDto = ApiResponseDto.Fail(ErrorCode.USER_UPDATE_FAILED);
                return failDto;
            }

            var successDto = ApiResponseDto.Success();
            return successDto;
        }

        private async Task<AspNetUser?> FindByIdAsync(string userId)
        {
            var entity = await _userManager.FindByIdAsync(userId);
            if (entity is null || entity.Status is ObjectStatus.DELETED)
            {
                return null;
            }
            return entity;
        }
    }
}
