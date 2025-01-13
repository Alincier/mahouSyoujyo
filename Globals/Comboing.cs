using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using static mahouSyoujyo.mahouSyoujyo;
using Terraria.Audio;
using Terraria.ID;

namespace mahouSyoujyo.Globals
{
    public class Comboing : ModPlayer
    {
        public string combo = "";
        public int purryCD = 0;
        public int CD = 0;
        public int keeping = 0;
        public int purry_bonus = 0;
        public int supertime = 0;
        public int purryCount = 0;
        public int[] printframe = new int[6] { 0, 0, 0, 0, 0, 0 };//记录绘制的计数帧数
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.ComboStateSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((int)purryCount);
            packet.Send(toWho, fromWho);
        }

        // Called in ExampleMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            purryCount = reader.ReadInt32();
        }

        public override void PostUpdate()
        {
            if (purryCount >=6)
            {
                if (Main.LocalPlayer == Player) SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/Blood_skill"));
                supertime = 420;
                purryCount = 0;
                //for (int i=0;i<20;i++)
            }
            if (supertime > 0)  
            {
                supertime--;
                Dust.NewDustDirect(Player.Center-new Vector2(16f, 24f),
                    32, 32, DustID.ArgonMoss, newColor: Color.BlueViolet);
                if (Main.LocalPlayer == Player) 
                {
                    if (supertime <= 120)
                    {
                        if (supertime % 30 ==0)
                            SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/heartbeat"));
                    }
                    else if (supertime % 60 ==0)
                        SoundEngine.PlaySound(new SoundStyle($"mahouSyoujyo/Radio/Sound/heartbeat"));
                }
                    
            }
            //Main.NewText(CD);
            if (purry_bonus>0) purry_bonus--;
            if (purryCD>0) purryCD--;
            if (CD>0) CD--;
            if (keeping>0) keeping--;
            if (keeping == 0)
            {
                combo = "";
                //purryCount = 0;
            }

            base.PostUpdate();

        }
        public override void CopyClientState(ModPlayer targetCopy)
        {
            Comboing clone = (Comboing)targetCopy;
            clone.purryCount = purryCount;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            Comboing clone = (Comboing)clientPlayer;

            if (clone.purryCount != purryCount)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
    }
}
