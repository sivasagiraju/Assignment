using Assignment.DTOs;
using Assignment.Helpers;
using Assignment.Repository.Collections;
using Assignment.Repository.Interfaces;
using Assignment.Services;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AssignmentTests.Services
{
    [TestFixture]
    public class TaskServiceTests
    {
        private Mock<ITaskRepository> _taskRepoMock;
        private Mock<IColumnRepository> _columnRepoMock;
        private Mock<IMapper> _mapperMock;
        private Fixture _fixture;
        private TaskService _taskService;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _taskRepoMock = new Mock<ITaskRepository>();
            _columnRepoMock = new Mock<IColumnRepository>();
            _mapperMock = new Mock<IMapper>();

            _taskService = new TaskService(
                _taskRepoMock.Object,
                _columnRepoMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public async Task GetByIdAsync_TaskExists_ReturnsMappedDto()
        {
            var task = _fixture.Create<TaskItem>();
            var dto = _fixture.Create<TaskDto>();

            _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
            _mapperMock.Setup(m => m.Map<TaskDto>(task)).Returns(dto);

            var result = await _taskService.GetByIdAsync(task.Id);

            result.Should().BeEquivalentTo(dto);
        }

        [Test]
        public async Task GetByIdAsync_TaskNotFound_ReturnsNull()
        {
            _taskRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TaskItem)null);

            var result = await _taskService.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Test]
        public async Task GetAllTasksAsync_ReturnsMappedPagedResult()
        {
            var pagedTasks = new PagedResult<TaskItem>
            {
                Items = _fixture.CreateMany<TaskItem>(3),
                TotalCount = 3
            };
            var mappedItems = _fixture.CreateMany<TaskDto>(3).ToList();

            _taskRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(pagedTasks);
            _mapperMock.Setup(m => m.Map<IEnumerable<TaskDto>>(pagedTasks.Items)).Returns(mappedItems);

            var result = await _taskService.GetAllTasksAsync(1, 10);

            result.TotalCount.Should().Be(3);
            result.Items.Should().BeEquivalentTo(mappedItems);
        }

        [Test]
        public async Task CreateTaskAsync_ValidDto_CreatesAndReturnsMappedTask()
        {
            var createDto = _fixture.Create<CreateTaskDto>();
            var taskItem = _fixture.Create<TaskItem>();
            var taskDto = _fixture.Create<TaskDto>();

            _mapperMock.Setup(m => m.Map<TaskItem>(createDto)).Returns(taskItem);
            _taskRepoMock.Setup(r => r.AddAsync(taskItem)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TaskDto>(taskItem)).Returns(taskDto);

            var result = await _taskService.CreateTaskAsync(createDto);

            result.Should().BeEquivalentTo(taskDto);
        }

        [Test]
        public async Task CreateTaskAsync_ThrowsException_WhenMapperFails()
        {
            var createDto = _fixture.Create<CreateTaskDto>();
            _mapperMock.Setup(m => m.Map<TaskItem>(createDto)).Throws<AutoMapperMappingException>();

            Func<Task> act = async () => await _taskService.CreateTaskAsync(createDto);

            await act.Should().ThrowAsync<ApplicationException>();
        }

        [Test]
        public async Task UpdateTaskAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            var dto = _fixture.Create<TaskDto>();
            _taskRepoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((TaskItem)null);

            Func<Task> act = async () => await _taskService.UpdateTaskAsync(dto);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Test]
        public async Task DeleteTaskAsync_ValidId_DeletesTask()
        {
            var task = _fixture.Create<TaskItem>();
            _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
            _taskRepoMock.Setup(r => r.DeleteAsync(task.Id)).Returns(Task.CompletedTask);

            await _taskService.DeleteTaskAsync(task.Id);

            _taskRepoMock.Verify(r => r.DeleteAsync(task.Id), Times.Once);
        }

        [Test]
        public async Task DeleteTaskAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            _taskRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TaskItem)null);

            Func<Task> act = async () => await _taskService.DeleteTaskAsync(Guid.NewGuid());

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Test]
        public async Task MoveTaskToColumnAsync_ColumnNotFound_ThrowsKeyNotFoundException()
        {
            var taskId = Guid.NewGuid();
            var columnId = Guid.NewGuid();

            _columnRepoMock.Setup(r => r.GetByIdAsync(columnId)).ReturnsAsync((ColumnItem)null);

            Func<Task> act = async () => await _taskService.MoveTaskToColumnAsync(taskId, columnId);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Test]
        public async Task UpdateTaskImagesAsync_ValidTask_UpdatesAndReturnsMappedTask()
        {
            var task = _fixture.Create<TaskItem>();
            var dto = _fixture.Create<UpdateTaskImagesDto>();
            var mapped = _fixture.Create<TaskDto>();

            _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
            _mapperMock.Setup(m => m.Map(dto, task));
            _taskRepoMock.Setup(r => r.UpdateAsync(task)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TaskDto>(task)).Returns(mapped);

            var result = await _taskService.UpdateTaskImagesAsync(task.Id, dto);

            result.Should().BeEquivalentTo(mapped);
        }

        [Test]
        public async Task UpdateTaskImagesAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            var dto = _fixture.Create<UpdateTaskImagesDto>();
            var id = Guid.NewGuid();
            _taskRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TaskItem)null);

            Func<Task> act = async () => await _taskService.UpdateTaskImagesAsync(id, dto);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Test]
        public async Task UpdateTaskFavouriteAsync_ValidTask_UpdatesAndReturnsMappedTask()
        {
            var task = _fixture.Create<TaskItem>();
            var dto = _fixture.Create<UpdateTaskFavouriteDto>();
            var mapped = _fixture.Create<TaskDto>();

            _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
            _mapperMock.Setup(m => m.Map(dto, task));
            _taskRepoMock.Setup(r => r.UpdateAsync(task)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TaskDto>(task)).Returns(mapped);

            var result = await _taskService.UpdateTaskFavouriteAsync(task.Id, dto);

            result.Should().BeEquivalentTo(mapped);
        }

        [Test]
        public async Task UpdateTaskFavouriteAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            var dto = _fixture.Create<UpdateTaskFavouriteDto>();
            var id = Guid.NewGuid();
            _taskRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TaskItem)null);

            Func<Task> act = async () => await _taskService.UpdateTaskFavouriteAsync(id, dto);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
