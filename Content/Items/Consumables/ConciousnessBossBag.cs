using mahouSyoujyo.Common.ItemDropRules;
using mahouSyoujyo.Content.Items.Consumables.StatIncreaseItem;
using mahouSyoujyo.Content.Items.SpecialWeapon;
using mahouSyoujyo.Content.NPCs.BOSSes.Majo_Consciousness;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace mahouSyoujyo.Content.Items.Consumables
{
    // Basic code for a boss treasure bag
    public class ConsciousnessBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {

            //Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));// Makes the item have an animation while in world (not held.).
            //要结合SetDefaults()里面的ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = false; // dev armor will be dropped

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            //ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 36;
            Item.height = 32;
            Item.rare = ItemRarityID.Purple;
            Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
        }

        public override bool CanRightClick()
        {
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Info", 
                string.Format( this.GetLocalizedValue("info"),ModContent.ItemType<TimePlate>(),
                ModContent.ItemType<BlessOfCircles>(), ModContent.ItemType<RPGweapon>()
                    ) ) );
            base.ModifyTooltips(tooltips);
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // We have to replicate the expert drops from MinionBossBody here
            LeadingConditionRule defeatplant = new LeadingConditionRule(new Conditions.DownedPlantera());
            defeatplant.OnSuccess(ItemDropRule.ByCondition(new Conditions.DownedAllMechBosses(), ModContent.ItemType<RPGweapon>(), 2, 1, 1));
            LeadingConditionRule downedMoonLord = new LeadingConditionRule(new downedMoonLordDropCondition());
            downedMoonLord.OnSuccess(new CommonDrop(ModContent.ItemType<BlessOfCircles>(),1 ,1, 1));
            downedMoonLord.OnFailedConditions(new CommonDrop(ItemID.MoonLordLegs,1));
            itemLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(),ModContent.ItemType<TimePlate>(), 1));
            itemLoot.Add(ItemDropRule.ByCondition(new Conditions.IsPreHardmode(), ModContent.ItemType<TimePlate>(), 4));
            itemLoot.Add(downedMoonLord);
            itemLoot.Add(defeatplant);
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Majo_Consciousness>()));
        }
    }
}
