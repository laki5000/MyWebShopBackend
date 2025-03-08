using Shared.BaseClasses.Models;

namespace ProductService.App.Models
{
    public class Product : BaseEntity
    {
        public required string Id { get; set; }
        public required string Title { get; set; } 
        public required string Description { get; set; } 
        public required decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public required string CategoryId { get; set; } 
        public required Category Category { get; set; } 
        public required string ArtistId { get; set; }
    }
}
