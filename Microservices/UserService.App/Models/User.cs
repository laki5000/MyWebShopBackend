using Shared.BaseClasses.Models;

namespace UserService.App.Models
{
    public class UserEntity : BaseEntity
    {
        public required string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
