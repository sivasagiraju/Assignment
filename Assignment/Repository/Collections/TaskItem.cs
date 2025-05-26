using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Assignment.Repository.Collections
{
    [ExcludeFromCodeCoverage]
    public class TaskItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime Deadline { get; set; }
        public bool IsFavourite { get; set; } = false;
        public Guid ColumnId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
