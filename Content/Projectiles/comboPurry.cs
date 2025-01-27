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
using static mahouSyoujyo.Content.Items.MeleeWeapon.BlueSword;
using mahouSyoujyo.Content.Items.MeleeWeapon;
using mahouSyoujyo.Content.Projectiles.Weapon;
using mahouSyoujyo.Globals;

namespace mahouSyoujyo.Content.Projectiles
{
    public class comboPurry : ModProjectile
    {
        private int damage = 35;
        private bool returnCD = false;
        SoundStyle hited = new SoundStyle($"mahouSyoujyo/Radio/Sound/immune")
        {
            SoundLimitBehavior=SoundLimitBehavior.IgnoreNew,
        };
        private int scale = 1;
        private float damage_bonus = 1;
        //剑的位置参数
        private Vector2 root;
        private float length = 1f;
        private int period1;
        private int period2;
        private int period3;
        private Vector2[] root0;//斜后方
        private float[] rotation0;
        private Vector2[] root1;//头顶
        private float[] rotation1;
        private Vector2[] root2;//斩落位置
        private float[] rotation2;
        private Vector2[] root3;//斜后方
        private float[] rotation3;
        private int direction = 0;//向右0，向左1
        private float length0 = 1f;
        private float length1 = 1f;
        private float length2 = 1f;
        private float length3 = 1f;
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
            Projectile.width = 240; // The width of projectile hitbox
            Projectile.height = 80; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 200; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            Projectile.frame=0;
            Projectile.usesLocalNPCImmunity = true;//不更新位置
            Projectile.localNPCHitCooldown=50;
            Projectile.Center = player.Center;
            //设置绘制图层
            Projectile.hide = true;

            //设置cd


            //轨迹记录
            frame_tail = 120;
            pos_old = new Vector2[frame_tail];
            vel_old = new Vector2[frame_tail];

            //离中心点的手柄位置,剑身长度为64像素
            root0=new Vector2[2] { new Vector2(24f, -8f), new Vector2(-24f, -8f) };//斜前方
            rotation0 =new float[2] { MathHelper.ToRadians(-60f), MathHelper.ToRadians(-120f) };
            length0 = 0.3f;
            root1 =new Vector2[2] { new Vector2(64f, 16f), new Vector2(-64f, 16f) };//前方
            rotation1 =new float[2] { MathHelper.ToRadians(0f), MathHelper.ToRadians(-180f) };
            length1 = 0.6f;
            root2 = new Vector2[2] { new Vector2(0f, 0f), new Vector2(0f, 0f) };//竖直位置
            rotation2 =new float[2] { MathHelper.ToRadians(90f), MathHelper.ToRadians(-270f) };
            length2 = 0.5f;
            root3 = new Vector2[2] { new Vector2(-56f, -16f), new Vector2(56f, -16f) };//结束位置
            rotation3 =new float[2] { MathHelper.ToRadians(180f), MathHelper.ToRadians(-360f) };
            length3 = 0.8f;
            //阶段帧数
            period1 = 50;
            period2 = 65;
            period3 = 75;
            Projectile.extraUpdates=5;//一秒360帧
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (player.HasBuff(ModContent.BuffType<MagicGirlPover>()))
            {
                Projectile.localNPCHitCooldown=40;
                scale = 2;
            }
            else
            {
                Projectile.localNPCHitCooldown=50;
                scale = 1;
            }
            Projectile.width = 240*scale; // The width of projectile hitbox
            Projectile.height = 80*scale; // The height of projectile hitbox
            // Main.NewText(player.channel);
            // Main.NewText(Projectile.ai[0]);
            if (Projectile.ai[0] == 0)
            {
                if (player.direction>=0) { direction =0; }// Projectile.rotation = Projectile.velocity.ToRotation(); }
                else { direction= 1; }// Projectile.rotation = Projectile.velocity.ToRotation()+MathHelper.Pi; }
                damage = Projectile.damage;
            }
            Vector2 position = Main.player[Projectile.owner].Center;
            Projectile.Center=position;
            // Projectile.velocity = Main.MouseWorld-player.Center;
            //计时器+1
            Projectile.frame =Math.Min(Main.projFrames[this.Type]-1, (int)Projectile.ai[0] / 1);
            if (Projectile.ai[0] < period1)
            {
                root = root0[direction]+Projectile.ai[0]*(root1[direction]-root0[direction])/period1;
                Projectile.velocity = new Vector2(24f, 0f).RotatedBy(rotation0[direction]+Projectile.ai[0]*(rotation1[direction]-rotation0[direction])/period1);
                length = length0+Projectile.ai[0]*(length1-length0)/period1;
                Projectile.damage = (int)(damage*3*damage_bonus);

            }
            else if (Projectile.ai[0] < period2)
            {
                root = root1[direction]+(Projectile.ai[0]-period1)*(root2[direction]-root1[direction])/(period2-period1);
                Projectile.velocity = new Vector2(24f, 0f).RotatedBy(rotation1[direction]+(Projectile.ai[0]-period1)*(rotation2[direction]-rotation1[direction])/(period2-period1));
                length = length1+(Projectile.ai[0]-period1)*(length2-length1)/(period2-period1);
                Projectile.damage = (int)(damage*3*damage_bonus);
            }
            else if (Projectile.ai[0] < period3)
            {
                root = root2[direction]+(Projectile.ai[0]-period2)*(root3[direction]-root2[direction])/(period3-period2);
                Projectile.velocity = new Vector2(24f, 0f).RotatedBy(rotation2[direction]+(Projectile.ai[0]-period2)*(rotation3[direction]-rotation2[direction])/(period3-period2));
                length = length2+(Projectile.ai[0]-period2)*(length3-length2)/(period3-period2);
                Projectile.damage = (int)(damage*3*damage_bonus);
            }
            else
            {
                root = root3[direction];
                Projectile.velocity = new Vector2(24f, 0f).RotatedBy(rotation3[direction]);
                length=length3;
                Projectile.damage = (int)(damage*3*damage_bonus);
                //
            }

