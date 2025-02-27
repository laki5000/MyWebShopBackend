﻿using Auth;
using AuthService.Interfaces.Services;
using AuthService.Shared.Dtos;
using AutoMapper;
using Grpc.Core;

namespace AuthService.App.Communication.Grpc
{
    public class AuthServiceUserImpl : AuthServiceUser.AuthServiceUserBase
    {
        private readonly ILogger<AuthServiceUserImpl> _logger;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IMapper _mapper;

        public AuthServiceUserImpl(ILogger<AuthServiceUserImpl> logger, IAspNetUserService aspNetUserService, IMapper mapper)
        {
            _logger = logger;
            _aspNetUserService = aspNetUserService;
            _mapper = mapper;
        }

        public override async Task<GrpcAspNetUserResponseDto> Create(GrpcCreateAspNetUserDto request, ServerCallContext context)
        {
            _logger.LogInformation("Create user request received for UserName: {UserName}", request.UserName);

            var createAspNetUserDto = _mapper.Map<CreateAspNetUserDto>(request);
            var result = await _aspNetUserService.CreateAsync(createAspNetUserDto);
            var response = _mapper.Map<GrpcAspNetUserResponseDto>(result);

            _logger.LogInformation("User created successfully with ID: {UserId}", result.Data);
            return response;
        }

        public override async Task<GrpcStringResponseDto> Login(GrpcLoginAspNetUserDto request, ServerCallContext context)
        {
            _logger.LogInformation("Login attempt for UserName: {UserName}", request.UserName);

            var loginAspNetUserDto = _mapper.Map<LoginAspNetUserDto>(request);
            var result = await _aspNetUserService.LoginAsync(loginAspNetUserDto);
            var response = _mapper.Map<GrpcStringResponseDto>(result);

            _logger.LogInformation("User logged in successfully: {UserName}", request.UserName);
            return response;
        }
    }
}
