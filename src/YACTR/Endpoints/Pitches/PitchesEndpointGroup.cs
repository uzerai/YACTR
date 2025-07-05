using FastEndpoints;

namespace YACTR.Endpoints;

public class PitchesEndpointGroup : Group
{
    public PitchesEndpointGroup()
    {
        Configure("pitches", ep => {});
    }
} 