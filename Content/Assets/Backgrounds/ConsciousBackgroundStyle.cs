using mahouSyoujyo.Content.NPCs.BOSSes.Majo_Consciousness;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace mahouSyoujyo.Backgrounds
{
    public class ConsciousBackgroundStyle : ModSurfaceBackgroundStyle
    {
        // Use this to keep far Backgrounds like the mountains.
        int tex0;// = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/Backgrounds/ConsciousBackgroundStyle0").Value;
        int tex1;// = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/Backgrounds/ConsciousBackgroundStyle1").Value;
        public override void Load()
        {
        }
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }
        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                int majoindex = NPC.FindFirstNPC(ModContent.NPCType<Majo_Consciousness>());
            Majo_Consciousness majo;
            /*if (majoindex >= 0 && Main.npc[majoindex].active)
            {
                majo = (Majo_Consciousness)Main.npc[majoindex].ModNPC;
                if (majo.getStage()>1) textureSlot = tex1;
            }*/
            float bgScale = 1.25f;
            double bgParallax = 0.37f;

            float a = 1800.0f;
            float b = 1750.0f;
            int textureSlot = ChooseCloseTexture(ref bgScale, ref bgParallax, ref a, ref b);

            if (textureSlot < 0 || textureSlot >= TextureAssets.Background.Length)
            {
                return false;
            }

            //Custom: bgScale, textureslot, patallaz, these 2 numbers...., Top and Start?
            Main.instance.LoadBackground(textureSlot);

            bgScale *= 2f;
            float bgWidthScaled = (int)((float)Main.backgroundWidth[textureSlot] * bgScale);

            SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);

            float bgStartX = (int)(-Math.IEEERemainder(Main.screenPosition.X * bgParallax, bgWidthScaled) - (bgWidthScaled / 2));
            float bgTopY = -Main.screenPosition.Y / ((float)Main.worldSurface * 16.0f);//(int)((-Main.screenPosition.Y ) / (Main.worldSurface * 16.0) * a + b);

            if (Main.gameMenu)
            {
                bgTopY = 320;
            }

            float bgLoops = Main.screenWidth / bgWidthScaled + 2;

            if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
            {
                for (int k = 0; k < bgLoops; k++)
                {
                    Main.spriteBatch.Draw(
                        TextureAssets.Background[textureSlot].Value,
                        new Vector2(bgStartX + bgWidthScaled * k, bgTopY),
                        new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]),
                        Color.White,
                        0f,
                        default,
                        bgScale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
            /*
            int width = tex.Width;
            int height = tex.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            spriteBatch.Draw(
                tex, Main.LocalPlayer.Center,
                rect, Color.White, 0,
                new Vector2(width / 2, height / 2),
                new Vector2(200f, 200f),
                SpriteEffects.None, 0);
            return false;*/
            return false;
        }
       // public override int ChooseFarTexture()
       // {

       // }
/*        private static int SurfaceFrameCounter;
        private static int SurfaceFrame;
        public override int ChooseMiddleTexture()
        {
            if (++SurfaceFrameCounter > 12)
            {
                SurfaceFrame = (SurfaceFrame + 1) % 4;
                SurfaceFrameCounter = 0;
            }
            switch (SurfaceFrame)
            {
                case 0:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/ExampleBiomeSurfaceMid0");
                case 1:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/ExampleBiomeSurfaceMid1");
                case 2:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/ExampleBiomeSurfaceMid2");
                case 3:
                    return BackgroundTextureLoader.GetBackgroundSlot("ExampleMod/Assets/Textures/Backgrounds/ExampleBiomeSurfaceMid3"); // You can use the full path version of GetBackgroundSlot too
                default:
                    return -1;
            }
        }*/
        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            tex0 = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Assets/Backgrounds/ConsciousBackgroundStyle0");
            tex1 = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Assets/Backgrounds/ConsciousBackgroundStyle1");
            a =7200; //4600f/ 25000f *Main.bottomWorld;
            b = Main.LocalPlayer.Center.Y / Main.bottomWorld *25000-1200f / Main.bottomWorld *25000;
            int majoindex = NPC.FindFirstNPC(ModContent.NPCType<Majo_Consciousness>());
            scale = 1f;
            parallax = 0.05f;
            Majo_Consciousness majo;
            if (majoindex >= 0 && Main.npc[majoindex].active)
            {
                majo = (Majo_Consciousness)Main.npc[majoindex].ModNPC;
                if (majo.getStage()>1) return tex1;
            }
            return tex0;
        }
        /*ิดย๋
	public void DrawCloseBackground(int style)
	{
		if (!GlobalBackgroundStyleLoader.loaded || MenuLoader.loading) {
			return;
		}

		if (Main.bgAlphaFrontLayer[style] <= 0f) {
			return;
		}

		var surfaceBackgroundStyle = Get(style);

		if (surfaceBackgroundStyle == null || !surfaceBackgroundStyle.PreDrawCloseBackground(Main.spriteBatch)) {
			return;
		}

		Main.bgScale = 1.25f;
		Main.instance.bgParallax = 0.37;

		float a = 1800.0f;
		float b = 1750.0f;
		int textureSlot = surfaceBackgroundStyle.ChooseCloseTexture(ref Main.bgScale, ref Main.instance.bgParallax, ref a, ref b);

		if (textureSlot < 0 || textureSlot >= TextureAssets.Background.Length) {
			return;
		}

		//Custom: bgScale, textureslot, patallaz, these 2 numbers...., Top and Start?
		Main.instance.LoadBackground(textureSlot);

		Main.bgScale *= 2f;
		Main.bgWidthScaled = (int)((float)Main.backgroundWidth[textureSlot] * Main.bgScale);

		SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)Main.instance.bgParallax);

		Main.instance.bgStartX = (int)(-Math.IEEERemainder(Main.screenPosition.X * Main.instance.bgParallax, Main.bgWidthScaled) - (Main.bgWidthScaled / 2));
		Main.instance.bgTopY = (int)((-Main.screenPosition.Y + Main.instance.screenOff / 2f) / (Main.worldSurface * 16.0) * a + b) + (int)Main.instance.scAdj;

		if (Main.gameMenu) {
			Main.instance.bgTopY = 320;
		}

		Main.instance.bgLoops = Main.screenWidth / Main.bgWidthScaled + 2;

		if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0) {
			for (int k = 0; k < Main.instance.bgLoops; k++) {
				Main.spriteBatch.Draw(
					TextureAssets.Background[textureSlot].Value,
					new Vector2(Main.instance.bgStartX + Main.bgWidthScaled * k, Main.instance.bgTopY),
					new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]),
					Main.ColorOfSurfaceBackgroundsModified,
					0f,
					default,
					Main.bgScale,
					SpriteEffects.None,
					0f
				);
			}
		}
	}         */ 
        
         
    }
}