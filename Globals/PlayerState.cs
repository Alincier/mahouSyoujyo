using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using static mahouSyoujyo.mahouSyoujyo;
using Terraria.GameInput;
using mahouSyoujyo.Content.Items.MeleeWeapon;
namespace mahouSyoujyo.Globals
{
    public class PlayerState : ModPlayer
    {
        public Vector2 clientmouse = Vector2.Zero;
        private bool wasHoldingRight = false;
        public bool mouseRight = false;        // 当前帧右键是否按下
        public bool mouseRightRelease = false; // 右键是否刚释放
        public int mouseRightHoldTime = 0; // 右键持续按下的帧数
        private bool wasHoldingLeft = false;
        public bool mouseLeft = false;        // 当前帧左键是否按下
        public bool mouseLeftRelease = false; // 右键是否刚释放
        public int mouseLeftHoldTime = 0; // 左键持续按下的帧数
        private bool wasHoldingUp = false;
        public bool keyUp = false;        // ↑
        public bool keyUpRelease = false;
        public int keyUpHoldTime = 0;
        private bool wasHoldingDown = false;
        public bool keyDown = false;        // ↑
        public bool keyDownRelease = false;
        public int keyDownHoldTime = 0;
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
            //packet.Write((bool)mouseRight);
            //packet.Write((bool)mouseRightRelease);
            //packet.Write((int)mouseRightHoldTime);
            packet.Send(toWho, fromWho);
        }

        // Called in ExampleMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            //mouseRight = reader.ReadBoolean();
            //mouseRightRelease = reader.ReadBoolean();
            //mouseRightHoldTime = reader.ReadInt32();
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

            if ( clone.clientmouse != clientmouse )
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
        public override void PreUpdate()
        {
            if (Main.LocalPlayer != Player) return;
            wasHoldingLeft = mouseLeft;
            wasHoldingRight = mouseRight;
            wasHoldingUp = keyUp;
            wasHoldingDown = keyDown;
        }
        public override void PostUpdate()
        {
            //捕捉本地玩家的按键状态
            if (Main.LocalPlayer == Player)
            {
                mouseRight = Main.mouseLeft || PlayerInput.Triggers.Current.MouseLeft;
                mouseLeftRelease = wasHoldingLeft && !mouseLeft;
                mouseRight = Main.mouseRight || PlayerInput.Triggers.Current.MouseRight;
                mouseRightRelease = wasHoldingRight && !mouseRight;
                keyUp = !Player.releaseUp || PlayerInput.Triggers.Current.Up;
                keyUpRelease = wasHoldingUp && !keyUp;
                keyDown = !Player.releaseDown || PlayerInput.Triggers.Current.Down;
                keyDownRelease = wasHoldingDown && !keyDown;


                // 更新持续按住时间
                //按住左键
                if (mouseLeft)
                {
                    if (!wasHoldingLeft)
                        mouseLeftHoldTime = 0;
                    mouseLeftHoldTime++;
                }
                else
                {
                    mouseLeftHoldTime = 0;
                }
                //按住右键
                if (mouseRight)
                {
                    if (!wasHoldingRight)
                        mouseRightHoldTime = 0;
                    mouseRightHoldTime++;
                }
                else
                {
                    mouseRightHoldTime = 0;
                }
                //按住↑
                if (keyUp)
                {
                    if (!wasHoldingUp)
                        keyUpHoldTime = 0;
                    keyUpHoldTime++;
                }
                else
                {
                    keyUpHoldTime = 0;
                }
                //按住↓
                if (keyDown)
                {
                    if (!wasHoldingDown)
                        keyDownHoldTime = 0;
                    keyDownHoldTime++;
                }
                else
                {
                    keyDownHoldTime = 0;
                }
                clientmouse = Main.MouseWorld;
            }
        }



    }
}
