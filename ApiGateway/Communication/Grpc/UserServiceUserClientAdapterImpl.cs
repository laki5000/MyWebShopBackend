using ApiGateway.Interfaces.Grpc;
using AutoMapper;
using Shared.Dtos;
using User;
using UserService.Shared.Dtos;

namespace ApiGateway.Communication.Grpc
{
    public class UserServiceUserClientAdapterImpl : IUserServiceUserClientAdapter
    {
        private readonly ILogger<UserServiceUserClientAdapterImpl> _logger;
        private readonly UserServiceUser.UserServiceUserClient _userServiceUserClient;
        private readonly IMapper _mapper;

        public UserServiceUserClientAdapterImpl(ILogger<UserServiceUserClientAdapterImpl> logger, UserServiceUser.UserServiceUserClient userServiceClient, IMapper mapper)
        {
            _logger = logger;
            _userServiceUserClient = userServiceClient;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto> CreateAsync(CreateUserDto request)
        {
            _logger.LogInformation("Create user request sent for ID: {UserId}", request.Id);

            var grpcCreateUserDto = _mapper.Map<GrpcCreateUserDto>(request);
            var result = await _userServiceUserClient.CreateAsync(grpcCreateUserDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            _logger.LogInformation("User created successfully with ID: {UserId}", request.Id);
            return response;
        }
    }
}
