using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Chat;
using Terraria.Localization;

namespace mahouSyoujyo.Common.Systems
{
    public class WorldMiscUpdate : ModSystem
    {
        public override void PostUpdateWorld()
        {



            // 魔女之夜事件
            if (Main.dayTime)
            {
                WitchNightSystem.TryEnd();
            }
            else
            {
                WitchNightSystem.TryBegin();
            }
            WitchNightSystem.Update();



        }

    }

}
