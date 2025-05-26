using System.Diagnostics.CodeAnalysis;

namespace Assignment.DTOs
{
    [ExcludeFromCodeCoverage]
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime Deadline { get; set; }
        public bool IsFavourite { get; set; }
        public Guid ColumnId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
