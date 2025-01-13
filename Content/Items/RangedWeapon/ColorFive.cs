using mahouSyoujyo.Content.Projectiles;
using mahouSyoujyo.Content.Projectiles.Weapon;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace mahouSyoujyo.Content.Items.RangedWeapon;
 
// This is a basic item template.
// Please see tModLoader's ExampleMod for every other example:
// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
public class ColorFive : ModItem
{
	public static ActiveSound acsound=null;
    // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.mahouSyoujyo.hjson' file.
    public override void SetDefaults()
	{

        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
		Item.damage = 100;
		Item.DefaultToRangedWeapon(ProjectileID.Bullet,0,6,30,true);
		Item.DamageType = DamageClass.Magic;
		Item.width = 42;
		Item.height = 30;
		Item.useTime = 5;
		Item.useAnimation = 5;
		//关闭使用时的贴图绘制
		Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
		Item.holdStyle = ItemHoldStyleID.None;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(platinum: 5, gold: 0, silver: 0, copper: 0);
        Item.rare = ItemRarityID.Red;
        Item.UseSound = null;
		Item.autoReuse = true;
		Item.useTurn = false;
        Item.useAmmo =AmmoID.None;
		Item.shootSpeed = 20f;
		Item.shoot =ProjectileID.Bullet;
		Item.channel = true;
        // 假如这是一个法杖类型，不写默认false，这里就用到物品Type了
        //Item.staff[Type] = false;
        // 一般来说，法杖类武器会使用Shoot的那个使用方式，但它的贴图不像枪一样是水平朝向而是向右上倾斜
        // 让它变成true就会导致使用时贴图再转45度，变成法杖尖端朝着射击方向
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override Color? GetAlpha(Color lightColor)
    {
		return null;
    }
    private static bool Summoned(ref int proj_Type, ref int proj_damage)
	{
        foreach (var proj in Main.ActiveProjectiles)
		{
            if (proj.type == ModContent.ProjectileType<HeartArrow>() && proj.owner == Main.myPlayer)
            {
                proj_Type = proj.type;
                proj_damage = proj.damage;
                return true;
            }
        }
		return false;
	}
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 gun_Position = player.Center+new Vector2(0, -128);
        bool no_gun=true;
		foreach (var proj in Main.ActiveProjectiles)
		{
			if (proj.type == ModContent.ProjectileType<ColorFive_Gun>() && proj.owner == player.whoAmI)
			{
				proj.timeLeft=10;
                proj.netUpdate=true;
                no_gun=false;
            }
        }
        if (no_gun)
        {
            float x = (Main.MouseWorld-player.Center).X;
            float y = (Main.MouseWorld-player.Center).Y;
            Projectile gun = Projectile.NewProjectileDirect(source, gun_Position, Vector2.Zero, ModContent.ProjectileType<ColorFive_Gun>(), damage, knockback, player.whoAmI);
        }
        //Main.NewText(Main.projectile[gun].ai[0]);
        //寻找可以复制的射弹
        //int proj_Type = type;
        //int proj_damage = damage;
        //if (ColorFive.Summoned(ref proj_Type,ref proj_damage)) {
		//	Projectile.NewProjectile(source, gun_Position, (Main.MouseWorld-gun_Position).RotatedBy(rotated_by).SafeNormalize(Vector2.Zero)*Item.shootSpeed, proj_Type, damage, knockback, player.whoAmI, ai1: DamageClass.Ranged.Type,ai2:1 );
		//}
		//射出子弹
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
}
