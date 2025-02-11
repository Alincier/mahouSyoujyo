using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using mahouSyoujyo.Content.Items;

namespace mahouSyoujyo.Common
{
    internal class RecipeCallbacks
    {
        /* 示例
        public static void RandomlySpawnFireworks(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (Main.rand.NextBool(3))
            {
                int fireworkProjectile = ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4);
                Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), Main.LocalPlayer.Top, new Microsoft.Xna.Framework.Vector2(0, -Main.rand.NextFloat(2f, 4f)).RotatedByRandom(0.3f), fireworkProjectile, 0, 0, Main.myPlayer);

                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), ItemID.Confetti, 5);
            }
        }
        public static void DontConsumeChain(Recipe recipe, int type, ref int amount)
        {
            if (type == ItemID.Chain)
            {
                amount = 0;
            }
        }
        */
        public static void SoulGemCallBack(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (item.ModItem is soulGem gem)
            {
                if (Main.netMode == NetmodeID.Server) return;
                gem.user_name = Main.LocalPlayer.name;
                if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem);
            }
        }
    }
}
