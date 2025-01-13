using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace mahouSyoujyo.Common.ItemDropRules
{
    public class downedMoonLordDropCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public downedMoonLordDropCondition()
        {
            Description ??= Language.GetOrRegister("Mods.mahouSyoujyo.DropConditions.downedMoonLord");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return NPC.downedMoonlord;
        }
        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }
}
