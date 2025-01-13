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
using System.Linq;
using mahouSyoujyo.Content.Items.MeleeWeapon;
using mahouSyoujyo.Globals;

namespace mahouSyoujyo.Content.Projectiles
{
    public class combo2 : ModProjectile
    {
        private int damage = 35;
        private int scale = 1;
        private float damage_bonus = 1;
        //剑的位置参数
        private Vector2 root;
        private float length = 1f;
        private int period1;
        private int period2;
        private Vector2[] root0;
        private float[] rotation0;
        private Vector2[] root1;
        private float[] rotation1;
        private Vector2[] root2;
        private float[] rotation2;
        private int direction = 0;//向右0，向左1
        private float length0 = 1f;
        private float length1 = 1f;
        private float length2 = 1f;
        //拖尾绘制
        int frame_tail;
        Vector2[] pos_old;
        Vector2[] vel_old;
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/Blue_sword").Value;
        Player player => Main.player[Projectile.owner];
        //public override string Texture => "mahouSyoujyo/Content/Items/RangedWeapon/ColorFive.png";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            //弹幕中心光照
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 160; // The width of projectile hitbox
            Projectile.height = 80; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            Projectile.frame=0;
            Projectile.usesLocalNPCImmunity = true;//不更新位置
            Projectile.localNPCHitCooldown=60;
            Projectile.Center = player.Center;
            //设置绘制图层
            Projectile.hide = true;

            //轨迹记录
            frame_tail = 120;
            pos_old = new Vector2[frame_tail];
            vel_old = new Vector2[frame_tail];

            //离中心点的手柄位置,剑身长度为64像素
            root0=new Vector2[2] { new Vector2(-48f, 0f), new Vector2(48f, 0f) };//斜后下方
            rotation0 =new float[2] { MathHelper.ToRadians(120f), MathHelper.ToRadians(-300f) };
            length0 = 0.8f;
            root1 =new Vector2[2] { new Vector2(0f, 0f), new Vector2(0f, 0f) };//头顶
            rotation1 =new float[2] { MathHelper.ToRadians(90f), MathHelper.ToRadians(-270f) };
            length1 = 0.6f;
            root2 = new Vector2[2] { new Vector2(24f, -32f), new Vector2(-24f, -32f) };//斩落位置
            rotation2 =new float[2] { MathHelper.ToRadians(-0f), MathHelper.ToRadians(-180f) };
            length2 = 0.8f;
            //阶段帧数
            period1 = 30;
            period2 = 45;
            Projectile.extraUpdates=5;//一秒180帧
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft <=120)
            {
                Projectile.friendly = false;
                for (int i = frame_tail -1; i>0; i--)
                {
                    pos_old[i]=pos_old[i-1];
                    vel_old[i]=vel_old[i-1];
                }
                pos_old[0] = root*scale+Projectile.velocity*scale;
                vel_old[0] = Projectile.velocity*length;
                //mahouSyoujyo.push(root*scale+Projectile.velocity*scale, Projectile.velocity*length, frame_tail, ref pos_old, ref vel_old);
                return;
            }
            if (player.HasBuff(ModContent.BuffType<MagicGirlPover>()))
            {
                Projectile.localNPCHitCooldown=30;
                scale = 2;
            }
            else
            {
                Projectile.localNPCHitCooldown=60;
                scale = 1;
            }
            Projectile.width = 160*scale; // The width of projectile hitbox
            Projectile.height = 80*scale; // The height of projectile hitbox
            // Main.NewText(player.channel);
            // Main.NewText(Projectile.ai[0]);
            if (Projectile.ai[0] == 0)
            {
                if (player.direction>=0) { direction =0; }// Projectile.rotation = Projectile.velocity.ToRotation(); }
                else { direction= 1; }// Projectile.rotation = Projectile.velocity.ToRotation()+MathHelper.Pi; }
                damage = Projectile.damage;
            }
            Vector2 position = Main.player[Projectile.owner].Center+new Vector2(-32f*scale*(direction*2-1), -8f*scale);
            Projectile.Center=position;
            // Projectile.velocity = Main.MouseWorld-player.Center;
            //计时器+1
            Projectile.frame =Math.Min(Main.projFrames[this.Type]-1, (int)Projectile.ai[0] / 1);
            if (Projectile.ai[0] < period1)
            {
                root = root0[direction]+Projectile.ai[0]*(root1[direction]-root0[direction])/period1;
                Projectile.velocity = new Vector2(24f, 0f).RotatedBy(rotation0[direction]+Projectile.ai[0]*(rotation1[direction]-rotation0[direction])/period1);
                length = length0+Projectile.ai[0]*(length1-length0)/period1;
                Projectile.damage = (int)(damage*2*damage_bonus);

            }
            else if (Projectile.ai[0] < period2)
            {
                root = root1[direction]+(Projectile.ai[0]-period1)*(root2[direction]-root1[direction])/(period2-period1);
                Projectile.velocity = new Vector2(24f, 0f).RotatedBy(rotation1[direction]+(Projectile.ai[0]-period1)*(rotation2[direction]-rotation1[direction])/(period2-period1));
                length = length1+(Projectile.ai[0]-period1)*(length2-length1)/(period2-period1);
                Projectile.damage = (int)(damage*3*damage_bonus);
            }
            else
            {
                root = root2[direction];
                Projectile.velocity = new Vector2(24f, 0f).RotatedBy(rotation2[direction]);
                length=length2;
                Projectile.damage = (int)(damage*3*damage_bonus);
            }

