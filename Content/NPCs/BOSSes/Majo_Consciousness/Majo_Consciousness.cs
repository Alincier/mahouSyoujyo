using mahouSyoujyo.Content.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Graphics.Effects;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using mahouSyoujyo.Content.Items.Placeable;
using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content.Items.Consumables;
using mahouSyoujyo.Common.ItemDropRules;
using mahouSyoujyo.Content.Items.Consumables.StatIncreaseItem;
using mahouSyoujyo.Content.Items.SpecialWeapon;
using Terraria.Graphics.Shaders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using mahouSyoujyo.Common.Configs;
using System.Linq;

namespace mahouSyoujyo.Content.NPCs.BOSSes.Majo_Consciousness
{
    [AutoloadBossHead]
    public class Majo_Consciousness : ModNPC
    {
        // It is applied in the BossHeadSlot hook when the boss is in its second stage
        public static int secondStageHeadSlot = -1;
        private bool playersActive;
        List<string> bosslist => ModContent.GetInstance<ServerConfigs>().bosslist;
        bool customed => ModContent.GetInstance<ServerConfigs>().CustomedMajoConsciousness;
        private int counter
        {
            get { return (int)NPC.ai[3]; }
            set { NPC.ai[3] = value; }
        }
        private Texture2D texorigin = ModContent.Request<Texture2D>("mahouSyoujyo/Content/NPCs/BOSSes/Majo_Consciousness/Majo_Consciousness").Value;
       // private Texture2D texeye = ModContent.Request<Texture2D>("mahouSyoujyo/Content/NPCs/BOSSes/Majo_Consciousness/Majo_Consciousness_Eye").Value;
        private Texture2D texeyeball = ModContent.Request<Texture2D>("mahouSyoujyo/Content/NPCs/BOSSes/Majo_Consciousness/Majo_Consciousness_EyeBall").Value;
        private Texture2D beauty = ModContent.Request<Texture2D>("mahouSyoujyo/Content/NPCs/BOSSes/Majo_Consciousness/Majo_Consciousness_Beauty").Value;
        int bonus =1;//难度加成系数
        private int projdamage
        {
            set; get;
        }
        //阶段属性
        private int stage
        {
            get { return (int)NPC.ai[1]+1; }
            set { NPC.ai[1] = value-1; }
        }
        public int getStage()
        {
            return stage;
        }
        private List<int> projList = new List<int>();

