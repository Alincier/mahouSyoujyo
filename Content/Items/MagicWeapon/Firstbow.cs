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
        int stic = 0;float delaymulti=1f;
        float manabonus = 1f;
        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(1, 1, 1);
            Item.damage = 10;
            Item.crit = 16;
            Item.DamageType = DamageClass.Magic;
            Item.width = 28;
            Item.height = 58;
            Item.noMelee = true;
            Item.mana = 10;
            Item.useTime = 90;
            Item.useAnimation = 90;
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
            manabonus=(float)Math.Min(64f ,(player.statMana*player.statMana)/2500);

            //     if (player.altFunctionUse == 2)
            if (colddown <=0)
            {
                return true;
            }
            return false;
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
    /*        if (stic <= 90) { stic++; };
            if (stic >= 90) { Item.useTime = 90; };
            if (player.statMana >=100 & Item.useTime >60)
            {
                Item.useTime = 45; stic = 0;
            };
            if (player.statMana >=150 & Item.useTime >38)
            {
                Item.useTime = 30; stic = 0;
            };
            if (player.statMana >=200)
            {
                Item.useTime = 10; stic = 0;
            };*/
            if (stic <= 90) { stic++; }
            Item.mana=(int)player.statMana /2;
        }

        //重写射击。return false的话默认射弹不发射。
        public override bool Shoot(Player player,EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {
            //如果当前蓝耗不低于200 且 满蓝，则可以释放右键并清空蓝量，并进入15秒冷却；
            if (player.altFunctionUse ==2  && Item.mana>=200 )
            {
                //右键有15秒冷却
                colddown=900;
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MagicCircle>(),damage*(int)manabonus, knockback, player.whoAmI);
                player.statMana = 0;
            }
            else {
            //正常生成箭矢
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HeartArrow>(),damage*(int)manabonus, knockback, player.whoAmI);
            }
            return false;
        }

        //使用时间的乘数。默认为1f。
        public override float UseTimeMultiplier(Player player)
        {
            float multi = 1f;
            if (stic >= 45) { delaymulti=1f; }
            if (Item.mana >=25)
            {
                multi=2f;
                stic=0;
                if (delaymulti<multi) { delaymulti=multi; }
            };
            if (Item.mana >=50)
            {
                multi=3f;
                stic=0;
                if (delaymulti<multi) { delaymulti=multi; }
            };
            if (Item.mana >=75)
            {
                multi=4f;
                stic=0;
                if (delaymulti<multi) { delaymulti=multi; }

            };
            if (Item.mana >=100)
            {
                multi=6f;
                stic=0;
                if (delaymulti<multi) { delaymulti=multi; }

            };
            if (Item.mana >=150)
            {
                multi=9f;
                stic=0;
                if (delaymulti<multi) { delaymulti=multi; }

            };
            if (player.altFunctionUse ==2) return 1f;
            return MathHelper.Min(1f/delaymulti,1f/multi);

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
