using mahouSyoujyo.Content.Buffs;
using mahouSyoujyo.Content.Items;
using mahouSyoujyo.Content.NPCs.BOSSes.Majo_Consciousness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using mahouSyoujyo.Common.Configs;
using Terraria.Localization;
using Humanizer;

namespace mahouSyoujyo.Globals
{
    public class MGPlayer : ModPlayer
    {
        public static LocalizedText DespairDeathMessage { get; private set; }
        public static LocalizedText LeftGemDeathMessage { get; private set; }
        public bool relief = false;
        public int left_gem_time = 0;
        public int polluted_time = 0;
        public int deadline = 90;
        //加成字段
        public int damage_bonus = 0;
        public int crit_bonus = 0;
        public int defense_bonus = 0;
        public int health_bonus = 0;
        public int mana_bonus = 0;
        public int regen_bonus = 0;
        public int enduce_bonus = 0;
        public int speed_bonus = 0;
        public int mana_reduce = 0;
        public int armor_penetration = 0;
        public int summon_bonus = 0;
        //是否为魔法少女的字段
        public bool magia = false;
        private bool initial_magia = false;
        public int power = 0;
        public string lastReforge = null;
        public override void SetStaticDefaults()
        {
            DespairDeathMessage = Language.GetText("Mods.mahouSyoujyo.Commons.DespairDeath");
            LeftGemDeathMessage = Language.GetText("Mods.mahouSyoujyo.Commons.LeftGemDeath");
        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("GetRelief", out bool isRelieved))
                relief = isRelieved;
            if (tag.TryGet("IsMagicGirl", out initial_magia))
                if (initial_magia == true)
                {
                    magia = true;
                }
            if (magia)
            {
                if (tag.TryGet("leftTime", out int value1)) left_gem_time = value1;
                if (tag.TryGet("pollutedTime", out int value2)) polluted_time = value2;
            }
            base.LoadData(tag);
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add("GetRelief", relief);
            tag.Add("IsMagicGirl", magia);
            tag.Add("leftTime", left_gem_time);
            tag.Add("pollutedTime", polluted_time);
            base.SaveData(tag);
        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)mahouSyoujyo.MessageType.MGPlayerSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write(left_gem_time);
            packet.Write(polluted_time);
            packet.Write(relief);
            packet.Send(toWho, fromWho);
        }

        // Called in ExampleMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            left_gem_time = reader.ReadInt32();
            polluted_time = reader.ReadInt32();
            relief = reader.ReadBoolean();
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            MGPlayer clone = (MGPlayer)targetCopy;
            clone.relief = relief;
            clone.left_gem_time = left_gem_time;
            clone.polluted_time = polluted_time;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            MGPlayer clone = (MGPlayer)clientPlayer;

            if (clone.relief != relief || clone.left_gem_time != left_gem_time || clone.polluted_time != polluted_time)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
        public bool notInDespair()
        {
            return polluted_time < deadline * 60 * 60;
        }
        public override void PostUpdate()
        {

            if (!NPC.AnyNPCs(ModContent.NPCType<Majo_Consciousness>()))
                mahouSyoujyo.DelSceneShader("GrayScaleMajoConciousness");


            //初始化所有加成

            damage_bonus = 0;
            crit_bonus = 0;
            defense_bonus = 0;
            health_bonus = 0;
            mana_bonus = 0;
            regen_bonus = 0;
            enduce_bonus = 0;
            speed_bonus = 0;
            mana_reduce = 0;
            armor_penetration = 0;
            summon_bonus = 0;
            power = 0;
            //计算强化加成
            culculate_bonus();
            if (power > 12) deadline = 57 - power;
            else if (power > 9) deadline = 69 - power * 2;
            else if (power > 6) deadline = 78 - power * 3;
            else deadline = 90 - power * 5;
            //初始化 buff状态
            if (initial_magia)
            {
                initial_magia = false;
                if (!Player.HasBuff<MagicGirlPover>()) Player.AddBuff(ModContent.BuffType<MagicGirlPover>(), 10);
            }
            magia = false;
            if (Player.HasBuff<MagicGirlPover>()) magia = true;
            //处理魔法少女buff的负面效果
            if (magia)
            {
                Player.GetModPlayer<InfoDisplayPlayer>().showPollution = true;
                if (!relief)
                {
                    if (left_gem_time < 1200) left_gem_time++;
                    else left_gem_time = 601;
                    if (polluted_time < 60 * 60 * deadline) polluted_time++;
                }
                if (Player.HasBuff(BuffID.ManaSickness))
                {
                    if (!relief) polluted_time += 12 * Player.buffTime[Player.FindBuffIndex(BuffID.ManaSickness)];
                    Player.ClearBuff(BuffID.ManaSickness);
                }
            }
            else left_gem_time = polluted_time = 0;
            if (left_gem_time > 300 && Player == Main.LocalPlayer)
            {
                Player.AddBuff(BuffID.Silenced, 2);
                Player.AddBuff(BuffID.Blackout, 2);
                Player.AddBuff(BuffID.Suffocation, 2);
                Player.AddBuff(BuffID.Slow, 2);
                Player.AddBuff(BuffID.Chilled, 2);
            }
            if (left_gem_time >= 600 && Player == Main.LocalPlayer)
            {

                if (left_gem_time % 120 == 0) SoundEngine.PlaySound(SoundID.Zombie83, Player.Center);
            }
        }
        //加成计算函数
        private void culculate_bonus()
        {
            if (NPC.downedBoss1)
            {
                power++;
                damage_bonus += 2;
                crit_bonus += 1;
                speed_bonus += 2;
                mana_reduce += 1;
            }
            if (NPC.downedSlimeKing)
            {
                power++;
                damage_bonus += 1;
                regen_bonus += 1;
                defense_bonus += 1;
                speed_bonus += 2;
            }
            if (NPC.downedBoss2)
            {
                power++;
                damage_bonus += 1;
                crit_bonus += 1;
                defense_bonus += 1;
                health_bonus += 5;
                mana_bonus += 5;
                enduce_bonus += 1;
                mana_reduce += 1;
            }
            if (NPC.downedBoss3)
            {
                power++;
                damage_bonus += 2;
                crit_bonus += 1;
                defense_bonus += 1;
                enduce_bonus += 1;
                armor_penetration += 1;
                mana_reduce += 1;
            }
            if (NPC.downedQueenBee)
            {
                power++;
                damage_bonus += 1;
                regen_bonus += 1;
                speed_bonus += 3;
            }
            if (NPC.downedDeerclops)
            {
                power++;
                damage_bonus += 1;
                crit_bonus += 1;
                defense_bonus += 1;
                enduce_bonus += 1;
            }
            if (Main.hardMode)
            {
                power++;
                damage_bonus += 2;//10
                crit_bonus += 1;//5
                defense_bonus += 1;//5
                health_bonus += 5;//10
                mana_bonus += 5;//10
                regen_bonus += 1;//3
                enduce_bonus += 2;//5
                speed_bonus += 3;//10
                armor_penetration += 1;//3
                mana_reduce += 2;//5
                summon_bonus += 1;//1
            }

            if (NPC.downedQueenSlime)
            {
                power++;
                damage_bonus += 2;
                crit_bonus += 1;
                defense_bonus += 1;
                enduce_bonus += 1;
                speed_bonus += 2;
                mana_reduce += 1;
            }
            if (NPC.downedMechBoss1)
            {
                power++;
                damage_bonus += 2;
                crit_bonus += 1;
                defense_bonus += 1;
                enduce_bonus += 1;
                armor_penetration += 1;
            }
            if (NPC.downedMechBoss2)
            {
                power++;
                damage_bonus += 2;
                crit_bonus += 1;
                defense_bonus += 1;
                regen_bonus += 1;
                armor_penetration += 1;
            }
            if (NPC.downedMechBoss3)
            {

                power++;
                damage_bonus += 2;
                crit_bonus += 1;
                defense_bonus += 1;
                armor_penetration += 1;
                mana_reduce += 1;

            }
            if (NPC.downedEmpressOfLight)
            {
                power++;
                damage_bonus += 3;
                crit_bonus += 2;
                defense_bonus += 2;
                health_bonus += 5;
                mana_bonus += 5;
                regen_bonus += 1;
                enduce_bonus += 1;
                speed_bonus += 3;
                mana_reduce += 1;
                armor_penetration += 1;
            }
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                health_bonus += 5;
                mana_bonus += 5;
                summon_bonus += 1;
                regen_bonus += 1;
                enduce_bonus += 1;
                armor_penetration += 2;
                mana_reduce += 1;
            }
            if (NPC.downedPlantBoss)
            {
                power++;
                damage_bonus += 2;
                crit_bonus += 2;
                defense_bonus += 1;
                enduce_bonus += 1;
                armor_penetration += 2;
                mana_reduce += 1;
            }
            if (NPC.downedGolemBoss)
            {
                power++;
                damage_bonus += 2;
                crit_bonus += 2;
                defense_bonus += 1;
                enduce_bonus += 2;
                armor_penetration += 2;
                mana_reduce += 2;
            }
            if (NPC.downedFishron)
            {
                power++;
                damage_bonus += 3;
                crit_bonus += 3;
                defense_bonus += 1;
                regen_bonus += 1;
                enduce_bonus += 2;
                speed_bonus += 3;
                armor_penetration += 2;
                mana_reduce += 1;
            }
            if (NPC.downedAncientCultist)
            {
                power++;
                damage_bonus += 2;
                crit_bonus += 2;
                defense_bonus += 1;
                enduce_bonus += 1;
                speed_bonus += 2;
                armor_penetration += 1;
                mana_reduce += 2;
            }
            if (NPC.downedMoonlord)
            {
                power++;
                damage_bonus += 10;
                crit_bonus += 10;
                defense_bonus += 5;
                health_bonus += 10;
                mana_bonus += 10;
                regen_bonus += 3;
                enduce_bonus += 5;
                speed_bonus += 5;
                armor_penetration += 5;
                mana_reduce += 5;
                summon_bonus += 1;
            }

        }
        //离开宝石和绝望判断
        public override void UpdateBadLifeRegen()
        {
            if (left_gem_time >= 60 * 10)
                Player.lifeRegen -= Player.statLifeMax2 / 2;
            if (!notInDespair())
                Player.lifeRegen -= Player.statLifeMax2 / 2;
            base.UpdateBadLifeRegen();
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (ModContent.GetInstance<ServerConfigs>().MajoSpawnInDespair && magia &&
                polluted_time >= 60 * 60 * deadline && !relief
                && !NPC.AnyNPCs(ModContent.NPCType<Majo_Consciousness>()))
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    // If the player using the item is the client
                    // (explicitly excluded serverside here)
                    SoundEngine.PlaySound(SoundID.Roar, Player.position);

                    int type = ModContent.NPCType<Majo_Consciousness>();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // If the player is not in multiplayer, spawn directly
                        NPC.SpawnOnPlayer(Player.whoAmI, type);
                    }
                    else
                    {
                        // If the player is in multiplayer, request a spawn
                        // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Player.whoAmI, number2: type);
                    }
                }
            }

            base.Kill(damage, hitDirection, pvp, damageSource);
        }
        public override void UpdateDead()
        {
            left_gem_time = 0;
            if (polluted_time >= 60 * 60 * deadline) polluted_time = 60 * 60 * (deadline - 10);
            base.UpdateDead();
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (magia && polluted_time >= 60 * 60 * deadline)
                damageSource = PlayerDeathReason.ByCustomReason(DespairDeathMessage.Format(Player.name));
            else if (magia && left_gem_time >= 60 * 10)
                damageSource = PlayerDeathReason.ByCustomReason(LeftGemDeathMessage.Format(Player.name));

            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
    }
}
