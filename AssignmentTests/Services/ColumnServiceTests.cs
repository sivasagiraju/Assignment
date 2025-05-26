using Assignment.DTOs;
using Assignment.Repository.Collections;
using Assignment.Repository.Interfaces;
using Assignment.Services;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace AssignmentTests.Services
{
    [TestFixture]
    public class ColumnServiceTests
    {
        private IFixture _fixture;
        private Mock<IColumnRepository> _repositoryMock;
        private Mock<IMapper> _mapperMock;
        private ColumnService _service;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _repositoryMock = new Mock<IColumnRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new ColumnService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnMappedColumns()
        {
            var columns = _fixture.CreateMany<ColumnItem>(3).ToList();
            var mapped = _fixture.CreateMany<ColumnDto>(3).ToList();

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(columns);
            _mapperMock.Setup(m => m.Map<IEnumerable<ColumnDto>>(columns)).Returns(mapped);

            var result = await _service.GetAllAsync();

            result.Should().BeEquivalentTo(mapped);
        }

        [Test]
        public async Task GetAllAsync_ShouldThrowApplicationException_OnError()
        {
            _repositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("db error"));

            Func<Task> act = () => _service.GetAllAsync();

            await act.Should()
                     .ThrowAsync<ApplicationException>()
                     .WithMessage("Failed to retrieve columns.*");
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnMappedColumn_WhenFound()
        {
            var column = _fixture.Create<ColumnItem>();
            var dto = _fixture.Create<ColumnDto>();

            _repositoryMock.Setup(r => r.GetByIdAsync(column.Id)).ReturnsAsync(column);
            _mapperMock.Setup(m => m.Map<ColumnDto>(column)).Returns(dto);

            var result = await _service.GetByIdAsync(column.Id);

            result.Should().Be(dto);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ColumnItem)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowApplicationException_OnError()
        {
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("db error"));

            Func<Task> act = () => _service.GetByIdAsync(id);

            await act.Should()
                     .ThrowAsync<ApplicationException>()
                     .WithMessage($"Error retrieving column with ID {id}*");
        }

        [Test]
        public async Task AddAsync_ShouldAddAndReturnMappedColumn()
        {
            var createDto = _fixture.Create<CreateColumnDto>();
            var column = _fixture.Build<ColumnItem>().Without(c => c.Id).Create();
            var expected = _fixture.Create<ColumnDto>();

            _mapperMock.Setup(m => m.Map<ColumnItem>(createDto)).Returns(column);
            _mapperMock.Setup(m => m.Map<ColumnDto>(It.IsAny<ColumnItem>())).Returns(expected);

            var result = await _service.AddAsync(createDto);

            _repositoryMock.Verify(r => r.AddAsync(It.Is<ColumnItem>(c => c.Id != Guid.Empty)), Times.Once);
            result.Should().Be(expected);
        }

        [Test]
        public async Task AddAsync_ShouldThrowArgumentException_WhenDtoIsNull()
        {
            Func<Task> act = () => _service.AddAsync(null);

            await act.Should()
                     .ThrowAsync<ArgumentException>()
                     .WithMessage("Column data must not be null*");
        }

        [Test]
        public async Task AddAsync_ShouldThrowApplicationException_OnRepositoryError()
        {
            var dto = _fixture.Create<CreateColumnDto>();
            var column = _fixture.Create<ColumnItem>();

            _mapperMock.Setup(m => m.Map<ColumnItem>(dto)).Returns(column);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ColumnItem>())).ThrowsAsync(new Exception("db fail"));

            Func<Task> act = () => _service.AddAsync(dto);

            await act.Should()
                     .ThrowAsync<ApplicationException>()
                     .WithMessage("Failed to add column.*");
        }
    }
}
