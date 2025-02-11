
using mahouSyoujyo.Common.Configs;
using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content.NPCs.BOSSes.Majo_Consciousness;
using mahouSyoujyo.Content.Projectiles;
using mahouSyoujyo.Globals;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace mahouSyoujyo.Content.Items.SpecialWeapon
{
    // ExampleDrill closely mimics Titanium Drill, except where noted.
    // Of note, this example showcases Item.tileBoost and teaches the basic concepts of a held projectile.
    public class RPGweapon : ModItem
    {
        int right_colddown = 0;
        int bomb = 0;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic; // ignores melee speed bonuses. There's no need for drill animations to play faster, nor drills to dig faster with melee speed.
            Item.width = 72;
            Item.height = 72;
            // IsDrill/IsChainsaw effects must be applied manually, so 60% or 0.6 times the time of the corresponding pickaxe. In this case, 60% of 7 is 4 and 60% of 25 is 15.
            // If you decide to copy values from vanilla drills or chainsaws, you should multiply each one by 0.6 to get the expected behavior.
            Item.autoReuse = true;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.reuseDelay =0;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(10, 0, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.DD2_KoboldExplosion;
            Item.shoot = ModContent.ProjectileType<HeartArrow>(); // Create the  projectile
            Item.shootSpeed = 30f; // 
            Item.noMelee = true; // 
            Item.noUseGraphic = false; // 
            Item.channel = true; // 
            Item.useTurn = false;
            Item.mana = 20;
            Item.staff[Type] = false;
            

        }
        public override void UpdateInventory(Player player)
        {
            if (right_colddown>0) right_colddown--;
            base.UpdateInventory(player);
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (right_colddown <= 0 && player.altFunctionUse == 2)
            {
                bomb = 3;
                right_colddown = (TimeStopSystem.TimeStopping)? 90:180;
            }
            if (player.altFunctionUse == 2 && bomb <= 0) return false;
            return true;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (TimeStopSystem.TimeStopping) mult *=2;
            base.ModifyManaCost(player, ref reduce, ref mult);
        }
        public override float UseAnimationMultiplier(Player player)
        {
            if (TimeStopSystem.TimeStopping) return 0.5f;
            return 1f;
        }
        public override float UseTimeMultiplier(Player player)
        {
            if (bomb>0)
            {
                return 0.15f;
            }
            return (TimeStopSystem.TimeStopping) ? 0.5f : 1f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position -= new Vector2(0f,8f);
            if (Main.netMode != NetmodeID.Server)
            {
                PunchCameraModifier modifier = new PunchCameraModifier(player.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
                Main.instance.CameraModifiers.Add(modifier);
            }

            var mysource = player.GetSource_ItemUse_WithPotentialAmmo(this.Item, AmmoID.None);
            if (bomb>0)
            {
                if (!TimeStopSystem.TimeStopping) player.velocity -= velocity.SafeNormalize(Vector2.Zero) * 5f;
                bomb--;
                if (player.whoAmI == Main.myPlayer)
                {
                    var proj = Projectile.NewProjectileDirect(mysource, position, velocity, ModContent.ProjectileType<TimeMissile>(), damage, knockback);
                    if (proj.ModProjectile is TimeMissile missile && proj.active)
                    {
                        missile.runtime = 360;
                        missile.Projectile.netUpdate = true;
                    }
                }
                if (bomb == 0) player.itemAnimation = 0;
                return false;
            }
            
            if (!TimeStopSystem.TimeStopping) player.velocity -= velocity.SafeNormalize(Vector2.Zero) * 10f;
            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectileDirect(mysource, position, velocity, ModContent.ProjectileType<TimeMissile>(), damage, knockback);
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-38f, -10f);
        }
        public override void ModifyItemScale(Player player, ref float scale)
        {
            scale = 1.4f;
            base.ModifyItemScale(player, ref scale);
        }
        public override void UseItemFrame(Player player)
        {
            player.bodyFrame.Y = player.bodyFrame.Height * 2;
            base.UseItemFrame(player);
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            Vector2 vel = player.GetModPlayer<PlayerState>().clientmouse-player.Center+new Vector2(0f, 8f); 
            player.itemRotation = vel.ToRotation()+ MathHelper.ToRadians((player.direction ==1) ? 45f : 135f);
            if (player.direction == 1)
            {
                if (vel.X<0)
                    player.itemRotation = MathHelper.ToRadians((vel.Y >= 0f) ? 130f : -40f);
            }
            else
            {
                if (vel.X>0)
                    player.itemRotation = MathHelper.ToRadians((vel.Y >= 0f) ? -130f : 40f);
            }
            
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            base.HoldStyle(player, heldItemFrame);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            tooltips.Add(new TooltipLine(Mod, "MagicGirlTips", this.GetLocalizedValue("MagicGirlTips")) { OverrideColor = Main.DiscoColor });

        }
        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<>()
            //    .AddTile<>()
            //    .Register();
        }
    }
}
