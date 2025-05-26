using System.Diagnostics.CodeAnalysis;

namespace Assignment.DTOs
{
    [ExcludeFromCodeCoverage]
    public class CreateTaskDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime Deadline { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
