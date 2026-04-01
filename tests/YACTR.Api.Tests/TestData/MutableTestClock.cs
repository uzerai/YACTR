using NodaTime;

namespace YACTR.Api.Tests.TestData;

public sealed class MutableTestClock : IClock
{
    private readonly Lock _currentInstantLock = new();
    private Instant _currentInstant;

    public MutableTestClock(Instant initialInstant)
    {
        _currentInstant = initialInstant;
    }

    public Instant GetCurrentInstant()
    {
        lock (_currentInstantLock)
        {
            return _currentInstant;
        }
    }

    public void SetCurrentInstant(Instant instant)
    {
        lock (_currentInstantLock)
        {
            _currentInstant = instant;
        }
    }

    public void Advance(Duration duration)
    {
        lock (_currentInstantLock)
        {
            _currentInstant = _currentInstant.Plus(duration);
        }
    }

    public void Rewind(Duration duration)
    {
        lock (_currentInstantLock)
        {
            _currentInstant = _currentInstant.Minus(duration);
        }
    }
}
