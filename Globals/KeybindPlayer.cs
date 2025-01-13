using mahouSyoujyo.Common.Configs;
using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content.Items.SpecialWeapon;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static mahouSyoujyo.mahouSyoujyo;

namespace mahouSyoujyo.Globals.Players
{
    //public class ExampleModAccessorySlot1 : ModAccessorySlot
    //{
        // If the class is empty, everything will default to a basic vanilla slot.
    //}
    // See Common/Systems/KeybindSystem for keybind registration.
    public class KeybindPlayer : ModPlayer
    {
        public bool StopTimeUse = false;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.KeyPressedSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((bool)StopTimeUse);
            packet.Send(toWho, fromWho);
        }

        // Called in ExampleMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            StopTimeUse = reader.ReadBoolean();
            
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Main.LocalPlayer == Player && KeybindSystem.StopTimeUse.JustPressed && (ModContent.GetInstance<ServerConfigs>().CanStopTimeWhenItemUseOverZero || Player.ItemTimeIsZero) )
            {
                foreach (Item item in Player.inventory)
                {
                    if (item.active &&  item.ModItem is TimePlate tp)
                    {
                        StopTimeUse = true;
                        if (Main.netMode == NetmodeID.MultiplayerClient) SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
                        return;
                    }
                }
                if (Player.HasItemInAnyInventory(ModContent.ItemType<TimePlate>()))
                {
                    StopTimeUse = true;
                    if (Main.netMode == NetmodeID.MultiplayerClient) SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
                    return;
                }
                /*if (Player.GetModPlayer<TimeStop>().bind)
                {
                    StopTimeUse = true;
                    if (Main.netMode == NetmodeID.MultiplayerClient) SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
                    return;
                }*/

            }
        }
        public override void PostUpdate()
        {
            if (StopTimeUse)
            {
                Item item = ModContent.GetModItem(ModContent.ItemType<TimePlate>()).Item;
                if (!ModContent.GetInstance<ServerConfigs>().CanStopTimeWhenItemUseOverZero)
                {
                    Player.ApplyItemTime(item, callUseItem: true);
                    Player.ApplyItemAnimation(item);
                }
                else item.ModItem.UseItem(Player);
                SoundEngine.PlaySound(item.UseSound, Player.Center);
                StopTimeUse = false;
            }
        }
        /*public override void CopyClientState(ModPlayer targetCopy)
        {
            KeybindPlayer clone = (KeybindPlayer)targetCopy;
            clone.StopTimeUse = StopTimeUse;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            KeybindPlayer clone = (KeybindPlayer)clientPlayer;

            if (StopTimeUse == true)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }*/
    }
        }
