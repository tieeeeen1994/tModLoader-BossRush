﻿using BossRushAPI;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;
using BRS = BossRushAPI.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public class Sharknados : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [ProjectileID.Cthulunado, ProjectileID.Sharknado];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.DukeFishron);

    protected override void Update(Projectile projectile)
    {
        if ((projectile.type == ProjectileID.Cthulunado ||
            projectile.type == ProjectileID.Sharknado) &&
            BRS.I.ReferenceBoss is NPC boss)
        {
            projectile.damage = Util.RoundOff(boss.damage * .2f);
        }
    }
}
