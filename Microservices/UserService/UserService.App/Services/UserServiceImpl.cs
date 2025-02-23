using AutoMapper;
using Shared.Dtos;
using UserService.App.Interfaces.Repositories;
using UserService.App.Interfaces.Services;
using UserService.App.Models;
using UserService.Shared.Dtos;

namespace UserService.App.Services
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        public UserServiceImpl(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponseDto> CreateAsync(CreateUserDto createUserDto)
        {
            var entity = _mapper.Map<UserEntity>(createUserDto);

            await _userRepository.AddAsync(entity);

            return ApiResponseDto.Success();
        }
    }
}
