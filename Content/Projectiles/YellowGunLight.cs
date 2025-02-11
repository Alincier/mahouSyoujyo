using mahouSyoujyo.Content.Items.RangedWeapon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System.Diagnostics;
using mahouSyoujyo.Common.Configs;
using mahouSyoujyo.Content.Items.MeleeWeapon;
using mahouSyoujyo.Content.Items;
using Terraria.GameContent.ItemDropRules;
using mahouSyoujyo.Globals;

namespace mahouSyoujyo.Content.Projectiles
{
    public class YellowGunLight : ModProjectile
    {
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/YellowGunLight").Value;
        private Texture2D yellowguncannon = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/YellowGunCannon").Value;
        private Texture2D cannonlight = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/YellowCannonLight").Value;
        private SoundStyle magical = new SoundStyle($"mahouSyoujyo/Radio/Sound/magical");
        private SoundStyle electriccharged = new SoundStyle($"mahouSyoujyo/Radio/Sound/ElectricCharged") with { MaxInstances=1, SoundLimitBehavior=SoundLimitBehavior.ReplaceOldest};
        private SoundStyle simpleshoot = new SoundStyle($"mahouSyoujyo/Radio/Sound/SimpleElectricShoot") with { MaxInstances=1, SoundLimitBehavior=SoundLimitBehavior.ReplaceOldest };
        private SoundStyle heavyshoot = new SoundStyle($"mahouSyoujyo/Radio/Sound/HeavyElectricShoot") with { MaxInstances=1, SoundLimitBehavior=SoundLimitBehavior.ReplaceOldest,Volume = 0.5f};
        private float laserspeed = 48f;
        private float laserstart = 0f;
        private float laserend = 0f;
        private int soundcounter = 0;
        Player player => Main.player[Projectile.owner];
        public bool released
        {
            get { return Projectile.ai[2] == 1; }
            set 
            {   if (value) Projectile.ai[2] = 1;
                else Projectile.ai[2] = 0;
            }
        }
        public override void SetStaticDefaults()
        {
            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = false;
            Projectile.aiStyle = -1; // Replace with 20 if you do not want custom code
            //Projectile.hide = true; // Hides the projectile, so it will draw in the player's hand when we set the player's heldProj to this one.
            Projectile.timeLeft = 30;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            if (!released && Collision.CheckAABBvAABBCollision(
                projHitbox.TopLeft(),
                projHitbox.Size(),
                targetHitbox.TopLeft(),
                targetHitbox.Size()
                )) return true;
            if (!released && Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f,
                60f, ref point
                )) return true;
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero)*laserstart,
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero)*laserend,
                120f, ref point
                )) return true;
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(30f))*laserstart,
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(30f))*laserend,
                120f, ref point
                )) return true;
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(20f))*laserstart,
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(20f))*laserend,
                120f, ref point
                )) return true;
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(10f))*laserstart,
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(10f))*laserend,
                120f, ref point
                )) return true;
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-30f))*laserstart,
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-30f))*laserend,
                120f, ref point
                )) return true;
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-20f))*laserstart,
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-20f))*laserend,
                120f, ref point
                )) return true;
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-10f))*laserstart,
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f+
                Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-10f))*laserend,
                120f, ref point
                )) return true;
            return false;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {

            base.ModifyDamageHitbox(ref hitbox);
        }
        public override bool ShouldUpdatePosition()
        {
            return !released;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            bool magia = player.magic().magia;
            float distance = Vector2.Distance(target.Center,Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f);
            float mul = Math.Clamp((laserspeed*15f-distance)/720f, 0f, 1f);
            if (!magia) mul = mul*mul;
            if (Projectile.ai[1] == 1) mul = (magia) ? 2f : 1f;
            else modifiers.Knockback+=20;
            modifiers.SourceDamage*= mul;
            if (target.realLife == -1) return;
            int who = target.realLife;
            int count = 0;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (who != -1 && npc.realLife==who) count++;
            }
            if (count>=16) modifiers.FinalDamage*=0.25f;
            else if (count>=8) modifiers.FinalDamage*=0.3f;
            else if (count>=4) modifiers.FinalDamage*=0.4f;
            else if (count>=2) modifiers.FinalDamage*=0.5f;
        }
        private float FrameCounter
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // This property encloses the internal AI variable Projectile.ai[1].
        private float NextManaFrame
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        // This property encloses the internal AI variable Projectile.localAI[0].
        // localAI is not automatically synced over the network, but that does not cause any problems in this case.
        private float ManaConsumptionRate
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        private const float MaxManaConsumptionDelay = 60f;
        private const float MinManaConsumptionDelay = 30f;
        private bool ShouldConsumeMana()
        {
            // If the mana consumption timer hasn't been initialized yet, initialize it and consume mana on frame 1.
            if (ManaConsumptionRate == 0f)
            {
                ManaConsumptionRate = MaxManaConsumptionDelay;
                NextManaFrame =FrameCounter+ManaConsumptionRate;
                return false;
            }

            // Should mana be consumed this frame?
            bool consume = FrameCounter == NextManaFrame;

            // If mana is being consumed this frame, update the rate of mana consumption and write down the next frame mana will be consumed.
            if (consume)
            {
                // MathHelper.Clamp(X,A,B) guarantees that A <= X <= B. If X is outside the range, it will be set to A or B accordingly.
                ManaConsumptionRate = MathHelper.Clamp(ManaConsumptionRate - 30f, MinManaConsumptionDelay, MaxManaConsumptionDelay);
                NextManaFrame += ManaConsumptionRate;
            }
            return consume;
        }
        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0 ) SoundEngine.PlaySound(magical, Projectile.Center);
                if (Projectile.ai[0] == 30 && player.GetModPlayer<YellowGunCharge>().charged
                    ) 
                    SoundEngine.PlaySound(electriccharged, Projectile.Center);
                if (player.GetModPlayer<YellowGunCharge>().charged && !released)
                {
                    if (Projectile.ai[1] == 0) Projectile.ai[0] = 0;
                    bool manaIsAvailable =!ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);
                    if (!manaIsAvailable)    
                    {
                        Projectile.ai[1] = 0;
                        ManaConsumptionRate = 0f;
                    }
                    else Projectile.ai[1] = 1;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.ai[1] = 0;
                    ManaConsumptionRate = 0f;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[1]!=0 && Projectile.ai[0]>=30 && Projectile.ai[0]<60)
            for (int i=0;i<10;i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f-new Vector2(128, 128), 256, 256, DustID.YellowStarDust);
                dust.scale =1f;
                dust.velocity =(Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*240f-dust.position) / 30f;
            }
            Projectile.ai[0]++;
            

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
            if (Main.myPlayer == Projectile.owner)
            {
                // This code must only be ran on the client of the projectile owner

                // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                // player.channel indicates whether the player is still holding down the mouse button to use the item.
                bool stillInUse = player.channel  && !player.noItems && !player.CCed;
                if (stillInUse && !released)
                {
                    float holdoutDistance = 32f;
                    // Calculate a normalized vector from player to mouse and multiply by holdoutDistance to determine resulting holdoutOffset
                    Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(Main.MouseWorld - playerCenter);
                    if (Projectile.ai[1] ==1 && Projectile.ai[0]>=60)
                    {
                        float rot = holdoutOffset.ToRotation()-Projectile.velocity.ToRotation();
                        if (rot > MathHelper.Pi) rot -= MathHelper.TwoPi;
                        else if (rot <= -MathHelper.Pi) rot += MathHelper.TwoPi;
                        holdoutOffset = Projectile.velocity.RotatedBy(rot/60f);

                    }
                    if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                    {
                        // This will sync the projectile, most importantly, the velocity.
                        Projectile.netUpdate = true;
                    }

                    // Projectile.velocity acts as a holdoutOffset for held projectiles.
                    Projectile.velocity = holdoutOffset;
                }
                else
                {
                    released = true;
                }
            }
            if (!released)
            {
                Projectile.timeLeft = 30;
                if (Projectile.velocity.X > 0f)
                {
                    player.ChangeDir(1);
                }
                else if (Projectile.velocity.X < 0f)
                {
                    player.ChangeDir(-1);
                }


                player.ChangeDir(Projectile.direction); // Change the player's direction based on the projectile's own
                player.heldProj = Projectile.whoAmI; // We tell the player that the drill is the held projectile, so it will draw in their hand
                player.SetDummyItemTime(2); // Make sure the player's item time does not change while the projectile is out
                Projectile.Center = playerCenter; // Centers the projectile on the player. Projectile.velocity will be added to this in later Terraria code causing the projectile to be held away from the player at a set distance.
                Projectile.rotation = Projectile.velocity.ToRotation();
                player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            }
            laserstart = 0f;
            laserend = 0f;
            if (Projectile.ai[1]!=0 && Projectile.ai[0]>=60)
            {
                if (soundcounter <= 0)
                {
                    soundcounter = 30;
                    //var tracker = new ProjectileAudioTracker(Projectile);
                    if (!released) SoundEngine.PlaySound(heavyshoot, Projectile.Center//, soundInstance => { soundInstance.Position = Projectile.Center;return tracker.IsActiveAndInGame();}
                    );
                }
                soundcounter--;
                Projectile.friendly = true;
                if (!released)
                {
                    for (int i = 0; i<5; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*152f-new Vector2(120, 120), 240, 240, DustID.YellowStarDust);
                        dust.scale =1f;
                        //dust.velocity =(Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*272f-dust.position) / 30f;
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*272f-new Vector2(64, 64), 128, 128, DustID.YellowStarDust);
                        dust.scale =1f;
                        dust.velocity =Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-30f, 30f)))*laserspeed;
                    }
                }
                if (Main.myPlayer == Projectile.owner && !released) player.GetModPlayer<YellowGunCharge>().yellowguncharge=Math.Clamp( player.GetModPlayer<YellowGunCharge>().yellowguncharge-((player.magic().magia)?2:4) ,0,600);
                laserstart =(30-Projectile.timeLeft)*laserspeed;
                laserend = Math.Clamp ((Projectile.ai[0]-60)*laserspeed,0,30*laserspeed);
                if (laserstart>laserend) laserstart = laserend;
                return;
            }
            if (Math.Abs((int)Projectile.ai[0] % 60-32)<3 && Projectile.ai[1]==0 && !released)
            {
                //player.velocity-=Projectile.velocity.SafeNormalize(Vector2.Zero)*20f;
                Projectile.friendly = true;
                laserstart =0f;
                laserend = 30f*laserspeed;
                if ((int)Projectile.ai[0] % 60 == 30)SoundEngine.PlaySound(simpleshoot, Projectile.Center);
            }
            else Projectile.friendly = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            int width = tex.Width;
            int height = tex.Height;
            Vector2 center = Projectile.Center;
            Color c = Color.Yellow;
            if (Projectile.ai[0]<30) 
            {
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                List<Vertex> ve1 = new List<Vertex>();
                ve1.Add(new Vertex(center - Main.screenPosition + new Vector2(0, -60*Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3((30-Projectile.ai[0])/30f, 0, 1),
                          c*((float)Projectile.timeLeft/30f)));
                ve1.Add(new Vertex(center - Main.screenPosition + new Vector2(0, 60*Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3((30-Projectile.ai[0])/30f, 1, 1),
                          c*((float)Projectile.timeLeft/30f)));
                ve1.Add(new Vertex(center - Main.screenPosition + new Vector2(240*Projectile.ai[0]/30f, -60*Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3(1f, 0, 1),
                          c*((float)Projectile.timeLeft/30f)));
                ve1.Add(new Vertex(center - Main.screenPosition + new Vector2(240*Projectile.ai[0]/30f, 60*Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3(1f, 1, 1),
                          c*((float)Projectile.timeLeft/30f)));
                if (ve1.Count >= 3)
                {
                    gd.Textures[0] = yellowguncannon;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve1.ToArray(), 0, ve1.Count - 2);
                }
            }
            else
            {
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                List<Vertex> ve2 = new List<Vertex>();
                c=Color.Lerp(Color.Yellow, Color.White, Math.Clamp((Projectile.ai[0]-30f)/30f, 0f, 1f));
                ve2.Add(new Vertex(center - Main.screenPosition + new Vector2(0, -60*Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3(0f, 0, 1),
                          c*((float)Projectile.timeLeft/30f)));
                ve2.Add(new Vertex(center - Main.screenPosition + new Vector2(0, 60*Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3(0f, 1, 1),
                          c*((float)Projectile.timeLeft/30f)));
                ve2.Add(new Vertex(center - Main.screenPosition + new Vector2(240, -60*Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3(1f, 0, 1),
                          c*((float)Projectile.timeLeft/30f)));
                ve2.Add(new Vertex(center - Main.screenPosition + new Vector2(240, 60*Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3(1f, 1, 1),
                          c*((float)Projectile.timeLeft/30f)));
                if (ve2.Count >= 3)
                {
                    gd.Textures[0] = yellowguncannon;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve2.ToArray(), 0, ve2.Count - 2);
                }
            }
            //if (Projectile.ai[0]>=60)
            c = Color.Yellow;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve0 = new List<Vertex>();
            ve0.Add(new Vertex(center - Main.screenPosition + new Vector2(0, -120).RotatedBy(Projectile.velocity.ToRotation()),
                      new Vector3(0.5f, 0, 1),
                      c));
            ve0.Add(new Vertex(center - Main.screenPosition + new Vector2(0, 120).RotatedBy(Projectile.velocity.ToRotation()),
                      new Vector3(0.5f, 1, 1),
                      c));
            ve0.Add(new Vertex(center - Main.screenPosition + new Vector2(100, -120).RotatedBy(Projectile.velocity.ToRotation()),
                      new Vector3(1f, 0, 1),
                      Color.White));
            ve0.Add(new Vertex(center - Main.screenPosition + new Vector2(100, 120).RotatedBy(Projectile.velocity.ToRotation()),
                      new Vector3(1f, 1, 1),
                      Color.White));
            if (ve0.Count >= 3)
            {
                gd.Textures[0] = tex;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve0.ToArray(), 0, ve0.Count - 2);
            }
            c =Color.LightGoldenrodYellow;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve3 = new List<Vertex>();
            int level = 160;
            for (int i=0; i<level; i++)
            {
                float texpos = (float)i/level*0.5f+0.5f-(float)(FrameCounter%60)/60f*0.5f;
                float length = laserstart+(laserend-laserstart)*i/level;
                Color c1 = c;
                if (released) c1=c1*Math.Clamp((float)i/(float)(level/2), 0f, 1f);
                if (laserend<=laserspeed*29f) c1=c1*Math.Clamp((float)(level-i)/(float)(level/2), 0f, 1f);
                if (Projectile.ai[1]==0) c1*=0.2f;
                ve3.Add(new Vertex(center - Main.screenPosition + new Vector2(240+length, -laserwidth(length)).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3(texpos, 0, 1),
                          c1*Math.Clamp((laserspeed*30f-length)/(laserspeed*15f), 0.1f,0.8f)));
                ve3.Add(new Vertex(center - Main.screenPosition + new Vector2(240+length, laserwidth(length)).RotatedBy(Projectile.velocity.ToRotation()),
                          new Vector3(texpos, 1, 1),
                          c1*Math.Clamp((laserspeed*30f-length)/(laserspeed*15f), 0.1f, 0.8f)));

            }
            if (ve3.Count >= 3)
            {
                gd.Textures[0] = cannonlight;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve3.ToArray(), 0, ve3.Count - 2);
            }
            /*
            Rectangle rect = new Rectangle(0, 0, width, height);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            sb.Draw(
            tex, Projectile.Center-Main.screenPosition,
            rect, Color.Yellow, Projectile.velocity.ToRotation(),
            new Vector2(width / 2, height / 2),
            new Vector2(2f, 4f),
            SpriteEffects.None, 0);*/

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life<=0)
            {
                player.GetModPlayer<YellowGunCharge>().yellowguncharge = Math.Clamp(player.GetModPlayer<YellowGunCharge>().yellowguncharge+60, 0, 600);
                player.statMana= Math.Clamp(player.statMana+20,0, player.statManaMax2);
            }
                
            base.OnHitNPC(target, hit, damageDone);
        }
        private float laserwidth(float length)
        {
            if (length <= 120) return (float)Math.Sqrt(240f*240f -(240f-length)*(240f-length));
            else return (length-120)/(float)Math.Sqrt(3)+120*(float)Math.Sqrt(3);
        }
    }
}
