using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using mahouSyoujyo.Content.NPCs.Critters;
using mahouSyoujyo.Content.Tiles.Statues;

namespace mahouSyoujyo.Content.Tiles.Banners
{
    public class QB_Banner : ModTile
    {
        public override void SetStaticDefaults()
        {

            //帧重要，如果你的物块并非普通的单物块请务必设置为true!!!
            Main.tileFrameImportant[Type] = true;
            
            //是否为实心物块
            Main.tileSolid[Type] = false;
            //为true时就不能在它旁边放上物块，例如插火把之类的就不行了
            Main.tileNoAttach[Type] = false;
            //顶部是否为实心，就和平台一样，玩家按下“下”时就可以从上面下来
            //请注意这个属性会覆盖掉tileSolid
            Main.tileSolidTop[Type] = false;
            //是否会被近战攻击，弹幕等所破坏
            Main.tileCut[Type] = false;
            //是否是桌子，为true的话就可以在上面放上玻璃瓶之类的东西
            Main.tileTable[Type] = false;
            //是否会和泥土“融合”，请注意这将改变贴图的读取方式
            Main.tileMergeDirt[Type] = false;
            //是否会被水冲掉
            Main.tileWaterDeath[Type] = false;
            //是否会被岩浆烫掉
            Main.tileLavaDeath[Type] = false;
            //是否阻挡光
            Main.tileBlockLight[Type] = false;
            //是否能发出闪光粒子，就像矿物一样，另外光照如果太低的话将无法发出粒子
            Main.tileShine2[Type] = false;
            //发出闪亮粒子的“频率”，这个数字越大则“频率”越低
            //Main.tileShine[Type] = 1000;
            
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            //TileID.Sets.IsAMechanism[Type] = true; // Ensures that this tile and connected pressure plate won't be removed during the "Remove Broken Traps" worldgen step
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Banners,0));
            //高几格，宽几格
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            //每一格的高度
            TileObjectData.newTile.CoordinateHeights=new int[3] { 16, 16, 16 };
            //每一格的宽度
            TileObjectData.newTile.CoordinateWidth = 16;
            //读取间隔
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.addTile(Type);

            DustType = DustID.PinkFairy;
            
            AddMapEntry(new Color(144, 148, 144), Language.GetText("MapObject.Banner"));
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            int y = j - Main.tile[i, j].TileFrameY / 18;
            int x = i - Main.tile[i, j].TileFrameX / 18;
            if (TileID.Sets.Platforms[Main.tile[x, y-1].TileType] && 
                !(Array.IndexOf(new int[] {5,8,10,19,20,21,22,23,24,25,26}, Main.tile[x, y-1].TileFrameX/18)>-1) )
            {
                offsetY = -8;
            }
            base.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref tileFrameX, ref tileFrameY);
        }
        
    }
    public class QB_BannerItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BunnyBanner);
            Item.DefaultToPlaceableTile(ModContent.TileType<QB_Banner>());
            Item.width = 12;
            Item.height = 28;
            ItemID.Sets.BannerStrength[Type]=new ItemID.BannerEffect(1f);
            
        }

        public override void AddRecipes()
        {
           /* CreateRecipe()
                .AddIngredient(ModContent.ItemType<Critter_QBItem>(), 5)
                .AddIngredient(ItemID.StoneBlock, 50)
                .AddCondition(Condition.InGraveyard)
                .AddTile(TileID.HeavyWorkBench)
                .SortAfterFirstRecipesOf(ItemID.BBQRibs) // places the recipe right after vanilla frog cage recipe.
                .Register();*/
        }
    }
}
