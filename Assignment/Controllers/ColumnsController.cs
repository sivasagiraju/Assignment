using Assignment.DTOs;
using Assignment.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ColumnsController : ControllerBase
{
    private readonly IColumnService _columnService;

    public ColumnsController(IColumnService columnService)
    {
        _columnService = columnService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ColumnDto>>> GetAll()
    {
        try
        {
            var columns = await _columnService.GetAllAsync();
            return Ok(columns);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching columns: {ex.Message}");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ColumnDto>> GetById(Guid id)
    {
        try
        {
            var column = await _columnService.GetByIdAsync(id);
            if (column == null)
                return NotFound($"Column with ID {id} was not found.");

            return Ok(column);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching column: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ColumnDto>> Create(CreateColumnDto dto)
    {
        try
        {
            var created = await _columnService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException argEx)
        {
            return BadRequest(argEx.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the column: {ex.Message}");
        }
    }
}
