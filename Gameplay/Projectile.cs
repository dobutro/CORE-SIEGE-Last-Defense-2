namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Простая ракета, наносящая урон по области при попадании.
/// </summary>
public sealed class Projectile
{
    private const float Speed = 6f;

    public Projectile(GridPosition origin, Enemy target, float damage, float explosionRadius)
    {
        Position = origin;
        Target = target;
        Damage = damage;
        ExplosionRadius = explosionRadius;
    }

    public GridPosition Position { get; private set; }
    public Enemy Target { get; }
    public float Damage { get; }
    public float ExplosionRadius { get; }
    public bool HasExploded { get; private set; }

    public void Update(float deltaTime, IList<Enemy> enemies)
    {
        if (HasExploded || !Target.IsAlive)
        {
            return;
        }

        var distance = Position.DistanceTo(Target.Position);
        if (distance <= 0.5f)
        {
            Explode(enemies);
            return;
        }

        var directionX = Math.Sign(Target.Position.X - Position.X);
        var directionY = Math.Sign(Target.Position.Y - Position.Y);
        var step = Speed * deltaTime;
        Position = new GridPosition(Position.X + (int)(directionX * step), Position.Y + (int)(directionY * step));
    }

    private void Explode(IList<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            if (enemy.Position.DistanceTo(Position) <= ExplosionRadius)
            {
                enemy.ApplyDamage(Damage);
            }
        }

        HasExploded = true;
    }
}
