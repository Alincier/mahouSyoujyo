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
using mahouSyoujyo.Content.Items.MeleeWeapon;
using System.IO;

namespace mahouSyoujyo.Content.Projectiles.Weapon
{
    public class ComboCounter : ModProjectile
    {
        Player player => Main.player[Projectile.owner]; 
        Texture2D tex_number = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/ComboCounter").Value;
        Comboing Combo => player.GetModPlayer<Comboing>();
        Vector2 position;
        public override void SetStaticDefaults()    
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            Main.projFrames[this.Type]=6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 24; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 60; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            Projectile.frame=0;
            Projectile.frameCounter=0;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.timeLeft);
            //writer.Write((int)Projectile.frameCounter);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadInt32();
            //Projectile.frameCounter = Math.Clamp( reader.ReadInt32(),0,5);
            base.ReceiveExtraAI(reader);
        }
        public override void AI()
        {
            position = player.Center+new Vector2(0, -60f);
            Projectile.Center=position;
            Projectile.frame =Math.Clamp(player.GetModPlayer<Comboing>().purryCount, 0, 5);
            //计时器+1

                

        }
        public override bool PreDraw(ref Color lightColor)  
        {
            int count; 
            if (Projectile.timeLeft >0)
            {
                float transparent;
                if (Projectile.timeLeft > 55)
                    transparent = 1f-0.2f*(Projectile.timeLeft-55);
                else if (Projectile.timeLeft >20) transparent=1f;
                else transparent = (float)Projectile.timeLeft / 20f;
                int width = tex_number.Width;
                int height = tex_number.Height / Main.projFrames[this.Type];
                Rectangle rect = new Rectangle(0, Projectile.frame*height, width, height*((Projectile.timeLeft>20)?20: Projectile.timeLeft)/20);
                Main.EntitySpriteDraw(
                    tex_number, position - Main.screenPosition,
                    rect,Color.LightBlue*(1f), 0f,
                    new Vector2(width / 2, height / 2),
                    new Vector2(1f, 1f),
                    SpriteEffects.None, 0);
                //  Dust.NewDustDirect(Player.Center-new Vector2(8f, 48f),
                //  16, 24, 206, newColor: Color.DarkBlue);

            }
            return false;
        }
        public override void Kill(int timeLeft)
        {
            
            
        }
    }
}