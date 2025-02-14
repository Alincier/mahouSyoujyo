using mahouSyoujyo.Common.Systems;
using Terraria;
using Terraria.ModLoader;

namespace mahouSyoujyo.Globals
{
    public class GlobalStopProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int runningFrame = 0;
        public int runningFrameTime = 0;


        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (TimeStopSystem.TimeStopping) { return false; }
            return base.CanHitPlayer(projectile, target);
        }
    }
}
