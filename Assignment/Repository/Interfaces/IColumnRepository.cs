using Assignment.Repository.Collections;

namespace Assignment.Repository.Interfaces
{
    public interface IColumnRepository
    {
        Task<IEnumerable<ColumnItem>> GetAllAsync();
        Task<ColumnItem?> GetByIdAsync(Guid id);
        Task AddAsync(ColumnItem column);
    }
}
