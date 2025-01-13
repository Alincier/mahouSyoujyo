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

namespace mahouSyoujyo.Content.Projectiles
{
    public class explosion2 : ModProjectile
    {
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/explosion2").Value;
        public override void SetStaticDefaults()
        {
            Main.projFrames[this.Type] = 5;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 176; // The width of projectile hitbox
            Projectile.height =144; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic;
            // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.

            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 30; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.stopsDealingDamageAfterPenetrateHits=true;
            //缩放
            //Projectile.scale =2f;
            //使用公用无敌帧
            //Projectile.usesIDStaticNPCImmunity=true;
            //每个射弹独立无敌帧
            Projectile.usesLocalNPCImmunity=true;
            //无敌帧
            Projectile.localNPCHitCooldown = 5;

        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((float)Projectile.scale);
            writer.Write((float)Projectile.Size.X);
            writer.Write((float)Projectile.Size.Y);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.scale = reader.ReadSingle();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            Projectile.Size = new Vector2(x, y);
            base.ReceiveExtraAI(reader);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        /*public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X = 50;
            hitbox.Y = 4;
            base.ModifyDamageHitbox(ref hitbox);
        }*/
        public override void AI()
        {
            Projectile.frame = Math.Clamp(4-Projectile.timeLeft / 6, 0, 4);
            //图像顺时针旋转角度

        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            int width = tex.Width;
            int height = tex.Height / 5;
            Rectangle rect = new Rectangle(0, Projectile.frame*height, width, height);
            sb.Draw(
                tex, Projectile.Center-Main.screenPosition,
                rect, Color.White, 0 ,
                new Vector2(width / 2, height / 2),
                new Vector2(Projectile.scale, Projectile.scale),
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
            if (count>=16) modifiers.SourceDamage*=0.1f;
            else if (count>=8) modifiers.SourceDamage*=0.2f;
            else if (count>=4) modifiers.SourceDamage*=0.3f;
            else if (count>=2) modifiers.SourceDamage*=0.5f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3,1200);
            target.AddBuff(BuffID.Oiled, 1200);
            target.netUpdate = true;
        }
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
    }
}
