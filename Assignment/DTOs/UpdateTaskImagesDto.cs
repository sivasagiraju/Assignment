using System.Diagnostics.CodeAnalysis;

namespace Assignment.DTOs
{
    [ExcludeFromCodeCoverage]
    public class UpdateTaskImagesDto
    {
        public List<string> ImageUrls { get; set; } = new();
    }
}
