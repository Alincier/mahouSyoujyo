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
using mahouSyoujyo.Content.Items.MeleeWeapon;
using Terraria.ModLoader.IO;

namespace mahouSyoujyo.Content.Projectiles
{
    public class RedSpearBullet : ModProjectile
    {
        private bool initial = true;
        private int frame_tail = 30;
        private Vector2 targetOffset = Vector2.Zero;
        private int insertedtime = 0;
        private int returning_direction = 1;
        private int puncture_damage = 1;
        private float oldState = 0f;
        public bool charged = false;
        private bool returning_sound = false;
        //音乐追踪初始化
        ProjectileAudioTracker tracker; 
        public float holdoutSpeed
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
                if (value) Projectile.ai[2] = 1 ;
            }
        }
        private bool inserted
        {
            get { return Projectile.ai[2] >=3 ; }
            set
            {
                if (value) Projectile.ai[2] = 3;
            }
        }
        private bool returning
        {
            get { return Projectile.ai[2] == 2; }
            set
            {
                if (value) Projectile.ai[2] = 2;
            }
        }
        private bool puncture
        {
            get { return targetIndex >= 0; }
        }
        private int targetIndex
        {
            get { return (Projectile.ai[2] >=4)?(int)(Projectile.ai[2] - 4): -1; }
            set
            {
                Projectile.ai[2] = value + 4;
            }
        }
        public void back()
        {
            if (returning) return;
            if (puncture)
            {

                if (Main.npc[targetIndex].active)
                {
                    Main.npc[targetIndex].SimpleStrikeNPC(damage: puncture_damage,hitDirection: (Projectile.velocity.X > 0) ? 1 : -1, crit: false, knockBack: Projectile.knockBack,damageType: Projectile.DamageType );
                }
            }
            returning = true;
            returning_direction = player.direction;
            Projectile.timeLeft = 90+frame_tail;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                Projectile.netUpdate = true;
        }
        //拖尾绘制
        Vector2[] pos_old;
        Vector2[] vel_old;
        private Texture2D tex = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Projectiles/RedSpearBullet").Value;
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
            Projectile.timeLeft = 180+frame_tail; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            Projectile.frame=0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown=24;
            Projectile.Center = player.Center;
            //设置绘制图层
            Projectile.hide = true;
            Projectile.extraUpdates = 2;
            //轨迹记录
            pos_old = new Vector2[frame_tail];
            vel_old = new Vector2[frame_tail];
            //音乐追踪初始化
            tracker = new ProjectileAudioTracker(Projectile);
        }
        public override bool ShouldUpdatePosition()
        {
            if (inserted  || returning) return false;
            if (Projectile.timeLeft <= frame_tail || !released) return false;
            return true;
            
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.timeLeft);
            writer.Write((int)Projectile.frame);
            writer.Write((int)returning_direction);
            writer.Write((int)puncture_damage);
            writer.Write((Single)targetOffset.X);
            writer.Write((Single)targetOffset.Y);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft =  reader.ReadInt32();
            Projectile.frame =  reader.ReadInt32();
            returning_direction = reader.ReadInt32();
            puncture_damage = reader.ReadInt32();
            targetOffset.X = reader.ReadSingle();
            targetOffset.Y = reader.ReadSingle();
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {
            //确定攻击频率
            Projectile.localNPCHitCooldown=24;
            /*if (player.HasBuff(ModContent.BuffType<MagicGirlPover>()))
            {
                Projectile.localNPCHitCooldown=12;
            }
            else
            {
                Projectile.localNPCHitCooldown=24;
            }*/
            
            //距离太远会消失
            if (Vector2.Distance(Projectile.Center,player.Center) > 3200f)
            {
                Projectile.Kill();
                Projectile.active = false;
            }
            //记录传入的速度到ai0
            if (Projectile.ai[0] == 0) 
            {
                Projectile.ai[0] = Projectile.velocity.Length();
                oldState = Projectile.ai[2];
            } 
            
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);


            //碰撞和重力
            if (returning) Projectile.tileCollide = false;
            else
            {
                if (!Projectile.tileCollide && Projectile.timeLeft <= 180 + frame_tail - 6) Projectile.tileCollide = true;
                if (Projectile.timeLeft<= 210-frame_tail && Projectile.timeLeft > frame_tail)
                {
                    if (Projectile.velocity.Y < 20f) Projectile.velocity.Y+=0.1f;
                    if (Projectile.velocity.Y >= 20f) Projectile.velocity.Y=20f;
                }
            }

            //回旋时候的位置和方向更新（已关闭自动的位置更新）
            if (returning)
            {
                if (!returning_sound)
                {
                    SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/spear_return"), Projectile.Center, soundInstance =>
                    {
                        // The SoundUpdateCallback can be inlined if desired, such as in this example.
                        soundInstance.Position = Projectile.Center;
                        return tracker.IsActiveAndInGame();
                    });
                    returning_sound = true;
                }
                if (Projectile.timeLeft <= frame_tail) Projectile.Center = playerCenter;
                else
                {
                    float split = Math.Max(1, Projectile.timeLeft - frame_tail);
                    Projectile.Center = Projectile.Center + (playerCenter - Projectile.Center) / split;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(4*returning_direction));
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        Projectile.netUpdate = true;
                }

            }

            //插入状态逻辑（排除正在回旋的状态）
            if (inserted && !returning) 
            {
                //刺入敌怪时间最长10s（一帧3更新）
                if ( puncture) insertedtime++;
                if (insertedtime >1800)
                {
                    Projectile.Kill();
                    Projectile.active = false;
                }
                Projectile.timeLeft = 180+frame_tail;
                Projectile.tileCollide=false;

                //如果穿刺敌怪则每帧更新位置
                if (puncture)
                {
                    if (Main.npc[targetIndex].active && Main.npc[targetIndex].life > 0) Projectile.Center=Main.npc[targetIndex].Center + targetOffset;
                    else released = true;
                }
                    
                return;
            } else insertedtime = 0;
            
            //手持弹幕逻辑
            if (Main.myPlayer == Projectile.owner)
            {

                // This code must only be ran on the client of the projectile owner

                // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                // player.channel indicates whether the player is still holding down the mouse button to use the item.
                bool stillInUse = player.channel  && !player.noItems && player.HeldItem.ModItem is RedSpear && !player.CCed;

                //充能则加满速度并释放
                if (player.GetModPlayer<RedSpearComboing>().bullet_charge > 0)
                {
                    holdoutSpeed = Projectile.ai[0];
                }
                if (initial || (stillInUse && !released) )
                {
                    initial = false;
                    if (holdoutSpeed < Projectile.ai[0]) holdoutSpeed +=.1f;
                    else holdoutSpeed = Projectile.ai[0];
                    // Calculate a normalized vector from player to mouse and multiply by holdoutSpeed to determine resulting holdoutOffset
                    Vector2 holdoutOffset = (Projectile.ai[0] / 4f + holdoutSpeed * 3f / 4f) * Vector2.Normalize(Main.MouseWorld - playerCenter);
                    if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                    {
                        // This will sync the projectile, most importantly, the velocity.
                        Projectile.netUpdate = true;
                    }

                    // Projectile.velocity acts as a holdoutOffset for held projectiles.
                    Projectile.velocity = holdoutOffset;
                }
                else
                {
                    if (!released) 
                    {
                        Projectile.damage = (int)(0.2f * Projectile.damage * (1f +holdoutSpeed / Projectile.ai[0]));
                        released = true;
                        if (player.direction > 0) Projectile.frame = 0 ;
                        else Projectile.frame = 1 ;
                        SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/spear_releasted"), Projectile.Center, soundInstance => {
                            // The SoundUpdateCallback can be inlined if desired, such as in this example.
                            soundInstance.Position = Projectile.Center;
                            return tracker.IsActiveAndInGame();
                        });
                    }
                         
                }
                if (oldState != Projectile.ai[2]) Projectile.netUpdate = true;
                oldState = Projectile.ai[2];
            }
            if (!released)
            {
                Projectile.timeLeft = 180+frame_tail;
                if (Projectile.velocity.X > 0f)
                {
                    player.ChangeDir(1);
                }
                else if (Projectile.velocity.X < 0f)
                {
                    player.ChangeDir(-1);
                }
                player.ChangeDir(Projectile.direction); // Change the player's direction based on the projectile's own
                player.heldProj = Projectile.whoAmI; // We tell the player that the drill is the held projectile, so it will draw in their hand
                player.SetDummyItemTime(10); // Make sure the player's item time does not change while the projectile is out
                Projectile.Center = playerCenter; // Centers the projectile on the player. Projectile.velocity will be added to this in later Terraria code causing the projectile to be held away from the player at a set distance.
                Projectile.rotation = Projectile.velocity.ToRotation();
                player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
                if (Projectile.rotation > -3.14f / 2f && Projectile.rotation <= 3.14f / 2f) Projectile.frame = 0;
                else Projectile.frame = 1;

            }

            if (returning && Projectile.timeLeft <= frame_tail &&  holdoutSpeed >=Projectile.ai[0]) player.GetModPlayer<RedSpearComboing>().swing_charge = 60;
            if (!puncture)
            {
                puncture_damage = 1;
            }

            //拖尾
            if (Projectile.timeLeft <=frame_tail)
            {
                Projectile.friendly = false;
                for (int i = frame_tail -1; i>0; i--)
                {
                    pos_old[i]=pos_old[i-1];
                    vel_old[i]=vel_old[i-1];
                }
                pos_old[0] = Projectile.Center;
                vel_old[0] = vel_old[1];
                return;
            }
            for (int i = frame_tail -1; i>0; i--)
            {
                pos_old[i]=pos_old[i-1];
                vel_old[i]=vel_old[i-1];
            }
            pos_old[0] = Projectile.Center;
            vel_old[0] = Projectile.velocity;
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            if (!released) return false;
            if (Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*144f,
                Projectile.Center-Projectile.velocity.SafeNormalize(Vector2.Zero)*48f,
                8f, ref point
                )) return true;
            if ( Collision.CheckAABBvAABBCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center+Projectile.velocity.SafeNormalize(Vector2.Zero)*104f + new Vector2(-24f , -24f),
                new Vector2(48f, 48f)
                )) return true;
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!inserted && !returning)
            {
                inserted = true;
                SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/spear_insert"), Projectile.Center, soundInstance => {
                    // The SoundUpdateCallback can be inlined if desired, such as in this example.
                    soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame();
                });
            }
                
            Projectile.velocity = oldVelocity;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0) shadowalpha = 0f;
            else shadowalpha = holdoutSpeed / Projectile.ai[0];
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();

            for (int i = 1; i < 20; i++)
            {
                if ((inserted || returning  )&& i>= 10) continue;
                Color b = Color.Lerp(Color.Red , Color.Black, (i<10)? 0f :i / 20.0f);	
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + new Vector2(100f - 16f * i, (i<10)? -8f : -1.6f * (20-i)).RotatedBy( Projectile.velocity.ToRotation()),
                          new Vector3(i / 20f, 0f, 1f ),
                          b*shadowalpha));
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + new Vector2(100f - 16f * i, (i<10)? 8f : 1.6f * (20-i)).RotatedBy( Projectile.velocity.ToRotation()),
                          new Vector3(i / 20f, 1f, 1f ),
                          b*shadowalpha));
            }
            if (ve.Count >= 3)
            {
                gd.Textures[0] = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/MidLight").Value;//获取刀光的拖尾贴图
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            int width = tex.Width;
            int height = tex.Height / Main.projFrames[this.Type];
            Rectangle rect = new Rectangle(0, height * Projectile.frame, width, height);
            Single alpha = Math.Min((Single)Projectile.timeLeft / (Single)frame_tail, 1f);
            if (returning)
            {
                for (int i = frame_tail-1; i>0; i--)
                {
                    sb.Draw(
                    tex, pos_old[i]-Main.screenPosition,
                    rect, Color.IndianRed * .4f * alpha, -3.14f /4f +vel_old[i].ToRotation(),
                    new Vector2(48, 48),
                    new Vector2(1, 1),
                    SpriteEffects.None, 0);
                }

            }
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            sb.Draw(
                tex, Projectile.Center-Main.screenPosition,
                rect, ((targetIndex >=0) ? Color.PaleVioletRed : Color.White) * alpha, -3.14f /4f +Projectile.velocity.ToRotation(),
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
            if (inserted) 
            {
                behindNPCsAndTiles.Remove(index);
                overPlayers.Remove(index);
                behindNPCs.Add(index);
                return;
            }

            behindNPCs.Remove(index);
            behindNPCsAndTiles.Remove(index);
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (returning) return;
            if (!target.active) return;
            if (!target.immortal && target.life < damageDone ) return;
            if (released && !inserted)
            {
                inserted = true;
                targetIndex = target.whoAmI;
                targetOffset = Projectile.Center - target.Center;
                SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/spear_puncture"), Projectile.Center, soundInstance => {
                    // The SoundUpdateCallback can be inlined if desired, such as in this example.
                    soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame();
                });
                Projectile.netUpdate = true;
            }
            if (puncture && target.whoAmI == targetIndex)
            {
                puncture_damage = Math.Clamp(puncture_damage + damageDone , 1 , Projectile.damage * 60);
            }
            //for (int i = 0;i < 10;i++) 
            //Dust.NewDustDirect(Projectile.Center + new Vector2(-target.width / 2f, -target.height / 2f),
            //target.width, target.height, DustID.RedTorch, newColor: Color.White);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int who = target.realLife;
            int count = 0;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (who != -1 && npc.realLife==who) count++;
            }
            if (count>=16) modifiers.FinalDamage*=0.6f;
            else if (count>=8) modifiers.FinalDamage*=0.7f;
            else if (count>=4) modifiers.FinalDamage*=0.8f;
            else if (count>=2) modifiers.FinalDamage*=0.9f;
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnKill(int timeLeft)
        {
            if (returning && holdoutSpeed >=Projectile.ai[0]) 
            {
                player.GetModPlayer<RedSpearComboing>().swing_charge = 60;
                player.GetModPlayer<RedSpearComboing>().bullet_charge = 60;
            }
            
            base.OnKill(timeLeft);
        }
        public Vector2 getTargetCenter() 
        {
            if (puncture && Main.npc[targetIndex].active) return Main.npc[targetIndex].Center;
            return Projectile.Center; 
        }
    }
}