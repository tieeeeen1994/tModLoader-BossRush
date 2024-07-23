// using Terraria;
// using Terraria.DataStructures;
// using Terraria.ID;
// using Terraria.ModLoader;
// using BRS = BossRush.BossRushSystem;

// namespace BossRush.NPCs.Old;

// public class BossAndSlaves : GlobalNPC
// {
//     public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => lateInstantiation;

//     public override void SetDefaults(NPC npc)
//     {
//         if (BRS.I.IsBossRushActive && BRS.I.CurrentBossData?.ModifiedAttributes is { } attributes)
//         {
//             npc.lifeMax = attributes.ComputeLife(npc.lifeMax);
//             npc.damage = attributes.ComputeDamage(npc.damage);
//             npc.defense = attributes.ComputeDefense(npc.defense);
//         }
//     }

//     public override void OnSpawn(NPC npc, IEntitySource source)
//     {
//         if (BRS.I.IsBossRushActive && BRS.I.CurrentBoss != null &&
//             BRS.I.CurrentBossData?.SubTypes?.Contains(npc.type) == true)
//         {
//             BRS.I.DynamicAddBoss(npc);
//         }
//     }

//     public override void OnKill(NPC npc)
//     {
//         if (BRS.I.IsBossRushActive && BRS.I.State == BRS.States.Run && BRS.I.CurrentBoss.Contains(npc))
//         {
//             BRS.I.MarkBossDefeat(npc);
//         }
//     }

//     public override void PostAI(NPC npc)
//     {
//         if (Main.netMode != NetmodeID.MultiplayerClient && BRS.I.IsBossRushActive &&
//             BRS.I.CurrentBoss != null && BRS.I.CurrentBossData is { } bossData)
//         {
//             bossData.BossUpdate(npc, bossData.AI);
//         }
//     }
// }
