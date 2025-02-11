using mahouSyoujyo.Content.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace mahouSyoujyo.Content.Items.MagicWeapon
{
    public class Firstbow : ModItem
    {
        private float manabonus = 1f;
        private int delay = 1;
        private int shootspeed_bonus = 1;
        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(1, 1, 1);
            Item.damage = 20;
            Item.crit = 16;
            Item.DamageType = DamageClass.Magic;
            Item.width = 28;
            Item.height = 58;
            Item.noMelee = true;
            Item.mana = 10;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.reuseDelay = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.holdStyle = ItemHoldStyleID.HoldHeavy;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(platinum: 10,gold: 0, silver: 0, copper: 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item165;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HeartArrow>();
            Item.shootSpeed = 20f;
            //使用时不会转身
            Item.useTurn = false;
         //   Item.CommonMaxStack = 1;
         //   Item.consumable = false;
            
        }
        public override bool AllowPrefix(int pre)
        {
            return true;
        }
        //右键冷却
        int colddown = 0;
        public override bool CanUseItem(Player player)
        {
            manabonus=(float)Math.Min(36f ,(player.statMana*player.statMana)/2500f)+1f;
            shootspeed_bonus = 1;
            if (Item.mana >=50)
            {
                shootspeed_bonus = 2;
            };
            if (Item.mana >=75)
            {
                shootspeed_bonus = 3;
            };
            if (Item.mana >=100)
            {
                shootspeed_bonus = 4;

            };
            if (Item.mana >=125)
            {
                shootspeed_bonus = 5;

            };
            if (Item.mana >=150)
            {
                shootspeed_bonus = 6;

            };
            return true;
        }
        //允许你设置useStyle为 ItemUseStyleID.Shoot 且不是法杖的物品的使用贴图偏移量
        //public override Vector2?HoldoutOffset()
       // {
       //     return new Vector2(-10f, 0f);
       // }

        //HoldoutOrigin 只对 useStyle 为 ItemUseStyleID.Shoot 且是法杖的物品有用，能修改物品贴图旋转中心的偏移量。
       // public override Vector2? HoldoutOrigin()
       // {
       //     return new Vector2(-100f, -50f); 
       // }

        //允许右键使用武器
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        //在道具栏中每帧生效
        public override async void UpdateInventory(Player player)
        {
  
            if (colddown>0) { colddown--; }
            Item.mana=(int)(player.statMana/2);
        }

        //重写射击。return false的话默认射弹不发射。
        public override bool Shoot(Player player,EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {
            //如果当前蓝耗不低于150，则可以释放右键并清空蓝量，并进入15秒冷却；
            if (player.altFunctionUse ==2  && Item.mana>=150 && colddown <= 0)
            {
                //右键有15秒冷却
                colddown=900;
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MagicCircle>(),damage*(int)manabonus, knockback, player.whoAmI);
                player.statMana = 0;
                player.SetDummyItemTime(Item.useAnimation);
            }
            else {
            //正常生成箭矢
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HeartArrow>(),damage*(int)manabonus, knockback, player.whoAmI);
            player.SetItemTime(Item.useAnimation /  shootspeed_bonus);
            }
            return false;
        }



        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            tooltips.Add(new TooltipLine(Mod, "MagicGirlTips", this.GetLocalizedValue("MagicGirlTips")) { OverrideColor = Main.DiscoColor });

        }
        //编辑玩家手持动作
        public override void HoldItemFrame(Player player)
        {
            // 选中武器的时候设置为第(4+1)=5帧
            player.bodyFrame.Y = player.bodyFrame.Height * 4;
        }

        //}
        public override void AddRecipes()
        {
            //生成个数
            CreateRecipe(1);
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LivingWoodWand, 1);
            recipe.AddIngredient(ItemID.AegisCrystal, 5);
            recipe.AddIngredient(ItemID.ManaCrystal, 5);
            recipe.AddIngredient(ItemID.ShimmerArrow, 99);
            //recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            //   recipe.SetResult(this, 1)
            recipe.Register();

        }
    }

}
