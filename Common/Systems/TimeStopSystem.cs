using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace mahouSyoujyo.Common.Systems
{
    public class TimeStopSystem : ModSystem
    {
        public static int TimeLast = 0;
        public static bool TimeStopping = false;
        public static int StopTimeLeft = 0;

        public override void ClearWorld()
        {
            TimeLast = 0;
            StopTimeLeft = 0;
            TimeStopping = false;
        }
        public override void PreUpdateTime()
        {

            if (StopTimeLeft<=0)
            {
                TimeStopping = false;
                TimeLast = 0;
            }
            else
            {
                TimeLast++;
                StopTimeLeft--;
                TimeStopping = true;
            }

        }
        public override void SaveWorldData(TagCompound tag)
        {

        }

        public override void LoadWorldData(TagCompound tag)
        {
            

        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((bool)TimeStopping);
            writer.Write((int)StopTimeLeft);
            writer.Write((int)TimeLast);

        }

        public override void NetReceive(BinaryReader reader)
        {
            TimeStopping = reader.ReadBoolean();
            StopTimeLeft = reader.ReadInt32();
            TimeLast = reader.ReadInt32();

        }
    }
}
