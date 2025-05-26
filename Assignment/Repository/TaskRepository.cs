using Assignment.Helpers;
using Assignment.Repository.Collections;
using Assignment.Repository.Interfaces;
using Assignment.Repository.MongoBase;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Assignment.Repository
{
    [ExcludeFromCodeCoverage]
    public class TaskRepository : ITaskRepository
    {
        private readonly IMongoCollection<TaskItem> _tasks;

        public TaskRepository(IOptions<MongoDbSettings> settings)
        {
            try
            {
                var client = new MongoClient(settings.Value.ConnectionString);
                var db = client.GetDatabase(settings.Value.DatabaseName);
                _tasks = db.GetCollection<TaskItem>(settings.Value.TaskCollectionName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error initializing MongoDB connection for TaskRepository.", ex);
            }
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _tasks.Find(t => t.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving task with ID {id}.", ex);
            }
        }

        public async Task<PagedResult<TaskItem>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var sort = Builders<TaskItem>.Sort
                    .Descending(t => t.IsFavourite)
                    .Ascending(t => t.Name);

                var skip = (pageNumber - 1) * pageSize;

                var tasks = await _tasks.Find(_ => true)
                                        .Sort(sort)
                                        .Skip(skip)
                                        .Limit(pageSize)
                                        .ToListAsync();

                var totalCount = await _tasks.CountDocumentsAsync(_ => true);

                return new PagedResult<TaskItem>
                {
                    Items = tasks,
                    TotalCount = (int)totalCount
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving paged task list.", ex);
            }
        }

        public async Task AddAsync(TaskItem task)
        {
            try
            {
                task.Id = Guid.NewGuid();
                await _tasks.InsertOneAsync(task);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inserting new task into MongoDB.", ex);
            }
        }

        public async Task UpdateAsync(TaskItem task)
        {
            try
            {
                var result = await _tasks.ReplaceOneAsync(t => t.Id == task.Id, task);
                if (result.MatchedCount == 0)
                    throw new KeyNotFoundException("Task not found during update.");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating task with ID {task.Id}.", ex);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var result = await _tasks.DeleteOneAsync(t => t.Id == id);
                if (result.DeletedCount == 0)
                    throw new KeyNotFoundException("Task not found during deletion.");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting task with ID {id}.", ex);
            }
        }

        public async Task MoveTaskToColumnAsync(Guid taskId, Guid newColumnId)
        {
            try
            {
                var update = Builders<TaskItem>.Update.Set(t => t.ColumnId, newColumnId);
                var result = await _tasks.UpdateOneAsync(t => t.Id == taskId, update);

                if (result.MatchedCount == 0)
                    throw new KeyNotFoundException("Task not found during column move.");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error moving task {taskId} to column {newColumnId}.", ex);
            }
        }
    }
}
