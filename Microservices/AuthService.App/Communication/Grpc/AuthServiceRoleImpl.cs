using Authservice.Proto;
using AuthService.App.Interfaces.Services;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace AuthService.App.Communication.Grpc
{
    public class AuthServiceRoleImpl : AuthServiceRole.AuthServiceRoleBase
    {
        private readonly ILogger<AuthServiceRoleImpl> _logger;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public AuthServiceRoleImpl(ILogger<AuthServiceRoleImpl> logger, IRoleService roleService, IMapper mapper)
        {
            _logger = logger;
            _roleService = roleService;
            _mapper = mapper;
        }

        public override async Task<GrpcStringListResponseDto> GetAll(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Get all roles request recieved");

            var result = await _roleService.GetAllAsync();
            var response = _mapper.Map<GrpcStringListResponseDto>(result);

            _logger.LogInformation("Roles retrieved successfully");
            return response;
        }
    }
}
