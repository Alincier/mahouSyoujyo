// WitchsNight.cs
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Chat;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using mahouSyoujyo.Content.NPCs.BOSSes.Majo_Consciousness;
using System;
using mahouSyoujyo.Common.Configs;
using System.IO;
using Terraria.ModLoader.IO;

namespace mahouSyoujyo.Common.Systems
{

    public struct SpawnData
    {
        public int InvasionContributionPoints { get; set; }
        public float SpawnRate { get; set; }
        public WhereToSpawn Wheretospawn { get; set; }
        public SpawnData(int totalPoints, float spawnRate, WhereToSpawn whereToSpawn)
        {
            InvasionContributionPoints = totalPoints;
            SpawnRate = spawnRate;
            Wheretospawn = whereToSpawn;
        }
    }
    public enum WhereToSpawn
    {
        Land,
        Air,
        Anywhere
    }
    public class WitchNightSystem : ModSystem
    {
        public static float shaderDegree = 0;
        public const float changeTime = 180;
        public static bool EventActive = false;
        public static float EventProgress = 0f;
        public static bool happenTryed = false;
        public static Dictionary<int, SpawnData> PossibleEnemiesPre;
        public static Dictionary<int, SpawnData> PossibleEnemiesHard;
        public static Dictionary<int, SpawnData> PossibleMinibossesPre;
        public static Dictionary<int, SpawnData> PossibleMinibossesHard;
        public override void OnModLoad()
        {
            PossibleEnemiesPre = new()
            {

            };
            PossibleEnemiesHard = new()
            {
                
            };

            PossibleMinibossesPre = new()
            {   
                { ModContent.NPCType<Majo_Consciousness>(), new SpawnData(1, 0.75f, WhereToSpawn.Anywhere) },
            };
            PossibleMinibossesHard = new()
            {
                { ModContent.NPCType<Majo_Consciousness>(), new SpawnData(1, 0.75f, WhereToSpawn.Anywhere) },
            };
        }

        // Clear enemy cache lists.
        public override void Unload()
        {
            PossibleEnemiesPre = new();
            PossibleEnemiesHard = new();
            PossibleMinibossesPre = new();
            PossibleMinibossesHard = new();
        }
        public static void TryBegin()
        {
            // 当进入夜晚时，有20%概率发生魔女之夜
            if (!Main.dayTime  && !happenTryed)
            {
                if (false)//Main.rand.NextBool(5))
                {
                    EventActive = true;
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.mahouSyoujyo.Commons.WitchNightBegin"),  new Color(50, 255, 130));
                    NetMessage.TrySendData(MessageID.WorldData);
                }
                happenTryed = true;
            }
        }
        public static void TryEnd()
        {
            // 如果是白天，则事件结束
            if (Main.dayTime)
            {
                if (EventActive)
                {
                    EventActive = false;
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.mahouSyoujyo.Commons.WitchNightEnd"), new Color(50, 255, 130));
                    NetMessage.TrySendData(MessageID.WorldData);
                }
                if (happenTryed) happenTryed = false;
                return;
            }
        }
        public static void Update()
        {
            if (!EventActive) return;
            if (NPC.AnyNPCs(ModContent.NPCType<Majo_Consciousness>())) return;

        }
        public static void DrawShader()
        {
            if (Main.netMode == NetmodeID.Server) return;   
            if (EventActive) shaderDegree++;
            else shaderDegree--;
            shaderDegree = Math.Clamp(shaderDegree, 0, changeTime);
            if (shaderDegree > 0 && Main.netMode != NetmodeID.Server && ( Main.LocalPlayer.ZoneOverworldHeight || Main.LocalPlayer.ZoneDirtLayerHeight || Main.LocalPlayer.ZoneSkyHeight))
            {
                Color color = Color.DarkViolet;
                mahouSyoujyo.SceneShader("ColorScaleDynamic", shaderDegree / changeTime, factor: 2 , r0: color.R, r1 : color.G , r2 : color.B);

            }
            else 
            {
                mahouSyoujyo.DelSceneShader("ColorScaleDynamic");
            }
        }
        public override void PostUpdateEverything()
        {
            DrawShader();
            base.PostUpdateEverything();
        }
        public override void ClearWorld()
        {
            EventActive = false;
            // downedOtherBoss = false;
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.
        public override void SaveWorldData(TagCompound tag)
        {
            if (EventActive)
            {
                tag["WitchNight"] = true;
            }

        }

        public override void LoadWorldData(TagCompound tag)
        {
            EventActive = tag.ContainsKey("WitchNight");
            // downedOtherBoss = tag.ContainsKey("downedOtherBoss");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = EventActive;
            writer.Write(flags);

        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            EventActive = (bool)flags[0];

        }




        public override void ModifyScreenPosition()
        {
            if (EventActive)
            {
                // 紫色滤镜效果
              //  Filters.Scene["YourModName:WitchsNight"].Activate(
              //      Main.LocalPlayer.position,
              //      Color.White);
            }
        }

        public override void ResetNearbyTileEffects()
        {
            if (EventActive)
            {
                // 应用着色器
              //  Filters.Scene.Activate("YourModName:WitchsNight");
            }
        }
    }
}