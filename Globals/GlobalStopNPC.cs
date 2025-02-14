using mahouSyoujyo.Common.Systems;
using Microsoft.Xna.Framework;
using System.Security.Policy;
using Terraria;
using Terraria.ModLoader;

namespace mahouSyoujyo.Globals
{
    public class GlobalStopNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public Vector2 oldCenter = Vector2.Zero;
        public Vector2 oldVelocity = Vector2.Zero;
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (TimeStopSystem.TimeStopping) { return false; }
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
    }
}
