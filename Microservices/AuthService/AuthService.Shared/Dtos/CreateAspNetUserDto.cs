namespace AuthService.Shared.Dtos
{
    public class CreateAspNetUserDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
