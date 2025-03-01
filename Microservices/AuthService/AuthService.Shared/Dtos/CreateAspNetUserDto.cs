namespace AuthService.Shared.Dtos
{
    public class CreateAspNetUserDto : LoginAspNetUserDto
    {
        public required string Email { get; set; }
    }
}
