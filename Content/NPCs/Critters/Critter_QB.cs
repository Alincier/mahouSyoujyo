using mahouSyoujyo.Content.Buffs;
using mahouSyoujyo.Content.Items;
using mahouSyoujyo.Content.NPCs.Critters;
using mahouSyoujyo.Content.Tiles.Banners;
using mahouSyoujyo.Content.Tiles.Statues;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace mahouSyoujyo.Content.NPCs.Critters
{
    /// <summary>
    /// This file shows off a critter npc. The unique thing about critters is how you can catch them with a bug net.
    /// The important bits are: Main.npcCatchable, NPC.catchItem, and Item.makeNPC.
    /// We will also show off adding an item to an existing RecipeGroup (see ExampleRecipes.AddRecipeGroups).
    /// Additionally, this example shows an involved IL edit.
    /// </summary>
    public class Critter_QB : ModNPC
    {
        private const int ClonedNPCID = NPCID.Bunny; // Easy to change type for your modder convenience

        public override void Load()
        {
            
           // IL_Wiring.HitWireSingle += HookFrogStatue;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 16; // Copy animation frames
            Main.npcCatchable[Type] = true; // This is for certain release situations

            // These three are typical critter values
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.TownCritter[Type] = true;
            // The QB is immune to confused
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            //旗帜d 
            // This is so it appears between the QB and the gold QB
            NPCID.Sets.NormalGoldCritterBestiaryPriority.Insert(NPCID.Sets.NormalGoldCritterBestiaryPriority.IndexOf(ClonedNPCID) + 1, Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //为NPC设置图鉴展示状态，赋予其Velocity即可展现出行走姿态
                Velocity = 1f,
            };
            //添加信息至图鉴
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            NPC.chaseable = false;
            
        }
        
        public override bool CanChat()
        {
            //Main.NewText(Item.NPCtoBanner(this.Type));
            return true;
        }
        public override string GetChat()
        {
            return this.GetLocalizedValue($"Chat"+Main.rand.Next(6).ToString());
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = this.GetLocalizedValue($"Button1");
            button2 = this.GetLocalizedValue($"Button2");
            base.SetChatButtons(ref button, ref button2);
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.npcChatText = this.GetLocalizedValue($"Chat"+Main.rand.Next(6).ToString());
            }
            else
            {
                Player player = Main.player[NPC.FindClosestPlayer()];
                if (player.HasBuff<MagicGirlPover>())
                    Main.npcChatText = this.GetLocalizedValue($"Signed");
                else
                {
                    Main.npcChatText = this.GetLocalizedValue($"Sign");
                    player.AddBuff(ModContent.BuffType<MagicGirlPover>(), 10);
                    int index = player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<soulGem>());
                    //soulGem thegem = (soulGem)Main.item[index].ModItem;//类型强制转换
                    //thegem.CanUseItem(player);
                    //player.AddBuff(ModContent.BuffType<MagicGirlPover>(),10);
                    //Main.NewText(index);
                    //Main.item[index].ModItem.CanUseItem(player);
                } 
            }
            base.OnChatButtonClicked(firstButton, ref shopName);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 32;
            NPC.aiStyle =7;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 200000;
            NPC.knockBackResist =0.2f;
            NPC.HitSound = SoundID.NPCHit46;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.catchItem = ModContent.ItemType<Critter_QBItem>();
            NPC.lavaImmune = false;
            NPCID.Sets.StatueSpawnedDropRarity[this.Type]=0.1f;
            Banner=this.Type;
            BannerItem=ModContent.ItemType<QB_BannerItem>();
            AIType = ClonedNPCID;
            AnimationType = NPCID.DarkMummy;
            NPC.scale =1.2f;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.AddTags(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("description")));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.TownGeneralCritter.Chance * 0.05f;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            
            npcLoot.Add(new CommonDrop(ModContent.ItemType<PieceofGrief>(), 2, 1));
            base.ModifyNPCLoot(npcLoot);
        }
        public override void OnKill()
        {
            
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"Critter_QB_Gore_Head").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"Critter_QB_Gore_Body").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"Critter_QB_Gore_Tail").Type, NPC.scale);
            base.OnKill();
            
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetCrit();
            modifiers.SourceDamage*=0;
            modifiers.FinalDamage.Flat=7775;
            base.ModifyHitByItem(player, item, ref modifiers);
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetCrit();
            modifiers.SourceDamage*=0;
            modifiers.FinalDamage.Flat=7775;
            base.ModifyHitByProjectile(projectile, ref modifiers);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 266, 2 * hit.HitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        dust.noGravity = true;
                        dust.scale = 1.2f * NPC.scale;
                    }
                    else
                    {
                        dust.scale = 0.7f * NPC.scale;
                    }
                }
                


            }
        }

        /*public override Color? GetAlpha(Color drawColor)
        {
            // GetAlpha gives our Lava Frog a red glow.
            return drawColor with
            {
                R = 255,
                // Both these do the same in this situation, using these methods is useful.
                G = Utils.Clamp<byte>(drawColor.G, 175, 255),
                B = Math.Min(drawColor.B, (byte)75),
                A = 255
            };
        }*/

        public override bool PreAI()
        {
            // Kills the NPC if it hits water, honey or shimmer
            if (Collision.LavaCollision(NPC.position, NPC.width, NPC.height))
            { // NPC.lavawet not 100% accurate for the frog
                NPC.life = 0;
                if (Main.rand.NextBool(4*((NPC.SpawnedFromStatue)?10:1)))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, ItemID.BBQRibs);
                NPC.HitEffect(instantKill:true);
                SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume=0.5f}, NPC.position); // plays a fizzle sound
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, /*Mod.Find<ModGore>($"{Name}_Gore_Head").Type*/11, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 12, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 13, NPC.scale);
                NPC.active = false;
            }

            return true;
        }

        public override void OnCaughtBy(Player player, Item item, bool failed)
        {
            if (failed)
            {
                return;
            }
            //生成岩浆，我们不需要
            /*Point npcTile = NPC.Center.ToTileCoordinates();

            if (!WorldGen.SolidTile(npcTile.X, npcTile.Y))
            { // Check if the tile the npc resides the most in is non solid
                Tile tile = Main.tile[npcTile];
                tile.LiquidAmount = tile.LiquidType == LiquidID.Lava ? // Check if the tile has lava in it
                    Math.Max((byte)Main.rand.Next(50, 150), tile.LiquidAmount) // If it does, then top up the amount
                    : (byte)Main.rand.Next(50, 150); // If it doesn't, then overwrite the amount. Technically this distinction should never be needed bc it will burn but to be safe it's here
                tile.LiquidType = LiquidID.Lava; // Set the liquid type to lava
                WorldGen.SquareTileFrame(npcTile.X, npcTile.Y, true); // Update the surrounding area in the tilemap*/
            //}
        }
    }

    public class Critter_QBItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsLavaBait[Type] = true; // While this item is not bait, this will require a lava bug net to catch.
        }

        public override void SetDefaults()
        {
            // useStyle = 1;
            // autoReuse = true;
            // useTurn = true;
            // useAnimation = 15;
            // useTime = 10;
            // maxStack = CommonMaxStack;
            // consumable = true;
            // width = 32;
            // height = 32;
            // makeNPC = 361;
            // noUseGraphic = true;

            // Cloning ItemID.Frog sets the preceding values
            Item.CloneDefaults(ItemID.Frog);
            Item.width = 32;
            Item.height = 32;
            Item.makeNPC = ModContent.NPCType<Critter_QB>();
            Item.value += Item.buyPrice(0, 0, 30, 0); // Make this critter worth slightly more than the frog
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            //生成个数
            //CreateRecipe(1);
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(this, 4);
            //recipe.AddTile(TileID.WorkBenches);
            recipe.ReplaceResult(ItemID.BBQRibs, 1);
            recipe.DisableDecraft();
            recipe.Register();

            base.AddRecipes();
        }
    }
}