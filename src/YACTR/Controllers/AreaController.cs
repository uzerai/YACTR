using YACTR.DI.Repository.Interface;
using YACTR.DTO.RequestData;
using YACTR.Model.Location;
using Microsoft.AspNetCore.Mvc;

namespace YACTR.Controllers;

[ApiController]
[Route("areas")]
public class AreaController : ControllerBase
{
    private readonly IAreaRepository _areaRepository;

    public AreaController(IAreaRepository areaRepository)
    {
        _areaRepository = areaRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Area>> Create([FromBody] AreaRequestData requestData)
    {
        var createdArea = await _areaRepository.CreateAsync(new() {
            Name = requestData.Name,
            Description = requestData.Description,
            Location = requestData.Location,
            Boundary = requestData.Boundary
        });
        
        return Created(createdArea.Id.ToString(), createdArea);
    }

    [HttpGet("{areaId}")]
    public async Task<ActionResult<Area>> GetById([FromRoute] Guid areaId)
    {
        var area = await _areaRepository.GetByIdAsync(areaId);
        if (area == null)
            return NotFound();

        return Ok(area);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Area>>> GetAll()
    {
        var areas = await _areaRepository.GetAllAsync();
        return Ok(areas);
    }

    [HttpPut("{areaId}")]
    public async Task<IActionResult> Update([FromRoute] Guid areaId, [FromBody] AreaRequestData requestData)
    {
        var area = await _areaRepository.GetByIdAsync(areaId);

        if (area == null)
            return NotFound();

        await _areaRepository.UpdateAsync(area);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var area = await _areaRepository.GetByIdAsync(id);
        if (area == null)
            return NotFound();

        await _areaRepository.DeleteAsync(area);
        return NoContent();
    }
} 