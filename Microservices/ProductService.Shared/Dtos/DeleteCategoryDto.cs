namespace ProductService.Shared.Dtos
{
    public class DeleteCategoryDto
    {
        public required string Id { get; set; }
        public string? DeletedBy { get; set; }
    }
}
