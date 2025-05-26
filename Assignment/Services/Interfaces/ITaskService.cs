using Assignment.DTOs;
using Assignment.Helpers;

namespace Assignment.Services.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto?> GetByIdAsync(Guid id);
        Task<PagedResult<TaskDto>> GetAllTasksAsync(int pageNumber, int pageSize);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);
        Task UpdateTaskAsync(TaskDto dto);
        Task DeleteTaskAsync(Guid id);
        Task MoveTaskToColumnAsync(Guid taskId, Guid newColumnId);
        Task<TaskDto> UpdateTaskImagesAsync(Guid taskId, UpdateTaskImagesDto dto);
        Task<TaskDto> UpdateTaskFavouriteAsync(Guid taskId, UpdateTaskFavouriteDto updateTaskFavouriteDto);
    }
}
