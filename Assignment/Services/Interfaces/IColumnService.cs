using Assignment.DTOs;

namespace Assignment.Services.Interfaces
{
    public interface IColumnService
    {
        Task<IEnumerable<ColumnDto>> GetAllAsync();
        Task<ColumnDto?> GetByIdAsync(Guid id);
        Task<ColumnDto> AddAsync(CreateColumnDto dto);
    }
}
