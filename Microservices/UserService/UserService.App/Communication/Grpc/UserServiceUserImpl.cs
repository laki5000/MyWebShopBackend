using AutoMapper;
using Grpc.Core;
using Shared.Dtos;
using User;
using UserService.App.Interfaces.Services;
using UserService.Shared.Dtos;

namespace UserService.App.Communication.Grpc
{
    public class UserServiceUserImpl : UserServiceUser.UserServiceUserBase
    {
        private readonly ILogger<UserServiceUserImpl> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserServiceUserImpl(ILogger<UserServiceUserImpl> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        public override async Task<GrpcResponseDto> Create(GrpcCreateUserDto request, ServerCallContext context)
        {
            _logger.LogInformation("Create user request received for Id: {Id}", request.Id);

            var createUserDto = _mapper.Map<CreateUserDto>(request);
            var result = await _userService.CreateAsync(createUserDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            _logger.LogInformation("User created successfully with ID: {UserId}", request.Id);

            return response;
        }
    }
}
