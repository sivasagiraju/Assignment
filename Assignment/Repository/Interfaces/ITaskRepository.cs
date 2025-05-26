using Assignment.Helpers;
using Assignment.Repository.Collections;

namespace Assignment.Repository.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<PagedResult<TaskItem>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(Guid id);
        Task MoveTaskToColumnAsync(Guid taskId, Guid newColumnId);
    }
}
