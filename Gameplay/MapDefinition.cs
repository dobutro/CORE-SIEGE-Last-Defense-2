using System.Collections.ObjectModel;

namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Holds pathing data and build slots for a map.
/// </summary>
public sealed class MapDefinition
{
    public MapDefinition(
        string name,
        int width,
        int height,
        IReadOnlyList<GridPosition> path,
        IReadOnlyCollection<GridPosition> buildSlots,
        IReadOnlyList<GridPosition>? flyingPath = null)
    {
        Name = name;
        Width = width;
        Height = height;
        Path = new ReadOnlyCollection<GridPosition>(path.ToList());
        BuildSlots = new ReadOnlyCollection<GridPosition>(buildSlots.ToList());
        FlyingPath = flyingPath is null
            ? Path
            : new ReadOnlyCollection<GridPosition>(flyingPath.ToList());
    }

    public string Name { get; }
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<GridPosition> Path { get; }
    public IReadOnlyList<GridPosition> FlyingPath { get; }
    public IReadOnlyCollection<GridPosition> BuildSlots { get; }

    public static IReadOnlyList<MapDefinition> CreateDefaultMaps()
    {
        return new List<MapDefinition>
        {
            CreateDesertMap(),
            CreateLabyrinthMap(),
            CreateCityMap()
        };
    }

    private static MapDefinition CreateDesertMap()
    {
        var path = new List<GridPosition>();
        for (var x = 0; x < 12; x++)
        {
            path.Add(new GridPosition(x, 4));
        }

        var buildSlots = new HashSet<GridPosition>
        {
            new(2, 2),
            new(4, 2),
            new(6, 2),
            new(8, 2),
            new(2, 6),
            new(4, 6),
            new(6, 6),
            new(8, 6),
        };

        return new MapDefinition("Пустыня", 12, 9, path, buildSlots);
    }

    private static MapDefinition CreateLabyrinthMap()
    {
        var path = new List<GridPosition>
        {
            new(0, 1),
            new(1, 1),
            new(2, 1),
            new(2, 2),
            new(2, 3),
            new(3, 3),
            new(4, 3),
            new(4, 4),
            new(4, 5),
            new(5, 5),
            new(6, 5),
            new(6, 4),
            new(6, 3),
            new(7, 3),
            new(8, 3),
            new(9, 3),
            new(10, 3),
            new(11, 3)
        };

        var buildSlots = new HashSet<GridPosition>
        {
            new(1, 4),
            new(3, 1),
            new(3, 5),
            new(5, 1),
            new(5, 6),
            new(7, 1),
            new(7, 5),
            new(9, 1),
            new(9, 5)
        };

        return new MapDefinition("Лабиринт", 12, 8, path, buildSlots);
    }

    private static MapDefinition CreateCityMap()
    {
        var path = new List<GridPosition>
        {
            new(0, 2),
            new(1, 2),
            new(2, 2),
            new(3, 2),
            new(4, 2),
            new(5, 2),
            new(6, 2),
            new(7, 2),
            new(8, 2),
            new(9, 2),
            new(10, 2),
            new(11, 2)
        };

        var flyingPath = new List<GridPosition>
        {
            new(0, 0),
            new(3, 1),
            new(6, 2),
            new(9, 3),
            new(11, 4)
        };

        var buildSlots = new HashSet<GridPosition>
        {
            new(2, 5),
            new(4, 5),
            new(6, 5),
            new(8, 5),
            new(2, 1),
            new(4, 1),
            new(6, 1),
            new(8, 1),
        };

        return new MapDefinition("Город", 12, 7, path, buildSlots, flyingPath);
    }
}
