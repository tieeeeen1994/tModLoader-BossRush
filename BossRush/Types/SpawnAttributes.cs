namespace BossRush.Types;

public struct SpawnAttributes(float maxMultiplier = 1f, int maxFlatIncrease = 0, float rateMultiplier = 1f)
{
    public float RateMultiplier { get; private set; } = rateMultiplier;
    public int MaxFlatIncrease { get; private set; } = maxFlatIncrease;
    public float MaxMultiplier { get; private set; } = maxMultiplier;

    public static SpawnAttributes NoSpawns => new(maxMultiplier: 0, rateMultiplier: 0f);
    public static SpawnAttributes Default => new();
    public static SpawnAttributes DoubleSpawns => new(maxMultiplier: 2f, rateMultiplier: 2f);
}
