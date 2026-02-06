namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Base tower class with upgradeable stats.
/// </summary>
public abstract class Tower
{
    private float _cooldown;

    protected Tower(string name, GridPosition position, IReadOnlyList<TowerStats> stats, int baseCost)
    {
        Name = name;
        Position = position;
        Stats = stats;
        BaseCost = baseCost;
        Level = 1;
        VisualVariant = $"Mk {Level}";
    }

    public string Name { get; }
    public GridPosition Position { get; }
    public int Level { get; private set; }
    public int MaxLevel => Stats.Count;
    public string VisualVariant { get; private set; }
    public int BaseCost { get; }
    public IReadOnlyList<TowerStats> Stats { get; }

    public float Range => Stats[Level - 1].Range;
    public float Damage => Stats[Level - 1].Damage;
    public float FireRate => Stats[Level - 1].FireRate;
    public int UpgradeCost => Stats[Level - 1].UpgradeCost;

    public bool CanUpgrade => Level < MaxLevel;

    public void Upgrade()
    {
        if (!CanUpgrade)
        {
            return;
        }

        Level++;
        VisualVariant = $"Mk {Level}";
        OnUpgrade();
    }

    public void Update(float deltaTime, IList<Enemy> enemies, IList<Projectile> projectiles)
    {
        _cooldown = MathF.Max(0, _cooldown - deltaTime);
        Tick(deltaTime, enemies, projectiles);
    }

    protected void SetCooldown() => _cooldown = 1f / FireRate;

    protected bool IsReady => _cooldown <= 0;

    protected Enemy? AcquireTarget(IList<Enemy> enemies)
    {
        return enemies
            .Where(enemy => enemy.IsAlive && enemy.Position.DistanceTo(Position) <= Range)
            .OrderBy(enemy => enemy.Progress)
            .LastOrDefault();
    }

    protected abstract void Tick(float deltaTime, IList<Enemy> enemies, IList<Projectile> projectiles);

    protected virtual void OnUpgrade()
    {
    }
}

public sealed record TowerStats(float Damage, float FireRate, float Range, int UpgradeCost);
