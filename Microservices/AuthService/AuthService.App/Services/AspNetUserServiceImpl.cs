﻿using AuthService.Interfaces.Repositories;
using AuthService.Interfaces.Services;
using AuthService.Models;
using AuthService.Shared.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Shared.Dtos;
using Shared.Enums;

namespace AuthService.Services
{
    public class AspNetUserServiceImpl : IAspNetUserService
    {
        private readonly IJwtService _jwtService;

        private readonly IAspNetUserRepository _aspNetUserRepository;

        private readonly IMapper _mapper;

        private readonly IPasswordHasher<AspNetUser> _passwordHasher;
        

        public AspNetUserServiceImpl(IJwtService jwtService, IAspNetUserRepository aspNetUserRepository, IMapper mapper, IPasswordHasher<AspNetUser> passwordHasher)
        {
            _jwtService = jwtService;
            _aspNetUserRepository = aspNetUserRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<ApiResponseDto<AspNetUserDto>> CreateAsync(CreateAspNetUserDto createAspNetUserDto)
        {
            var normalizedUsername = createAspNetUserDto.UserName.Normalize();
            var existsByUserName = await _aspNetUserRepository.ExistsByNormalizedUserNameAsync(normalizedUsername);

            if (existsByUserName)
            {
                var failDto = ApiResponseDto<AspNetUserDto>.Fail(ErrorCodeEnum.USERNAME_ALREADY_EXISTS);

                return failDto;

            }

            var entity = _mapper.Map<AspNetUser>(createAspNetUserDto);

            entity.PasswordHash = _passwordHasher.HashPassword(entity, createAspNetUserDto.Password);
            entity.NormalizedUserName = normalizedUsername;

            entity = await _aspNetUserRepository.AddAsync(entity);

            var dto = _mapper.Map<AspNetUserDto>(entity);
            var successDto = ApiResponseDto<AspNetUserDto>.Success(dto);

            return successDto;
        }

        public async Task<ApiResponseDto<string>> LoginAsync(LoginAspNetUserDto loginAspNetUserDto)
        {
            var normalizedUsername = loginAspNetUserDto.UserName.Normalize();
            var entity = await _aspNetUserRepository.GetByNormalizedUserNameAsync(normalizedUsername);

            if (entity == null)
            {
                var failDto = ApiResponseDto<string>.Fail(ErrorCodeEnum.USERNAME_OR_PASSWORD_IS_WRONG);

                return failDto;
            }

            var passwordHash = entity.PasswordHash ?? throw new ArgumentNullException(nameof(entity.PasswordHash));
            var userName = entity.UserName ?? throw new ArgumentNullException(nameof(entity.UserName));

            var result = _passwordHasher.VerifyHashedPassword(entity, passwordHash, loginAspNetUserDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                var failDto = ApiResponseDto<string>.Fail(ErrorCodeEnum.USERNAME_OR_PASSWORD_IS_WRONG);

                return failDto;
            }

            var token = _jwtService.GenerateToken(entity);
            var successDto = ApiResponseDto<string>.Success(token);

            return successDto;
        }
    }
}
