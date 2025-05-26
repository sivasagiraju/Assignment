using Assignment.DTOs;
using Assignment.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Assignment.Tests.Controllers
{
    [TestFixture]
    public class ColumnsControllerTests
    {
        private Mock<IColumnService> _columnServiceMock;
        private ColumnsController _controller;

        [SetUp]
        public void Setup()
        {
            _columnServiceMock = new Mock<IColumnService>();
            _controller = new ColumnsController(_columnServiceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkWithColumns()
        {
            var columns = new List<ColumnDto>
            {
                new ColumnDto { Id = Guid.NewGuid(), Name = "To Do" },
                new ColumnDto { Id = Guid.NewGuid(), Name = "Done" }
            };
            _columnServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(columns);

            var result = await _controller.GetAll();

            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(columns);
        }

        [Test]
        public async Task GetAll_ExceptionThrown_Returns500()
        {
            _columnServiceMock.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("DB failure"));

            var result = await _controller.GetAll();

            result.Result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task GetById_ColumnExists_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var column = new ColumnDto { Id = id, Name = "In Progress" };
            _columnServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(column);

            var result = await _controller.GetById(id);

            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(column);
        }

        [Test]
        public async Task GetById_ColumnNotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _columnServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((ColumnDto?)null);

            var result = await _controller.GetById(id);

            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Column with ID {id} was not found.");
        }

        [Test]
        public async Task GetById_ExceptionThrown_Returns500()
        {
            var id = Guid.NewGuid();
            _columnServiceMock.Setup(s => s.GetByIdAsync(id)).ThrowsAsync(new Exception("db error"));

            var result = await _controller.GetById(id);

            result.Result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task Create_ValidInput_ReturnsCreatedResult()
        {
            var createDto = new CreateColumnDto { Name = "New Column" };
            var created = new ColumnDto { Id = Guid.NewGuid(), Name = createDto.Name };

            _columnServiceMock.Setup(s => s.AddAsync(createDto)).ReturnsAsync(created);

            var result = await _controller.Create(createDto);

            var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdAtResult.Value.Should().BeEquivalentTo(created);
        }

        [Test]
        public async Task Create_ArgumentException_ReturnsBadRequest()
        {
            var createDto = new CreateColumnDto { Name = "Invalid" };
            _columnServiceMock.Setup(s => s.AddAsync(createDto)).ThrowsAsync(new ArgumentException("Invalid column"));

            var result = await _controller.Create(createDto);

            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Invalid column");
        }

        [Test]
        public async Task Create_Exception_Returns500()
        {
            var createDto = new CreateColumnDto { Name = "Test" };
            _columnServiceMock.Setup(s => s.AddAsync(createDto)).ThrowsAsync(new Exception("Unexpected"));

            var result = await _controller.Create(createDto);

            result.Result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }
    }
}
