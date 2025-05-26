using YACTR.DI.Repository.Interface;
using YACTR.DTO.RequestData;
using YACTR.Model.Location;
using Microsoft.AspNetCore.Mvc;

namespace YACTR.Controllers;

[ApiController]
[Route("sectors")]
public class SectorController : ControllerBase
{
    private readonly IEntityRepository<Sector> _sectorRepository;

    public SectorController(IEntityRepository<Sector> sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Sector>> Create([FromBody] SectorRequestData sector)
    {
        var createdSector = await _sectorRepository.CreateAsync(new() {
            Name = sector.Name, 
            SectorArea = sector.SectorArea,
            EntryPoint = sector.EntryPoint,
            AreaId = sector.AreaId,
            RecommendedParkingLocation = sector.RecommendedParkingLocation,
            ApproachPath = sector.ApproachPath,
        });

        return Created(createdSector.Id.ToString(), createdSector);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sector>> GetById([FromRoute] Guid id)
    {
        var sector = await _sectorRepository.GetByIdAsync(id);
        if (sector == null)
            return NotFound();

        return Ok(sector);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sector>>> GetAll()
    {
        var sectors = await _sectorRepository.GetAllAsync();
        return Ok(sectors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] SectorRequestData sector)
    {
        var existingSector = await _sectorRepository.GetByIdAsync(id);
        if (existingSector == null)
            return NotFound();

        if (sector.Name != null) existingSector.Name = sector.Name;
        if (sector.SectorArea != null) existingSector.SectorArea = sector.SectorArea;

        await _sectorRepository.UpdateAsync(existingSector);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var sector = await _sectorRepository.GetByIdAsync(id);
        if (sector == null)
            return NotFound();

        await _sectorRepository.DeleteAsync(sector);
        return NoContent();
    }
} 