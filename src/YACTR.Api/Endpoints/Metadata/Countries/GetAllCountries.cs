using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model;

namespace YACTR.Api.Endpoints.Metadata.Countries;

public record GetAllCountriesResponseItem(
    int Id,
    string Name,
    string Code
);

public class GetAllCountries : Endpoint<EmptyRequest, List<GetAllCountriesResponseItem>>
{
    public required IRepository<CountryData> CountryDataRepository { get; init; }

    public override void Configure()
    {
        Get("countries");
        Group<MetadataEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken cancellationToken)
    {
        var result = await CountryDataRepository.BuildReadonlyQuery()
            .OrderBy(e => e.AdminName)
            .Select(e => new GetAllCountriesResponseItem(
                e.Id, e.AdminName, e.Code))
            .ToListAsync(cancellationToken);
        
        await Send.OkAsync(result, cancellationToken);
    }
}