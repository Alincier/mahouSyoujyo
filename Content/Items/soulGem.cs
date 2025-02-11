
using mahouSyoujyo.Content.Items.Consumables.BOSS_Summon;
using mahouSyoujyo.Content.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using mahouSyoujyo.Common;
using mahouSyoujyo.Globals;


namespace mahouSyoujyo.Content.Items
{
    public class soulGem : ModItem
    {

        public string user_name = "";
        MGPlayer mgplayer => Main.LocalPlayer.magic();
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));
            // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            ItemID.Sets.ItemNoGravity[Item.type] = false;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 0, 0, 1);
            Item.rare = ItemRarityID.Purple;
            // 使用属性
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item4;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Item.consumable = true; // 设定为消耗品（但不消耗）
            Item.accessory = true;
            //Item.lifeRegen = 5;
            Item.defense = 5;
            
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(user_name);
            base.NetSend(writer);
        }
        public override void NetReceive(BinaryReader reader)
        {
            user_name = reader.ReadString();
            base.NetReceive(reader);
        }
        public override void UpdateInventory(Player player)
        {
            if (user_name == "" || user_name != player.name) return;
            mgplayer.left_gem_time = 0;
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (user_name == "" || user_name != player.name) return;
            MGPlayer mgplayer = player.magic();
            if (user_name == player.name)
            {
                mgplayer.left_gem_time = 0;
                //如果有魔法少女buff就应用属性
                if (player.HasBuff<Buffs.MagicGirlPover>())
                {
                    player.GetDamage(DamageClass.Generic) += mgplayer.damage_bonus / 100f;
                    player.GetCritChance(DamageClass.Generic) += mgplayer.crit_bonus / 1f;
                    player.statDefense += mgplayer.defense_bonus;
                    player.statLifeMax2 += player.statLifeMax*mgplayer.health_bonus / 100;
                    player.statManaMax2 +=  player.statManaMax*mgplayer.mana_bonus / 100;
                    player.lifeRegen +=  mgplayer.regen_bonus;
                    player.endurance = Math.Min(1f, player.endurance + mgplayer.enduce_bonus / 100f);
                    player.moveSpeed +=  mgplayer.speed_bonus / 100f;
                    player.manaCost  = Math.Max(0f, player.manaCost- mgplayer.mana_reduce / 100f);
                    player.GetArmorPenetration(DamageClass.Generic) += mgplayer.armor_penetration;
                    player.maxMinions += mgplayer.summon_bonus;
                }
            }
            //player.GetAttackSpeed(DamageClass.Melee) += 0.1f; // 玩家近战攻速增加10%
            //if (hideVisual) player.GetCritChance(DamageClass.Magic) += 20f; // 玩家魔法暴击率增加20%,当关掉饰品可见性时
        }
        public override void OnSpawn(IEntitySource source)
        {

            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<MagicGirlPover>())) 
                user_name = Main.LocalPlayer.name;
        }
        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            if (user_name!="" && user_name != player.name) return false;
            player.AddBuff(ModContent.BuffType<MagicGirlPover>(), 10);
            user_name=player.name;
            //Item.NetStateChanged();
            return true;
        }
        public override bool ConsumeItem(Player player)
        {
            return false; // true是消耗，false不消耗
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void LoadData(TagCompound tag)
        {

            if (tag.TryGet<string>(this.ToString(), out string name)) user_name = name;
            else user_name = "";
            
            base.LoadData(tag);
        }
        public override void PreReforge()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            mgplayer.lastReforge=user_name;
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncItem, Item.whoAmI);
                NetMessage.SendData(MessageID.SyncPlayer,Main.myPlayer);
            }
            base.PreReforge();
        }
        public override void PostReforge()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (mgplayer.lastReforge != null)
                user_name=mgplayer.lastReforge;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncItem, -1, Item.whoAmI);
            base.PostReforge();
        }
        public override bool CanRightClick()
        {
            //Main.player[Item.playerIndexTheItemIsReservedFor].ClearBuff(ModContent.BuffType<MagicGirlPover>());
            return false;
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add(this.ToString(),user_name);
            base.SaveData(tag);
        }
        public override void OnCraft(Recipe recipe)
        {

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            
            // Here we give the item name a rainbow effect.
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "ItemName")
                {
                    line.OverrideColor = Main.DiscoColor;
                }
                if (line.Mod == "Terraria" && line.Name == "Consumable")
                {
                    line.Hide();
                }
            }
            tooltips.Add(new TooltipLine(Mod, "ChargeTips", this.GetLocalizedValue("Charge")) { OverrideColor = Main.cursorColor });
            tooltips.Add(new TooltipLine(Mod, "RescissionTips", this.GetLocalizedValue("Rescission")) { OverrideColor = Main.cursorColor });
            if (user_name == "") 
                tooltips.Add(new TooltipLine(Mod, "Ownership", this.GetLocalizedValue("No_owner")) { OverrideColor = Main.creativeModeColor });
             else
            { 
                tooltips.Add(new TooltipLine(Mod,"Ownership", this.GetLocalizedValue("Has_owner")+user_name) { OverrideColor = Main.creativeModeColor });
                //污染进度和污染时间
                tooltips.Add(new TooltipLine(Mod, "Pollution", 
                    String.Format(this.GetLocalizedValue("Pollution"),
                        Math.Round(((double)mgplayer.polluted_time/(double)(mgplayer.deadline*3600)*100),2),
                        mgplayer.deadline)
                        ) { OverrideColor = Main.creativeModeColor });
            }
            tooltips.Add(new TooltipLine(Mod, "PowerTips", String.Format(this.GetLocalizedValue("PowerTips"), mgplayer.power)) { OverrideColor = Main.DiscoColor });
            if (mgplayer.power<=0)
                tooltips.Add(new TooltipLine(Mod, "No_Power", this.GetLocalizedValue("No_Power")) { OverrideColor = Main.creativeModeColor });
           
            //属性展示
            if (mgplayer.damage_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGDamage", String.Format(Language.GetTextValue("CommonItemTooltip.PercentIncreasedDamage"), mgplayer.damage_bonus)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.crit_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGCrit", String.Format(Language.GetTextValue("CommonItemTooltip.PercentIncreasedCritChance"), mgplayer.crit_bonus)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.defense_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGDef", String.Format(Language.GetTextValue("CommonItemTooltip.IncreasesDefenseBy"), mgplayer.defense_bonus)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.health_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGLife", String.Format(Language.GetTextValue("CommonItemTooltip.IncreasesMaxLifeByPercent"), mgplayer.health_bonus)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.regen_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGRegen", String.Format(this.GetLocalizedValue("Regen"), mgplayer.regen_bonus)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.mana_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGMana", String.Format(Language.GetTextValue("CommonItemTooltip.IncreasesMaxManaByPercent"), mgplayer.mana_bonus)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.mana_reduce>0)
                tooltips.Add(new TooltipLine(Mod, "MGManaReduce", String.Format(Language.GetTextValue("CommonItemTooltip.PercentReducedManaCost"), mgplayer.mana_reduce)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.enduce_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGEndure", String.Format(Language.GetTextValue("CommonItemTooltip.ReducesDamageTakenByPercent"), mgplayer.enduce_bonus)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.speed_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGSpeed", String.Format(Language.GetTextValue("CommonItemTooltip.PercentIncreasedMovementSpeed"), mgplayer.speed_bonus)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.armor_penetration>0)
                tooltips.Add(new TooltipLine(Mod, "MGArmorPenetration", String.Format(Language.GetTextValue("CommonItemTooltip.IncreasesArmorPenBy"), mgplayer.armor_penetration)) { OverrideColor = Main.creativeModeColor });
            if (mgplayer.summon_bonus>0)
                tooltips.Add(new TooltipLine(Mod, "MGSummon", String.Format(Language.GetTextValue("CommonItemTooltip.IncreasesMaxMinionsBy"), mgplayer.summon_bonus)) { OverrideColor = Main.creativeModeColor });
        }
        
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                //.AddIngredient(ItemID.DirtBlock, 1)
                //.AddTile(TileID.WorkBenches)
                .AddCondition(new Condition("hasPower", () => Main.LocalPlayer.HasBuff<MagicGirlPover>()))
                .AddCustomShimmerResult(ModContent.ItemType<DespairSoul>(), 1)
                .AddOnCraftCallback(RecipeCallbacks.SoulGemCallBack)
                .Register();
        }
    }
}