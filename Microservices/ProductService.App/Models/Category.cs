using Shared.BaseClasses.Models;

namespace ProductService.App.Models
{
    public class Category : BaseEntity
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
    }
}
