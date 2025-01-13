using mahouSyoujyo.Backgrounds;
using mahouSyoujyo.Common.Configs;
using mahouSyoujyo.Content.NPCs.BOSSes.Majo_Consciousness;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace ExampleMod.Content.Biomes
{
    // Shows setting up two basic biomes. For a more complicated example, please request.
    public class ConsciousSky : ModBiome
    {
        // Select all the scenery
    //    public override ModWaterStyle WaterStyle => ModContent.GetInstance<ExampleWaterStyle>(); // Sets a water style for when inside this biome
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle =>ModContent.GetInstance<ConsciousBackgroundStyle>();
        //    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

        // Select Music
        public override int Music => Getmusic();//MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");
        private int Getmusic()
        {
            int majoindex = NPC.FindFirstNPC(ModContent.NPCType<Majo_Consciousness>());
            if (majoindex >= 0 && Main.npc[majoindex].active)
            {
                return ((Majo_Consciousness)Main.npc[majoindex].ModNPC).Music;
            }
            return 0;
        }
    //    public override int BiomeTorchItemType => ModContent.ItemType<ExampleTorch>();
    //    public override int BiomeCampfireItemType => ModContent.ItemType<ExampleCampfire>();

        // Populate the Bestiary Filter
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

        // Calculate when the biome is active.
        public override bool IsBiomeActive(Player player)
        {
            return (!ModContent.GetInstance<ClientConfigs>().CloseConsciousEnchantment) && NPC.AnyNPCs(ModContent.NPCType<Majo_Consciousness>());
        }

        // Declare biome priority. The default is BiomeLow so this is only necessary if it needs a higher priority.
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    }
}