        private List<int> makeProjList()
        {
            List<int> pList = new List<int>() { (int)ProjectileID.EyeLaser };
            if (customed)
            {
                //{
                //    "KingSlime", "TheEyeOfCthulhu", "EvilBoss", "QueenBee", "Skeletron",
                //                        "Deerclops", "WallOfFlesh", "QueenSlime", "TheTwins", "TheDestroyer", "SkeletronPrime",
                //                        "Plantera", "Golem", "DukeFishron", "EmpressOfLight", "LunaticCultist", "MoonLord"}
                if (bosslist.Contains("Deerclops")) pList.Add(ProjectileID.InsanityShadowHostile);
                if (bosslist.Contains("WallOfFlesh")) pList.Add(ProjectileID.DemonSickle);
                if (bosslist.Contains("TheTwins")) pList.Add(ProjectileID.DeathLaser);
                if (bosslist.Contains("SkeletronPrime")) pList.Add(ProjectileID.BombSkeletronPrime);
                if (bosslist.Contains("EmpressOfLight")) pList.Add(ProjectileID.HallowBossRainbowStreak);
                if (bosslist.Contains("Golem")) pList.Add(ProjectileID.Fireball);
                if (bosslist.Contains("QueenSlime")) pList.Add(ProjectileID.QueenSlimeGelAttack);
                if (bosslist.Contains("Plantera")) pList.Add(ProjectileID.PoisonSeedPlantera);
                return pList;
            }
            if (NPC.downedDeerclops) pList.Add(ProjectileID.InsanityShadowHostile);
            if (Main.hardMode) pList.Add(ProjectileID.DemonSickle);
            if (NPC.downedMechBoss2) pList.Add(ProjectileID.DeathLaser);
            if (NPC.downedMechBoss3) pList.Add(ProjectileID.BombSkeletronPrime);
            if (NPC.downedEmpressOfLight) pList.Add(ProjectileID.HallowBossRainbowStreak);
            if (NPC.downedGolemBoss) pList.Add(ProjectileID.Fireball);
            if (NPC.downedQueenSlime) pList.Add(ProjectileID.QueenSlimeGelAttack);
            if (NPC.downedPlantBoss) pList.Add(ProjectileID.PoisonSeedPlantera);

            return pList;
        }
        private int makePower()
        {
            int count = 0;
            if (customed)
            {
                return Math.Clamp(bosslist.Count ,0,17);
            } 
            if (NPC.downedBoss1)
            {
                count++;
            }
            if (NPC.downedSlimeKing)
            {
                count++;
            }
            if (NPC.downedBoss2)
            {
                count++;
            }
            if (NPC.downedBoss3)
            {
                count++;
            }
            if (NPC.downedQueenBee)
            {
                count++;
            }
            if (NPC.downedDeerclops)
            {
                count++;
            }
            if (Main.hardMode)
            {
                count++;
            }

            if (NPC.downedQueenSlime)
            {
                count++;
            }
            if (NPC.downedMechBoss1)
            {
                count++;
            }
            if (NPC.downedMechBoss2)
            {
                count++;
            }
            if (NPC.downedMechBoss3)
            {
                count++;

            }
            if (NPC.downedEmpressOfLight)
            {
                count++;
            }
            if (NPC.downedPlantBoss)
            {
                count++;
            }
            if (NPC.downedGolemBoss)
            {
                count++;
            }
            if (NPC.downedFishron)
            {
                count++;
            }
            if (NPC.downedAncientCultist)
            {
                count++;
            }
            if (NPC.downedMoonlord)
            {
                count++;
            }
            //Main.NewText(count);
            return count;
        }
        //修饰射弹、召唤的NPC的伤害和速度
        private enum modifyID :int
        {
            projectileDamage = 0,
            projectileSpeed = 1,
            npcDamage = 2,
            npcSpeed = 3,
        }
        private float Modify(int type,int id)
        {
            if (type<2)
            {
                if (id == ProjectileID.EyeLaser) return (type==0) ? (((customed) ? bosslist.Contains("TheDestroyer") : NPC.downedMechBoss1) ? 0.8f : 0.5f) : (  (  (customed)?bosslist.Contains("TheDestroyer"):NPC.downedMechBoss1  )?2f:1f  );
                if (id == ProjectileID.HallowBossRainbowStreak) return (type==0) ? 0.5f : 0.5f;
                if (id == ProjectileID.DemonSickle) return (type==0) ? 0.5f : 1f;
                if (id == ProjectileID.DeathLaser) return (type==0) ? (((customed) ? bosslist.Contains("TheDestroyer") : NPC.downedMechBoss1) ? 1f : 0.8f) : (((customed) ? bosslist.Contains("TheDestroyer") : NPC.downedMechBoss1) ? 3f : 1.5f);
                if (id == ProjectileID.InsanityShadowHostile) return (type==0) ? 1f : 3f;
                if (id == ProjectileID.Fireball) return (type==0) ? 0.5f : 3f;
                if (id == ProjectileID.QueenBeeStinger) return (type==0) ? 0.5f : 2f;
                if (id == ProjectileID.CultistBossLightningOrb) return (type==0) ? 0.3f : 2f;
                if (id == ProjectileID.PoisonSeedPlantera) return (type==0) ? 0.5f : 0.2f;
                return 1f;
            }
            return 1f;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.TileContactDamage;
            return base.CanHitPlayer(target, ref cooldownSlot);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.BrokenArmor,300);
            target.AddBuff(BuffID.WitheredArmor, 120);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerBuffs,-1, target.whoAmI, null,target.whoAmI);
            base.OnHitPlayer(target, hurtInfo);
        }
        //动作的ID
        private enum actionID :int
        {
            
            move=0,//转阶段
            Throw=1,//邪教徒+蜜蜂
            Sprint=2,//克眼
            Round=3,//克脑+世纪之花
            Rush=4,//猪鲨+史莱姆王
            Trans=5,//史莱姆王
        }
        private int action
        {
            get { return (int)NPC.ai[2]; }
            set { NPC.ai[2] = value; }
        }
        private Vector2 transpos
        {
            get {return new Vector2(NPC.localAI[1], NPC.localAI[2]); }
            set { NPC.localAI[1]=value.X; NPC.localAI[2]=value.Y; }
        }
        public override void Load()
        {
            // We want to give it a second boss head icon, so we register one
            string texture = BossHeadTexture + "_SecondStage"; // Our texture is called "ClassName_Head_Boss_SecondStage"
            secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1); // -1 because we already have one registered via the [AutoloadBossHead] attribute, it would overwrite it otherwise
        }
        public override void BossHeadSlot(ref int index)
        {
            int slot = secondStageHeadSlot;
            if (stage>1 && slot != -1)
            {
                // If the boss is in its second stage, display the other head icon instead
                index = slot;
            }
        }
        public override void SetStaticDefaults()
        {
            //游戏内显示的称呼
            //总帧数，根据使用贴图的实际帧数进行填写，这里我们直接调用全部商人的数据
            Main.npcFrameCount[Type] = 6;
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    BuffID.Slow,
                    BuffID.Confused,
                }
                
            };
            debuffData.ApplyToNPC(NPC);
            NPCID.Sets.MPAllowedEnemies[Type] = true;//允许使用召唤物召唤
            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "mahouSyoujyo/Content/NPCs/BOSSes/Majo_Consciousness/Majo_Consciousness_Preview",
                PortraitScale = 0.4f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
                Velocity = 1f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        
        public override void SetDefaults()
        {
            NPC.width = 240;
            NPC.height = 240;
            NPC.damage = 20+makePower()*10;//
            projdamage = 20;
            NPC.defense = 10+makePower()*5;
            NPC.lifeMax = 10000+makePower()*makePower()*1000;//
            NPC.HitSound = SoundID.NPCDeath8;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0f;
            NPC.friendly = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5+5*makePower());
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            
            //NPC.
            NPC.npcSlots = 10f; // Take up open spawn slots, preventing random NPCs from spawning during the fight
            //NPC.color = Color.White;
            // Default buff immunities should be set in SetStaticDefaults through the NPCID.Sets.ImmuneTo{X} arrays.
            // To dynamically adjust immunities of an active NPC, NPC.buffImmune[] can be changed in AI: NPC.buffImmune[BuffID.OnFire] = true;
            // This approach, however, will not preserve buff immunities. To preserve buff immunities, use the NPC.BecomeImmuneTo and NPC.ClearImmuneToBuffs methods instead, as shown in the ApplySecondStageBuffImmunities method below.

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // Custom boss bar
            //NPC.BossBar = ModContent.GetInstance<>();
            //NPC.color = Color.White;
            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Radio/Music/Majo_consciousness_p1");
            }
            NPC.value = Item.buyPrice(0, 4, 50, 0);//NPC的爆出来的MONEY的数量，四个空从左到右是铂金，金，银，铜
            NPC.lavaImmune = true;//对岩浆免疫
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Description"))
            });
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(crazy);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            crazy = reader.ReadBoolean();
            base.ReceiveExtraAI(reader);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Majo_ConsciousnessTrophy>(), 10));

            // All the Classic Mode drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            LeadingConditionRule defeatplant = new LeadingConditionRule(new Conditions.DownedPlantera());
            LeadingConditionRule defeatMoonLord = new LeadingConditionRule(new downedMoonLordDropCondition());
            defeatplant.OnSuccess(ItemDropRule.ByCondition(new Conditions.DownedAllMechBosses(), ModContent.ItemType<RPGweapon>(), 4, 1, 1));
            defeatMoonLord.OnSuccess(new CommonDrop(ModContent.ItemType<BlessOfCircles>(), 1, 1, 1));
            defeatMoonLord.OnSuccess(ItemDropRule.ByCondition(new Conditions.IsHardmode(),ModContent.ItemType<TimePlate>(), 2, 1, 1));
            defeatMoonLord.OnFailedConditions(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<TimePlate>(), 3, 1, 1));
            defeatMoonLord.OnFailedConditions(new CommonDrop(ItemID.MoonLordLegs, 2));
            notExpertRule.OnSuccess(defeatMoonLord);
            notExpertRule.OnSuccess(defeatplant);
            notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.IsPreHardmode(), ModContent.ItemType<TimePlate>(), 10, 1, 1));
            npcLoot.Add(notExpertRule);
            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Mask>(), 7));

            // which requires these parameters to be defined
            int itemType = ModContent.ItemType<PieceofGrief>();
            var parameters = new DropOneByOne.Parameters()
            {
                ChanceNumerator = 1,//概率分子
                ChanceDenominator = 1,//概率分母
                MinimumStackPerChunkBase = 2,
                MaximumStackPerChunkBase = 2,
                MinimumItemDropsCount = 20,
                MaximumItemDropsCount = 25,
            };

            //notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

            // Finally add the leading rule
            //npcLoot.Add(notExpertRule);

            npcLoot.Add(new DropOneByOne(itemType, parameters));

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ConsciousnessBossBag>()));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Majo_ConsciousnessRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<>(), 4));

            base.ModifyNPCLoot(npcLoot);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            name = this.GetLocalizedValue("defeatedName");
            potionType = ModContent.ItemType<GriefSeed>();
            base.BossLoot(ref name, ref potionType);
        }
        public override void OnSpawn(IEntitySource source)
        {
            setdefaultstat();

            base.OnSpawn(source);
        }
        private void setdefaultstat()
        {
            bonus = 1;
            //if (Main.masterMode) bonus = 3;
            //else if (Main.expertMode) bonus = 2;
            //NPC.lifeMax = 10000+power*power*1000*bonus;
            //NPC.life = NPC.lifeMax;
            //NPC.defense = 0;
            //NPC.damage=(50+power*10)*bonus;
            //projdamage = (20+power*2);
            //NPC.value = Item.buyPrice(0, 4+4*power, 50, 0)*bonus;
        }
        //public override 
        //全员死亡离开的时间
        int lefttime = 60;
        public override void BossHeadRotation(ref float rotation)
        {
            rotation=NPC.rotation;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.netMode == NetmodeID.Server) return false;
            if (!ModContent.GetInstance<ClientConfigs>().GrayScaleClosed_Consciousness && NPC.life>stagepoint) 
                mahouSyoujyo.SceneShader("GrayScaleMajoConciousness", 0.7f*(float)Math.Max(0, NPC.life-stagepoint) / (float)(NPC.lifeMax-stagepoint));
            else mahouSyoujyo.DelSceneShader("GrayScaleMajoConciousness");
            float transparnt = (float)(255-NPC.alpha) / 255f;
            Texture2D tex = null;
            if (!ModContent.GetInstance<ClientConfigs>().ConsciousBeauty) tex=texorigin;
            else tex=beauty;
            float direction = moving_diection(NPC.velocity);
            int width = tex.Width;
            int height = tex.Height /((!ModContent.GetInstance<ClientConfigs>().ConsciousBeauty)?Main.npcFrameCount[Type]:1);
           // int widtheye = texeye.Width;
           // int heighteye = texeye.Height;
            int widtheyeball = texeyeball.Width;
            int heighteyeball = texeyeball.Height;
            NPC.rotation = (stage>1) ? MathHelper.ToRadians(direction*2) : 0f; //BOSS的旋转度，用于记录绘制拖尾
            Rectangle rect = new Rectangle(0, (!ModContent.GetInstance<ClientConfigs>().ConsciousBeauty)?NPC.frame.Y:0, width, height);
          //  Rectangle recteye = new Rectangle(0, 0, widtheye, heighteye);
            Rectangle recteyeball = new Rectangle(0, 0, widtheyeball, heighteyeball);
            //幻影的透明度
            float shadowAlphaMultipy = Math.Min(1f,
                        ((float)(stagepoint -NPC.life)/ (float)stagepoint)
                        *((float)(stagepoint -NPC.life)/ (float)stagepoint)
                        );
            //先画光环特效
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(
            tex, NPC.Center-Main.screenPosition,
            rect, Main.DiscoColor*0.5f*transparnt, NPC.rotation,
            new Vector2(width / 2, height / 2),
            new Vector2(1.05f, 1.05f),
            SpriteEffects.None
                , 0);
            spriteBatch.Draw(
            tex, NPC.Center-Main.screenPosition,
            rect, Color.DarkGray*0.6f*transparnt, NPC.rotation,
            new Vector2(width / 2, height / 2),
            new Vector2(1.1f, 1.1f),
            SpriteEffects.None
                , 0);
            if (((customed)? bosslist.Contains("EvilBoss") :NPC.downedBoss2) && NPC.life<stagepoint / 2 )
            {
                // Main.NewText((stagepoint -NPC.life).ToString()+" "+stagepoint.ToString()+" "+((float)(stagepoint -NPC.life)/ (float)stagepoint)
                //     *((float)(stagepoint -NPC.life)/ (float)stagepoint));
                spriteBatch.Draw(
                    tex, NPC.Center+2*(Main.LocalPlayer.Center-NPC.Center)-Main.screenPosition,
                    rect, Main.DiscoColor*0.5f*shadowAlphaMultipy*transparnt, -NPC.rotation,
                    new Vector2(width / 2, height / 2),
                    new Vector2(1.05f, 1.05f),
                    SpriteEffects.None
                    , 0);
                spriteBatch.Draw(
                    tex, NPC.Center+2*(Main.LocalPlayer.Center-NPC.Center)-Main.screenPosition,
                    rect, Color.DarkGray*0.6f*shadowAlphaMultipy*transparnt, -NPC.rotation,
                    new Vector2(width / 2, height / 2),
                    new Vector2(1.1f, 1.1f),
                    SpriteEffects.None
                    , 0);
            }
            //再画拖影
            int howlong = (3 <= action && action <= 11) ? 2 : 1;//是否在猪鲨阶段

            for (int i = 0;i < howlong * 5; i++)
            {
                spriteBatch.Draw(
                    tex, pos_old[i]-Main.screenPosition,
                    rect, ( (howlong<2)?Color.Gray*0.4f : Color.DarkBlue*0.3f)*transparnt, rot_old[i],
                    new Vector2(width / 2, height / 2),
                    new Vector2(1f, 1f),
                    SpriteEffects.None
                    , 0);
                if (((customed) ? bosslist.Contains("EvilBoss") : NPC.downedBoss2) && NPC.life<stagepoint / 2 )
                {
                    // Main.NewText((stagepoint -NPC.life).ToString()+" "+stagepoint.ToString()+" "+((float)(stagepoint -NPC.life)/ (float)stagepoint)
                    //     *((float)(stagepoint -NPC.life)/ (float)stagepoint));

                    spriteBatch.Draw(
                        tex, pos_old[i]+2*(Main.LocalPlayer.Center-pos_old[i])-Main.screenPosition,
                        rect, ((howlong<2) ? Color.Gray*0.4f : Color.DarkBlue*0.3f)*shadowAlphaMultipy*transparnt, -rot_old[i],
                        new Vector2(width / 2, height / 2),
                        new Vector2(1f, 1f),
                        SpriteEffects.None
                        , 0);
                }
            }
            //最后画本体图像
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //GameShaders.Misc["SmoothBackgroundMajoConciousness"].UseShaderSpecificData(new Vector4(240,280, 0 ,0)).UseColor(1, 1, 1).UseSecondaryColor(width,height,48).Apply();
            //身体
            spriteBatch.Draw(
                tex, NPC.Center-Main.screenPosition,
                rect, NPC.color * transparnt, NPC.rotation,
                new Vector2(width / 2, height / 2),
                new Vector2(1f, 1f),
                SpriteEffects.None
                , 0);
            //眼睛
            if (!ModContent.GetInstance<ClientConfigs>().ConsciousBeauty)
            {
                /*spriteBatch.Draw(
                    texeye, NPC.Center-Main.screenPosition,
                    recteye, Color.White*Math.Clamp((float)(NPC.lifeMax-NPC.life)/ (float)(NPC.lifeMax-stagepoint), 0f, 1f), 0,
                    new Vector2(widtheye / 2, heighteye / 2),
                    new Vector2(1f, 1f),
                    SpriteEffects.None
                    , 0);*/
                spriteBatch.Draw(
                    texeyeball, NPC.Center-Main.screenPosition,
                    recteyeball, Color.White*((stage>1)?1f:0f) *transparnt, (Main.LocalPlayer.Center-NPC.Center).ToRotation(),
                    new Vector2(widtheyeball / 2, heighteyeball / 2),
                    new Vector2(1.4f, 1.4f),
                    SpriteEffects.None
                    , 0);
            }

            if (((customed) ? bosslist.Contains("EvilBoss") : NPC.downedBoss2) && NPC.life<stagepoint / 2 )
            {
                spriteBatch.Draw(
                    tex, NPC.Center+2*(Main.LocalPlayer.Center-NPC.Center)-Main.screenPosition,
                    rect, NPC.color*shadowAlphaMultipy * transparnt, -NPC.rotation,
                    new Vector2(width / 2, height / 2),
                    new Vector2(1f, 1f),
                    SpriteEffects.None
                    , 0);
                if (!ModContent.GetInstance<ClientConfigs>().ConsciousBeauty)
                {
                    /*spriteBatch.Draw(
                    texeye, NPC.Center+2*(Main.LocalPlayer.Center-NPC.Center)-Main.screenPosition,
                    recteye, Color.White*Math.Clamp((float)(NPC.lifeMax-NPC.life)/ (float)(NPC.lifeMax-stagepoint), 0f, 1f)*shadowAlphaMultipy, 0,
                    new Vector2(widtheye / 2, heighteye / 2),
                    new Vector2(1f, 1f),
                    SpriteEffects.None
                    , 0);*/
                    spriteBatch.Draw(
                    texeyeball, NPC.Center+2*(Main.LocalPlayer.Center-NPC.Center)-Main.screenPosition,
                    recteyeball, Color.White*((stage>1) ? 1f : 0f)*shadowAlphaMultipy * transparnt, (Main.LocalPlayer.Center-NPC.Center).ToRotation()+MathHelper.Pi,
                    new Vector2(widtheyeball / 2, heighteyeball / 2),
                    new Vector2(1f, 1f),
                    SpriteEffects.None
                    , 0);
                }
            }
            

            // Main.NewText(direction);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            base.PostDraw(spriteBatch, screenPos, drawColor);
        }
        int frame_direction = 1;
        public override void FindFrame(int frameHeight)
        {
            
            Color color=(stage==1)?Color.White:Color.Gray;
            NPC.color = color*1f*((float)(255-NPC.alpha)/ 256f );
            //Main.NewText(action);
            // This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
            // In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"
            int startFrame = 0;
            int finalFrame = 2;

            if (stage > 1)
            {
                startFrame = 3;
                finalFrame = 4;

            }
            if (crazy)
                finalFrame = 5;
            int frameSpeed = 20;
            NPC.frameCounter += 1f;
            NPC.frameCounter += Main.player[NPC.target].velocity.Length() / 2f; // Make the counter go faster with more movement speed
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight*frame_direction;

                
            }
            if (NPC.frame.Y <= startFrame * frameHeight)
            {
                // If we were animating the first stage frames and then switch to second stage, immediately change to the start frame of the second stage
                NPC.frame.Y = startFrame * frameHeight;
                frame_direction  =1;
            }
            if (NPC.frame.Y >= finalFrame * frameHeight)
            {
                NPC.frame.Y = finalFrame * frameHeight;
                frame_direction =-1;
            }
            
        }
        private float moving_diection(Vector2 velocity)
        {
            if (turning) return ((int)Main.time % 30)*6; 
            if (stage == 1) return 0 ;
            if (velocity.X< -30f)
                return -30f;
            else if (velocity.X>30f)
                return 30f;
            return velocity.X;
        }
        bool crazy = false;
        int stagepoint= 5;
        int length = 10;
        Vector2[] pos_old = new Vector2[10];
        float[] rot_old = new float[10];
        public override void AI()
        {
            
            //记录位置
            for (int i = length -1; i>0; i--)
            {
                pos_old[i]=pos_old[i-1];
                rot_old[i]=rot_old[i-1];
            }
            pos_old[0] = NPC.Center;
            rot_old[0] = NPC.rotation;

            //行为开始，索敌判断
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];
            if (player.dead)
            {
                // If the targeted player is dead, flee
                NPC.velocity= (NPC.velocity+new Vector2(1f, 0f)).RotatedBy(MathHelper.ToRadians(10f));
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                mahouSyoujyo.DelSceneShader("GrayScaleMajoConciousness");
                NPC.EncourageDespawn(10);
                
                return;
            }
            stagepoint = (((customed) ? bosslist.Contains("MoonLord") : NPC.downedMoonlord)) ? NPC.lifeMax*4/5 : NPC.lifeMax /2;
            if (!crazy &&NPC.life < stagepoint / 2)
            {
                SoundEngine.PlaySound(SoundID.Zombie122);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    crazy = true;
                    NPC.netUpdate = true;
                }
                
            }
            else if (NPC.life < stagepoint)
            {
                Music= MusicLoader.GetMusicSlot(Mod, "Radio/Music/Majo_consciousness_p2");
                //stage= 2;
            }
            else
            {
                stage=1;

            }
            if (stage == 1 && NPC.life < stagepoint)
                ToSecond();
            if (stage == 1) DoFisrtStage();
            else DoSecondStage();
            NPC.ai[0]++;
            if (NPC.ai[0] >= 30)
            {
                NPC.netUpdate = true;
                NPC.ai[0]=0;
            }
            if (3 <= action && action <= 11)
                for (int j = 0; j < 5; j++)
                {
                    Dust k = Dust.NewDustDirect(NPC.Center-new Vector2(NPC.width, NPC.height)*0.5f, NPC.width, NPC.height, 217, Scale: 1.2f);
                    k.noGravity = true;
                    k.color = Color.DodgerBlue;
                    k.noLight = false;
                    Lighting.AddLight(NPC.Center.ToWorldCoordinates(), Color.White.ToVector3() * 0.5f);
                }
        }
        private void DoFisrtStage()
        {
            NPC.damage = 0;
            NPC.defense = NPC.defDefense;
            NPC.alpha = ((int)Main.time*8 % 512>255) ? 511-(int)Main.time*8 % 512 : (int)Main.time*8 % 512;
            //Main.NewText(NPC.alpha);
            //Main.NewText(Main.time);
            //NPC.damage=0;
            SpawnProj();
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            Vector2 targetpos = Main.player[NPC.target].Center - new Vector2(0f, 160f);
            if ((targetpos -NPC.Center).Length()>800f)
            {
                NPC.Center = targetpos;
                NPC.netUpdate = true;
            }
            Vector2 oldvel = NPC.velocity;
            NPC.velocity = targetpos -NPC.Center;
            Vector2 vel = NPC.velocity;
            Vector2 velnomalized = vel.SafeNormalize(Vector2.Zero);
            float speedmax = (vel.Length()>400f) ? 15f : 10f;
            NPC.velocity = velnomalized*((speedmax< vel.Length() )? speedmax : vel.Length());
            if (Main.netMode == NetmodeID.Server && NPC.velocity !=oldvel)
            {
                //NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                NPC.netUpdate = true;
            }
        }
        private int totalAction=22;
        private void DoSecondStage()
        {
            SpawnProj();

            //Main.NewText(action);
            Player player = Main.player[NPC.target];
            int transtime = (Main.expertMode) ? 30 : 60;
            switch (action)
            {
                case 0: ToSecond(); break;
                case 1: Move(offsetX:0,offsetY:0); break;
                case 2: Checksprint(); break;
                case 3:
                    Transport(transtime, -600, -500, reset:30);
                    break;
                case 4:
                    Move(maxspeed: 30f,stagetime: 60);
                    break;
                case 5:
                    Transport(transtime, -600, -500, reset: 30);
                    break;
                case 6:
                    Move(maxspeed: 30f, stagetime: 60);
                    break;
                case 7:
                    Move(maxspeed: 30f, stagetime: 60);
                    break;
                case 8:
                    Transport(transtime, -600, -500, reset: 30);
                    break;
                case 9:
                    Move(maxspeed: 30f, stagetime: 60);
                    break;
                case 10:
                    Move(maxspeed: 30f, stagetime: 60);
                    break;
                case 11:
                    Move(maxspeed: 30f, stagetime: 60);
                    break;
                case 12: Checkbee();break;
                case 13: 
                    Transport(transtime, -600, -500);
                    break;
                case 14:
                    
                    Move(maxspeed: 30f,offsetX:0, offsetY: NPC.Center.Y- player.Center.Y,stagetime: 120);
                    spawnbee(player);
                    break;
                case 15: Move(); break;
                case 16: Checkflash(); break;
                case 17:
                    Transport(transtime, 0, -600);
                    break; 
                case 18:
                    Transport((Main.expertMode) ? 300 : 240, 800, -600);
                    spawnflash(player);
                    break;
                case 19: Move(); break;
                case 20: Checkturnround(); break;
                case 21:
                    Transport(transtime, Math.Abs(player.velocity.X)*transtime+600f, player.velocity.Y*transtime); 
                    break;
                default:
                    Turning();
                    break;
            }
            //NPC.Center = Main.player[NPC.target].Center - new Vector2(0f, 160f);
        }

        private void SpawnProj()
        {
            NPC.localAI[0]++;
            int colddown = Math.Max(30, 90-makePower()*5);
            if (NPC.localAI[0]<colddown)
                return;
            NPC.localAI[0] = 0;
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int prefocus = Math.Min(15, 5+makePower());
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player tplayer = Main.player[NPC.target];
            projList = makeProjList();
            int num = (stage>1) ? 1 : makePower() / 8+1;
            if (Main.expertMode) num++;
            if (crazy) num++;
            for (int i = 0; i <num; i++)
            {
                float shootangle = Math.Clamp((float)(NPC.lifeMax-NPC.life) / (float)(NPC.lifeMax-stagepoint), 0f, 1f)*Main.rand.NextFloat(-45f,45f);
                Vector2 pos = tplayer.Center + new Vector2(0f , Main.rand.NextFloat(-400f, -320f)-10*makePower()).RotatedBy(MathHelper.ToRadians(shootangle));
                int index = Main.rand.Next(projList.Count);
                Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                    pos,
                    (tplayer.Center+prefocus*tplayer.velocity-pos).SafeNormalize(Vector2.Zero)*2f//速度方向加预判
                        *Modify((int)modifyID.projectileSpeed, projList[index]),//速度修正
                    projList[index],//射弹类型
                                    //伤害修正，传入的是伤害加成前的伤害（经典到大师是2/4/6倍加成
                    (int)(NPC.defDamage/6*Modify((int)modifyID.projectileDamage, projList[index])),
                    5f);//, owner:NPC.whoAmI会使射弹消失
                //Main.NewText(NPC.defDamage);
                //Main.NewText(NPC.damage);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, number: projectile.whoAmI);
                }
                
            }
            NPC.netUpdate = true;


        }
        private void spawnbee(Player tplayer)
        {
            
            
            NPC.localAI[3]++;
            int colddown = (Main.expertMode) ? 12 : 20;
            if (crazy) colddown =colddown * 2 / 3;
            if (NPC.localAI[3]<colddown) 
                return;
            NPC.localAI[3]=0;
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int prefocus = Math.Min(15, 5+makePower());
            Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                    NPC.Center,
                    (tplayer.Center+prefocus*tplayer.velocity-NPC.Center).SafeNormalize(Vector2.Zero)*2f//速度方向加预判
                        *Modify((int)modifyID.projectileSpeed, ProjectileID.QueenBeeStinger),//速度修正
                    ProjectileID.QueenBeeStinger,//射弹类型
                                                 //伤害修正，传入的是伤害加成前的伤害（经典到大师是2/4/6倍加成
                    (int)(NPC.defDamage/6*Modify((int)modifyID.projectileDamage, ProjectileID.QueenBeeStinger)),
                    5f);//, owner:NPC.whoAmI会使射弹消失
                        //Main.NewText(NPC.defDamage);
                        //Main.NewText(NPC.damage);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile.whoAmI);
            }
            NPC.netUpdate = true;


        }
        private void spawnflash(Player tplayer)
        {
            NPC.localAI[3]++;
            int colddown = 60;
            if (NPC.localAI[3]<colddown)
                return;
            NPC.localAI[3]=0;
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int prefocus = Math.Min(15, 5+makePower());
            Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                    NPC.Center,
                    (tplayer.Center+prefocus*tplayer.velocity-NPC.Center).SafeNormalize(Vector2.Zero)*2f//速度方向加预判
                        *Modify((int)modifyID.projectileSpeed, ProjectileID.CultistBossLightningOrb),//速度修正
                    ProjectileID.CultistBossLightningOrb,//射弹类型
                                                 //伤害修正，传入的是伤害加成前的伤害（经典到大师是2/4/6倍加成
                    (int)(NPC.defDamage/6*Modify((int)modifyID.projectileDamage, ProjectileID.CultistBossLightningOrb)),
                    5f);//, owner:NPC.whoAmI会使射弹消失
                        //Main.NewText(NPC.defDamage);
                        //Main.NewText(NPC.damage);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile.whoAmI);
            }
            NPC.netUpdate = true;


        }
        private void ToSecond()
        {
            NPC.damage = 0;
            NPC.defense = NPC.defDefense;
            if (stage==1)
            {
                SoundEngine.PlaySound(SoundID.Roar);

                stage = 2;    
                NPC.netUpdate = true;
            }
            if (counter > 120 /2) NPC.damage = NPC.defDamage / 2;
            else NPC.damage = 0;
            NPC.alpha=(counter>120 /2) ? (120-counter)*255/(120/2) : counter*255/(120/2);
            counter++;
            NPC.velocity = Vector2.Zero;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    NPC.TargetClosest();
                }
                Player player = Main.player[NPC.target];

                if (counter == 120 / 2)
                {
                    NPC.Center = player.Center + player.direction*(new Vector2(1f, 0f))*((Main.expertMode) ? 600f : 800f);
                    NPC.netUpdate = true;
                }
            }
            if (counter>=120)
            {
                counter = 0;
                action = 1;
            }
        }
        private void Transport(int time, float offsetX, float offsetY, int reset = 0)//传送
        {
            if (counter == Math.Max(time / 2-((reset>=0) ? reset : 0), 1))
            {
                if (action== 3 || action == 5 || action ==8)
                    SoundEngine.PlaySound(SoundID.Zombie20);
            }
            turning = false;
            if (!((customed) ? bosslist.Contains("KingSlime") : NPC.downedSlimeKing))
            {
                Move(offsetX:offsetX, offsetY:offsetY,stagetime: time );
                return;
            }
            
            if (counter > time /2) NPC.damage = NPC.defDamage / 2;
            else NPC.damage = 0;
            NPC.defense = NPC.defDefense;
            NPC.alpha=(counter>time /2 ) ? (time-counter)*255/(time/2) : counter*255/(time/2);
            counter++;

            NPC.velocity = Vector2.Zero;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    NPC.TargetClosest();
                }
                Player player = Main.player[NPC.target];

                if (counter == Math.Max(time / 2-((reset>=0)?reset:0),1))
                {
                    NPC.Center = player.Center + (new Vector2(player.direction*offsetX, offsetY));
                    NPC.netUpdate = true;
                    
                }
            }
            if (counter>=time)
            {
                counter = 0;
                action= action % totalAction +1;
                NPC.localAI[3]=0;
            }
        }
        Vector2 nomalized = Vector2.Zero;
        private void Move(float maxspeed = 10f , float offsetX = 0, float offsetY = 0, int stagetime = 120)//移动
        {
            NPC.damage = NPC.defDamage / 2;
            if (action== 4 || action == 6 || action ==7 ||
                action== 9 || action == 10 || action ==11)
                NPC.defense = 0;
            else NPC.defense = NPC.defDefense;
            NPC.alpha = 0;
            turning = false;
            counter++;
            if (Main.netMode != NetmodeID.MultiplayerClient) 
            {
                if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    NPC.TargetClosest();
                }
                Player player = Main.player[NPC.target];
                maxspeed = (Main.expertMode) ? maxspeed+5f+makePower() : maxspeed+makePower();
                if (((customed) ? bosslist.Contains("TheEyeOfCthulhu") : NPC.downedBoss1)) maxspeed += 5f;
                if (player.Center.Distance(NPC.Center)>1200f) maxspeed*=2f;
                else if (player.Center.Distance(NPC.Center)>800f) maxspeed*=1.5f;


                if (counter == 1)
                {
                    NPC.velocity = Vector2.Zero;
                    transpos=player.Center+new Vector2(player.direction*offsetX, offsetY);
                    nomalized = (transpos-NPC.Center).SafeNormalize(Vector2.Zero);
                    //Main.NewText(NPC.velocity);
                }
                if (counter <=stagetime / 2) NPC.velocity+=nomalized*maxspeed/(stagetime /2);
                else NPC.velocity-=nomalized*maxspeed/(stagetime /2);
                NPC.netUpdate = true;
            }
            

            //Main.NewText(nomalized);

            if (counter>=stagetime)
            {
                counter = 0;
                action= action % totalAction +1;
                NPC.localAI[3]=0;
            }
        }
        bool turning = false;
        private void Turning(int time = 300)
        {
            NPC.damage = NPC.defDamage / 2 ;
            NPC.alpha = 0;
            NPC.defense = 0;
            turning = true;
            counter++;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    NPC.TargetClosest();
                }
                Player player = Main.player[NPC.target];
                Vector2 toward = player.Center-NPC.Center;
                float length=toward.Length();
                float speed = (Main.expertMode) ? 5f : 3f;
                speed += makePower()/3;
                if (length>1000f) speed *= 3;
                else if (length>600f) speed *= 2;
                nomalized = toward.SafeNormalize(Vector2.Zero);
                NPC.velocity=nomalized*(Math.Min(speed, toward.Length()));
                NPC.netUpdate = true;
            }


            //Main.NewText(nomalized);

            if (counter>=time)
            {
                counter = 0;
                action= action % totalAction +1;
                NPC.localAI[3]=0;
                turning = false;
            }
        }
        private void Checkbee()
        {
            if (!((customed)? bosslist.Contains("QueenBee") : NPC.downedQueenBee) )
                action+=3;
            else
            {
                action++;
                SoundEngine.PlaySound(SoundID.Zombie125);
            }
            counter = 0;
            NPC.localAI[3]=0;
            
        }
        private void Checksprint()
        {

            if (!((customed) ? bosslist.Contains("DukeFishron") : NPC.downedFishron)  || !crazy) action+=10;
            else
            {
                action++;
                
            }
            counter = 0;
            NPC.localAI[3]=0;
        }
        private void Checkflash()
        {

            if (!((customed) ? bosslist.Contains("LunaticCultist") : NPC.downedAncientCultist) || !crazy)
                action+=3;
            else
            {
                action++;
                SoundEngine.PlaySound(SoundID.Zombie89);
            }
            counter = 0;
            NPC.localAI[3]=0;
        }
        private void Checkturnround()
        {

            if (!((customed) ? bosslist.Contains("Skeletron") : NPC.downedBoss3)) action=1;
            else
            {
                action++;
                SoundEngine.PlaySound(SoundID.Roar);
            }
            counter = 0;
            NPC.localAI[3]=0;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            // If the NPC dies, spawn gore and play a sound
            if (Main.netMode == NetmodeID.Server)
            {
                // We don't want Mod.Find<ModGore> to run on servers as it will crash because gores are not loaded on servers
                return;
            }
            Dust hitd = Dust.NewDustDirect(NPC.position,  NPC.width,NPC.height, (!ModContent.GetInstance<ClientConfigs>().ConsciousBeauty) ? DustID.t_Granite : 142, Scale: 1.5f);
            hitd.noGravity = false;
            if (NPC.life <= 0)
            {
                for (int loops = 0; loops < 10; loops++)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                        Dust d = Dust.NewDustPerfect(NPC.Center, (!ModContent.GetInstance<ClientConfigs>().ConsciousBeauty)?DustID.t_Granite: 142, speed * 5 * (loops + 1), Scale: 1.5f);
                        d.noGravity = true;
                    }
                }
                // These gores work by simply existing as a texture inside any folder which path contains "Gores/"
                int left1GoreType = Mod.Find<ModGore>("Majo_Consciousness_Left1").Type;
                int left2GoreType = Mod.Find<ModGore>("Majo_Consciousness_Left2").Type;
                int right1GoreType = Mod.Find<ModGore>("Majo_Consciousness_Right1").Type;
                int right2GoreType = Mod.Find<ModGore>("Majo_Consciousness_Right2").Type;
                var entitySource = NPC.GetSource_Death();
                Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), left1GoreType);
                Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), left2GoreType);
                Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), right1GoreType);
                Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), right2GoreType);
                SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);

                // This adds a screen shake (screenshake) similar to Deerclops
                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
                Main.instance.CameraModifiers.Add(modifier);
                
            }
        }
        public override void OnKill()
        {
            mahouSyoujyo.DelSceneShader("GrayScaleMajoConciousness");
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedConciousBoss, -1);
            base.OnKill();
        }
    }

}
