using mahouSyoujyo.Common.Configs;
using mahouSyoujyo.Content.Buffs;
using mahouSyoujyo.Content.Items;
using mahouSyoujyo.Content.Items.MeleeWeapon;
using mahouSyoujyo.Content.Items.RangedWeapon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace mahouSyoujyo.Globals
{
    public class MGNPCdrops : GlobalNPC
    {

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!npc.CountsAsACritter && !npc.boss && !npc.townNPC && !ModContent.GetInstance<ServerConfigs>().NoSeedDrop)
            {
                LeadingConditionRule con_BloodMoonAndnotStatue = new LeadingConditionRule(new Conditions.IsBloodMoonAndNotFromStatue());
                LeadingConditionRule con_notStatue = new LeadingConditionRule(new Conditions.NotFromStatue());
                con_notStatue.OnSuccess(new CommonDrop(ModContent.ItemType<PieceofGrief>(), 10, 1, 2));
                con_BloodMoonAndnotStatue.OnSuccess(new CommonDrop(ModContent.ItemType<GriefSeed>(), 50, 1));
                con_BloodMoonAndnotStatue.OnFailedConditions(con_notStatue);
                npcLoot.Add(con_BloodMoonAndnotStatue);
            }
            if (npc.boss)
                npcLoot.Add(new CommonDrop(ModContent.ItemType<GriefSeed>(), 1, 1));

            if (npc.type ==NPCID.DukeFishron && !Main.expertMode)
                npcLoot.Add(new CommonDrop(ModContent.ItemType<BlueSword>(), 3, 1));
            if (npc.type ==NPCID.HallowBoss && !Main.expertMode)
                npcLoot.Add(new CommonDrop(ModContent.ItemType<ColorFive>(), 3, 1));

        }


    }
    public class MGItemdrops : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.FishronBossBag)
            {
                itemLoot.Add(new CommonDrop(ModContent.ItemType<BlueSword>(), 2, 1));
            }
            if (item.type == ItemID.FairyQueenBossBag)
            {
                itemLoot.Add(new CommonDrop(ModContent.ItemType<ColorFive>(), 2, 1));
            }
            base.ModifyItemLoot(item, itemLoot);
        }


    }
}