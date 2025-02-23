using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    public class AspNetUser : IdentityUser
    {
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
