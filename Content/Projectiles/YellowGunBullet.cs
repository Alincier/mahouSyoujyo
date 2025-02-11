using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using System.Collections.Generic;
using System.Drawing.Imaging;
using mahouSyoujyo.Globals;
using mahouSyoujyo.Content.Items.RangedWeapon;


namespace mahouSyoujyo.Content.Projectiles
{
    public class YellowGunBullet : ModProjectile
    {
        //材质
        private Texture2D tex_gun = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/YellowGun").Value;
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/YellowGunBullet").Value;
        bool initial = true;
        bool copy = false;
        Vector2 pos0 = Vector2.Zero; 
        Vector2 vel0 = Vector2.Zero;
        Vector2 offset0 = Vector2.Zero;
        float rot0 = 0f;
        //拖尾绘制
        int frame_tail;
        Vector2[] pos_old;
        Vector2[] vel_old;
        Player player => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; 
            //Main.projFrames[this.Type]=5;
        }
        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 90; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            //每个射弹独立无敌帧
            Projectile.usesLocalNPCImmunity=true;
            //无敌帧
            Projectile.localNPCHitCooldown = -1;


            //轨迹记录
            frame_tail = 15;
            pos_old = new Vector2[frame_tail];
            vel_old = new Vector2[frame_tail];

        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0]>=30;
        }

        float angle_gun = 0f;
        int move_gun = 0;
        float alpha_gun = 1f;
        public override void AI()
        {
            if (Projectile.ai[0] == 0 && player == Main.LocalPlayer) 
            {
                offset0 = Projectile.Center-player.Center;
                vel0 = Projectile.velocity;
                rot0 = (Main.MouseWorld-player.Center).ToRotation();
            }
            if (Projectile.ai[0]<=30 && player == Main.LocalPlayer)
            {
                if (player.active)
                {
                    float rot1= (Main.MouseWorld-player.Center).ToRotation();
                    
                    Projectile.Center =player.Center+offset0.RotatedBy(rot1-rot0);
                    Projectile.velocity =vel0.RotatedBy(rot1-rot0);
                }
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0]<=30)
            {
                pos0 =Projectile.Center;
            }
            if (Projectile.ai[0]<=10f) alpha_gun= Math.Clamp((Projectile.ai[0]-0f)/10f, 0f, 1f);
            if (Projectile.ai[0]>=40f) alpha_gun= Math.Clamp((60f-Projectile.ai[0])/20f, 0f, 1f);
            if (Projectile.ai[0]>=30f)
            {
                angle_gun +=(60f-Projectile.ai[0])/12;
                move_gun -=(int)(60f-Projectile.ai[0]) /12;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0]==30)
            {
                SoundEngine.PlaySound(SoundID.Item40 with { Volume = 0.8f, Pitch =-0.9f, PitchVariance = 0.2f }, Projectile.Center);
                for (int i = 0; i<20; i++)
                {
                    Dust dust=Dust.NewDustDirect(Projectile.Center, 8, 8, DustID.Smoke);
                    dust.velocity +=Projectile.velocity.SafeNormalize(Vector2.Zero)*3f;
                }
                Projectile.friendly=true;
            }
            if (Projectile.ai[0]>=30 && Projectile.ai[0] % 5 == 0) Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.YellowStarDust);
            mahouSyoujyo.push(Projectile.Center, Projectile.velocity, frame_tail, ref pos_old, ref vel_old);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (Projectile.ai[0]<=60)
            {
                int width_gun = tex_gun.Width;
                int height_gun = tex_gun.Height;
                Rectangle rect_gun = new Rectangle(0, 0, width_gun, height_gun);
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                
                
                float rot_gun = MathHelper.ToRadians(angle_gun)*((Projectile.velocity.X>0) ? -1 : 1);
                    sb.Draw(
                    tex_gun, pos0+new Vector2(-32+move_gun, 0).RotatedBy(Projectile.velocity.ToRotation())-Main.screenPosition,
                    rect_gun, Color.LightGoldenrodYellow*alpha_gun, Projectile.velocity.ToRotation()+rot_gun,
                    new Vector2(width_gun / 2, height_gun / 2),
                    new Vector2(1f, 1f),
                    (Projectile.velocity.X>0)?SpriteEffects.None: SpriteEffects.FlipVertically, 0);
            }
            if (Projectile.ai[0] <30) return false;
            int width = tex.Width;
            int height = tex.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();

            for (int i = 0; i < frame_tail; i++)
            {
                Color b = Color.Lerp(Color.White, Color.Black, i / (float)frame_tail);
                if (pos_old[i]!= Vector2.Zero) ve.Add(new Vertex(pos_old[i] - Main.screenPosition + new Vector2(width/2, -0.8f*height*(1-i / (float)frame_tail)/2).RotatedBy(vel_old[i].ToRotation()),
                          new Vector3(i / (float)frame_tail, 1, 1),
                          b));
                if (pos_old[i]!= Vector2.Zero) ve.Add(new Vertex(pos_old[i] - Main.screenPosition + new Vector2(width/2, 0.8f*height*(1-i / (float)frame_tail)/2).RotatedBy(vel_old[i].ToRotation()),
                          new Vector3(i / (float)frame_tail, 0, 1),
                          b));
            }
            if (ve.Count >= 3)
            {
                gd.Textures[0] = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/YellowGunBullet").Value;//获取刀光的拖尾贴图
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            /*for (int i = frame_tail-1; i>0; i--) 
            {
                sb.Draw(
                    tex, pos_old[i]-Main.screenPosition,
                    rect, Color.LightGoldenrodYellow*(0.3f-0.02f*i), (pos_old[i-1]-pos_old[i]).ToRotation(),
                    new Vector2(width / 2, height / 2),
                    new Vector2(1f, 1f),
                    SpriteEffects.None, 0);
            }*/
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            sb.Draw(
                    tex, Projectile.Center-Main.screenPosition,
                    rect, Color.LightGoldenrodYellow, Projectile.velocity.ToRotation(),
                    new Vector2(width / 2, height / 2),
                    new Vector2(1f, 1f),
                    SpriteEffects.None, 0);
            return false;
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
            if (count>=16) modifiers.SourceDamage*=0.25f;
            else if (count>=8) modifiers.SourceDamage*=0.3f;
            else if (count>=4) modifiers.SourceDamage*=0.4f;
            else if (count>=2) modifiers.SourceDamage*=0.5f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].GetModPlayer<YellowGunCharge>().yellowguncharge =Math.Clamp(Main.player[Projectile.owner].GetModPlayer<YellowGunCharge>().yellowguncharge+((target.boss)?40:20),0,600);
            if (target.life<=0)
            {
                player.GetModPlayer<YellowGunCharge>().yellowguncharge = Math.Clamp(player.GetModPlayer<YellowGunCharge>().yellowguncharge+60, 0, 600);
                player.statMana= Math.Clamp(player.statMana+20, 0, player.statManaMax2);
            }
            for (int i = 0; i<5; i++)
                Dust.NewDustDirect(target.Center, target.width, target.height, DustID.YellowStarDust);
            Projectile.damage =(int)Projectile.damage*1;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i<5; i++)
                Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.YellowStarDust);
            //SoundEngine.PlaySound(SoundID.Item94.WithVolumeScale(0.3f), Projectile.Center);
            /*Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
            Projectile.velocity,
                     ModContent.ProjectileType<explosion2>(),
                    Projectile.damage *  2, 2, Projectile.owner);*/
        }
    }
}