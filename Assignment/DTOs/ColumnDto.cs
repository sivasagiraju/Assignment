using System.Diagnostics.CodeAnalysis;

namespace Assignment.DTOs
{
    [ExcludeFromCodeCoverage]
    public class ColumnDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
