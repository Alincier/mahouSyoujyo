using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using mahouSyoujyo.Content.Buffs;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.Audio;
using System.IO;
using System.Reflection.Metadata;
using mahouSyoujyo.Content.Items.MeleeWeapon;
using Mono.Cecil;
using Terraria.DataStructures;
using mahouSyoujyo.Globals;

namespace mahouSyoujyo.Content.Projectiles.Weapon
{
    public class RedSpearBend : ModProjectile
    {
        public const float loading = 900f;
        private float rotted_angle = 0f;
        private int frame_tail = 30;
        private Vector2 targetOffset = Vector2.Zero;
        private int insertedtime = 0;
        private int returning_direction = 1;
        public bool charged = false;
        
        //音乐追踪初始化
        ProjectileAudioTracker tracker;
        public float holdoutTime
        {
            get { return Projectile.ai[1]; }
            set
            {
                Projectile.ai[1] = value;
            }
        }
        private float shadowalpha = 0f;
        private bool released
        {
            get { return Projectile.ai[2] > 0; }
            set
            {
                if (value) Projectile.ai[2] = 1;
            }
        }

        //拖尾绘制
        Vector2[] pos_old;
        Vector2[] vel_old;
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/Weapon/RedSpearBend").Value;
        Player player => Main.player[Projectile.owner];
        //public override string Texture => "mahouSyoujyo/Content/Items/RangedWeapon/ColorFive.png";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            //弹幕中心光照
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
            Main.projFrames[this.Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = frame_tail; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            Projectile.frame = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
            //设置绘制图层
            //Projectile.hide = true;
            Projectile.extraUpdates = 2;
            //轨迹记录
            pos_old = new Vector2[frame_tail];
            vel_old = new Vector2[frame_tail];
            //音乐追踪初始化
            tracker = new ProjectileAudioTracker(Projectile);
        }
        public override bool ShouldUpdatePosition()
        {
             return false;

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.frame);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.frame =  reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {

            if (Projectile.ai[0] == 0) 
            { 
                Projectile.ai[0] = 160f;
            }
                

            //确定攻击频率和修正大小
            if (player.magic().magia)
            {
                Projectile.localNPCHitCooldown = 30;
            }
            else
            {
                Projectile.localNPCHitCooldown = 60;
            }

            if (released) Projectile.friendly = false;


            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);


