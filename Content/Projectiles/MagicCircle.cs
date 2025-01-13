using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using System.Threading;
using Terraria.Audio;
using Mono.Cecil;
using mahouSyoujyo.Content.Projectiles;

namespace mahouSyoujyo.Content.Projectiles
{
    public class MagicCircle : ModProjectile
    {
        int timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 1712; // The width of projectile hitbox
            Projectile.height = 1776; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 1000f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255;//透明度，越大越透明，0-255.
        }
        public override void AI()
        {
            //计时器+1
            timer++;
            //圆环伤害默认关闭。
            Projectile.friendly = false;
            //速度归0。
            if (timer <= 1)
            {
                Projectile.velocity=Projectile.velocity*0;
            }
            //声效
            if (timer == 30)
            {
                SoundEngine.PlaySound(SoundID.Item113, Projectile.Center);
            }
            if (timer == 420)
            {
                SoundEngine.PlaySound(SoundID.Item123, Projectile.Center);
            }
            //每五帧生成一个射弹
            if (timer % 5 == 0)
            {
                Vector2 vel = new Vector2(20f, 0);
                // vel=vel.RotatedBy(MathHelper.TwoPi*Main.rand.Next());
                if (Main.myPlayer == Projectile.owner) 
                { 
                    var p =Projectile.NewProjectileDirect(
                        Projectile.InheritSource(Projectile), 
                        Projectile.Center, 
                        vel.RotatedBy(MathHelper.TwoPi*timer/60), ModContent.ProjectileType<HeartArrow>(),
                        Projectile.damage,
                        Projectile.knockBack, Projectile.owner);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        p.netUpdate = true;
                    }
             /*   Projectile.NewProjectile(
                    Projectile.InheritSource(Projectile),
                    Projectile.Center,
                    -vel.RotatedBy(MathHelper.TwoPi*timer/60), ModContent.ProjectileType<HeartArrow>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
             */
                }

            }
            //圆环每一秒钟造成一帧伤害。
            if (timer % 60 == 0)
            {
                Projectile.friendly = true;
            }


                //一秒转60度
                Projectile.rotation = MathHelper.ToRadians(1f*timer);
            if (timer <65)
            {
                Projectile.alpha=Math.Max(255-4*timer,0);
            }else
            {
                if (timer <= 535)
                {
                    Projectile.alpha = 0;
                } else
                {
                    Projectile.alpha = Math.Min(255,4*(timer-535));
                }
            }
            
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.realLife == -1) return;
            int who = target.realLife;
            int count = 0;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.realLife==who) count++;
            }
            if (count>=16) modifiers.SourceDamage*=0.1f;
            else if (count>=8) modifiers.SourceDamage*=0.2f;
            else if (count>=4) modifiers.SourceDamage*=0.3f;
            else if (count>=2) modifiers.SourceDamage*=0.5f;
        }
        //   public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        //   {
        //       Projectile.damage =(int)Projectile.damage*1;
        //   }
    }
}