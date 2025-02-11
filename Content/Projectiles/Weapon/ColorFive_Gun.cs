using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using Terraria.Audio;
using mahouSyoujyo.Content.Projectiles;
using Terraria.Utilities;
using mahouSyoujyo;
using mahouSyoujyo.Content.Items.RangedWeapon;
using mahouSyoujyo.Content.Buffs;
using System.Collections.Generic;
using System.IO;
using mahouSyoujyo.Common.Configs;
using ReLogic.Utilities;
using Microsoft.Xna.Framework.Input;
using mahouSyoujyo.Globals;

namespace mahouSyoujyo.Content.Projectiles.Weapon
{
    public class ColorFive_Gun : ModProjectile
    {
        SlotId soundSlot;
        SoundStyle style = mahouSyoujyo.animationSound;
        private int damage = 100;
        private float speed = 20f;
        private float knockback = 6f;
        Player player => Main.player[Projectile.owner];
        //public override string Texture => "mahouSyoujyo/Content/Items/RangedWeapon/ColorFive.png";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            Main.projFrames[this.Type]=2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 2f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 10; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            Projectile.frame=0;
            Projectile.Center = Main.player[Projectile.owner].Center+new Vector2(0, -128);
            
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.timeLeft);
            writer.Write((float)Projectile.rotation);
            writer.Write((int)Projectile.frame);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft =  reader.ReadInt32();
            Projectile.rotation = reader.ReadSingle();
            Projectile.frame =  reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
        int oldtimeleft = 0;
        float oldrotation = 0;
        int oldframe = -1;
        public override void AI()
        {
            if (!ModContent.GetInstance<ClientConfigs>().NoSongs_ColorFive && player.altFunctionUse != 2)
            {
                if (!SoundEngine.TryGetActiveSound(soundSlot, out var _))
                {
                    float[] weight = { 0, 0, 0, 0, 0 };
                    Vector2 offset = player.GetModPlayer<PlayerState>().clientmouse-player.Center;
                    if (offset.Length()<480)
                    {
                        if (offset.X<0)
                        {
                            if (offset.Y<0) weight[0]=1;
                            else weight[1]=1;
                        }
                        else
                        {
                            if (offset.Y<=0) weight[2]=1;
                            else weight[3]=1;
                        }
                    }
                    else 
                    {
                        weight[4]=1;
                    }

                    var tracker = new ProjectileAudioTracker(Projectile);
                    soundSlot = SoundEngine.PlaySound(style with { VariantsWeights = weight }, Projectile.Center, soundInstance => {
                        // The SoundUpdateCallback can be inlined if desired, such as in this example.
                        soundInstance.Position = Projectile.Center;
                        return tracker.IsActiveAndInGame();
                    });
                }
            }
            if (Main.myPlayer != Projectile.owner) return;
            //记录旧数据，有变化则同步（只保留外观）
            oldtimeleft = Projectile.timeLeft;
            oldrotation = Projectile.rotation;
            oldframe = Projectile.frame;
            Vector2 position = Main.player[Projectile.owner].Center+new Vector2(0, -128);
            Projectile.Center=position;
            Vector2 target = Main.MouseWorld-position;
            if (Math.Abs(target.ToRotation())<= MathHelper.Pi / 2) { Projectile.frame=0; Projectile.rotation = target.ToRotation(); }
            else { Projectile.frame=1; Projectile.rotation = target.ToRotation()+MathHelper.Pi; }
            //计时器+1
            if (Projectile.ai[0] == 0) { 
                damage = Projectile.damage;   
            } 
            Projectile.ai[0]++;
            int usetime = 60;
            if (Projectile.ai[0] > 120) usetime=30;
            if (Projectile.ai[0] > 400) usetime=20;
            if (Projectile.ai[0] > 600) usetime=10;
            if (Projectile.ai[0] > 720 && player.HasBuff(ModContent.BuffType<MagicGirlPover>())) usetime=6;
            Projectile.ai[1]++;
            if (player.altFunctionUse != 2 && Projectile.ai[1]>=usetime)
            {
                Projectile.ai[1]=0;
                
                //if (player.statMana<=0) return;
                //player.statMana-=1;
                //6度散射范围
                if (Main.myPlayer == Projectile.owner)
                {
                    float rotated_by = (float)(new Random().NextDouble())*MathHelper.ToRadians(6f);
                    rotated_by -=MathHelper.ToRadians(3f);
                    var proj = Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromAI(),
                    position,
                    target.RotatedBy(rotated_by).SafeNormalize(Vector2.Zero)*speed,
                    ModContent.ProjectileType<ColorFiveBullet>(),
                    damage,
                    knockback,
                    player.whoAmI);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        proj.netUpdate = true;
                }

                SoundEngine.PlaySound(SoundID.Item12);
            }
            if (player.altFunctionUse!= 2)
            {
                if (Projectile.timeLeft>=oldtimeleft || Projectile.rotation != oldrotation || Projectile.frame != oldframe)
                    Projectile.netUpdate = true;
                return;
            }
            Projectile.ai[2]--;
            if ( Projectile.ai[2]<=0)
            {
                Projectile.ai[2]= 60 ;
                //if (player.statMana<=0) return;
                //player.statMana-=1;
                player.itemAnimation = player.itemTime = 60;
                Projectile.timeLeft = 50; 
            
                float rotation = (float)(new Random().NextDouble())*MathHelper.ToRadians(6f);

                if (Main.myPlayer == Projectile.owner) 
                {
                    int bullet_count = (player.HasBuff(ModContent.BuffType<MagicGirlPover>())) ? 5 : 3;
                    rotation -=MathHelper.ToRadians(2f*bullet_count+1f);
                    for (int i = 0; i<bullet_count; i++)
                    {
                        rotation+=MathHelper.ToRadians(4f);
                        var proj = Projectile.NewProjectile(
                            Projectile.GetSource_FromAI(),
                            position,
                            target.RotatedBy(rotation).SafeNormalize(Vector2.Zero)*speed,
                            ModContent.ProjectileType<ColorFiveBullet>(),
                            damage,
                            knockback,
                            player.whoAmI);

                    }
                }
                SoundEngine.PlaySound(SoundID.Item62);


            }
            if (Projectile.ai[2] %10 == 0)
            {
                SoundEngine.PlaySound(
                    (new Random().Next(7)) switch
                    {
                        0 => SoundID.Item140.WithVolumeScale(1f),
                        1 => SoundID.Item141.WithVolumeScale(1f),
                        2 => SoundID.Item142.WithVolumeScale(1f),
                        3 => SoundID.Item146.WithVolumeScale(1f),
                        4 => SoundID.Item147.WithVolumeScale(1f),
                        5 => SoundID.Item148.WithVolumeScale(1f),
                        _ => SoundID.Item143.WithVolumeScale(1f),
                        //  5 => SoundID.Item144.WithVolumeScale(0.5f),
                        //  6 => SoundID.Item145.WithVolumeScale(0.5f),
                        //0 => SoundID.Item139.WithVolumeScale(0.5f),
                    }

                    );
            }
            if (Projectile.timeLeft>=oldtimeleft || Projectile.rotation != oldrotation || Projectile.frame != oldframe)
                Projectile.netUpdate = true;
        }
        private bool AdvancedSoundUpdateCallback(ProjectileAudioTracker tracker, ActiveSound soundInstance)
        {
            soundInstance.Position = Projectile.position;

            // Dynamic pitch example: Pitch rises each time the projectile bounces
            //soundInstance.Pitch = (Projectile.maxPenetrate - Projectile.penetrate) * 0.15f;

            // Muffle the sound if the projectile is wet
            if (Projectile.wet)
            {
                soundInstance.Pitch -= 0.4f;
                soundInstance.Volume = MathHelper.Clamp(soundInstance.Style.Volume - 0.4f, 0f, 1f);
            }

            return tracker.IsActiveAndInGame();
        }
        public override void Kill(int timeLeft)
        {
            //停止声音
            //ActiveSound acSound = null;
            //SoundStyle[] color_five_songs = { mahouSyoujyo.animationSound };//, mahouSyoujyo.magia, mahouSyoujyo.ed1, mahouSyoujyo.ed2 };
            //foreach (SoundStyle style in color_five_songs)
            //{
            //    acSound=SoundEngine.FindActiveSound(style);
            //    if (acSound != null ) acSound.Stop();
            //}
        }
    }  
}