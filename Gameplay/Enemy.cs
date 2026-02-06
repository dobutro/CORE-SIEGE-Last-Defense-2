namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Base enemy type. Extend for specific stats/behaviors.
/// </summary>
public abstract class Enemy
{
    private readonly List<StatusEffect> _effects = new();
    private float _shield;

    protected Enemy(string name, float maxHp, float speed, int reward, int baseDamage, float shield = 0f)
    {
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
        Speed = speed;
        Reward = reward;
        BaseDamage = baseDamage;
        _shield = shield;
    }

    public string Name { get; }
    public float MaxHp { get; }
    public float Hp { get; private set; }
    public float Speed { get; private set; }
    public int Reward { get; }
    public int BaseDamage { get; }
    public bool IsFlying { get; protected init; }
    public bool CanDamageTowers { get; protected init; }
    public float Progress { get; private set; }
    public bool IsAlive => Hp > 0;

    public GridPosition Position { get; private set; }

    public void Update(float deltaTime, IReadOnlyList<GridPosition> path)
    {
        var speedMultiplier = 1f;
        foreach (var effect in _effects.ToList())
        {
            effect.Update(deltaTime);
            speedMultiplier *= effect.SpeedMultiplier;
            if (effect.IsExpired)
            {
                _effects.Remove(effect);
            }
        }

        Progress += Speed * speedMultiplier * deltaTime;
        var index = Math.Clamp((int)MathF.Floor(Progress), 0, path.Count - 1);
        Position = path[index];
    }

    public bool HasReachedBase(IReadOnlyList<GridPosition> path) => Progress >= path.Count - 1;

    public void ApplyDamage(float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (_shield > 0)
        {
            var absorbed = MathF.Min(_shield, amount * 0.6f);
            _shield -= absorbed;
            amount -= absorbed;
        }

        Hp = MathF.Max(0, Hp - amount);
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        _effects.Add(new StatusEffect("Замедление", duration, slowMultiplier));
    }

    public void ApplyBurn(float damagePerSecond, float duration)
    {
        _effects.Add(new BurnEffect(duration, damagePerSecond));
    }

    public void TickStatusDamage(float deltaTime)
    {
        foreach (var effect in _effects.OfType<BurnEffect>())
        {
            ApplyDamage(effect.DamagePerSecond * deltaTime);
        }
    }

    protected void SetShield(float shield) => _shield = shield;
}

public sealed class StatusEffect
{
    public StatusEffect(string name, float duration, float speedMultiplier)
    {
        Name = name;
        Duration = duration;
        SpeedMultiplier = speedMultiplier;
        Remaining = duration;
    }

    public string Name { get; }
    public float Duration { get; }
    public float SpeedMultiplier { get; }
    public float Remaining { get; private set; }
    public bool IsExpired => Remaining <= 0;

    public virtual void Update(float deltaTime)
    {
        Remaining -= deltaTime;
    }
}

public sealed class BurnEffect : StatusEffect
{
    public BurnEffect(float duration, float damagePerSecond)
        : base("Горение", duration, 1f)
    {
        DamagePerSecond = damagePerSecond;
    }

    public float DamagePerSecond { get; }
}
