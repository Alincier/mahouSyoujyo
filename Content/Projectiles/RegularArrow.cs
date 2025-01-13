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

namespace mahouSyoujyo.Content.Projectiles
{
    public class RegularArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 40; // The width of projectile hitbox
            Projectile.height = 20; // The height of projectile hitbox

            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 2f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.stopsDealingDamageAfterPenetrateHits=false;
        }

        // Custom AI(符合函数的射弹）
        int timer = 0;
        Vector2 start_point = Vector2.Zero;
        Vector2 angle = Vector2.Zero;
        Vector2 position= Vector2.Zero;
        public override void AI()
        {
            if (timer==0)
            {   
                start_point=Projectile.Center;
                angle =Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
            } 
            timer++;
            //设置一秒水平前进多少格
            float speed = 30f;
            position = new Vector2(16*speed*(float)timer/60, 16*running_function(timer));
            position = position.RotatedBy(angle.ToRotation())+start_point;
            Projectile.velocity=position-Projectile.Center;
            float directed = MathHelper.ToRadians(45f);
            Projectile.rotation =directed +  Projectile.velocity.ToRotation();
        }

        //设置前进路径的函数,以正右方发射为例，输入水平向右的格数，输出竖直方向向下的格数。
        public float running_function(float right)
        {
            float down = (float)(Math.Sin(MathHelper.TwoPi*right/16));
            return down;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage =(int)Projectile.damage *9/10;
        }
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null

    }
}
