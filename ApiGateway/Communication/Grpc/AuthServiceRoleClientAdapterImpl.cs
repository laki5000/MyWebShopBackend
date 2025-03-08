using ApiGateway.Interfaces.Grpc;
using Authservice.Proto;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Shared.Dtos;

namespace ApiGateway.Communication.Grpc
{
    public class AuthServiceRoleClientAdapterImpl : IAuthServiceRoleClientAdapter
    {
        private readonly ILogger<AuthServiceRoleClientAdapterImpl> _logger;
        private readonly AuthServiceRole.AuthServiceRoleClient _authServiceRoleClient;
        private readonly IMapper _mapper;

        public AuthServiceRoleClientAdapterImpl(ILogger<AuthServiceRoleClientAdapterImpl> logger, AuthServiceRole.AuthServiceRoleClient authServiceRoleClient, IMapper mapper)
        {
            _logger = logger;
            _authServiceRoleClient = authServiceRoleClient;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<List<string>>> GetAllAsync()
        {
            _logger.LogInformation("Get all roles request sent");

            var result = await _authServiceRoleClient.GetAllAsync(new Empty());
            var response = _mapper.Map<ApiResponseDto<List<string>>>(result);

            _logger.LogInformation("Roles recieved successfully");
            return response;
        }
    }
}
