using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using mahouSyoujyo.Content.Items.RangedWeapon;
using System;
namespace mahouSyoujyo.Globals
{
    internal class LocalUIPlayer : ModPlayer
    {
        public bool YellowGunChargeBar = false;
        public bool RedSpearChargeBar = false;
        private Texture2D chargebar = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/YellowGunBar").Value;
        private Texture2D chargestrip = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/YellowGunStrip").Value;
        private Texture2D yellowgun = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/YellowGun").Value;
        private Texture2D yellowguncannon = ModContent.Request<Texture2D>("mahouSyoujyo/Content/Assets/YellowGunCannon").Value;
        public override void SetStaticDefaults()
        {

        }
        public override void ResetEffects()
        {
            YellowGunChargeBar = false;
            RedSpearChargeBar = false;
            base.ResetEffects();
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //终曲能量条绘制
            if (Main.LocalPlayer == Player)
            {
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (YellowGunChargeBar) 
                {
                    Vector2 center = Player.RotatedRelativePoint(Player.MountedCenter);
                    int width = chargebar.Width;
                    int height = chargebar.Height;
                    Rectangle rect = new Rectangle(0, 0, width, height);
                    Rectangle rectstrip = new Rectangle((int)(center.X - Main.screenPosition.X -17), (int)(center.Y - Main.screenPosition.Y -36), (int)(36*(float)Player.GetModPlayer<YellowGunCharge>().yellowguncharge / 600f), 8);
                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    Color c = (Player.GetModPlayer<YellowGunCharge>().charged) ? Color.Lerp(Color.Yellow,Color.Transparent,((float)Math.Abs((int)Main.time % 120 - 60) / 60f))  : Color.Yellow;
                    List<Vertex> ve = new List<Vertex>();
                    ve.Add(new Vertex(center - Main.screenPosition + new Vector2(-18,-56),
                              new Vector3(0, 0, 1),
                              Color.White));
                    ve.Add(new Vertex(center - Main.screenPosition + new Vector2(-18,-40),
                              new Vector3(0, 1, 1),
                              Color.White));
                    ve.Add(new Vertex(center - Main.screenPosition + new Vector2(18,-56),
                      new Vector3(1, 0, 1),
                      Color.White));
                    ve.Add(new Vertex(center - Main.screenPosition + new Vector2(18/*+36*(float)Player.GetModPlayer<YellowGunCharge>().yellowguncharge / 600f*/, -40),
                              new Vector3(1, 1, 1),
                              Color.White));
                    if (ve.Count >= 3)
                    {
                        gd.Textures[0] = (Player.GetModPlayer<YellowGunCharge>().shootmode== 0)?yellowgun:yellowguncannon;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                    sb.Draw(chargestrip,rectstrip,c);
                    /*sb.Draw(
                    chargestrip, Player.Center-Main.screenPosition+new Vector2(-18, -40),
                    rectstrip, c, 0,
                    new Vector2(0, 0),
                    new Vector2(0.118f*(float)Player.GetModPlayer<YellowGunCharge>().yellowguncharge / 600f, 0.075f),
                    SpriteEffects.None, 0);*/
                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    sb.Draw(
                    chargebar, center-Main.screenPosition+new Vector2(0, -32),
                    rect, Color.Yellow, 0,
                    new Vector2(width / 2, height / 2),
                    new Vector2(0.15f, 0.1f),
                    SpriteEffects.None, 0);
                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }
                
            }
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }
        public override void LoadData(TagCompound tag)
        {

        }
        public override void SaveData(TagCompound tag)
        {

        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            /*ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.PlayerStateSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((float)clientmouse.X);
            packet.Write((float)clientmouse.Y);
            packet.Send(toWho, fromWho);*/
        }

                // Called in ExampleMod.Networking.cs
                public void ReceivePlayerSync(BinaryReader reader)
        {
            /*float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            clientmouse = new Vector2(x, y);*/
        }

        /*public override void CopyClientState(ModPlayer targetCopy)
        {
            PlayerState clone = (PlayerState)targetCopy;
            clone.clientmouse = clientmouse;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            PlayerState clone = (PlayerState)clientPlayer;

            if (clone.clientmouse != clientmouse)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }*/

        public override void PostUpdate()
        {
            
        }



    }
}