            //位置和方向更新（已关闭自动的位置更新）
            Projectile.Center = playerCenter;    
            if (Main.netMode == NetmodeID.MultiplayerClient)     
                Projectile.netUpdate = true;
            if (player.direction > 0) Projectile.frame = 0;
            else Projectile.frame = 1;
            //手持弹幕逻辑
                if (Main.myPlayer == Projectile.owner)
            {

                bool stillInUse = player.channel && !player.noItems && !player.CCed;
                if (stillInUse && !released)
                {
                    if (player.GetModPlayer<RedSpearComboing>().swing_charge > 0) holdoutTime = loading; 
                    else holdoutTime ++;
                    if (rotted_angle > 225f)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/spear_releasted"), Projectile.Center, soundInstance =>
                        {
                            // The SoundUpdateCallback can be inlined if desired, such as in this example.
                            soundInstance.Position = Projectile.Center;
                            return tracker.IsActiveAndInGame();
                        });
                        rotted_angle -= 360f;
                    }
                    float angle = Math.Clamp(holdoutTime * 6f / loading, 0f,6f);
                    if (player.magic().magia) angle *= 2f;
                    rotted_angle += angle;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(angle * player.direction));
                }
                else
                {
                    if (!released)
                    {
                        released = true;
                        
                    }

                }
            }
            if (!released)
            {
                Projectile.timeLeft =2;
                player.heldProj = Projectile.whoAmI; // We tell the player that the drill is the held projectile, so it will draw in their hand
                player.SetDummyItemTime(2); // Make sure the player's item time does not change while the projectile is out
                Projectile.Center = playerCenter; // Centers the projectile on the player. Projectile.velocity will be added to this in later Terraria code causing the projectile to be held away from the player at a set distance.
                Projectile.rotation = Projectile.velocity.ToRotation();
                player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            }

            if (holdoutTime >= loading) player.GetModPlayer<RedSpearComboing>().bullet_charge = 60;

            //拖尾


            for (int i = frame_tail - 1; i > 0; i--)
            {
                pos_old[i] = pos_old[i - 1];
                vel_old[i] = vel_old[i - 1];
            }
            pos_old[0] = Projectile.Center;
            vel_old[0] = Projectile.velocity;


            


        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            if (holdoutTime < loading)
            {
                if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*160f,
                Projectile.Center-Projectile.velocity.SafeNormalize(Vector2.Zero)*32f,
                8f, ref point
                )) return true;
                if (Collision.CheckAABBvAABBCollision(
                    targetHitbox.TopLeft(),
                    targetHitbox.Size(),
                    Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*120f + new Vector2(-24f, -24f),
                    new Vector2(48f, 48f)
                )) return true;
                return false;
            }
            Vector2 line = new Vector2(Projectile.ai[0], 0);
            float width = Projectile.ai[0] / (float)Math.Sqrt(3);
            for (int i = 0; i < 3; i++)
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center - line.RotatedBy(MathHelper.ToRadians(i * 60f)),
                Projectile.Center + line.RotatedBy(MathHelper.ToRadians(i * 60f)),
                width, ref point
                )) return true;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0) shadowalpha = 0f;
            else shadowalpha = Math.Clamp( holdoutTime / loading, 0f, 1f);
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();
            float step = 120f;
            for (int i = 1; i < step; i++)
            {
                Color b = Color.Lerp(Color.Red, Color.Black,i / step);
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + new Vector2(96f, 0).RotatedBy(Projectile.velocity.ToRotation()+ MathHelper.ToRadians(-(3 * i ) * player.direction)),
                          new Vector3(i / step, 0f, 1f),
                          b * .6f * shadowalpha));
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition  + new Vector2(172f, 0).RotatedBy(Projectile.velocity.ToRotation()+ MathHelper.ToRadians( -(3 * i ) * player.direction)),
                          new Vector3(i / step, 1f, 1f),
                          b * .6f * shadowalpha));
            }
            if (ve.Count >= 3)
            {
                gd.Textures[0] = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/MidLight").Value;//获取刀光的拖尾贴图
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            int width = tex.Width;
            int height = tex.Height / Main.projFrames[this.Type];
            Rectangle rect = new Rectangle(0, height * Projectile.frame, width, height);
            if (holdoutTime < loading)
            for (int i = frame_tail - 1; i > 0; i--)
            {
                sb.Draw(
                tex, player.Center - Main.screenPosition,
                rect, Color.IndianRed * .3f * shadowalpha, -3.14f / 4f + vel_old[i].ToRotation(),
                new Vector2(48, 48),
                new Vector2(1, 1),
                SpriteEffects.None, 0);
            }

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            sb.Draw(
                tex, Projectile.Center - Main.screenPosition,
                rect, Color.White , -3.14f / 4f + Projectile.velocity.ToRotation(),
                new Vector2(48, 48),
                new Vector2(1f, 1f),
                 SpriteEffects.None, 0);
            //if (Projectile.timeLeft>120) 
            {
                //Dust.NewDustDirect(Projectile.Center + new Vector2(-8,-8),
                //16, 16, DustID.RedTorch, newColor: Color.White);
            }

            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int who = target.realLife;
            int count = 0;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.realLife==who) count++;
            }
            if (count>=16) modifiers.SourceDamage*=0.5f;
            else if (count>=8) modifiers.SourceDamage*=0.6f;
            else if (count>=4) modifiers.SourceDamage*=0.7f;
            else if (count>=2) modifiers.SourceDamage*=0.8f;
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner && holdoutTime >= loading)
            {

                if (player.GetModPlayer<RedSpearComboing>().spearcount >= player.GetModPlayer<RedSpearComboing>().maxspear) return;
                int up = Projectile.NewProjectile(Projectile.InheritSource(Projectile),
                    player.Center+(Main.MouseWorld-player.Center).SafeNormalize(Vector2.Zero)*16,
                    (Main.MouseWorld-player.Center).SafeNormalize(Vector2.Zero)*Projectile.velocity.Length(),
                    ModContent.ProjectileType<RedSpearBullet>(), //ModContent.ProjectileType<YellowGunBullet>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
                player.SetDummyItemTime(10);
            }

        }
    }
}