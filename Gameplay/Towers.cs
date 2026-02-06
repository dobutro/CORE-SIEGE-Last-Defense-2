namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Быстрая пулемётная башня.
/// </summary>
public sealed class MachineGunTower : Tower
{
    private static readonly IReadOnlyList<TowerStats> LevelStats = new List<TowerStats>
    {
        new(damage: 6, fireRate: 4f, range: 3.5f, upgradeCost: 40),
        new(damage: 9, fireRate: 5f, range: 3.7f, upgradeCost: 60),
        new(damage: 12, fireRate: 6f, range: 4.0f, upgradeCost: 80),
    };

    public MachineGunTower(GridPosition position) : base("Пулемётная", position, LevelStats, baseCost: 50)
    {
    }

    protected override void Tick(float deltaTime, IList<Enemy> enemies, IList<Projectile> projectiles)
    {
        if (!IsReady)
        {
            return;
        }

        var target = AcquireTarget(enemies);
        if (target is null)
        {
            return;
        }

        target.ApplyDamage(Damage);
        SetCooldown();
    }
}

/// <summary>
/// Лазерная башня с постоянным лучом.
/// </summary>
public sealed class LaserTower : Tower
{
    private static readonly IReadOnlyList<TowerStats> LevelStats = new List<TowerStats>
    {
        new(damage: 18, fireRate: 1f, range: 3.8f, upgradeCost: 60),
        new(damage: 24, fireRate: 1f, range: 4.4f, upgradeCost: 90),
        new(damage: 30, fireRate: 1f, range: 5.0f, upgradeCost: 120),
    };

    public LaserTower(GridPosition position) : base("Лазерная", position, LevelStats, baseCost: 80)
    {
    }

    protected override void Tick(float deltaTime, IList<Enemy> enemies, IList<Projectile> projectiles)
    {
        var target = AcquireTarget(enemies);
        if (target is null)
        {
            return;
        }

        var dps = Damage;
        target.ApplyDamage(dps * deltaTime);
    }
}

/// <summary>
/// Ракетная башня с взрывным уроном.
/// </summary>
public sealed class RocketTower : Tower
{
    private static readonly IReadOnlyList<TowerStats> LevelStats = new List<TowerStats>
    {
        new(damage: 35, fireRate: 0.7f, range: 4.5f, upgradeCost: 80),
        new(damage: 45, fireRate: 0.8f, range: 4.8f, upgradeCost: 120),
        new(damage: 60, fireRate: 0.9f, range: 5.2f, upgradeCost: 150),
    };

    private readonly float[] _explosionRadius = { 1.2f, 1.5f, 1.8f };

    public RocketTower(GridPosition position) : base("Ракетная", position, LevelStats, baseCost: 110)
    {
    }

    protected override void Tick(float deltaTime, IList<Enemy> enemies, IList<Projectile> projectiles)
    {
        if (!IsReady)
        {
            return;
        }

        var target = AcquireTarget(enemies);
        if (target is null)
        {
            return;
        }

        var projectile = new Projectile(Position, target, Damage, _explosionRadius[Level - 1]);
        projectiles.Add(projectile);
        SetCooldown();
    }
}

/// <summary>
/// Башня замедления.
/// </summary>
public sealed class SlowTower : Tower
{
    private static readonly IReadOnlyList<TowerStats> LevelStats = new List<TowerStats>
    {
        new(damage: 2, fireRate: 1.2f, range: 3.5f, upgradeCost: 40),
        new(damage: 3, fireRate: 1.4f, range: 3.9f, upgradeCost: 65),
        new(damage: 4, fireRate: 1.6f, range: 4.3f, upgradeCost: 90),
    };

    private readonly float[] _slowMultiplier = { 0.7f, 0.6f, 0.5f };

    public SlowTower(GridPosition position) : base("Замедляющая", position, LevelStats, baseCost: 60)
    {
    }

    protected override void Tick(float deltaTime, IList<Enemy> enemies, IList<Projectile> projectiles)
    {
        if (!IsReady)
        {
            return;
        }

        var target = AcquireTarget(enemies);
        if (target is null)
        {
            return;
        }

        target.ApplyDamage(Damage);
        target.ApplySlow(_slowMultiplier[Level - 1], duration: 2.5f);
        SetCooldown();
    }
}

/// <summary>
/// Электрическая цепь.
/// </summary>
public sealed class ChainLightningTower : Tower
{
    private static readonly IReadOnlyList<TowerStats> LevelStats = new List<TowerStats>
    {
        new(damage: 16, fireRate: 1.1f, range: 4.0f, upgradeCost: 70),
        new(damage: 20, fireRate: 1.2f, range: 4.4f, upgradeCost: 95),
        new(damage: 26, fireRate: 1.4f, range: 4.8f, upgradeCost: 130),
    };

    private readonly int[] _chainCount = { 2, 3, 4 };

    public ChainLightningTower(GridPosition position) : base("Электрическая цепь", position, LevelStats, baseCost: 90)
    {
    }

    protected override void Tick(float deltaTime, IList<Enemy> enemies, IList<Projectile> projectiles)
    {
        if (!IsReady)
        {
            return;
        }

        var inRange = enemies
            .Where(enemy => enemy.IsAlive && enemy.Position.DistanceTo(Position) <= Range)
            .OrderByDescending(enemy => enemy.Progress)
            .Take(_chainCount[Level - 1])
            .ToList();

        if (!inRange.Any())
        {
            return;
        }

        foreach (var enemy in inRange)
        {
            enemy.ApplyDamage(Damage);
        }

        SetCooldown();
    }
}

/// <summary>
/// Огнемётная башня.
/// </summary>
public sealed class FlamethrowerTower : Tower
{
    private static readonly IReadOnlyList<TowerStats> LevelStats = new List<TowerStats>
    {
        new(damage: 8, fireRate: 2.0f, range: 3.2f, upgradeCost: 55),
        new(damage: 10, fireRate: 2.2f, range: 3.6f, upgradeCost: 80),
        new(damage: 12, fireRate: 2.4f, range: 4.0f, upgradeCost: 110),
    };

    private readonly float[] _burnDuration = { 2.5f, 3.2f, 4.0f };

    public FlamethrowerTower(GridPosition position) : base("Огнемёт", position, LevelStats, baseCost: 75)
    {
    }

    protected override void Tick(float deltaTime, IList<Enemy> enemies, IList<Projectile> projectiles)
    {
        if (!IsReady)
        {
            return;
        }

        var inRange = enemies
            .Where(enemy => enemy.IsAlive && enemy.Position.DistanceTo(Position) <= Range)
            .ToList();

        if (!inRange.Any())
        {
            return;
        }

        foreach (var enemy in inRange)
        {
            enemy.ApplyDamage(Damage);
            enemy.ApplyBurn(damagePerSecond: Damage * 0.4f, duration: _burnDuration[Level - 1]);
        }

        SetCooldown();
    }
}
