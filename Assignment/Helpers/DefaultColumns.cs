using Assignment.Repository.Collections;
using Assignment.Repository.Interfaces;

namespace Assignment.Helpers
{
    public static class DefaultColumns
    {
        public static readonly Guid ToDoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid InProgressId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid DoneId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static async Task SeedDefaultColumnsAsync(IColumnRepository repo)
        {
            var existingColumns = await repo.GetAllAsync();

            if (!existingColumns.Any(c => c.Id == DefaultColumns.ToDoId))
            {
                await repo.AddAsync(new ColumnItem { Id = DefaultColumns.ToDoId, Name = "ToDo" });
            }

            if (!existingColumns.Any(c => c.Id == DefaultColumns.InProgressId))
            {
                await repo.AddAsync(new ColumnItem { Id = DefaultColumns.InProgressId, Name = "In Progress" });
            }

            if (!existingColumns.Any(c => c.Id == DefaultColumns.DoneId))
            {
                await repo.AddAsync(new ColumnItem { Id = DefaultColumns.DoneId, Name = "Done" });
            }
        }
    }
}
