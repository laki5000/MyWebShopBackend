using Microsoft.AspNetCore.Identity;
using Shared.Enums;

namespace AuthService.Models
{
    public class AspNetUser : IdentityUser
    {
        public ObjectStatus Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
