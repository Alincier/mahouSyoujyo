using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using static mahouSyoujyo.mahouSyoujyo;
namespace mahouSyoujyo.Globals
{
    public class PlayerState : ModPlayer
    {
        public Vector2 clientmouse = Vector2.Zero;
        public override void SetStaticDefaults()
        {

        }
        public override void LoadData(TagCompound tag)
        {

        }
        public override void SaveData(TagCompound tag)
        {

        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.PlayerStateSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((float)clientmouse.X);
            packet.Write((float)clientmouse.Y);
            packet.Send(toWho, fromWho);
        }

        // Called in ExampleMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            clientmouse = new Vector2(x, y);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            PlayerState clone = (PlayerState)targetCopy;
            clone.clientmouse = clientmouse;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            PlayerState clone = (PlayerState)clientPlayer;

            if ( clone.clientmouse != clientmouse)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }

        public override void PostUpdate()
        {
            if (Main.LocalPlayer == Player)
            {
                clientmouse = Main.MouseWorld;
            }
        }



    }
}
