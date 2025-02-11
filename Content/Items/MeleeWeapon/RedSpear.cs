using mahouSyoujyo.Content.Projectiles;
using mahouSyoujyo.Content.Projectiles.Weapon;
using mahouSyoujyo.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static mahouSyoujyo.mahouSyoujyo;
using Terraria.ModLoader.IO;
using rail;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using mahouSyoujyo.Content.Buffs;
using System.Runtime.CompilerServices;
using Humanizer;

namespace mahouSyoujyo.Content.Items.MeleeWeapon;

// This is a basic item template.
// Please see tModLoader's ExampleMod for every other example:
// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
public class RedSpear : ModItem
{
    public int cd = 0;
    public int usetime = 0;
    //public int consume = 0;
    // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.mahouSyoujyo.hjson' file.
    public override void SetDefaults()
    {

        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.damage = 200;
        Item.DefaultToRangedWeapon(ProjectileID.Bullet, 0, 6, 30, true);
        Item.DamageType = DamageClass.Magic;
        Item.width = 80;
        Item.height = 80;
        Item.useTime = 1;
        Item.useAnimation = 1;
        Item.noUseGraphic = true;
        //关闭使用时的贴图绘制
        //Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.holdStyle = ItemHoldStyleID.None;
        Item.knockBack = 5;
        Item.value = Item.buyPrice(platinum: 5, gold: 0, silver: 0, copper: 0);
        Item.rare = ItemRarityID.Red;
        Item.UseSound = null;
        Item.autoReuse = true;
        Item.useTurn = false;
        Item.useAmmo =AmmoID.None;
        Item.shootSpeed = 20f;
        Item.shoot =ProjectileID.Bullet;
        Item.channel = true;
        Item.scale = 0.5f;
        Item.mana =25;
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
    {
        if (player.GetModPlayer<RedSpearComboing>().shouldCostMana) return;
        else mult *= 0;
        base.ModifyManaCost(player, ref reduce, ref mult);
    }
    public override void UpdateInventory(Player player)
    {
        if (player.channel)
        {
            usetime++;
            cd++;
        }
        else
        {
            usetime = 0;
            cd = 0;
        }

    }
    public override void HoldItem(Player player)
    {
        player.GetModPlayer<LocalUIPlayer>().RedSpearChargeBar = true;
        base.HoldItem(player);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.LocalPlayer != player) return false;

        if (player.altFunctionUse ==2)
        {
            player.GetModPlayer<RedSpearComboing>().backing();
            return false;
        } 
        else
        {
            //↑+左:投矛
            if (!player.releaseUp)
            {
                if (player.GetModPlayer<RedSpearComboing>().spearcount >= player.GetModPlayer<RedSpearComboing>().maxspear) return false;
                Projectile.NewProjectile(source,
                    player.Center+(Main.MouseWorld-player.Center).SafeNormalize(Vector2.Zero)*16,
                    (Main.MouseWorld-player.Center).SafeNormalize(Vector2.Zero)*Item.shootSpeed,
                    ModContent.ProjectileType<RedSpearBullet>(), //ModContent.ProjectileType<YellowGunBullet>(),
                    damage, knockback, player.whoAmI);
                player.SetDummyItemTime(10);
                return false;
            }
            if (player.GetModPlayer<RedSpearComboing>().spearcount >= player.GetModPlayer<RedSpearComboing>().maxspear) return false;
            Projectile.NewProjectile(source,
                player.Center,
                new Vector2(player.direction, 1f)//(Main.MouseWorld-player.Center)
                .SafeNormalize(Vector2.Zero)*Item.shootSpeed,
                ModContent.ProjectileType<RedSpearBend>(), //ModContent.ProjectileType<YellowGunBullet>(),
                damage, knockback, player.whoAmI);
            player.SetDummyItemTime(10);
        }
            

        return false;

    }
    
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        foreach (TooltipLine line in tooltips) 
        {
            if (line.Name.EndsWith("UseMana"))
                line.Text =this.GetLocalizedValue("ManaInfo").FormatWith((int)(Main.LocalPlayer.manaCost * (float)Item.mana));
        }
        tooltips.Add(new TooltipLine(Mod, "MagicGirlTips", this.GetLocalizedValue("MagicGirlTips")) { OverrideColor = Main.DiscoColor });
        
    }
    //public override float UseSpeedMultiplier(Player player)
    //{
    //    return 1f+gun;
    //}
    public override void AddRecipes()
    {
        //生成个数
        CreateRecipe(1);
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.SunStone, 1);
        recipe1.AddIngredient(ItemID.Chain, 5);
        recipe1.AddIngredient(ItemID.AdamantiteGlaive, 1);
        recipe1.AddTile(TileID.MythrilAnvil);
        recipe1.Register();
        CreateRecipe(1);
        Recipe recipe2 = CreateRecipe();
        recipe2.AddIngredient(ItemID.SunStone, 1);
        recipe2.AddIngredient(ItemID.Chain, 5);
        recipe2.AddIngredient(ItemID.TitaniumTrident, 1);
        recipe2.AddTile(TileID.MythrilAnvil);
        recipe2.Register();

    }
}

