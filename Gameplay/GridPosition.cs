namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Simple integer grid coordinates used for map tiles.
/// </summary>
public readonly record struct GridPosition(int X, int Y)
{
    public float DistanceTo(GridPosition other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
}
