namespace BossRush;

public partial class BossRushSystem
{
    /// <summary>
    /// Contains information about the modifications to the stats of the boss.
    /// </summary>
    public struct ModifiedAttributes
    {

        /// <summary>
        /// Multiplier for the boss's life.
        /// Computed after the flat increase.
        /// This also affects its minions.
        /// </summary>
        public float lifeMultiplier { get; private set; }

        /// <summary>
        /// Multiplier for the boss's damage.
        /// Computed after the flat increase.
        /// This also affects its minions.
        /// </summary>
        public float damageMultiplier { get; private set; }

        /// <summary>
        /// Flat increase for the boss's life.
        /// Computed before the multiplier.
        /// This also affects its minions.
        /// </summary>
        public int lifeFlatIncrease { get; private set; }

        /// <summary>
        /// Flat increase for the boss's damage.
        /// Computed before the multiplier.
        /// This also affects its minions.
        /// </summary>
        public int damageFlatIncrease { get; private set; }

        /// <summary>
        /// Constructor for ModifiedAttributes.
        /// </summary>
        /// <param name="lifeMultiplier">Multiplier to life</param>
        /// <param name="damageMultiplier">Multiplier to damage</param>
        /// <param name="lifeFlatIncrease">Flat increase to life</param>
        /// <param name="damageFlatIncrease">Flat increase to damage</param>
        public ModifiedAttributes(float lifeMultiplier = 1f, float damageMultiplier = 1f,
                                  int lifeFlatIncrease = 0, int damageFlatIncrease = 0)
        {
            this.lifeMultiplier = lifeMultiplier;
            this.damageMultiplier = damageMultiplier;
            this.lifeFlatIncrease = lifeFlatIncrease;
            this.damageFlatIncrease = damageFlatIncrease;
        }
    }
}
