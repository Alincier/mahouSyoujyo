using mahouSyoujyo.Common.Configs;
using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content;
using mahouSyoujyo.Content.Projectiles;
using mahouSyoujyo.Content.Projectiles.Weapon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Security.Policy;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace mahouSyoujyo.Globals
{
    
    public class TimeStop : ModPlayer
    {
        public int usetip = 0;
        public int spawnTime = 30; 
        public static readonly int[] immuneProjectile =
        {
            ModContent.ProjectileType<ColorFive_Gun>(),
            ModContent.ProjectileType<combo0>(),
            ModContent.ProjectileType<combo1>(),
            ModContent.ProjectileType<combo2>(),
            ModContent.ProjectileType<comboPurry>(),
            ModContent.ProjectileType<ComboCounter>(),
            ModContent.ProjectileType<MagicCircle>(),
            ModContent.ProjectileType<YellowGunLight>(),
            ModContent.ProjectileType<RedSpearBend>(),
            ModContent.ProjectileType<RedSpearBullet>(),
            ModContent.ProjectileType<RedSpearChainProj>(),
            //ÊÖ³ÖÉËº¦µ¯Ä»
            //Á´Çò
            ProjectileID.BallOHurt,
            ProjectileID.Sunfury,
            ProjectileID.BlueMoon,
            ProjectileID.FlowerPow,
            ProjectileID.FlowerPowPetal,
            ProjectileID.TheDaoofPow,
            ProjectileID.Flairon,
            ProjectileID.FlaironBubble,
            ProjectileID.DripplerFlail,
            ProjectileID.DripplerFlailExtraBall,
            //±Þ×Ó
            ProjectileID.BlandWhip,
            ProjectileID.BoneWhip,
            ProjectileID.CoolWhip,
            ProjectileID.CoolWhipProj,
            ProjectileID.FireWhip,
            ProjectileID.FireWhipProj,
            ProjectileID.IvyWhip,
            ProjectileID.MaceWhip,
            ProjectileID.RainbowWhip,
            ProjectileID.ScytheWhip,
            ProjectileID.ScytheWhipProj,
            ProjectileID.SwordWhip,
            ProjectileID.ThornWhip,
            //Ä§ÕÈ
            ProjectileID.MagicMissile,
            ProjectileID.Flamelash,
            ProjectileID.RainbowRodBullet,
            ProjectileID.IceBlock,
            //»ØÐýïÚ
            ProjectileID.Flamarang,
            ProjectileID.EnchantedBoomerang,
            ProjectileID.WoodenBoomerang,
            ProjectileID.IceBoomerang,
            ProjectileID.Bananarang,
            ProjectileID.Shroomerang,
            ProjectileID.Trimarang,
            ProjectileID.LightDisc,
            ProjectileID.FlyingKnife,
            ProjectileID.FruitcakeChakram,
            //ÌØÊâ
            ProjectileID.RainbowFront,
            ProjectileID.RainbowBack,
            ProjectileID.LastPrism,
            ProjectileID.LastPrismLaser,
            ProjectileID.DirtBall,
            ProjectileID.RainCloudMoving,
            ProjectileID.RainCloudRaining,
            ProjectileID.RainFriendly,
            ProjectileID.BloodCloudMoving,
            ProjectileID.BloodCloudRaining,
            ProjectileID.BloodRain,
            ProjectileID.FlyingPiggyBank,
            ProjectileID.VoidLens,
            ProjectileID.ChargedBlasterCannon,
            ProjectileID.ChargedBlasterOrb,
            ProjectileID.ChargedBlasterLaser,
            ProjectileID.SpiritHeal,
            ProjectileID.StardustGuardian,
            ProjectileID.GolemFist,
            //Õæ½üÕ½Éäµ¯
            ProjectileID.MonkStaffT2Ghast,
            ProjectileID.MonkStaffT3_Alt,
            ProjectileID.MonkStaffT3_AltShot,
            ProjectileID.Terragrim,
            ProjectileID.Arkhalis,
            //yoyo
            ProjectileID.TerrarianBeam,
            ProjectileID.BlackCounterweight,
            ProjectileID.BlueCounterweight,
            ProjectileID.GreenCounterweight,
            ProjectileID.PurpleCounterweight,
            ProjectileID.RedCounterweight,
            ProjectileID.YellowCounterweight,
        };
        public int timeLast = 0;
        public int timeLeft = 0;
        public bool bind = false;
        public override void PreUpdate()
        {
            bind = false;
        }
        public override void PostUpdate()
        {
            if (!ModContent.GetInstance<ClientConfigs>().FilterClosed_TimeStop && TimeStopSystem.TimeStopping)
            {
                timeLast=TimeStopSystem.TimeLast;
                timeLeft=TimeStopSystem.StopTimeLeft;
                int radias = Math.Min(1200 * Math.Min(timeLast, timeLeft) / 30, 1200);
                mahouSyoujyo.SceneShader(tech: "GrayScaleTimeStop", degree: 0.5f,factor:0, r0: 32 , r1: radias, r2: 48, targetX: Main.LocalPlayer.Center.X, targetY: Main.LocalPlayer.Center.Y);
                mahouSyoujyo.SceneShader(tech: "ShockWaveTechnique", degree: radias / 400f, factor: 100-radias / 12f, r0: 5, r1: 5f, r2: 5, targetX: Main.LocalPlayer.Center.X, targetY: Main.LocalPlayer.Center.Y);
            }
            else
            {
                mahouSyoujyo.DelSceneShader("GrayScaleTimeStop");
                mahouSyoujyo.DelSceneShader("ShockWaveTechnique");
                timeLast = 0;
                
            }
            

            base.PostUpdate();
        }
        public override void OnRespawn()
        {
            spawnTime = 30;
        }
        public override void OnEnterWorld()
        {
            spawnTime = 30;
        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)mahouSyoujyo.MessageType.TimeStopSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((bool)bind);
            packet.Send(toWho, fromWho);
        }

        // Called in ExampleMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            bind = reader.ReadBoolean();
        }
        public override void CopyClientState(ModPlayer targetCopy)
        {
            TimeStop clone = (TimeStop)targetCopy;
            clone.bind = bind;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            TimeStop clone = (TimeStop)clientPlayer;

            if ( clone.bind != bind)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
        public override void Load()
        {
            
            base.Load();
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //if (usetip>0)
            //{
                usetip--;
                //Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, 
                //    Player.name +" Drawed", Player.Center.X-Main.screenPosition.X, Player.Center.Y-Main.screenPosition.X, Color.LightBlue, Color.LightBlue, new Vector2(0, 0));
            //}
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }
    }
}
