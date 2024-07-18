namespace BossRush;

public partial class BossRushSystem
{
    public struct ModifiedAttributes
    {
        public float LifeMultiplier { get; private set; }
        public float DamageMultiplier { get; private set; }
        public int LifeFlatIncrease { get; private set; }
        public int DamageFlatIncrease { get; private set; }

        public ModifiedAttributes()
        {
            LifeMultiplier = 1f;
            DamageMultiplier = 1f;
            LifeFlatIncrease = 0;
            DamageFlatIncrease = 0;
        }

        public ModifiedAttributes(float lifeMultiplier = 1f, float damageMultiplier = 1f,
                                  int lifeFlatIncrease = 0, int damageFlatIncrease = 0)
        {
            LifeMultiplier = lifeMultiplier;
            DamageMultiplier = damageMultiplier;
            LifeFlatIncrease = lifeFlatIncrease;
            DamageFlatIncrease = damageFlatIncrease;
        }
    }
}
