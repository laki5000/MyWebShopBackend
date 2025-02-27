using ApiGateway.Interfaces.Grpc;
using Auth;
using AuthService.Shared.Dtos;
using AutoMapper;
using Shared.Dtos;

namespace ApiGateway.Communication.Grpc
{
    public class AuthServiceUserClientAdapterImpl : IAuthServiceUserClientAdapter
    {
        private readonly AuthServiceUser.AuthServiceUserClient _authServiceUserClient;

        private readonly IMapper _mapper;

        public AuthServiceUserClientAdapterImpl(AuthServiceUser.AuthServiceUserClient authServiceUserClient, IMapper mapper)
        {
            _authServiceUserClient = authServiceUserClient ?? throw new ArgumentNullException(nameof(authServiceUserClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponseDto<AspNetUserDto>> CreateAsync(CreateAspNetUserDto request)
        {
            var grpcCreateAspNetUserDto = _mapper.Map<GrpcCreateAspNetUserDto>(request);
            var result = await _authServiceUserClient.CreateAsync(grpcCreateAspNetUserDto);
            var response = _mapper.Map<ApiResponseDto<AspNetUserDto>>(result);

            return response;
        }

        public async Task<ApiResponseDto<string>> LoginAsync(LoginAspNetUserDto request)
        {
            var grpcLoginAspNetUserDto = _mapper.Map<GrpcLoginAspNetUserDto>(request);
            var result = await _authServiceUserClient.LoginAsync(grpcLoginAspNetUserDto);
            var response = _mapper.Map<ApiResponseDto<string>>(result);

            return response;
        }
    }
}
