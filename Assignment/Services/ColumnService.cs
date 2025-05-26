using Assignment.DTOs;
using Assignment.Repository.Collections;
using Assignment.Repository.Interfaces;
using Assignment.Services.Interfaces;
using AutoMapper;

namespace Assignment.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnRepository _repository;
        private readonly IMapper _mapper;

        public ColumnService(IColumnRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ColumnDto>> GetAllAsync()
        {
            try
            {
                var columns = await _repository.GetAllAsync();
                return _mapper.Map<IEnumerable<ColumnDto>>(columns);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve columns.", ex);
            }
        }

        public async Task<ColumnDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var column = await _repository.GetByIdAsync(id);
                return column == null ? null : _mapper.Map<ColumnDto>(column);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving column with ID {id}.", ex);
            }
        }

        public async Task<ColumnDto> AddAsync(CreateColumnDto dto)
        {
            if (dto == null)
                throw new ArgumentException("Column data must not be null");

            try
            {
                var column = _mapper.Map<ColumnItem>(dto);
                column.Id = Guid.NewGuid();

                await _repository.AddAsync(column);

                return _mapper.Map<ColumnDto>(column);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add column.", ex);
            }
        }
    }
}
