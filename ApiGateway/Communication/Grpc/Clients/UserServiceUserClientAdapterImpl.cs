using ApiGateway.Interfaces.Grpc;
using AutoMapper;
using Shared.Dtos;
using User;
using UserService.Shared.Dtos;

namespace ApiGateway.Communication.Grpc.Clients
{
    public class UserServiceUserClientAdapterImpl : IUserServiceUserClientAdapter
    {
        private readonly UserServiceUser.UserServiceUserClient _userServiceClient;

        private readonly IMapper _mapper;

        public UserServiceUserClientAdapterImpl(UserServiceUser.UserServiceUserClient userServiceClient, IMapper mapper)
        {
            _userServiceClient = userServiceClient ?? throw new ArgumentNullException(nameof(userServiceClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponseDto> CreateAsync(CreateUserDto request)
        {
            var grpcCreateUserDto = _mapper.Map<GrpcCreateUserDto>(request);
            var result = await _userServiceClient.CreateAsync(grpcCreateUserDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            return response;
        }
    }
}
