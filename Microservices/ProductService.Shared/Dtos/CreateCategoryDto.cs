namespace ProductService.Shared.Dtos
{
    public class CreateCategoryDto
    {
        public string? CreatedBy { get; set; }
        public required string Name { get; set; }
    }
}
