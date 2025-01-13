using mahouSyoujyo.Content.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace mahouSyoujyo.Content.Items
{
    public class GriefSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));
            // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.consumable = true;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 0, 0, 1);
            Item.rare = ItemRarityID.Purple;
            // 使用属性
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item4;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Item.consumable = true; // 设定为消耗品
            Item.maxStack = 9999;
            Item.shopCustomPrice =Item.buyPrice(0, 30, 0, 0);
        }


        public override bool CanUseItem(Player player)
        {
            if (player.HasBuff<MagicGirlPover>()) 
            {
                MGPlayer mgplayer = player.GetModPlayer<MGPlayer>();
                mgplayer.polluted_time = 0;
                return true; 
            }
            return false;
        }
        public override bool? UseItem(Player player)
        {

            return true;
        }
        public override bool ConsumeItem(Player player)
        {
            return true; // true是消耗，false不消耗
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool CanRightClick()
        {
            //Main.player[Item.playerIndexTheItemIsReservedFor].ClearBuff(ModContent.BuffType<MagicGirlPover>());
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we give the item name a rainbow effect.
            foreach (TooltipLine line in tooltips)
            {
                //Main.NewText(line.Name);
                if (line.Mod == "Terraria" && line.Name == "ItemName")
                {
                    line.OverrideColor = Main.DiscoColor;
                }
            }
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
              CreateRecipe()
                  .AddIngredient(ModContent.ItemType<PieceofGrief>(), 4)
                  .Register();
            
        }
    }
}