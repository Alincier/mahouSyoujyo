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

namespace mahouSyoujyo.Content.Items.RangedWeapon;

// This is a basic item template.
// Please see tModLoader's ExampleMod for every other example:
// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
public class YellowGun : ModItem
{
    public int cd = 0;
    public int  usetime = 0;
    //public int consume = 0;
    // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.mahouSyoujyo.hjson' file.
    public override void SetDefaults()
    {

        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.damage = 100;
        Item.DefaultToRangedWeapon(ProjectileID.Bullet, 0, 6, 30, true);
        Item.DamageType = DamageClass.Magic;
        Item.width = 104;
        Item.height = 36;
        Item.useTime = 1;
        Item.useAnimation = 1;
        Item.noUseGraphic = true;
        //关闭使用时的贴图绘制
        //Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.holdStyle = ItemHoldStyleID.None;
        Item.knockBack = 10;
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
        Item.mana =40;
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
    {
        if (player.altFunctionUse ==2 || player.GetModPlayer<YellowGunCharge>().shootmode==0)
            mult*=0;
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
        player.GetModPlayer<LocalUIPlayer>().YellowGunChargeBar = true;
        base.HoldItem(player);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse ==2) 
        {
            player.GetModPlayer<YellowGunCharge>().shootmode=1-player.GetModPlayer<YellowGunCharge>().shootmode;
            player.SetDummyItemTime(20);
            return false;
        }
        if (player.GetModPlayer<YellowGunCharge>().shootmode ==1)
        {
            //player.GetModPlayer<YellowGunCharge>().yellowguncharge = 0;
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<YellowGunLight>(), damage*3, knockback, player.whoAmI);
            //consume+=1;
            return false;
        }
        if (usetime == 0) return false;
        if (usetime<60)
        {
            //if (usetime >=40 && usetime % 10 == 0) SoundEngine.PlaySound(SoundID.Item40 with { Volume = 0.8f, Pitch =-0.9f, PitchVariance = 0.2f }, player.Center);
            if (cd < 10) return false;
            cd = 0;
        }
        else if (usetime<120)
        {
            if (cd < 8) return false;
            cd = 0;
        }
        else if (usetime<180)
        {
            if (cd < 6) return false;
            cd = 0;
        }
        else 
        {
            int finalcd = 6;
            if (player.magic().magia) finalcd=4; 
            if (cd < finalcd) return false;
            cd = 0;
        } 
        Vector2 shoot_center = player.Center+new Vector2(-48, 0).RotatedBy((Main.MouseWorld-player.Center).ToRotation());
        Vector2 shoot_pos = shoot_center+new Vector2(Main.rand.Next(-80, 9), Main.rand.Next(-80, 81)).RotatedBy((Main.MouseWorld-player.Center).ToRotation());
        if (Main.LocalPlayer == player)
        {
            Projectile.NewProjectile(source, shoot_pos, (Main.MouseWorld-shoot_center).SafeNormalize(Vector2.Zero)*Item.shootSpeed, ModContent.ProjectileType<YellowGunBullet>(), damage*2, knockback, player.whoAmI);
            //consume+=20;
        }

        return false;
    }
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {

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
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.ChargedBlasterCannon, 1);
        recipe.AddIngredient(ItemID.YellowString, 2);
        recipe.AddIngredient(ItemID.IllegalGunParts, 2);
        //recipe.AddIngredient(ItemID.LunarBar, 10);
        recipe.AddTile(TileID.MythrilAnvil);
        //   recipe.SetResult(this, 1)
        recipe.Register();

    }
}

public class YellowGunCharge : ModPlayer
{
    public Vector2 clientmouse = Vector2.Zero;
    public int shootmode = 0;
    public int yellowguncharge = 0;
    public bool charged = false;
    public SoundStyle ChargedSound = new SoundStyle($"mahouSyoujyo/Radio/Sound/GunLoaded");
    public override void SetStaticDefaults()
    {
    }
    public override void ResetEffects()
    {
        base.ResetEffects();
    }
    public override void PostUpdate()
    {
        
        if (yellowguncharge>=600)
        {
            yellowguncharge=600;
            if (!charged && Main.myPlayer==Player.whoAmI) SoundEngine.PlaySound(ChargedSound with { Pitch = 0f});
            charged=true;
        }
        if (yellowguncharge<=0)
        {
            yellowguncharge=0;
            charged=false;
        }
        //if (!charged) yellowguncharge--;
    }
}

