namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Управляет волнами, экономикой и состоянием уровня.
/// </summary>
public sealed class Level
{
    private readonly Queue<WaveDefinition> _waves;
    private readonly Dictionary<Tower, int> _towerIntegrity = new();
    private float _spawnTimer;
    private WaveDefinition? _currentWave;
    private int _spawnedInWave;

    public Level(MapDefinition map)
    {
        Map = map;
        BaseHp = 10;
        Money = 150;
        _waves = new Queue<WaveDefinition>(WaveDefinition.CreateDefaultWaves());
    }

    public MapDefinition Map { get; }
    public int BaseHp { get; private set; }
    public int Money { get; private set; }
    public bool IsGameOver => BaseHp <= 0;
    public bool IsCompleted => !_waves.Any() && !Enemies.Any();

    public IList<Enemy> Enemies { get; } = new List<Enemy>();
    public IList<Tower> Towers { get; } = new List<Tower>();
    public IList<Projectile> Projectiles { get; } = new List<Projectile>();

    public bool CanBuildTower(GridPosition position) =>
        Map.BuildSlots.Contains(position) && Towers.All(tower => tower.Position != position);

    public bool BuildTower(Tower tower)
    {
        if (!CanBuildTower(tower.Position) || Money < tower.BaseCost)
        {
            return false;
        }

        Money -= tower.BaseCost;
        Towers.Add(tower);
        _towerIntegrity[tower] = 3;
        return true;
    }

    public bool UpgradeTower(Tower tower)
    {
        if (!tower.CanUpgrade || Money < tower.UpgradeCost)
        {
            return false;
        }

        Money -= tower.UpgradeCost;
        tower.Upgrade();
        return true;
    }

    public void Update(float deltaTime)
    {
        if (IsGameOver)
        {
            return;
        }

        HandleSpawning(deltaTime);

        foreach (var enemy in Enemies)
        {
            var path = enemy.IsFlying ? Map.FlyingPath : Map.Path;
            enemy.Update(deltaTime, path);
            enemy.TickStatusDamage(deltaTime);
        }

        foreach (var tower in Towers)
        {
            tower.Update(deltaTime, Enemies, Projectiles);
        }

        foreach (var projectile in Projectiles)
        {
            projectile.Update(deltaTime, Enemies);
        }

        CleanupEntities();
        HandleEnemyContact();
    }

    private void HandleSpawning(float deltaTime)
    {
        if (_currentWave is null && _waves.Any())
        {
            _currentWave = _waves.Dequeue();
            _spawnedInWave = 0;
            _spawnTimer = 0;
        }

        if (_currentWave is null)
        {
            return;
        }

        _spawnTimer -= deltaTime;
        if (_spawnTimer > 0)
        {
            return;
        }

        if (_spawnedInWave < _currentWave.Count)
        {
            Enemies.Add(_currentWave.Factory());
            _spawnedInWave++;
            _spawnTimer = _currentWave.SpawnInterval;
        }
        else if (!Enemies.Any(enemy => enemy.IsAlive))
        {
            _currentWave = null;
        }
    }

    private void CleanupEntities()
    {
        foreach (var enemy in Enemies.Where(enemy => !enemy.IsAlive).ToList())
        {
            Enemies.Remove(enemy);
            Money += enemy.Reward;
        }

        foreach (var projectile in Projectiles.Where(p => p.HasExploded).ToList())
        {
            Projectiles.Remove(projectile);
        }
    }

    private void HandleEnemyContact()
    {
        foreach (var enemy in Enemies.ToList())
        {
            var path = enemy.IsFlying ? Map.FlyingPath : Map.Path;
            if (enemy.HasReachedBase(path))
            {
                BaseHp -= enemy.BaseDamage;
                Enemies.Remove(enemy);
                continue;
            }

            if (!enemy.CanDamageTowers)
            {
                continue;
            }

            foreach (var tower in Towers.ToList())
            {
                if (tower.Position.DistanceTo(enemy.Position) > 0)
                {
                    continue;
                }

                _towerIntegrity[tower]--;
                if (_towerIntegrity[tower] <= 0)
                {
                    Towers.Remove(tower);
                    _towerIntegrity.Remove(tower);
                }

                break;
            }
        }
    }
}

public sealed record WaveDefinition(Func<Enemy> Factory, int Count, float SpawnInterval)
{
    public static IReadOnlyList<WaveDefinition> CreateDefaultWaves()
    {
        return new List<WaveDefinition>
        {
            new(() => new BasicEnemy(), 8, 0.8f),
            new(() => new FastEnemy(), 10, 0.6f),
            new(() => new HeavyEnemy(), 6, 1.1f),
            new(() => new ShieldEnemy(), 6, 0.9f),
            new(() => new FlyingEnemy(), 8, 0.7f),
            new(() => new DestroyerEnemy(), 5, 1.2f),
            new(() => new HeavyEnemy(), 8, 1.0f),
            new(() => new FastEnemy(), 12, 0.5f),
            new(() => new ShieldEnemy(), 10, 0.8f),
            new(() => new DestroyerEnemy(), 8, 1.1f),
        };
    }
}
