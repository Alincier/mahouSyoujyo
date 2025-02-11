using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using mahouSyoujyo.Content.Buffs;
using Terraria.Audio;

namespace mahouSyoujyo.Content.Projectiles
{
    public class HeartArrow : ModProjectile
    {
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/HeartArrow").Value;
        int frame_tail;
        Vector2[] pos_old;
        Vector2[] vel_old;
        public override void SetStaticDefaults()
        {
            Main.projFrames[this.Type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height =16; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic;
            // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = 1; //How many enemies could be hit and gone through.
            Projectile.light = 2f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.stopsDealingDamageAfterPenetrateHits=false;
            //缩放
            //Projectile.scale =2f;
            //使用公用无敌帧
            //Projectile.usesIDStaticNPCImmunity=true;
            //每个射弹独立无敌帧
            Projectile.usesLocalNPCImmunity=true;
            //无敌帧
            Projectile.localNPCHitCooldown = 20;
            frame_tail = 10;
            pos_old = new Vector2[frame_tail];
            vel_old = new Vector2[frame_tail];

        }


        // Custom AI
        public override void AI()
        {
            //ai[2]用来判断是否改变类型，若是，使用ai[1]来生成射弹伤害类型，默认为0；
            if (Projectile.ai[0] == 0 && Projectile.ai[2] == 1)
            {
                Projectile.DamageType = DamageClassLoader.GetDamageClass((int)Projectile.ai[1]);
            }
            Projectile.ai[0]++;
            //图像顺时针旋转角度
            float directed = MathHelper.ToRadians(0f);
            Projectile.rotation = directed + Projectile.velocity.ToRotation();
            //如果有魔法少女BUFF则追踪
            if (Main.player[Projectile.owner].HasBuff<MagicGirlPover>())
            {
                Projectile.tileCollide=false;
                float maxDetectRadius = 500f; // The maximum radius at which a projectile can detect a target
                float projSpeed = 20f; // The speed at which the projectile moves towards the target
                                       // Trying to find NPC closest to the projectile
                NPC closestNPC = null;
                float focus_strenth = 0f;
                closestNPC = mahouSyoujyo.FindClosestNPC(maxDetectRadius, Projectile.Center);
                focus_strenth = Math.Min(16, Projectile.ai[0]-5);
                if (closestNPC != null)
                {
                    // If found, change the velocity of the projectile and turn it in the direction of the target
                    // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                    Projectile.velocity = mahouSyoujyo.focus(Projectile.Center, Projectile.velocity, closestNPC.Center, projSpeed, focus_strenth);// (Projectile.velocity+focus_strenth*(closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero)).SafeNormalize(Vector2.Zero) * projSpeed;
                    Projectile.rotation =directed +  Projectile.velocity.ToRotation();
                }
            }
            else Projectile.tileCollide=true;
            //记录轨迹
            //if (Main.time % 2 ==0)
            mahouSyoujyo.push(Projectile.Center, Projectile.velocity, frame_tail,ref pos_old,ref vel_old);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = frame_tail-1; i>0; i--)
            {
                mahouSyoujyo.draw_Center(
                    tex: tex,
                    frame_num: 2, frame: 1,
                    pos: pos_old[i], color: Color.DeepPink*(0.5f-0.05f*i),
                    rot: (pos_old[i-1]-pos_old[i]).ToRotation(),
                    scale_X: 1-0.1f*i, scale_Y: 1-0.1f*i);
                if (i == 2) Dust.NewDustDirect(pos_old[i], 16, 16, DustID.PinkTorch);
            }
            mahouSyoujyo.draw_Center(
                tex: tex,
                frame_num: 2, frame: 0,
                pos: Projectile.Center, color: Color.White*1f,
                rot: Projectile.velocity.ToRotation(),
                scale_X: 1f, scale_Y: 1f);
            //Dust.NewDustDirect(Projectile.Center, 16, 16, DustID.PinkTorch);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage =(int)Projectile.damage *5/10;
            for (int i = 0; i<5; i++)
            Dust.NewDustDirect(target.Center, target.width, target.height, DustID.PinkTorch);
        }
        public override void Kill(int timeLeft)
        {
            for (int i =0;i<20;i++)
            Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.PinkTorch);
            SoundEngine.PlaySound(SoundID.Item118,Projectile.Center);
        }
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
    }
}
