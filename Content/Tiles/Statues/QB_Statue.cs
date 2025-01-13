using mahouSyoujyo.Content.NPCs.Critters;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace mahouSyoujyo.Content.Tiles.Statues
{
    // ExampleStatue shows off correctly using wiring to spawn items and NPC.
    // See StatueWorldGen to see how ExampleStatue is added as an option for naturally spawning statues during worldgen.
    public class QB_Statue : ModTile
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
            Main.tileShine2[Type] = true;
            //发出闪亮粒子的“频率”，这个数字越大则“频率”越低
            Main.tileShine[Type] = 1000;
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IsAMechanism[Type] = true; // Ensures that this tile and connected pressure plate won't be removed during the "Remove Broken Traps" worldgen step
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            //高几格，宽几格
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            //每一格的高度
            TileObjectData.newTile.CoordinateHeights=new int[3] { 16,16,16 };
            //每一格的宽度
            TileObjectData.newTile.CoordinateWidth = 16;
            //读取间隔
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit= 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
            TileObjectData.addTile(Type);
            
            DustType = DustID.Dirt;

            AddMapEntry(new Color(144, 148, 144), Language.GetText("MapObject.Statue"));
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            
            base.PlaceInWorld(i, j, item);
        }
        // This hook allows you to make anything happen when this statue is powered by wiring.
        // In this example, powering the statue either spawns a random coin with a 95% chance, or, with a 5% chance - a goldfish.
        public override void HitWire(int i, int j)
        {
            // Find the coordinates of top left tile square through math
            int y = j - (Main.tile[i, j].TileFrameY % 36)/ 18;
            int x = i - (Main.tile[i, j].TileFrameX % 36)/ 18;

            const int TileWidth = 2;
            const int TileHeight = 3;

            // Here we call SkipWire on all tile coordinates covered by this tile. This ensures a wire signal won't run multiple times.
            for (int yy = y; yy < y + TileHeight; yy++)
            {
                for (int xx = x; xx < x + TileWidth; xx++)
                {
                    Wiring.SkipWire(xx, yy);
                }
            }

            // Calculcate the center of this tile to use as an entity spawning position.
            // Note that we use 0.5 for height 
            float spawnX = (x + TileWidth * 0.5f) * 16;
            float spawnY = (y + TileHeight * 0.5f) * 16;
            //Main.NewText(Main.tile[i, j].TileFrameX);
            // This example shows both item spawning code and npc spawning code, you can use whichever code suits your mod
            // There is a 95% chance for item spawn and a 5% chance for npc spawn
            // If you want to make a item spawning statue, see below.

            var entitySource = new EntitySource_TileUpdate(x, y, context: "QB_Statue");

            /*if (Main.rand.NextFloat() < .95f)
            {
                if (Wiring.CheckMech(x, y, 60) && Item.MechSpawn(spawnX, spawnY, ItemID.SilverCoin))
                {
                    int id = ItemID.SilverCoin;

                    Item.NewItem(entitySource, (int)spawnX, (int)spawnY - 20, 0, 0, id, 1, false, 0, false);
                }
            }
            else*/
            {
                // If you want to make an NPC spawning statue, see below.
                int npcIndex = -1;

                // 30 is the time before it can be used again. NPC.MechSpawn checks nearby for other spawns to prevent too many spawns. 3 in immediate vicinity, 6 nearby, 10 in world.
                int spawnedNpcId = ModContent.NPCType<Critter_QB>();

                if (Wiring.CheckMech(x, y, 120) && NPC.MechSpawn(spawnX, spawnY, spawnedNpcId))
                {
                    npcIndex = NPC.NewNPC(entitySource, (int)spawnX, (int)spawnY - 12, spawnedNpcId);
                }

                if (npcIndex >= 0)
                {
                    var npc = Main.npc[npcIndex];

                    npc.value = 0f;
                    npc.npcSlots = 0f;
                    // Prevents Loot if NPCID.Sets.NoEarlymodeLootWhenSpawnedFromStatue and !Main.HardMode or NPCID.Sets.StatueSpawnedDropRarity != -1 and NextFloat() >= NPCID.Sets.StatueSpawnedDropRarity or killed by traps.
                    // Prevents CatchNPC
                    npc.SpawnedFromStatue = true;
                }
            }
        }
    }
    public class QB_StatueItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BunnyStatue);
            Item.DefaultToPlaceableTile(ModContent.TileType<QB_Statue>());
            Item.width = 36;
            Item.height = 54;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Critter_QBItem>(),5)
                .AddIngredient(ItemID.StoneBlock,50)
                .AddCondition(Condition.InGraveyard)
                .AddTile(TileID.HeavyWorkBench)
                .SortAfterFirstRecipesOf(ItemID.BBQRibs) // places the recipe right after vanilla frog cage recipe.
                .Register();
        }
    }
}