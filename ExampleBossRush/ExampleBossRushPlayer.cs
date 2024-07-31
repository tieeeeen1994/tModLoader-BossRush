using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush;

public class ExampleBossRushPlayer : ModPlayer
{
    public override void PostUpdate()
    {
        if (BRS.I.IsBossRushActive && BRS.I.ReferenceBoss is NPC boss)
        {
            switch (boss.type)
            {
                case NPCID.EaterofWorldsHead:
                    Player.ZoneCorrupt = true;
                    break;
                case NPCID.BrainofCthulhu:
                    Player.ZoneCrimson = true;
                    break;
                case NPCID.QueenBee:
                    Player.ZoneJungle = true;
                    break;
                case NPCID.Golem:
                    Player.ZoneLihzhardTemple = false;
                    goto case NPCID.Plantera;
                case NPCID.Plantera:
                    Player.ZoneJungle = false;
                    break;
            }
        }
    }
}
