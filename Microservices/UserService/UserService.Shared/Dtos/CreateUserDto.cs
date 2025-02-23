namespace UserService.Shared.Dtos
{
    public class CreateUserDto
    {
        public required string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
