using System.Diagnostics.CodeAnalysis;

namespace Assignment.Helpers
{
    [ExcludeFromCodeCoverage]
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
    }
}
