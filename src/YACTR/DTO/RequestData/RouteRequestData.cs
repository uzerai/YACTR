namespace YACTR.DTO.RequestData;

public record RouteRequestData(
    Guid SectorId,
    PitchRequestData[] Pitches,
    string Name,
    string? Description,
    string? Grade,
    string? FirstAscentClimberName,
    string? BolterName);
