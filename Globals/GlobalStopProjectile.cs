using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace mahouSyoujyo.Globals
{
    public class GlobalStopProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int runningFrame = 0;
        public int runningFrameTime = 0;

    }
}
