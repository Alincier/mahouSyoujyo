using Microsoft.Xna.Framework;
using Terraria;
using mahouSyoujyo.Globals;

namespace mahouSyoujyo
{
    public static partial class MagicUtils
    {
        public static PlayerState input(this Player player) => player.GetModPlayer<PlayerState>();
        public static MGPlayer magic(this Player player) => player.GetModPlayer<MGPlayer>();
    }
}
