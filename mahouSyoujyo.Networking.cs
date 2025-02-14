using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content.Items.MeleeWeapon;
using mahouSyoujyo.Globals;
using mahouSyoujyo.Globals.Players;
using System.IO;
using Terraria;
using Terraria.ID;

namespace mahouSyoujyo
{
    // This is a partial class, meaning some of its parts were split into other files. See ExampleMod.*.cs for other portions.
    partial class mahouSyoujyo
    {
		internal enum MessageType : byte
		{
			MGPlayerSync,
			TimeStopSync,
			TimeStopSubmit,
			PlayerStateSync,
            KeyPressedSync,
            ComboStateSync,
        }

		// Override this method to handle network packets sent for this mod.
		//TODO: Introduce OOP packets into tML, to avoid this god-class level hardcode.
		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType) {
				// This message syncs ExampleStatIncreasePlayer.exampleLifeFruits and ExampleStatIncreasePlayer.exampleManaCrystals
				case MessageType.MGPlayerSync:
					byte playerNumber = reader.ReadByte();
					MGPlayer mgPlayer = Main.player[playerNumber].magic();
					mgPlayer.ReceivePlayerSync(reader);

					if (Main.netMode == NetmodeID.Server) {
						// Forward the changes to the other clients
						mgPlayer.SyncPlayer(-1, whoAmI, false);
					}
					break;
                case MessageType.TimeStopSync:
                    byte playerNumber2 = reader.ReadByte();
                    TimeStop timestoper = Main.player[playerNumber2].GetModPlayer<TimeStop>();
                    timestoper.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        timestoper.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MessageType.KeyPressedSync:
                    byte playerNumber3 = reader.ReadByte();
                    KeybindPlayer keybindplayer = Main.player[playerNumber3].GetModPlayer<KeybindPlayer>();
                    keybindplayer.ReceivePlayerSync(reader);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        keybindplayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MessageType.TimeStopSubmit:
                    int time = reader.ReadInt32();
                    if (time > 0)
                        
                    {
                        if (time>TimeStopSystem.StopTimeLeft)
                            TimeStopSystem.StopTimeLeft = time;

                    }
                    if (Main.netMode == NetmodeID.Server)
                    {

                        // Forward the changes to the other clients
                        
                    }
					
                    break;
                case MessageType.PlayerStateSync:
                    byte playerNumber4 = reader.ReadByte();
                    PlayerState state = Main.player[playerNumber4].GetModPlayer<PlayerState>();
                    state.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        state.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MessageType.ComboStateSync:
                    byte playerNumber5 = reader.ReadByte();
                    Comboing combo = Main.player[playerNumber5].GetModPlayer<Comboing>();
                    combo.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        combo.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                default:
					Logger.WarnFormat("mahouSyoujyo: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
}