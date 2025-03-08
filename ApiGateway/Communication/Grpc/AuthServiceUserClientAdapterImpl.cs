using ApiGateway.Interfaces.Grpc;
using Authservice.Proto;
using AuthService.Shared.Dtos;
using AutoMapper;
using Shared.Dtos;

namespace ApiGateway.Communication.Grpc
{
    public class AuthServiceUserClientAdapterImpl : IAuthServiceUserClientAdapter
    {
        private readonly ILogger<AuthServiceUserClientAdapterImpl> _logger;
        private readonly AuthServiceUser.AuthServiceUserClient _authServiceUserClient;
        private readonly IMapper _mapper;

        public AuthServiceUserClientAdapterImpl(ILogger<AuthServiceUserClientAdapterImpl> logger, AuthServiceUser.AuthServiceUserClient authServiceUserClient, IMapper mapper)
        {
            _logger = logger;
            _authServiceUserClient = authServiceUserClient;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<string>> CreateAsync(CreateAspNetUserDto request)
        {
            _logger.LogInformation("Create user request sent for UserName: {UserName}", request.UserName);

            var grpcCreateAspNetUserDto = _mapper.Map<GrpcCreateAspNetUserDto>(request);
            var result = await _authServiceUserClient.CreateAsync(grpcCreateAspNetUserDto);
            var response = _mapper.Map<ApiResponseDto<string>>(result);

            _logger.LogInformation("User created successfully with ID: {UserId}", response.Data);
            return response;
        }

        public async Task<ApiResponseDto<string>> LoginAsync(LoginAspNetUserDto request)
        {
            _logger.LogInformation("Login attempt sent for UserName: {UserName}", request.UserName);

            var grpcLoginAspNetUserDto = _mapper.Map<GrpcLoginAspNetUserDto>(request);
            var result = await _authServiceUserClient.LoginAsync(grpcLoginAspNetUserDto);
            var response = _mapper.Map<ApiResponseDto<string>>(result);

            _logger.LogInformation("User logged in successfully: {UserName}", request.UserName);
            return response;
        }

        public async Task<ApiResponseDto> ChangePasswordAsync(ChangeAspNetUserPasswordDto request)
        {
            _logger.LogInformation("Change password request sent for UserId: {UserId}", request.UserId);

            var grpcChangeAspNetUserPasswordDto = _mapper.Map<GrpcChangeAspNetUserPasswordDto>(request);
            var result = await _authServiceUserClient.ChangePasswordAsync(grpcChangeAspNetUserPasswordDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            _logger.LogInformation("Password changed successfully for UserId: {UserId}", request.UserId);
            return response;
        }
    }
}
