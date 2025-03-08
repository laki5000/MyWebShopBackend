namespace AuthService.Shared.Dtos
{
    public class LoginAspNetUserDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
