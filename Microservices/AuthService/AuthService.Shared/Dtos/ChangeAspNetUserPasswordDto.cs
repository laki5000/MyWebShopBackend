namespace AuthService.Shared.Dtos
{
    public class ChangeAspNetUserPasswordDto
    {
        public string? UserId { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
