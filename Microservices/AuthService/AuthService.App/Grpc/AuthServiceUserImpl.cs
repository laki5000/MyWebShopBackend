using Auth;
using AuthService.Interfaces.Services;
using AuthService.Shared.Dtos;
using AutoMapper;
using Grpc.Core;

namespace AuthService.GrpcServices
{
    public class AuthServiceUserImpl : AuthServiceUser.AuthServiceUserBase
    {
        private readonly IAspNetUserService _aspNetUserService;

        private readonly IMapper _mapper;

        public AuthServiceUserImpl(IAspNetUserService aspNetUserService, IMapper mapper)
        {
            _aspNetUserService = aspNetUserService;
            _mapper = mapper;
        }

        public override async Task<GrpcAspNetUserResponseDto> Create(GrpcCreateAspNetUserDto request, ServerCallContext context)
        {
            var createAspNetUserDto = _mapper.Map<CreateAspNetUserDto>(request);
            var result = await _aspNetUserService.CreateAsync(createAspNetUserDto);
            var response = _mapper.Map<GrpcAspNetUserResponseDto>(result);

            return response;
        }

        public override async Task<GrpcStringResponseDto> Login(GrpcLoginAspNetUserDto request, ServerCallContext context)
        {
            var loginAspNetUserDto = _mapper.Map<LoginAspNetUserDto>(request);
            var result = await _aspNetUserService.LoginAsync(loginAspNetUserDto);
            var response = _mapper.Map<GrpcStringResponseDto>(result);

            return response;
        }
    }
}
