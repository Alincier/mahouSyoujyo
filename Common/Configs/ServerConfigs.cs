using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace mahouSyoujyo.Common.Configs
{
    public class ServerConfigs : ModConfig
    {
        // ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
        // ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviors
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // The things in brackets are known as "Attributes".

        [Header("BOSS")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category. 
        [DefaultValue(true)] // This sets the configs default value.
        public bool MajoSpawnInDespair;
        [DefaultValue(false)] // This sets the configs default value.
        public bool CustomedMajoConsciousness;

        [DrawTicks]
        [OptionStrings(new string[] { "KingSlime", "TheEyeOfCthulhu", "EvilBoss", "QueenBee", "Skeletron",
                                        "Deerclops", "WallOfFlesh", "QueenSlime", "TheTwins", "TheDestroyer", "SkeletronPrime",
                                        "Plantera", "Golem", "DukeFishron", "EmpressOfLight", "LunaticCultist", "MoonLord"})]
        [DefaultListValue("KingSlime")]
        public List<string> bosslist = new List<string>();
        
        [Header("Item")]
        [DefaultValue(true)]
        public bool QBshop;

        [ReloadRequired]
        [DefaultValue(false)]
        public bool NoSeedDrop;

        [DefaultValue(true)]
        public bool CanStopTimeWhenItemUseOverZero;
        // A method annotated with OnDeserialized will run after deserialization. You can use it for enforcing things like ranges, since Range and Increment are UI suggestions.
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            // RangeAttribute is just a suggestion to the UI. If we want to enforce constraints, we need to validate the data here. Users can edit config files manually with values outside the RangeAttribute, so we fix here if necessary.
            // Both enforcing ranges and not enforcing ranges have uses in mods. Make sure you fix config values if values outside the range will mess up your mod.
            //去重
            bosslist = bosslist.Distinct().ToList();
        }
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
        {
            return true;
        }
    }
}