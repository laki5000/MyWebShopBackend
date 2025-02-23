using AutoMapper;
using Grpc.Core;
using Shared.Dtos;
using User;
using UserService.App.Interfaces.Services;
using UserService.Shared.Dtos;

namespace UserService.App.Grpc
{
    public class UserServiceUserImpl : UserServiceUser.UserServiceUserBase
    {
        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        public UserServiceUserImpl(IUserService userService, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<GrpcResponseDto> Create(GrpcCreateUserDto request, ServerCallContext context)
        {
            var createUserDto = _mapper.Map<CreateUserDto>(request);
            var result = await _userService.CreateAsync(createUserDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            return response;
        }
    }
}
