using Assignment.DTOs;
using Assignment.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id)
    {
        try
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
                return NotFound($"Task with ID {id} not found.");

            return Ok(task);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _taskService.GetAllTasksAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to fetch tasks: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdTask = await _taskService.CreateTaskAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating task: {ex.Message}");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaskDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Task ID in the URL does not match the body.");

        try
        {
            await _taskService.UpdateTaskAsync(dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating task: {ex.Message}");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting task: {ex.Message}");
        }
    }

    [HttpPut("{taskId:guid}/move-to/{columnId:guid}")]
    public async Task<IActionResult> MoveTaskToColumn(Guid taskId, Guid columnId)
    {
        try
        {
            await _taskService.MoveTaskToColumnAsync(taskId, columnId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error moving task: {ex.Message}");
        }
    }

    [HttpPut("{taskId}/images")]
    public async Task<IActionResult> UpdateImages(Guid taskId, [FromBody] UpdateTaskImagesDto dto)
    {
        try
        {
            var updatedTask = await _taskService.UpdateTaskImagesAsync(taskId, dto);
            return Ok(updatedTask);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {taskId} not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating task images: {ex.Message}");
        }
    }

    [HttpPut("{taskId}/favourite")]
    public async Task<IActionResult> UpdateFavourite(Guid taskId, [FromBody] UpdateTaskFavouriteDto dto)
    {
        try
        {
            var updatedTask = await _taskService.UpdateTaskFavouriteAsync(taskId, dto);
            return Ok(updatedTask);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {taskId} not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating favourite status: {ex.Message}");
        }
    }
}
