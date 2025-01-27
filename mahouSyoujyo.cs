using mahouSyoujyo.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using ReLogic.Content;
using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content.Projectiles;
using mahouSyoujyo.Content.Buffs;
namespace mahouSyoujyo
{
    public struct Vertex : IVertexType
    {
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
        {
            new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
            new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
            new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
        });
        public Vector2 Position;
        public Color Color;
        public Vector3 TexCoord;
        public Vertex(Vector2 position, Vector3 texCoord, Color color)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
        }
        public VertexDeclaration VertexDeclaration
        {
            get => _vertexDeclaration;
        }
    }
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public partial class mahouSyoujyo: Mod
    {
        public static Effect screeenEffect;
        public static bool stoptime = false;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                // First, you load in your shader file.
                // You'll have to do this regardless of what kind of shader it is,
                // and you'll have to do it for every shader file.
                // This example assumes you have both armor and screen shaders.

                // Asset<Effect> dyeShader = this.Assets.Request<Effect>("Effects/MyDyes");
                // Asset<Effect> specialShader = this.Assets.Request<Effect>("Effects/MySpecials");
                Asset<Effect> filterShader1 = this.Assets.Request<Effect>("Effects/GrayScale");
                Asset<Effect> filterShader2 = this.Assets.Request<Effect>("Effects/ShockWave");
                // To add a dye, simply add this for every dye you want to add.
                // "PassName" should correspond to the name of your pass within the *technique*,
                // so if you get an error here, make sure you've spelled it right across your effect file.

                // GameShaders.Armor.BindShader(ModContent.ItemType<MyDyeItem>(), new ArmorShaderData(dyeShader, "PassName"));

                // If your dye takes specific parameters such as color, you can append them after binding the shader.
                // IntelliSense should be able to help you out here.   

                // GameShaders.Armor.BindShader(ModContent.ItemType<MyColorDyeItem>(), new ArmorShaderData(dyeShader, "ColorPass")).UseColor(1.5f, 0.15f, 0f);
                //  GameShaders.Armor.BindShader(ModContent.ItemType<MyNoiseDyeItem>(), new ArmorShaderData(dyeShader, "NoisePass")).UseImage("Images/Misc/noise"); // Uses the default Terraria noise map.

                // To bind a miscellaneous, non-filter effect, use this.
                // If you're actually using this, you probably already know what you're doing anyway.
                // This type of shader needs an additional parameter: float4 uShaderSpecificData;
                GameShaders.Misc["NormalGray"] = new MiscShaderData(this.Assets.Request<Effect>("Effects/GrayScale"), "NormalGrayScale");
                GameShaders.Misc["PixelHorizonGSMajoConciousness"] = new MiscShaderData(this.Assets.Request<Effect>("Effects/GaussianBlur"), "CustomPixelhorizonGS");
                //GameShaders.Misc["SmoothBackgroundMajoConciousness"] = new MiscShaderData(this.Assets.Request<Effect>("Effects/SmoothBackground"), "MajoConsciousnessShader");
                //绘制效果记得加上参数：float4 uShaderSpecificData;
                // To bind a screen shader, use this.
                // EffectPriority should be set to whatever you think is reasonable.   

                Filters.Scene["GrayScaleNormal"] = new Filter(new ScreenShaderData(filterShader1, "NormalGrayScale"), EffectPriority.VeryHigh);
                Filters.Scene["GrayScaleDynamic"] = new Filter(new ScreenShaderData(filterShader1, "DynamicGrayScale"), EffectPriority.VeryHigh);
                Filters.Scene["GrayScaleMajoConciousness"] = new Filter(new ScreenShaderData(filterShader1, "DynamicGrayScale"), EffectPriority.VeryHigh);
                Filters.Scene["GrayScaleTimeStop"] = new Filter(new ScreenShaderData(filterShader1, "CustomedGrayScale"), EffectPriority.VeryHigh);
                Filters.Scene["ShockWaveTechnique"] = new Filter(new ScreenShaderData(filterShader2, "NormalShockWave"), EffectPriority.VeryHigh);

                

            }



            On_Projectile.Update +=On_Projectile_Update;
            On_Player.Update += On_Player_Update;
            On_Main.UpdateTime +=On_Main_UpdateTime;
            On_NPC.UpdateNPC +=On_NPC_UpdateNPC;
            On_Rain.Update +=On_Rain_Update;
            On_Dust.UpdateDust +=On_Dust_UpdateDust;
            On_NPC.SpawnNPC  +=On_NPC_SpawnNPC;
            base.Load();
        }

        public static void SceneShader(string tech, float degree, float factor = 0, float r0 = 0, float r1 = 0,float r2 = 0,
            float targetX = -1, float targetY = -1)
        {

            if (Main.netMode != NetmodeID.Server)
            {
                if (tech == "GrayScaleMajoConciousness")
                {
                    float progress = degree;
                    //Main.NewText(progress);
                    if (!Filters.Scene[tech].IsActive())
                        Filters.Scene.Activate(tech).GetShader().UseProgress(progress);
                    else
                    {
                        Filters.Scene[tech].GetShader().UseProgress(progress);
                    }
                }
                if (tech == "GrayScaleTimeStop")
                {
                    float progress = degree;
                    float uIntensity = factor;
                    float inner = r0;
                    float outer = r1;
                    float ring = r2;
                    Vector3 ucolor = new Vector3(inner, outer, ring);
                    Vector2 position = new Vector2(targetX, targetY);
                    if (!Filters.Scene[tech].IsActive())
                        Filters.Scene.Activate(tech).GetShader().UseIntensity(uIntensity)
                            .UseProgress(progress).UseColor(ucolor).UseTargetPosition(position);
                    else
                    {
                        Filters.Scene[tech].GetShader().UseIntensity(uIntensity)
                            .UseProgress(progress).UseColor(ucolor).UseTargetPosition(position);
                    }


                }

                if (tech == "ShockWaveTechnique")
                {
                    
                    float progress = degree;//阶段
                    float opacity = factor;//扭曲度
                    //波纹的数量 （uColor.x）、每个波纹有多窄（uColor.y） 和冲击波传播速度 （uColor.z）
                    float count = r0;
                    float size = r1;
                    float speed = r2;
                    Vector3 ucolor = new Vector3(count, size, speed);
                    Vector2 position = new Vector2(targetX, targetY);
                    if (!Filters.Scene[tech].IsActive())
                        Filters.Scene.Activate(tech).GetShader().UseProgress(progress).UseColor(ucolor).UseTargetPosition(position).UseOpacity(opacity);
                    else
                    {
                        Filters.Scene[tech].GetShader().UseProgress(progress).UseColor(ucolor).UseTargetPosition(position).UseOpacity(opacity);
                    }



                }
                
            }
        }
        public static void DelSceneShader(string tech)
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene[tech].IsActive())
            {
                Filters.Scene[tech].Deactivate();
            }
        }
        private void On_NPC_SpawnNPC(On_NPC.orig_SpawnNPC orig)
        {
            //if (TimeStopSystem.TimeStopping) return;
            orig();
        }
        private void On_Dust_UpdateDust(On_Dust.orig_UpdateDust orig)
        {
            if (TimeStopSystem.TimeStopping) return;
            orig();

        }
        private void On_Rain_Update(On_Rain.orig_Update orig, Rain self)
        {
            if (TimeStopSystem.TimeStopping) return;
            if (self.active) orig(self);
        }
        private void On_NPC_UpdateNPC(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (TimeStopSystem.TimeStopping) 
            {
                if (self.active) 
                for (i=0; i<256; i++)
                {
                    if (self.immune[i]>0) self.immune[i]--;
                    else self.immune[i] = 0;
                }
                self.CheckActive();
                return;
            }
            if (self.active) orig(self,i);
        }

        private void On_Main_UpdateTime(On_Main.orig_UpdateTime orig)
        {
            if (TimeStopSystem.TimeStopping) return;
            orig(); 
            //throw new NotImplementedException();
        }
        private void On_Player_Update(On_Player.orig_Update orig, Player self, int i)
        {
            bool isBind = self.GetModPlayer<TimeStop>().bind;
            if (TimeStopSystem.TimeStopping && !isBind)
                return;
            if (self.active) orig(self, i);
            //throw new NotImplementedException();
        }

        //public override 
        private void On_Projectile_Update(On_Projectile.orig_Update orig, Projectile self, int i)
        {
            //时间导弹特供
            if (self.type == ModContent.ProjectileType<TimeMissile>() && self.active)
            {
                TimeMissile missile = (TimeMissile)self.ModProjectile;
                missile.runtime++;
                int magic = 0;
                if (Main.player[missile.Projectile.owner].HasBuff(ModContent.BuffType<MagicGirlPover>())) magic = 1;
                if (missile.runtime < 30) missile.stage = 0;
                else if (missile.runtime < 40) missile.stage = 1;
                else if (missile.runtime < 50) missile.stage = 2;
                else if (missile.runtime < 60) missile.stage = 3;
                else if (missile.runtime < 90) missile.stage = 4;
                else missile.stage = 4+magic; 
            }
            bool specialtype = false;
            if (self.aiStyle == ProjAIStyleID.Flail || self.aiStyle == ProjAIStyleID.Yoyo ||
                self.aiStyle == ProjAIStyleID.Whip || self.aiStyle == ProjAIStyleID.WireKite ||
                self.aiStyle == ProjAIStyleID.Pet || self.aiStyle == ProjAIStyleID.FloatBehindPet ||
                self.aiStyle == ProjAIStyleID.FloatInFrontPet || self.aiStyle == ProjAIStyleID.FloatingFollow ||
                self.aiStyle == ProjAIStyleID.WormPet ||
                self.aiStyle == ProjAIStyleID.MagicMissile || self.aiStyle == ProjAIStyleID.Boomerang ||
                self.aiStyle == ProjAIStyleID.Bubble || self.aiStyle == ProjAIStyleID.WaterJet ||
                self.aiStyle == ProjAIStyleID.Beam || self.aiStyle == ProjAIStyleID.ThickLaser ||
                self.aiStyle == ProjAIStyleID.ScutlixLaser || self.aiStyle == ProjAIStyleID.Drill ||
                self.aiStyle == ProjAIStyleID.FallingStar ||self.aiStyle == ProjAIStyleID.Spear ||
                self.aiStyle == ProjAIStyleID.ShortSword || self.aiStyle == ProjAIStyleID.SmallFlying ||
                self.aiStyle == ProjAIStyleID.FlowerPetal || self.aiStyle == ProjAIStyleID.TitaniumShard ||
                self.aiStyle == ProjAIStyleID.CrystalLeaf || self.aiStyle == ProjAIStyleID.CrystalLeafShot ||
                self.aiStyle == ProjAIStyleID.Harpoon || self.aiStyle == ProjAIStyleID.HeldProjectile ||
                self.aiStyle == ProjAIStyleID.NightsEdge || self.aiStyle == ProjAIStyleID.TrueNightsEdge ||
                self.aiStyle == ProjAIStyleID.PrincessWeapon

                )
                specialtype = true;
            bool held = false;
            for (int j=0;j<256;j++)
                if (Main.player[j].heldProj==self.whoAmI)
                    { held = true;  break; }

            if (TimeStopSystem.TimeStopping && ((Array.IndexOf(TimeStop.immuneProjectile, self.type)<0 && !held && !specialtype && !self.hide) || self.hostile) )
            {
                GlobalStopProjectile sp;
                if (self.active)
                {
                    if (!Main.projPet[self.type] && !self.minion && !self.sentry && !Main.projHook[self.type])
                    {
                        sp = self.GetGlobalProjectile<GlobalStopProjectile>();
                        if (sp.runningFrameTime >= 5)
                            return;
                        sp.runningFrameTime++;
                    }
                    //if (sp.runningFrameTime > sp.runningFrame /2)
                    //{
                    //    sp.runningFrameTime = 0;
                    //    sp.runningFrame++;
                    //}
                    //else return;
                        
                }
            }
            
            if (self.active)
            {
                bool isBind = Main.LocalPlayer.GetModPlayer<TimeStop>().bind;
                if (TimeStopSystem.TimeStopping && !isBind) return;
                orig(self, i);
            }
        }


        public static void push(Vector2 pos, Vector2 vel, int length, ref Vector2[] pos_old,ref Vector2[] vel_old)
        {
            for (int i = length -1;i>0; i--)
            {
                pos_old[i]=pos_old[i-1];
                vel_old[i]=vel_old[i-1];
            }
            pos_old[0] = pos;
            vel_old[0] = vel;
        }
        public static void draw_Center (Texture2D tex, int frame_num, int frame, Vector2 pos, Color color, 
            float rot, float scale_X, float scale_Y, SpriteEffects effects=SpriteEffects.None, float layerDepth=0)
        {
            int width=tex.Width;
            int height=tex.Height / frame_num;
            Rectangle rect=new Rectangle(0, frame*height, width, height);
            Main.EntitySpriteDraw(
                tex, pos-Main.screenPosition,
                rect, color, rot,
                new Vector2(width / 2 ,height / 2),
                new Vector2(scale_X, scale_Y),
                effects, layerDepth);
        }
        
        public static SoundStyle animationSound = new SoundStyle($"mahouSyoujyo/Radio/Music/Madoka",type:SoundType.Music)
        {
            Volume = 0.9f,
            PitchVariance = 0f,
            MaxInstances = 1,
            IsLooped = true,
            SoundLimitBehavior=SoundLimitBehavior.IgnoreNew,
            PlayOnlyIfFocused=false,
            Variants =new int[] { 1, 2, 3, 4, 5},
            VariantsWeights = new float[] { 1, 1, 1, 1, 1 },
        };

        public static void print(Type type, int typeid, int frame_num, int frame, Vector2 pos, Color color, float rotation, int direction)
        {
            Texture2D tex;
            //if (type.GetType() == Type.GetType("Projectile")) {
            tex = TextureAssets.Projectile[typeid].Value;// } //声明材质
            Rectangle rectangle = new Rectangle(0,//帧图左上水平坐标
                tex.Height /frame_num * frame,//帧图左上竖直坐标
                tex.Width,//帧图宽度
                tex.Height / frame_num //帧图宽度高度
                );
            /*Main.EntitySpriteDraw(
                tex,//材质
                pos-Main.screenPosition,//确定屏幕位置
                rectangle,//帧图
                color,//渲染颜色
                rotation,//旋转
                new Vector2(tex.Width /2, tex.Height /2 / frame_num),//图片中心点相对于帧图(0,0)的向量，也是放大和旋转的中心。
                new Vector2(1, 1),//缩放比例
                (direction ==1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally,//方向为1则不变，否则翻转
                0//绘制层级，不太好用，填0
                );*/
        }
        public static Vector2 focus(Vector2 pos, Vector2 vel, Vector2 target, float speed = 20f, float strenth = 10f)
        {

            return (vel+  ((target  - pos).SafeNormalize(Vector2.Zero))*strenth  ).SafeNormalize(Vector2.Zero) * speed;
             
        }
        public static NPC FindClosestNPC(float maxDetectDistance,Vector2 position)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {

                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, position);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}