public class RedSpearComboing : ModPlayer
{
    public int  remaintime=0;
    public bool returning = false;
    public int spearcount = 1;
    public int maxspear = 1;
    public bool shouldCostMana = false;
    public int swing_charge = 0;
    public int bullet_charge = 0;
    public int chainCount = 0;
    public int chainCooldown = 0;
    public int spearProj = -1; 
    private int maxchainProj = 16;
    public override void ResetEffects()
    {
        spearProj = -1;
        shouldCostMana=false;
        if (swing_charge > 0) swing_charge--;
        else swing_charge = 0;
        if (bullet_charge > 0) bullet_charge--;
        else bullet_charge = 0;
        returning = false;
        //if (Player.magic().magia)
        //    maxspear = 2;
        //else maxspear = 1;
        spearcount = 0;
        foreach (Projectile proj in Main.ActiveProjectiles){
            if (proj.ModProjectile is RedSpearBullet && proj.owner == Player.whoAmI)
            {
                spearcount++;
            }
        }
        base.ResetEffects();
    }
    public override void PostUpdate()
    {
        //人都走了就拜拜把您嘞
        if (!Player.active) return;
        if (Player.magic().magia) maxchainProj = 32;
        else maxchainProj = 16;
        //若存在长枪，找到并赋值给spearProj
        CheckSpear();
        //冷却
        if (chainCooldown > 0)
        {
            chainCooldown--;
        }
        //按下键时生成锁链弹幕
        if (Main.myPlayer == Player.whoAmI )
        {

            if (Main.netMode == NetmodeID.Server) return;
            //确认手持武器
            if (!(Player.HeldItem.ModItem is RedSpear )) return;
            //有生成计数的情况下一松手 或者 噶了 就计数归零，并进入冷却
            if (chainCount > 0 && !Player.input().keyDown)
            {
                chainCount = 0;
                chainCooldown = 180;
            }
            //没按住↓ 或 武器不对 或 没冷却好 就无事发生
            if (!Player.input().keyDown ||Player.noItems || !(Player.HeldItem.ModItem is RedSpear) || chainCooldown > 0) return;
            //超过上限不生成
            if (chainCount >= maxchainProj) return;
            //没蓝不生成
            if (!Player.CheckMana(Player.HeldItem.mana, true)) return;
            //计数
            chainCount+= 4;
            for (int i = 3; i >= 0; i--)
            {
                Vector2 spawnCenter = Player.RotatedRelativePoint(Player.MountedCenter);
                if (spearProj > -1 && Main.projectile[spearProj].active)
                    spawnCenter = Main.projectile[spearProj].Center;
                int index = chainCount -i;
                int p = Projectile.NewProjectile(Player.GetSource_ItemUse_WithPotentialAmmo(Player.HeldItem, AmmoID.None),
                    RedSpearChainProj.CalculatePosition(
                        Player,
                        spawnCenter,
                        index,
                        chainCount),
                    //速度分别代表透明度和旋转
                    new Vector2(127 , index * MathHelper.PiOver2),
                    ModContent.ProjectileType<RedSpearChainProj>(),
                    Player.GetWeaponDamage(Player.HeldItem) / 2,
                    0,
                    Player.whoAmI,
                    index);
                Main.projectile[p].netUpdate = true;
            }
            Player.manaRegenDelay = 180;
            shouldCostMana = true;
            if (chainCount < maxchainProj / 2) chainCooldown = 8;
            else chainCooldown = 16;
            if (!Player.magic().magia) chainCooldown *= 2;

        }
        base.PostUpdate();
    }
    public void backing()
    {
        RedSpearBullet proj3 = null;
        foreach (Projectile proj2 in Main.ActiveProjectiles)
        {
            if (proj2.ModProjectile is RedSpearBullet && proj2.owner == Player.whoAmI)
                proj3 = (RedSpearBullet)proj2.ModProjectile;
            if (proj3 != null) proj3.back();
        }
    }
    private bool CheckSpear()      
    {
        if (Player != Main.LocalPlayer) return false;
        {
            
        }
        foreach (Projectile proj4 in Main.ActiveProjectiles)
        {
            if (proj4.ModProjectile is RedSpearBullet  && proj4.owner == Player.whoAmI)
            {
                spearProj = proj4.whoAmI;
                return true;
            }
            if (proj4.ModProjectile is RedSpearBend  && proj4.owner == Player.whoAmI)
            {
                spearProj = proj4.whoAmI;
                return true;
            }
        }
        return false;
    }
}

