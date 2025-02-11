using mahouSyoujyo.Content.Projectiles;
using mahouSyoujyo.Content.Projectiles.Weapon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using mahouSyoujyo.Globals;

namespace mahouSyoujyo.Content.Items.MeleeWeapon;

// This is a basic item template.
// Please see tModLoader's ExampleMod for every other example:
// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
public class BlueSword : ModItem
{
    Player player => Main.LocalPlayer;
    Comboing Combo => player.GetModPlayer<Comboing>();

    SoundStyle swing = new SoundStyle($"mahouSyoujyo/Radio/Sound/swing");
    private int reset = 0;
    // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.mahouSyoujyo.hjson' file.
    public override void SetDefaults()
    {
        Item.damage = 40;
        Item.DefaultToMagicWeapon(ProjectileID.Excalibur,5,10);
        Item.DamageType = DamageClass.Magic;
        Item.width = 56;
        Item.height = 58;
        Item.useTime = 1;
        Item.useAnimation =1;
        //关闭使用时的贴图绘制
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.MowTheLawn;
        Item.holdStyle = ItemHoldStyleID.None;
        Item.knockBack = 10f;
        Item.value = Item.buyPrice(platinum: 5, gold: 0, silver: 0, copper: 0);
        Item.rare = ItemRarityID.Red;
        Item.UseSound = null;
        Item.autoReuse = true;
        Item.useTurn = false;
        Item.useAmmo =AmmoID.None;
        Item.shootSpeed = 20f;
        Item.shoot =ProjectileID.Excalibur;
        Item.channel = true;
        Item.useTurn = true;
        
        
        // 假如这是一个法杖类型，不写默认false，这里就用到物品Type了
        //Item.staff[Type] = false;
        // 一般来说，法杖类武器会使用Shoot的那个使用方式，但它的贴图不像枪一样是水平朝向而是向右上倾斜
        // 让它变成true就会导致使用时贴图再转45度，变成法杖尖端朝着射击方向
    }

    public override bool CanUseItem(Player player)
    {
        if (player.altFunctionUse == 2 || Combo.supertime>0)
        {
            if (Combo.purryCD <=0 || Combo.purry_bonus>0    )
            purry(player);
            //return false;
            if (Combo.supertime<=0) return false;
        }   
        return true;
    }
    public override void UpdateInventory(Player player)
    {
        //打印计数
        
        base.UpdateInventory(player);
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    private static bool Summoned(ref int proj_Type, ref int proj_damage)
    {
        /*foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == ModContent.ProjectileType<HeartArrow>())
            {
                proj_Type = proj.type;
                proj_damage = proj.damage;
                return true;
            }
        }*/
        return false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Combo.CD<=0)
            if (Combo.combo =="AAA") combo_majo(player);
            else if (Combo.combo =="AA") combo_2(player);
            else if (Combo.combo == "A") combo_1(player);
            else combo_0(player);

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
        //	Recipe recipe = CreateRecipe();
        //	recipe.AddIngredient(ItemID.DirtBlock, 1);
        //	recipe.AddTile(TileID.WorkBenches);
        //	recipe.Register();
    }

    //技能组合
    public void purry(Player player)
    {
        //Combo.combo = "";
        if (Combo.purry_bonus>0) 
        {
            Combo.purry_bonus = 0;
            SoundEngine.PlaySound(SoundID.Item104.WithVolumeScale((Combo.supertime>0) ? 0.3f : 1f));
        }
        else
        {
            Combo.purryCD = 240;
            SoundEngine.PlaySound(swing.WithVolumeScale((Combo.supertime>0) ? 0.3f : 1f));
        }
            //Combo.CD = 30;
        Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(Item, AmmoID.None), player.Center, Main.MouseWorld-player.Center, ModContent.ProjectileType<comboPurry>(), Item.damage, Item.knockBack, player.whoAmI);
        //player.velocity.X = (player.magic().magia ? 15f : 10f)*player.direction;
        //Combo.keeping = 40;

    }
    public void combo_0(Player player)
    {
        SoundEngine.PlaySound(swing.WithVolumeScale((Combo.supertime>0) ? 0.3f : 1f));
        Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(Item, AmmoID.None), player.Center, Main.MouseWorld-player.Center, ModContent.ProjectileType<combo0>(), Item.damage, Item.knockBack, player.whoAmI);
        Combo.combo="A";
        Combo.keeping = 40;
        Combo.CD = 10;
        Combo.purry_bonus=30;
        //Combo.keeping = 40;

    }
    public void combo_1(Player player)
    {
        SoundEngine.PlaySound(swing.WithVolumeScale((Combo.supertime>0) ? 0.3f : 1f));
        Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(Item, AmmoID.None), player.Center, Main.MouseWorld-player.Center, ModContent.ProjectileType<combo1>(), Item.damage, Item.knockBack, player.whoAmI);
        Combo.combo="AA";
        Combo.keeping = 40;
        Combo.CD = 15;
        Combo.purry_bonus=30;
        //Combo.keeping = 40;

    }
    public void combo_2(Player player)
    {
        SoundEngine.PlaySound(swing.WithVolumeScale((Combo.supertime>0) ? 0.3f : 1f));
        Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(Item, AmmoID.None), player.Center, Main.MouseWorld-player.Center, ModContent.ProjectileType<combo2>(), Item.damage, Item.knockBack, player.whoAmI);
        Combo.combo="AAA";
        Combo.keeping = 40;
        Combo.CD = 20;
        Combo.purry_bonus=30;
        //player.velocity.Y = -8f;
        //Combo.keeping = 40;

    }
    public void combo_majo(Player player)
    {
        SoundEngine.PlaySound(swing.WithVolumeScale((Combo.supertime>0) ? 0.3f : 1f));
        Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(Item, AmmoID.None), player.Center, Main.MouseWorld-player.Center, ModContent.ProjectileType<comboPurry>(), Item.damage, Item.knockBack, player.whoAmI);
        Combo.combo="";
        Combo.keeping = 40;
        Combo.CD = 30;
        Combo.purry_bonus=30;
        //Combo.keeping = 40;

    }

    
    
}
//连段属性

