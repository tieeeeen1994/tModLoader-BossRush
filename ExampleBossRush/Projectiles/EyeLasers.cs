using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.Projectiles;

public class EyeLasers : SharedBossProjectiles
{
    protected override List<int> ApplicableTypes => [ProjectileID.EyeLaser];

    protected override bool AbsoluteCheck => IsCurrentBoss(ApplicableBosses);

    protected override void CalculateDamage(List<NPC> bosses, ref int damage, ref int hits, ref float multiplier)
    {
        FetchBoss(bosses, NPCID.WallofFlesh, ref damage, ref hits, ref multiplier, .06f);
        FetchBoss(bosses, NPCID.Retinazer, ref damage, ref hits, ref multiplier, .12f);
    }

    private List<int> ApplicableBosses => [NPCID.Retinazer, NPCID.WallofFlesh];
}
