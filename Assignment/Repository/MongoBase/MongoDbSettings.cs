using System.Diagnostics.CodeAnalysis;

namespace Assignment.Repository.MongoBase
{
    [ExcludeFromCodeCoverage]
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
        public string TaskCollectionName { get; set; } = "Tasks";
    }
}
