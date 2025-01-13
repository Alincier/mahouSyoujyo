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
using System.Threading;
using Terraria.Audio;
using Mono.Cecil;
using mahouSyoujyo.Content.Projectiles;
using Terraria.Utilities;
using Steamworks;
using mahouSyoujyo.Content.Projectiles.Weapon;
using System.IO;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;
using mahouSyoujyo.Content.Items.RangedWeapon;

namespace mahouSyoujyo.Content.Projectiles
{
    public class ColorFiveBullet : ModProjectile
    {
        //材质
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/ColorFiveBullet").Value;
        bool initial = true;
        bool copy = false;
        //拖尾绘制
        int frame_tail;
        Vector2[] pos_old;
        Vector2[] vel_old;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            Main.projFrames[this.Type]=5;
        }
        private int col{
            get { return (int)Projectile.ai[2]; }
            set { Projectile.ai[2]=value; }
        }
        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = 1; //How many enemies could be hit and gone through.
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            Projectile.frame=0;
            //每个射弹独立无敌帧
            Projectile.usesLocalNPCImmunity=true;
            //无敌帧
            Projectile.localNPCHitCooldown = 10;


            //轨迹记录
            frame_tail = 15;
            pos_old = new Vector2[frame_tail];
            vel_old = new Vector2[frame_tail];
            
        }
        /*public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)col);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            col = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }*/
        public override void AI()
        {
            //   Main.spriteBatch.Draw(
            //       new Texture2D(
            //       Projectile.Center-Main.screenPosition,
            //       );
            if (Projectile.ai[0] == 0 && Projectile.owner == Main.myPlayer)
            {
                col=(new Random().Next(10)+1) / 2;
                if (col == 0 || col == 5) copy = true;
                Projectile.netUpdate = true;
            }
            switch (col)
            {
                case 0:
                    Projectile.frame=0;
                    break;
                case 1:
                    Projectile.frame=1;
                    break;
                case 2:
                    Projectile.frame=2;
                    break;
                case 3:
                    Projectile.frame=3;
                    break;
                case 4:
                    Projectile.frame=4;
                    break;
                default:
                    Projectile.frame=0;
                    break;
            }

            Projectile.ai[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (copy && Projectile.ai[0]>=5) 
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    var proj = Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromAI(),
                        Main.player[Projectile.owner].Center+new Vector2(0, -128),
                        (Main.MouseWorld-(Main.player[Projectile.owner].Center+new Vector2(0, -128))).SafeNormalize(Vector2.Zero)*30f,
                        ModContent.ProjectileType<ColorFiveBullet>(),
                        Projectile.damage,
                        Projectile.knockBack, Projectile.owner);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        proj.netUpdate = true;
                }
                
                copy = false;
            }
            if ((col == 2 || col == 5) && initial) 
            { 
                Projectile.extraUpdates=1; 
                Projectile.timeLeft=240; 
                Projectile.penetrate=-1; 
                Projectile.tileCollide = false;
            }
            //if (color == 2) Projectile.velocity=Projectile.velocity *3 /2;
            if ((col == 3 || col == 5) && initial) { 
            Main.player[Projectile.owner].Heal(
                (Main.player[Projectile.owner]).statLifeMax2/100);  }
            if ((col == 4 || col == 5) && initial) { Projectile.damage*=2;  }
            if ((col == 1 || col == 5))
            {
                if (Projectile.ai[0]<=30) Projectile.velocity =Projectile.velocity*9/10;
                else if (Projectile.ai[0]<=60) Projectile.velocity +=Projectile.velocity.SafeNormalize(Vector2.Zero)*2f;
                Projectile.damage =(int)(Projectile.damage*1.02+1);
            }
            if (col == 5) Projectile.frame = ((int)Projectile.ai[0] / 10) % 5;
            initial=false;
            //记录轨迹
            //if (Main.time % 2 ==0)
            mahouSyoujyo.push(Projectile.Center, Projectile.velocity, frame_tail, ref pos_old, ref vel_old);
            if (Projectile.ai[0]<=2 && Projectile.owner== Main.myPlayer)
            {
                Projectile.netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = frame_tail-1; i>0; i--)
                mahouSyoujyo.draw_Center(
                    tex: tex,
                    frame_num: 5, frame: Projectile.frame,
                    pos: pos_old[i], color: Color.LightGoldenrodYellow*(0.3f-0.02f*i),
                    rot: (pos_old[i-1]-pos_old[i]).ToRotation(),
                    scale_X: 1-0.05f*i, scale_Y: 1-0.05f*i);
            mahouSyoujyo.draw_Center(
                tex: tex,
                frame_num: 5, frame: Projectile.frame,
                pos: Projectile.Center, color: Color.White*1f,
                rot: Projectile.velocity.ToRotation(),
                scale_X: 1f, scale_Y: 1f);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i<5; i++)
            Dust.NewDustDirect(target.Center, target.width, target.height, DustID.PortalBoltTrail);
            Projectile.damage =(int)Projectile.damage*1;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i<5; i++)
            Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.Electric);
            SoundEngine.PlaySound(SoundID.Item94.WithVolumeScale(0.3f),Projectile.Center);
            /*Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
            Projectile.velocity,
                     ModContent.ProjectileType<explosion2>(),
                    Projectile.damage *  2, 2, Projectile.owner);*/
        }
    }
}