using AuthService.Shared.Dtos;
using UserService.Shared.Dtos;

public class CreateAspNetUserAndUserDto
{
    public required CreateAspNetUserDto AspNetUser { get; set; }
    public required CreateUserDto User { get; set; }
}
