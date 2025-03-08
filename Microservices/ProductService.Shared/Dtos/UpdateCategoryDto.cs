namespace ProductService.Shared.Dtos
{
    public class UpdateCategoryDto
    {
        public required string Id { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Name { get; set; }
    }
}
