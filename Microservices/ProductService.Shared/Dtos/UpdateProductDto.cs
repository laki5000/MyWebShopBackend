namespace ProductService.Shared.Dtos
{
    public class UpdateProductDto
    {
        public string? Id { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public string? CategoryId { get; set; }
    }
}
