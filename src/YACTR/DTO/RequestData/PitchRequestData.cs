using YACTR.Data.Model.Location;

namespace YACTR.DTO.RequestData;

public record PitchRequestData(
    Guid SectorId,
    string Name,
    PitchType Type,
    string? Description,
    string? Grade);