            //Main.NewText(length);
            Projectile.ai[0]++;
            for (int i = frame_tail -1; i>0; i--)
            {
                pos_old[i]=pos_old[i-1];
                vel_old[i]=vel_old[i-1];
            }
            pos_old[0] = root*scale+Projectile.velocity*scale;
            vel_old[0] = Projectile.velocity*length;
            //mahouSyoujyo.push(root*scale+Projectile.velocity*scale, Projectile.velocity*length, frame_tail, ref pos_old, ref vel_old);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();

            for (int i = 1; i < frame_tail; i++)
            {
                Color b = Color.Lerp(Color.Blue, Color.Red, i / (float)frame_tail);
                if (pos_old[i]!= Vector2.Zero) ve.Add(new Vertex(Projectile.Center+pos_old[i] - Main.screenPosition + new Vector2(40f* (vel_old[i].Length()/24f) * scale, 0).RotatedBy(vel_old[i].ToRotation()),
                          new Vector3(i / (float)frame_tail, 1, 1),
                          b));
                if (pos_old[i]!= Vector2.Zero) ve.Add(new Vertex(Projectile.Center+pos_old[i] - Main.screenPosition + new Vector2(-24f* (vel_old[i].Length()/24f) * scale, 0).RotatedBy(vel_old[i].ToRotation()),
                          new Vector3(i / (float)frame_tail, 0, 1),
                          b));
            }
            if (ve.Count >= 3)
            {
                gd.Textures[0] = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/BlueSwordLight").Value;//获取刀光的拖尾贴图
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            /*for (int i = frame_tail-1; i>0; i--)
            { 
                mahouSyoujyo.draw_Center(
                    tex: tex,
                    frame_num: 1, frame: 0,
                    pos: pos_old[i] //+ new Vector2(0 , 16f*scale), 
                    , color: Color.AliceBlue*(0.3f-0.03f*i),
                    rot: vel_old[i].ToRotation(),
                    scale_X: 1f* vel_old[i].Length()/24f * scale, scale_Y: 1f * scale, (direction==1) ? SpriteEffects.None : SpriteEffects.FlipVertically, layerDepth: 0);
                Dust.NewDustDirect(Projectile.Center + (root+2*Projectile.velocity)*scale,
                32*scale, 32*scale, 206, newColor: Color.SkyBlue);
            }*/

                mahouSyoujyo.draw_Center(
                tex: tex,
                frame_num: 1, frame: 0,
                pos: Projectile.Center + (root+Projectile.velocity)*scale /*+ new Vector2(0 , 16f*scale)*/, 
                color: Color.AliceBlue*Math.Clamp((float)(Projectile.timeLeft-60) / 60f, 0f, 1f),
                rot: Projectile.velocity.ToRotation(),
                scale_X: 1f * length *scale, scale_Y: 1f * scale, (direction==1) ? SpriteEffects.None : SpriteEffects.FlipVertically, layerDepth: 0);
            if (Projectile.timeLeft>120)
            {                
                Dust.NewDustDirect(Projectile.Center + root+(2*Projectile.velocity)*scale,
                16*scale, 16*scale, 206, newColor: Color.SkyBlue);
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.boss) target.velocity.Y -= 10f*target.knockBackResist;
            if (damage_bonus>=3f) damage_bonus = 0;                                                                                               //{ Volume = 3f, });
            if (damage_bonus != 0) damage_bonus+=0.1f;
            //狂暴状态击中敌人回血
            if (player.GetModPlayer<Comboing>().supertime >0) player.Heal(1);
            Dust.NewDustDirect(target.Center, target.width, target.height, DustID.BlueFairy, newColor: Color.LightBlue);
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void Kill(int timeLeft)
        {
            //停止声音
            //Main.NewText(damage_bonus);

        }
    }
}