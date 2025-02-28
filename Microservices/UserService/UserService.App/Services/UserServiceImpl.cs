using AutoMapper;
using Shared.Dtos;
using Shared.Enums;
using UserService.App.Interfaces.Repositories;
using UserService.App.Interfaces.Services;
using UserService.App.Models;
using UserService.Shared.Dtos;

namespace UserService.App.Services
{
    public class UserServiceImpl : IUserService
    {
        private readonly ILogger<UserServiceImpl> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserServiceImpl(ILogger<UserServiceImpl> logger, IUserRepository userRepository, IMapper mapper)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto> CreateAsync(CreateUserDto createUserDto)
        {
            if (createUserDto.Id == null) 
            {
                var errorResult = ApiResponseDto.Fail(ErrorCodeEnum.UNKNOWN_ERROR);
                _logger.LogError("User creation failed. Id cannot be null");

                return errorResult;
            }

            var entity = _mapper.Map<UserEntity>(createUserDto);
            await _userRepository.AddAsync(entity);

            var result = ApiResponseDto.Success();
            return result;
        }
    }
}
