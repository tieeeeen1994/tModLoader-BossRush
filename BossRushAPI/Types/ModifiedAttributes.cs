namespace BossRushAPI.Types;

public struct ModifiedAttributes
{
    public float LifeMultiplier { get; private set; }
    public float DamageMultiplier { get; private set; }
    public float DefenseMultiplier { get; private set; }
    public int LifeFlatIncrease { get; private set; }
    public int DamageFlatIncrease { get; private set; }
    public int DefenseFlatIncrease { get; private set; }
    public bool ProjectilesAffected { get; private set; }

    public ModifiedAttributes()
    {
        LifeMultiplier = 1f;
        DamageMultiplier = 1f;
        DefenseMultiplier = 1f;
        LifeFlatIncrease = 0;
        DamageFlatIncrease = 0;
        DefenseFlatIncrease = 0;
    }

    public ModifiedAttributes(float lifeMultiplier = 1f, float damageMultiplier = 1f, float defenseMultiplier = 1f,
                              int lifeFlatIncrease = 0, int damageFlatIncrease = 0, int defenseFlatIncrease = 0,
                              bool projectilesAffected = false)
    {
        LifeMultiplier = lifeMultiplier;
        DamageMultiplier = damageMultiplier;
        DefenseMultiplier = defenseMultiplier;
        LifeFlatIncrease = lifeFlatIncrease;
        DamageFlatIncrease = damageFlatIncrease;
        DefenseFlatIncrease = defenseFlatIncrease;
        ProjectilesAffected = projectilesAffected;
    }

    public readonly int ComputeLife(int life) => ComputeStat(life, LifeFlatIncrease, LifeMultiplier);

    public readonly int ComputeDamage(int damage) => ComputeStat(damage, DamageFlatIncrease, DamageMultiplier);

    public readonly int ComputeDefense(int defense) => ComputeStat(defense, DefenseFlatIncrease, DefenseMultiplier);

    private readonly int ComputeStat(int baseStat, int flat, float multiplier)
    {
        return Util.RoundOff((baseStat + flat) * multiplier);
    }
}
