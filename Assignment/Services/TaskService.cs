using Assignment.DTOs;
using Assignment.Helpers;
using Assignment.Repository.Collections;
using Assignment.Repository.Interfaces;
using Assignment.Services.Interfaces;
using AutoMapper;

namespace Assignment.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IColumnRepository _columnRepository;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository taskRepository, IColumnRepository columnRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _columnRepository = columnRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                return task is null ? null : _mapper.Map<TaskDto>(task);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving task with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<PagedResult<TaskDto>> GetAllTasksAsync(int pageNumber, int pageSize)
        {
            try
            {
                var pagedResult = await _taskRepository.GetAllAsync(pageNumber, pageSize);
                var mappedItems = _mapper.Map<IEnumerable<TaskDto>>(pagedResult.Items);

                return new PagedResult<TaskDto>
                {
                    Items = mappedItems,
                    TotalCount = pagedResult.TotalCount
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving paginated task list.", ex);
            }
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
        {
            try
            {
                var task = _mapper.Map<TaskItem>(dto);
                await _taskRepository.AddAsync(task);
                return _mapper.Map<TaskDto>(task);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error creating task.", ex);
            }
        }

        public async Task UpdateTaskAsync(TaskDto dto)
        {
            try
            {
                var existing = await _taskRepository.GetByIdAsync(dto.Id);
                if (existing is null)
                    throw new KeyNotFoundException("Task not found");

                _mapper.Map(dto, existing);
                await _taskRepository.UpdateAsync(existing);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating task {dto.Id}: {ex.Message}", ex);
            }
        }

        public async Task DeleteTaskAsync(Guid id)
        {
            try
            {
                var existing = await _taskRepository.GetByIdAsync(id);
                if (existing is null)
                    throw new KeyNotFoundException("Task not found");

                await _taskRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting task {id}: {ex.Message}", ex);
            }
        }

        public async Task MoveTaskToColumnAsync(Guid taskId, Guid newColumnId)
        {
            try
            {
                var column = await _columnRepository.GetByIdAsync(newColumnId);
                if (column == null)
                    throw new KeyNotFoundException("Target column does not exist");

                await _taskRepository.MoveTaskToColumnAsync(taskId, newColumnId);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error moving task {taskId} to column {newColumnId}: {ex.Message}", ex);
            }
        }

        public async Task<TaskDto> UpdateTaskImagesAsync(Guid taskId, UpdateTaskImagesDto updateTaskImagesDto)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(taskId);
                if (task == null)
                    throw new KeyNotFoundException("Task not found");

                _mapper.Map(updateTaskImagesDto, task);
                await _taskRepository.UpdateAsync(task);

                return _mapper.Map<TaskDto>(task);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating images for task {taskId}: {ex.Message}", ex);
            }
        }

        public async Task<TaskDto> UpdateTaskFavouriteAsync(Guid taskId, UpdateTaskFavouriteDto updateTaskFavouriteDto)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(taskId);
                if (task == null)
                    throw new KeyNotFoundException("Task not found");

                _mapper.Map(updateTaskFavouriteDto, task);
                await _taskRepository.UpdateAsync(task);

                return _mapper.Map<TaskDto>(task);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating favourite for task {taskId}: {ex.Message}", ex);
            }
        }
    }
}
