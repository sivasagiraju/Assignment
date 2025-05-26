using Assignment.DTOs;
using Assignment.Helpers;
using Assignment.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Assignment.Tests.Controllers
{
    [TestFixture]
    public class TasksControllerTests
    {
        private Mock<ITaskService> _taskServiceMock;
        private TasksController _controller;

        [SetUp]
        public void Setup()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _controller = new TasksController(_taskServiceMock.Object);
        }

        [Test]
        public async Task GetById_TaskExists_ReturnsOkWithTask()
        {
            var task = new TaskDto { Id = Guid.NewGuid() };
            _taskServiceMock.Setup(s => s.GetByIdAsync(task.Id)).ReturnsAsync(task);

            var result = await _controller.GetById(task.Id);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(task);
        }

        [Test]
        public async Task GetById_TaskNotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _taskServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((TaskDto?)null);

            var result = await _controller.GetById(id);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task GetById_ExceptionThrown_Returns500()
        {
            var id = Guid.NewGuid();
            _taskServiceMock.Setup(s => s.GetByIdAsync(id)).ThrowsAsync(new Exception("db error"));

            var result = await _controller.GetById(id);

            result.Result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task GetAllTasks_ReturnsOk()
        {
            var paged = new PagedResult<TaskDto> { TotalCount = 2, Items = new[] { new TaskDto(), new TaskDto() } };
            _taskServiceMock.Setup(s => s.GetAllTasksAsync(1, 10)).ReturnsAsync(paged);

            var result = await _controller.GetAllTasks(1, 10);

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(paged);
        }

        [Test]
        public async Task GetAllTasks_Exception_Returns500()
        {
            _taskServiceMock.Setup(s => s.GetAllTasksAsync(1, 10)).ThrowsAsync(new Exception("failure"));

            var result = await _controller.GetAllTasks(1, 10);

            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }

        [Test]
        public async Task Create_ValidDto_ReturnsCreated()
        {
            var dto = new CreateTaskDto { Name = "Test" };
            var created = new TaskDto { Id = Guid.NewGuid(), Name = dto.Name };

            _taskServiceMock.Setup(s => s.CreateTaskAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);

            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.Value.Should().BeEquivalentTo(created);
        }

        [Test]
        public async Task Create_Exception_Returns500()
        {
            var dto = new CreateTaskDto { Name = "Test" };
            _taskServiceMock.Setup(s => s.CreateTaskAsync(dto)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Create(dto);

            result.Result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }

        [Test]
        public async Task Update_TaskIdMismatch_ReturnsBadRequest()
        {
            var dto = new TaskDto { Id = Guid.NewGuid() };
            var result = await _controller.Update(Guid.NewGuid(), dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Update_ValidRequest_ReturnsNoContent()
        {
            var dto = new TaskDto { Id = Guid.NewGuid() };
            _taskServiceMock.Setup(s => s.UpdateTaskAsync(dto)).Returns(Task.CompletedTask);

            var result = await _controller.Update(dto.Id, dto);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task Update_TaskNotFound_ReturnsNotFound()
        {
            var dto = new TaskDto { Id = Guid.NewGuid() };
            _taskServiceMock.Setup(s => s.UpdateTaskAsync(dto)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Update(dto.Id, dto);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            _taskServiceMock.Setup(s => s.DeleteTaskAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task Delete_TaskNotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _taskServiceMock.Setup(s => s.DeleteTaskAsync(id)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Delete(id);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task MoveTaskToColumn_ValidRequest_ReturnsNoContent()
        {
            var taskId = Guid.NewGuid();
            var columnId = Guid.NewGuid();

            _taskServiceMock.Setup(s => s.MoveTaskToColumnAsync(taskId, columnId)).Returns(Task.CompletedTask);

            var result = await _controller.MoveTaskToColumn(taskId, columnId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task MoveTaskToColumn_ColumnNotFound_ReturnsNotFound()
        {
            var taskId = Guid.NewGuid();
            var columnId = Guid.NewGuid();

            _taskServiceMock.Setup(s => s.MoveTaskToColumnAsync(taskId, columnId))
                .ThrowsAsync(new KeyNotFoundException("Column not found"));

            var result = await _controller.MoveTaskToColumn(taskId, columnId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task UpdateImages_ValidRequest_ReturnsOk()
        {
            var taskId = Guid.NewGuid();
            var dto = new UpdateTaskImagesDto();
            var updated = new TaskDto { Id = taskId };

            _taskServiceMock.Setup(s => s.UpdateTaskImagesAsync(taskId, dto)).ReturnsAsync(updated);

            var result = await _controller.UpdateImages(taskId, dto);

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(updated);
        }

        [Test]
        public async Task UpdateImages_TaskNotFound_ReturnsNotFound()
        {
            var taskId = Guid.NewGuid();
            var dto = new UpdateTaskImagesDto();

            _taskServiceMock.Setup(s => s.UpdateTaskImagesAsync(taskId, dto)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateImages(taskId, dto);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task UpdateFavourite_ValidRequest_ReturnsOk()
        {
            var taskId = Guid.NewGuid();
            var dto = new UpdateTaskFavouriteDto();
            var updated = new TaskDto { Id = taskId };

            _taskServiceMock.Setup(s => s.UpdateTaskFavouriteAsync(taskId, dto)).ReturnsAsync(updated);

            var result = await _controller.UpdateFavourite(taskId, dto);

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(updated);
        }

        [Test]
        public async Task UpdateFavourite_TaskNotFound_ReturnsNotFound()
        {
            var taskId = Guid.NewGuid();
            var dto = new UpdateTaskFavouriteDto();

            _taskServiceMock.Setup(s => s.UpdateTaskFavouriteAsync(taskId, dto)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateFavourite(taskId, dto);

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
