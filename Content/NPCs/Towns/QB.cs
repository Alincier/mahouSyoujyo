using mahouSyoujyo.Content.Buffs;
using mahouSyoujyo.Content.Items;
using mahouSyoujyo.Content.Items.MeleeWeapon;
using mahouSyoujyo.Content.Items.MagicWeapon;
using mahouSyoujyo.Content.Items.RangedWeapon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using mahouSyoujyo.Content.Items.SpecialWeapon;
using mahouSyoujyo.Common.Configs;
using System.Runtime.Intrinsics.Arm;
using mahouSyoujyo.Content.Items.Placeable;

namespace mahouSyoujyo.Content.NPCs.Towns
{ 
	[AutoloadHead]
	public class QB : ModNPC
	{
        int npcID = NPCID.Merchant;
        public override void SetStaticDefaults()
        {
            //游戏内显示的称呼
            //总帧数，根据使用贴图的实际帧数进行填写，这里我们直接调用全部商人的数据
            Main.npcFrameCount[Type] = Main.npcFrameCount[npcID];
            //特殊交互帧（如坐下，攻击）的数量，其作用就是规划这个NPC的最大行走帧数为多少，
            //最大行走帧数即Main.npcFrameCount - NPCID.Sets.ExtraFramesCount
            NPCID.Sets.ExtraFramesCount[Type] = NPCID.Sets.ExtraFramesCount[npcID];
            //攻击帧的数量，取决于你的NPC属于哪种攻击类型，
            NPCID.Sets.AttackFrameCount[Type] = NPCID.Sets.AttackFrameCount[npcID];
            //NPC的攻击方式，同样取决于你的NPC属于哪种攻击类型，投掷型填0，远程型填1，魔法型填2，近战型填3，
            //如果是宠物没有攻击手段那么这条将不产生影响
            NPCID.Sets.AttackType[Type] = NPCID.Sets.AttackType[npcID];
            //NPC的帽子位置中Y坐标的偏移量，这里特指派对帽，
            //当你觉得帽子戴的太高或太低时使用这个做调整（所以为什么不给个X的）  
            NPCID.Sets.HatOffsetY[Type] =16;
            int[] offset_QB =new int[25] { 0, 0, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            //这个名字比较抽象，可以理解为 [记录了NPC的某些帧带来的身体起伏量的数组] 的索引值，
            //而这个数组的名字叫 NPCID.Sets.TownNPCsFramingGroups ，详情请在源码的NPCID.cs与Main.cs内进行搜索。
            //举个例子：你应该注意到了派对帽或是机械师背后的扳手在NPC走动时是会不断起伏的，靠的就是用这个进行调整，
            //所以说在画帧图时最好比着原版NPC的帧图进行绘制，方便各种数据调用
            //补充：这个属性似乎是针对城镇NPC的。
            //魔法型NPC在攻击时产生的魔法光环的颜色，如果NPCID.Sets.AttackType不为2那就不会产生光环
            //如果NPCID.Sets.AttackType为2那么默认为白色
            NPCID.Sets.MagicAuraColor[Type] = Color.White;
            //NPC的单次攻击持续时间，如果你的NPC需要持续施法进行攻击可以把这里设置的很长，
            //比如树妖的这个值就高达600
            //补充说明一点：如果你的NPC的AttackType为3即近战型，
            //这里最好选择套用，因为近战型NPC的单次攻击时间是固定的
            NPCID.Sets.AttackTime[Type] = NPCID.Sets.AttackTime[npcID];
            //NPC的危险检测范围，以像素为单位，这个似乎是半径
            NPCID.Sets.DangerDetectRange[Type] = 500;
            //NPC在遭遇敌人时发动攻击的概率，如果为0则该NPC不会进行攻击（待验证）
            //遇到危险时，该NPC在可以进攻的情况下每帧有 1 / (NPCID.Sets.AttackAverageChance * 2) 的概率发动攻击
            //注：每帧都判定
            NPCID.Sets.AttackAverageChance[Type] = 1;
            //图鉴设置部分
            //将该NPC划定为城镇NPC分类
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                //为NPC设置图鉴展示状态，赋予其Velocity即可展现出行走姿态
                Velocity = 1f,
            };
            //添加信息至图鉴
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            //设置对邻居和环境的喜恶，也就是幸福度设置
            //幸福度相关对话需要写在hjson里，见下文所讲
            NPC.Happiness
                .SetBiomeAffection<CrimsonBiome>(AffectionLevel.Hate)//憎恶猩红环境
                .SetBiomeAffection<CorruptionBiome>(AffectionLevel.Hate)//憎恶腐化环境
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)//讨厌地下环境
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Like)//喜欢森林环境
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Love)//最爱神圣环境
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)//讨厌与渔夫做邻居
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Like)//喜欢与向导做邻居
                                                                  //邻居的喜好级别和环境的AffectionLevel是一样的
            ;
        }
        public override void SetDefaults()
        {
            //判断该NPC是否为城镇NPC，决定了这个NPC是否拥有幸福度对话，是否可以对话以及是否会被地图保存
            //当然以上这些属性也可以靠其他的方式开启或关闭，我们日后再说
            NPC.townNPC = true;
            //该NPC为友好NPC，不会被友方弹幕伤害且会被敌对NPC伤害
            NPC.friendly = true;
            //碰撞箱宽，不做过多解释，此处为标准城镇NPC数据
            NPC.width = 18;
            //碰撞箱高，不做过多解释，此处为标准城镇NPC数据
            NPC.height = 40;
            //套用原版城镇NPC的AIStyle，这样我们就不用自己费劲写AI了，
            //同时根据我以往的观测结果发现这个属性也决定了NPC是否会出现在入住列表里，还请大佬求证
            NPC.aiStyle = NPCAIStyleID.Passive;
            //伤害，由于城镇NPC没有体术所以这里特指弹幕伤害（虽然弹幕伤害也是单独设置的所以理论上这个可以不写？）
            NPC.damage = 40;
            //防御力
            NPC.defense = 9999;
            //最大生命值
            NPC.lifeMax = 50;
            //受击音效
            NPC.HitSound = SoundID.Item57;
            //死亡音效
            NPC.DeathSound = SoundID.NPCDeath1;
            //抗击退性，数据越小抗性越高
            NPC.knockBackResist = 0.01f;
            //模仿的动画类型，这样就不用自己费劲写动画播放了
            AnimationType = NPCID.Merchant;
            
        }
        //设置NPC的攻击力
        //设置图鉴内信息
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
        //设置所属环境，一般填写他最喜爱的环境
        BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
        //图鉴内描述
        new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("description"))});
        }
        //设置入住条件
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            //返回条件为：存在玩家基础最大魔力达到100。
            foreach (var player in Main.player )
            {
                if (player.statManaMax >= 100)
                {
                 // if
                 return true;
                }
            } 
            return false;
        }
        //设置姓名
        public override List<string> SetNPCNameList()
        {
            //所有可能出现的名字
            return new List<string>() {
        "Ta**** U*",
            };
        }
        //决定NPC会被哪座雕像传送
        public override bool CanGoToStatue(bool toKingStatue)
        {
            //可以被国王雕像传送
            return !toKingStatue;
        }
        //当NPC在被雕像传送时会发生什么
        public override void OnGoToStatue(bool toKingStatue)
        {
            //在左下角弹出一句话
            if (toKingStatue)
                Main.NewText("[c/AADASD:小丘比:啾？CHU?]");
            else
                Main.NewText("[c/AADASD:小丘比:啾？CHU?]");
        }

        //设置对话
        public override string GetChat()
        {
            //声明一个int类型变量，查找一个whoAmI最靠前的、种类为向导的NPC并返回他的whoAmI
            int guide = NPC.FindFirstNPC(NPCID.Guide);
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                //当血月和日食都没有发生时
                if (!Main.bloodMoon && !Main.eclipse)
                {
                    //无家可归时
                    if (NPC.homeless)
                    {
                        chat.Add($"啾 唔唔~\n（没地方住啦~）\n(No Home for Me~)");
                    }
                    else
                    {
                        //自我介绍，NPC.FullName就是带上称呼的姓名，比如“小丘比T.U.”
                        chat.Add($"啾比！唔姆！\n（你好！我是小丘比！奇迹和魔法是有代价的，要三思而后行哦！）\n" +
                            $"(Nice to meet you! I am little Chubby! \n Miracles and magic girl power come at a cost, think twice!)");
                        //当查找到向导NPC时
                        if (guide != -1)
                        {
                            //GivenName上面有提
                            //稀有对话
                            chat.Add($"啾比！\n（向导{Main.npc[guide].GivenName}喜欢教别人，这让我想起以前学校里的老师……啊，我是说，你们的老师！）\n" +
                                $"(Guide {Main.npc[guide].GivenName} loves to teach other,he remains me of teacher at school in old days...\n" +
                                $"Oops, I, I mean... your teacher!)",0.4);
                        }
                        //正在举行派对时
                        if (BirthdayParty.PartyIsUp)
                        {
                            chat.Add($"啾比!\n（派对好开心！）\n(Happy Party! )");
                        }
                    }
                }
                //日食时
                if (Main.eclipse)
                {
                    chat.Add("啾姆姆？！~\n（这是魔女结界吗~？！~）\n(Is this the enchantment of witch?!)");
                }
                //血月时
                if (Main.bloodMoon)
                {
                    chat.Add("唔姆， 啾啾 啾萌……\n（红色的月亮，讨厌……）\n(Such bloody moon, I hate it...)");
                }
                return chat;
            }
        }
        //设置对话按钮的文本
        public override void SetChatButtons(ref string button, ref string button2)
        {
            //直接引用原版的“商店”文本
            button = Language.GetTextValue("LegacyInterface.28");
            //设置第二个按钮
            button2 = this.GetLocalizedValue("rescission");
        }
        //设置当对话按钮被摁下时会发生什么
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            //当第一个按钮被按下时
            if (firstButton)
            {
                //打开商店
                shopName="magicgirlShop";
            }
            //如果是第二个按钮被按下时
            else
            {
                //出现一句对话，使用这个属性可以直接设置NPC要说的话。
                Player player = Main.player[NPC.FindClosestPlayer()];
                if (player.HasBuff<MagicGirlPover>())
                {
                    if (player.CountItem(ModContent.ItemType<GriefSeed>())>= 13)
                    {
                        for (int i = 0; i < 13; i++)
                            player.ConsumeItem(ModContent.ItemType<GriefSeed>());
                        Main.npcChatText = this.GetLocalizedValue("get_rescission");
                        player.ClearBuff(ModContent.BuffType<MagicGirlPover>());
                        SoundEngine.PlaySound(SoundID.ResearchComplete);
                    }
                    else
                    {
                        Main.npcChatText = this.GetLocalizedValue("get_rescission_fail");
                    }
                }else
                    Main.npcChatText = this.GetLocalizedValue("NotMagicGirl");

            }
        }
        //设置商店内容
        public override void AddShops()
        {
            var enableshop = new Condition("QBShop", () => ModContent.GetInstance<ServerConfigs>().QBshop);
            var npcShop = new NPCShop(Type, "magicgirlShop");
            npcShop.Add<Items.NoteTips>()
            .Add<Items.soulGem>()
            .Add<PieceofGrief>()
            .Add<GriefSeed>(Condition.BloodMoon)
            .Add<MusicBox_Connect>()
            .Add<MusicBox_Mataashita>()
            .Add<MusicBox_AndImHome>()
            .Add<MusicBox_Magia>()
            .Add<MusicBox_Decretum>()
            .Add<MusicBox_SisPuellaMagica>()
            .Add<MusicBox_Naturally>()
            .Add<MusicBox_CredensJustitiam>()
            .Add<Firstbow>(Condition.Hardmode, enableshop)
            .Add<TimePlate>(Condition.Hardmode, enableshop)
            .Add<ColorFive>(Condition.Hardmode, enableshop)
            .Add<BlueSword>(Condition.Hardmode, enableshop)
            .Add<RPGweapon>(Condition.Hardmode, enableshop)
            .Add<YellowGun>(Condition.Hardmode, enableshop)
            .AllowFillingLastSlot()
            .Register();
            //在物品的SetDefault里面用Item.shopCustomPrice =Item.buyPrice(20, 0, 0, 0);设置价格
            //if (npcShop.TryGetEntry(ModContent.ItemType<TimePlate>(), out NPCShop.Entry entry1))
            //    entry1.Item.value = Item.buyPrice(20, 0, 0, 0);
        }
        //设置NPC的攻击力
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            //伤害，直接调用NPC本体的伤害
            damage = NPC.damage;
            //击退力，中规中矩的数据
            knockback = 3f;
        }
        //设置每次攻击完毕后的冷却时间
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            //基础冷却时间
            cooldown = 60;
            //额外冷却时间
            randExtraCooldown = 30;
        }
        //设置发射的弹幕
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            //射弹种类
            projType =ProjectileID.FlowerPetal;
            //弹幕发射延迟，最好只给魔法型NPC设置较高数据
            attackDelay = 30;
        }
        //设置发射弹幕的向量
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            //发射速度
            multiplier = 6f;
            //射击角度额外向上偏移的量
            gravityCorrection = 0f;
            //射击时产生的最大额外向量偏差
            randomOffset = 0.5f;
        }
    }
}
