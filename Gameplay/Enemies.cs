namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// конкретные типы врагов для волн.
/// </summary>
public sealed class BasicEnemy : Enemy
{
    public BasicEnemy() : base("Обычный", maxHp: 40, speed: 1.2f, reward: 10, baseDamage: 1)
    {
    }
}

public sealed class FastEnemy : Enemy
{
    public FastEnemy() : base("Быстрый", maxHp: 25, speed: 2.4f, reward: 12, baseDamage: 1)
    {
    }
}

public sealed class HeavyEnemy : Enemy
{
    public HeavyEnemy() : base("Тяжёлый", maxHp: 120, speed: 0.8f, reward: 25, baseDamage: 2)
    {
    }
}

public sealed class FlyingEnemy : Enemy
{
    public FlyingEnemy() : base("Летающий", maxHp: 50, speed: 2.0f, reward: 18, baseDamage: 1)
    {
        IsFlying = true;
    }
}

public sealed class ShieldEnemy : Enemy
{
    public ShieldEnemy() : base("Щитовой", maxHp: 70, speed: 1.0f, reward: 22, baseDamage: 1, shield: 40)
    {
    }
}

public sealed class DestroyerEnemy : Enemy
{
    public DestroyerEnemy() : base("Разрушитель", maxHp: 90, speed: 1.1f, reward: 30, baseDamage: 2)
    {
        CanDamageTowers = true;
    }
}
