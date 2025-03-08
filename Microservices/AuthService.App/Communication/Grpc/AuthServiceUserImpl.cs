using Auth;
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

        public override async Task<GrpcStringResponseDto> Create(GrpcCreateAspNetUserDto request, ServerCallContext context)
        {
            _logger.LogInformation("Create user request received for UserName: {UserName}", request.UserName);

            var createAspNetUserDto = _mapper.Map<CreateAspNetUserDto>(request);
            var result = await _aspNetUserService.CreateAsync(createAspNetUserDto);
            var response = _mapper.Map<GrpcStringResponseDto>(result);

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

        public override async Task<GrpcResponseDto> ChangePassword(GrpcChangeAspNetUserPasswordDto request, ServerCallContext context)
        {
            _logger.LogInformation("Change password request received for UserId: {UserId}", request.UserId);

            var changeAspNetUserPasswordDto = _mapper.Map<ChangeAspNetUserPasswordDto>(request);
            var result = await _aspNetUserService.ChangePasswordAsync(changeAspNetUserPasswordDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            _logger.LogInformation("Password changed successfully for UserId: {UserId}", request.UserId);
            return response;
        }
    }
}
