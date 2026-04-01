using NodaTime;

namespace YACTR.Api.Tests;

public sealed class MutableTestClock : IClock
{
    private readonly object _sync = new();
    private Instant _currentInstant;

    public MutableTestClock(Instant initialInstant)
    {
        _currentInstant = initialInstant;
    }

    public Instant GetCurrentInstant()
    {
        lock (_sync)
        {
            return _currentInstant;
        }
    }

    public void SetCurrentInstant(Instant instant)
    {
        lock (_sync)
        {
            _currentInstant = instant;
        }
    }

    public void Advance(Duration duration)
    {
        lock (_sync)
        {
            _currentInstant = _currentInstant.Plus(duration);
        }
    }
}
