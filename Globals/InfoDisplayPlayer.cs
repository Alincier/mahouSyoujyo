using mahouSyoujyo.Content;
using Terraria;
using Terraria.ModLoader;

namespace mahouSyoujyo.Globals
{
    public class InfoDisplayPlayer : ModPlayer
    {
        // Flag checking
        public bool showPollution;

        // Make sure to use the right Reset hook.
        public override void ResetInfoAccessories()
        {
            showPollution = false;
        }
        // If we have another nearby player on our team, we want to get their info accessories working on us,
        // just like in vanilla. This is what this hook is for.
        public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer)
        {
            //if (otherPlayer.GetModPlayer<InfoDisplayPlayer>().showPollution)
            //{
            //    showPollution = true;
            //}
        }
    }
}
