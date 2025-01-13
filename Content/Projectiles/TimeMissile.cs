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
using ReLogic.Content;
using mahouSyoujyo.Content.Buffs;
using Terraria.Audio;
using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content.Items.SpecialWeapon;
using System.IO;
using System.Security.Policy;
using Microsoft.Win32;
using System.Reflection;
using Mono.Cecil;

namespace mahouSyoujyo.Content.Projectiles
{
    public class TimeMissile : ModProjectile
    {
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/TimeMissile").Value;
        public int runtime = 0;
        public int stage = 0;
        SoundStyle explosion1 = new SoundStyle($"mahouSyoujyo/Radio/Sound/explosion1") with { PitchVariance = 0.4f };
        public override void SetStaticDefaults()
        {
            Main.projFrames[this.Type] = 7;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 24; // The width of projectile hitbox
            Projectile.height =24; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic;
            // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = 1; //How many enemies could be hit and gone through.

            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.timeLeft = 360; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.stopsDealingDamageAfterPenetrateHits=false;
            //缩放
            //Projectile.scale =2f;
            //使用公用无敌帧
            //Projectile.usesIDStaticNPCImmunity=true;
            //每个射弹独立无敌帧
            Projectile.usesLocalNPCImmunity=true;
            //无敌帧
            Projectile.localNPCHitCooldown = 20;
            
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)runtime);
            writer.Write((int)stage);
            writer.Write((int)Projectile.timeLeft);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            runtime = reader.ReadInt32();
            stage = reader.ReadInt32();
            Projectile.timeLeft = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }

        int oldstage=0;
        public override void AI()
        {
               
            if (stage != oldstage && Projectile.owner == Main.myPlayer) Projectile.netUpdate = true;
            oldstage = stage;

            //图像顺时针旋转角度
            float directed = MathHelper.ToRadians(0f);
            if (stage == 1 ||stage == 2 || stage == 3)
            {
                Projectile.velocity.Y = Math.Min(25f, Projectile.velocity.Y+0.3f);
                Projectile.rotation =directed +  Projectile.velocity.ToRotation();
                return;
            }
            if (stage >=4)
            {
                float maxDetectRadius = (stage == 4) ? 500f : 6000f; // The maximum radius at which a projectile can detect a target
                float projSpeed = Math.Clamp(runtime /2f - 10f, 20f,50f); // The speed at which the projectile moves towards the target
                                       // Trying to find NPC closest to the projectile
                NPC closestNPC = null;
                float maxDetectDistance = maxDetectRadius;
                float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
                foreach (var target in Main.ActiveNPCs)
                {

                    if (target.CanBeChasedBy())
                    {
                        // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                        float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                        // Check if it is within the radius
                        if (sqrDistanceToTarget < sqrMaxDetectDistance)
                        {
                            sqrMaxDetectDistance = sqrDistanceToTarget;
                            closestNPC = target;
                        }
                    }
                }
                float focus_strenth = (stage == 4) ? 3f : 20f;
                if (closestNPC != null)
                {
                    // If found, change the velocity of the projectile and turn it in the direction of the target
                    // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                    Projectile.velocity =
                    (Projectile.velocity +((closestNPC.Center  - Projectile.Center).SafeNormalize(Vector2.Zero))*focus_strenth)
                        .SafeNormalize(Vector2.Zero) * projSpeed;
                    Projectile.rotation =directed +  Projectile.velocity.ToRotation();
                }
                else
                {
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero)* projSpeed;// (Projectile.velocity+focus_strenth*(closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero)).SafeNormalize(Vector2.Zero) * projSpeed;
                    Projectile.rotation =directed +  Projectile.velocity.ToRotation();
                }
            }
            
            if (stage >= 4) Projectile.tileCollide=false;
            //记录轨迹
            //if (Main.time % 2 ==0)
            //画尾气
            Vector2 dust_offset = -64f*Projectile.scale*Projectile.velocity.SafeNormalize(Vector2.Zero)-new Vector2(Projectile.width/2, Projectile.height / 2);
            if (stage >= 5)
            {
                var d = Dust.NewDustDirect(Projectile.Center+dust_offset, Projectile.width, Projectile.height, DustID.SandstormInABottle);
                d.scale = 2f;
                d.noGravity = true;
            }
            if (stage >= 4 || stage == 0)
            {
                var d = Dust.NewDustDirect(Projectile.Center+dust_offset, Projectile.width, Projectile.height, DustID.SandstormInABottle);
                d.scale = 1f;
                d.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
        
            if (stage == 0) Projectile.frame = 0;
            else if (stage == 1) Projectile.frame = 1;
            else if (stage == 2) Projectile.frame = 2;
            else if (stage == 3) Projectile.frame = 3;
            else if (stage == 4) Projectile.frame = 4;
            else Projectile.frame = ((int)Main.time / 20) % 2 +5;
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            int width = tex.Width;
            int height = tex.Height / Main.projFrames[this.Type];
            Rectangle rect = new Rectangle(0, Projectile.frame*height, width, height);
            
            sb.Draw(
                tex, Projectile.Center-Main.screenPosition,
                rect, (TimeStopSystem.TimeStopping || Main.gamePaused)? Main.mcColor:Color.White, Projectile.velocity.ToRotation(),
                new Vector2(width * 4 / 5, height / 2),
                new Vector2(2, 2),
                SpriteEffects.None, 0);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            var source = Projectile.GetSource_FromThis();
            if (stage <=3)
            {
                if (Main.myPlayer == Projectile.owner )
                {
                    int index = Projectile.NewProjectile(source, Projectile.Center,
                    Projectile.velocity,
                    ModContent.ProjectileType<explosion1>(),
                    Projectile.damage, 5, Projectile.owner);
                    var p = Main.projectile[index];
                    p.rotation = Projectile.rotation-MathHelper.ToRadians(90f);
                    p.Center = Projectile.Center;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p.whoAmI);
                }
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
            else if (stage ==4)
            {
                if (Main.myPlayer == Projectile.owner  )
                {
                    int index = Projectile.NewProjectile(source, Projectile.Center,
                    Projectile.velocity,
                    ModContent.ProjectileType<explosion1>(),
                    Projectile.damage * 2, 10, Projectile.owner);
                    var p = Main.projectile[index];
                    p.rotation = Projectile.rotation-MathHelper.ToRadians(90f);
                    p.Size *= 1.5f;
                    p.scale *= 1.5f;
                    p.Center = Projectile.Center;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p.whoAmI);
                }
                SoundEngine.PlaySound(explosion1, Projectile.Center);
            }
            else
            {
                if (Main.myPlayer == Projectile.owner )
                {
                    bool isVariant = Main.rand.NextBool(Math.Clamp((360-runtime) / 60,1,4));
                    int index = Projectile.NewProjectile(source, Projectile.Center,
                    Projectile.velocity,
                    isVariant ? ModContent.ProjectileType<explosion2>() : ModContent.ProjectileType<explosion1>(),
                    Projectile.damage * (isVariant ? 2 : 4), (isVariant ? 2 : 10), Projectile.owner);
                    var p = Main.projectile[index];
                    p.rotation = Projectile.rotation-MathHelper.ToRadians(90f);
                    p.Size *= 2;
                    p.scale *= 2;
                    p.Center = Projectile.Center;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p.whoAmI);
                }
                SoundEngine.PlaySound(explosion1, Projectile.Center);
            }
            Projectile.netUpdate = true;
            int dustcount = 50;
            int boomscale = 16;
            if (stage <= 3)
            {
                dustcount = 20;
                boomscale =8;
            }
            else if (stage == 4) 
            {
                dustcount = 40;
                boomscale =16;
            }
             
             for (int i = 0; i < dustcount; i++)
             {
                 Dust dust = Dust.NewDustDirect(Projectile.Center-new Vector2(Projectile.width *boomscale / 2, Projectile.width *boomscale / 2), Projectile.width *boomscale, Projectile.width *boomscale, DustID.Smoke, 0f, 0f, 100, default, 2f);
                 dust.velocity *= 1.4f;
             }
            
            // Fire Dust spawn
            for (int i = 0; i < dustcount+30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center-new Vector2(Projectile.width *boomscale / 2, Projectile.width *boomscale / 2), Projectile.width *boomscale, Projectile.width *boomscale, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }


        }
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
    }
}
