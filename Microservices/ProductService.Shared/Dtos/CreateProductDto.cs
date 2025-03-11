namespace ProductService.Shared.Dtos
{
    public class CreateProductDto
    {
        public string? CreatedBy { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public required string CategoryId { get; set; }
    }
}
