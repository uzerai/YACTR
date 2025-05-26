using YACTR.DI.Repository.Interface;
using YACTR.DTO.RequestData;
using YACTR.Model.Location;
using Microsoft.AspNetCore.Mvc;

using Route = YACTR.Model.Location.Route;

namespace YACTR.Controllers;

[ApiController]
[Route("routes")]
public class RouteController : ControllerBase
{
    private readonly IEntityRepository<Route> _routeRepository;

    public RouteController(IEntityRepository<Route> routeRepository)
    {
        _routeRepository = routeRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Route>> Create([FromBody] RouteRequestData route)
    {
        var createdRoute = await _routeRepository.CreateAsync(new() {
            Name = route.Name,
            Description = route.Description,
            Grade = route.Grade,
            SectorId = route.SectorId,
            FirstAscentClimberName = route.FirstAscentClimberName,
            BolterName = route.BolterName,
            Pitches = [ ..route.Pitches.Select(p => new Pitch {
                Name = p.Name,
                Description = p.Description,
                Type = p.Type,
                SectorId = route.SectorId,
            })],
        });

        return Created(createdRoute.Id.ToString(), createdRoute);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Route>> GetById([FromRoute] Guid id)
    {
        var route = await _routeRepository.GetByIdAsync(id);
        if (route == null)
            return NotFound();
        return route;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Route>>> GetAll()
    {
        var routes = await _routeRepository.GetAllAsync();
        return Ok(routes);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] RouteRequestData route)
    {
        var existingRoute = await _routeRepository.GetByIdAsync(id);
        if (existingRoute == null)
            return NotFound();

        if (route.Name != null) existingRoute.Name = route.Name;
        if (route.Description != null) existingRoute.Description = route.Description;
        if (route.Grade != null) existingRoute.Grade = route.Grade;
        

        await _routeRepository.UpdateAsync(existingRoute);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var route = await _routeRepository.GetByIdAsync(id);
        if (route == null)
            return NotFound();

        await _routeRepository.DeleteAsync(route);
        return NoContent();
    }
} 