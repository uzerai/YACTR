using FastEndpoints;

namespace YACTR.Api.Endpoints.Pitches;

public class PitchesEndpointGroup : Group
{
    public PitchesEndpointGroup()
    {
        Configure("pitches", ep => { });
    }
}