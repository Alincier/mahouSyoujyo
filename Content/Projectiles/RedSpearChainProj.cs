using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using mahouSyoujyo.Content.Items.MeleeWeapon;
using System.Security.Policy;
using Terraria.ID;
using Terraria.Audio;
using System.Diagnostics;

namespace mahouSyoujyo.Content.Projectiles
{
    public class RedSpearChainProj : ModProjectile
    {
        private int waiting = 0;
        private const int totalframe = 11;
        //音乐追踪初始化
        ProjectileAudioTracker tracker;
        public Vector2 targetPos
        {
            get {
                return new Vector2(Projectile.ai[1], Projectile.ai[2]);
            }
            set {
                Projectile.ai[1] = value.X; 
                Projectile.ai[2] = value.Y;
            }
        }

        //用速度记录透明度和旋转
        public int alpha
        {
            get
            {
                return (int)Projectile.velocity.X;
            }
            set
            {
                Projectile.velocity.X = value;
            }
        }
        public float rotation
        {
            get
            {
                return Projectile.velocity.Y;
            }
            set 
            { 
                Projectile.velocity.Y = value;
            }
        }



        public override void SetStaticDefaults()
        {
            Main.projFrames[this.Type]=totalframe;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 60;
            Projectile.alpha = 127;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.frame = 0;
            Projectile.frameCounter = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            tracker = new ProjectileAudioTracker(Projectile);
        }

        public override void AI()
        {
            //ai0 = -1为释放后状态，ai0 = -2 为释放中状态（转化后需要同步），ai0 为正数则为跟随状态（需要同步），数字决定位置

            // 确认激活延迟
            if (Projectile.ai[0] > 0) waiting = 4 * (((int)Projectile.ai[0]) / 4);

            //释放中状态
            if (Projectile.ai[0] == -2)
            {
                Projectile.timeLeft = 600;
                if (waiting>0)
                {
                    waiting--;
                    return;
                }
                Projectile.friendly = true;
                Projectile.ai[0] = -1;
                if (Main.netMode != NetmodeID.Server)
                    SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/chains"), Projectile.Center, soundInstance =>
                    {
                        // The SoundUpdateCallback can be inlined if desired, such as in this example.
                        soundInstance.Position = Projectile.Center;
                        return tracker.IsActiveAndInGame();
                    });
                alpha = 0;
                return;
            }
            if (Projectile.ai[0] > 0) alpha = 127;
            Player player = Main.player[Projectile.owner];
            Vector2 theCenter = Vector2.Zero;
            int theProj = -1;
            if (player.active)
            {
                theCenter = player.RotatedRelativePoint(player.MountedCenter);
                theProj = player.GetModPlayer<RedSpearComboing>().spearProj;
            }
            if (theProj > -1)
            {
                if (Main.projectile[theProj].active)
                {
                    if (Main.projectile[theProj].ModProjectile is RedSpearBullet bullet) theCenter = bullet.getTargetCenter();
                        else theCenter =  Main.projectile[theProj].Center;
                }
            }
            if (player == Main.LocalPlayer)
            {
                //手持状态按住↓跟随， 走了 或者 噶了 或者 松手 或者 武器不对 或者 被怪物打玉玉了 或者 生成计数归零了 就释放
                if (Projectile.ai[0] > 0 && player.active && !player.dead && player.input().keyDown && !player.releaseDown &&!player.noItems && player.HeldItem.ModItem is RedSpear && !player.CCed && player.GetModPlayer<RedSpearComboing>().chainCount > 0)
                {
                    Projectile.timeLeft = 60;
                    //获取位置并同步
                    targetPos = CalculatePosition(player, theCenter, (int)Projectile.ai[0] - 1,
                    player.GetModPlayer<RedSpearComboing>().chainCount);
                    Projectile.Center = targetPos;
                    Projectile.netUpdate = true;

                }
                else
                {
                    if (Projectile.ai[0] > 0) Projectile.ai[0] = -2;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[0] > 0) rotation = ((int)Projectile.ai[0] % 4) *MathHelper.PiOver2;
            if (Projectile.ai[0] > 0 || Projectile.ai[0] == -2) return;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frame = (Projectile.frame+1) % totalframe;
                Projectile.frameCounter = 0;
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
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
        public override bool PreDraw(ref Color lightColor)
        {
            // 绘制
            SpriteBatch sb = Main.spriteBatch;
            Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/RedSpearChainProj").Value;
            int width = tex.Width;
            int height = tex.Height /totalframe;
            Color color = Color.White*((255f -alpha) / 255f);
            Rectangle rect = new Rectangle(0, Projectile.frame *height, width, height);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, (Projectile.ai[0] == -1) ? BlendState.NonPremultiplied : BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            sb.Draw(
                tex,
                Projectile.Center - Main.screenPosition,
                rect,
                color,
                rotation,
                rect.Size() / 2,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
            return false;
        }
        public static Vector2 CalculatePosition(Player player , Vector2 playerCenter ,int index, int total)
        {
            // 井字形算法
            float distance = 12f * (total / 4);
            float position = 48f * (index / 4) - 24f * ((total / 4) - 1);
            float angle = (index % 4) * MathHelper.PiOver2;


            return new Vector2((float)position,(float)distance).RotatedBy(angle) + playerCenter;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 120);
        }
    }
}
