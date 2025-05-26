using Assignment.Repository.Collections;
using Assignment.Repository.Interfaces;
using Assignment.Repository.MongoBase;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class ColumnRepository : IColumnRepository
{
    private readonly IMongoCollection<ColumnItem> _collection;

    public ColumnRepository(IOptions<MongoDbSettings> settings)
    {
        try
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<ColumnItem>("Columns");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to connect to MongoDB or initialize column collection.", ex);
        }
    }

    public async Task<IEnumerable<ColumnItem>> GetAllAsync()
    {
        try
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to retrieve column items from the database.", ex);
        }
    }

    public async Task<ColumnItem?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to retrieve column with ID {id}.", ex);
        }
    }

    public async Task AddAsync(ColumnItem column)
    {
        if (column == null)
            throw new ArgumentNullException(nameof(column), "Column item cannot be null.");

        try
        {
            await _collection.InsertOneAsync(column);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to add the column item to the database.", ex);
        }
    }
}
