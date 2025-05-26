using YACTR.DI.Repository.Interface;
using YACTR.DTO.RequestData;
using YACTR.Model.Location;
using Microsoft.AspNetCore.Mvc;

namespace YACTR.Controllers;

[ApiController]
[Route("pitches")]
public class PitchController : ControllerBase
{
    private readonly IEntityRepository<Pitch> _pitchRepository;

    public PitchController(IEntityRepository<Pitch> pitchRepository)
    {
        _pitchRepository = pitchRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Pitch>> Create([FromBody] PitchRequestData pitch)
    {
        var createdPitch = await _pitchRepository.CreateAsync(new() {
            Name = pitch.Name,
            Description = pitch.Description,
            Type = pitch.Type,
            SectorId = pitch.SectorId
        });

        return Created(createdPitch.Id.ToString(), createdPitch);
    }

    [HttpGet("{pitchId}")]
    public async Task<ActionResult<Pitch>> GetById([FromRoute] Guid pitchId)
    {
        var pitch = await _pitchRepository.GetByIdAsync(pitchId);

        if (pitch == null)
            return NotFound();
        
        return Ok(pitch);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pitch>>> GetAll()
    {
        var pitches = await _pitchRepository.GetAllAsync();
        return Ok(pitches);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] PitchRequestData pitch)
    {
        var existingPitch = await _pitchRepository.GetByIdAsync(id);
        if (existingPitch == null)
            return NotFound();

        if (pitch.Name != null) existingPitch.Name = pitch.Name;
        if (pitch.Description != null) existingPitch.Description = pitch.Description;

        existingPitch.Type = pitch.Type;
        existingPitch.SectorId = pitch.SectorId;

        await _pitchRepository.UpdateAsync(existingPitch);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var pitch = await _pitchRepository.GetByIdAsync(id);
        if (pitch == null)
            return NotFound();

        await _pitchRepository.DeleteAsync(pitch);
        return NoContent();
    }
} 