            //Main.NewText(length);
            Projectile.ai[0]++;
            for (int i = frame_tail -1; i>0; i--)
            {
                pos_old[i]=pos_old[i-1];
                vel_old[i]=vel_old[i-1];
            }
            pos_old[0] = (Projectile.timeLeft<=120) ? Vector2.Zero : root*scale+Projectile.velocity*scale;
            vel_old[0] = Projectile.velocity*length;
            //mahouSyoujyo.push((Projectile.timeLeft<=120)?Vector2.Zero:root*scale+Projectile.velocity*scale, Projectile.velocity*length, frame_tail, ref pos_old, ref vel_old);
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
                    scale_X: 1f* vel_old[i].Length()/24f * scale, scale_Y: 1f * scale, (direction==0) ? SpriteEffects.None : SpriteEffects.FlipVertically, layerDepth: 0);
                Dust.NewDustDirect(Projectile.Center + (root+2*Projectile.velocity)*scale,
                32*scale, 32*scale, 206, newColor: Color.DarkBlue);
            }*/
            mahouSyoujyo.draw_Center(
                tex: tex,
                frame_num: 1, frame: 0,
                pos: Projectile.Center + (root+Projectile.velocity)*scale /*+ new Vector2(0 , 16f*scale)*/, 
                color: Color.AliceBlue*Math.Clamp((float)(Projectile.timeLeft-60) / 60f, 0f, 1f),
                rot: Projectile.velocity.ToRotation(),
                scale_X: 1f * length *scale, scale_Y: 1f * scale, (direction==0) ? SpriteEffects.None : SpriteEffects.FlipVertically, layerDepth: 0);
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
            target.AddBuff(BuffID.Frostburn2, 120);
            //狂暴状态击中敌人回血
            if (player.GetModPlayer<Comboing>().supertime >0) player.Heal(1);
            //首次集中敌怪
            if (!returnCD)
            {
                //返还cd
                player.GetModPlayer<Comboing>().purryCD /=2;
                //不在狂暴状态就更新计数和弹幕
                if (player.GetModPlayer<Comboing>().supertime<=0)
                {
                    if (player.GetModPlayer<Comboing>().purryCount<6) //防御性代码
                    {
                        bool no_counter = true;
                        foreach (var projectile in Main.ActiveProjectiles)
                            if (projectile.type == ModContent.ProjectileType<ComboCounter>() && Main.myPlayer==projectile.owner)
                            {
                                projectile.Kill();
                                //projectile.frameCounter = player.GetModPlayer<Comboing>().purryCount;
                                projectile.netUpdate = true;
                                //Main.NewText(projectile.frameCounter);
                                //no_counter = false;
                            }
                        if (no_counter)
                        {
                            if (Projectile.owner==Main.myPlayer)
                            {
                                int index = Projectile.NewProjectile(player.GetSource_FromAI(),
                                player.Center+new Vector2(0, -60f),
                                Vector2.Zero,
                                ModContent.ProjectileType<ComboCounter>(), 0, 0, player.whoAmI);
                                //Main.projectile[index].frameCounter=player.GetModPlayer<Comboing>().purryCount;
                                Main.projectile[index].netUpdate = true;
                            }
                        }

                    }
                    player.GetModPlayer<Comboing>().purryCount++;
                }
                player.velocity.X = (player.GetModPlayer<MGPlayer>().magia ? 15f : 10f)*player.direction;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer,-1,-1,null,player.whoAmI);
                //Main.NewText(player.GetModPlayer<Comboing>().purryCount);
                returnCD=true;
            }
            player.SetImmuneTimeForAllTypes(20);
            SoundEngine.PlaySound(hited.WithVolumeScale((player.GetModPlayer<Comboing>().supertime>0)?0.3f:1f));//
            if (damage_bonus>=3f) damage_bonus = 0;                                                                                               //{ Volume = 3f, });
            if (damage_bonus!=0f) damage_bonus += 0.1f;
            //for (int i = 0; i<3; i++)
                Dust.NewDustDirect(target.Center, target.width, target.height, DustID.BlueFairy, newColor: Color.LightBlue);
            base.OnHitNPC(target, hit, damageDone);
        }
        
        public override void Kill(int timeLeft)
        {
            
            //Main.NewText(damage_bonus);

        }
    }
}