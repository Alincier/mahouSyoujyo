
using mahouSyoujyo.Content.Items.Consumables.BOSS_Summon;
using mahouSyoujyo.Content.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using mahouSyoujyo.Globals;
using mahouSyoujyo.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Graphics;
using Humanizer;
using System.Linq;
using mahouSyoujyo.Common.Configs;


namespace mahouSyoujyo.Content.Items.SpecialWeapon;

public class TimePlate : ModItem
{

    //public string user_name = "";
    MGPlayer mgplayer => Main.LocalPlayer.GetModPlayer<MGPlayer>();
    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));
        // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
        ItemID.Sets.ItemNoGravity[Item.type] = false;
    }
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 36;
        Item.value = Item.sellPrice(1, 0, 0, 0);
        Item.rare = ItemRarityID.Purple;
        // 使用属性
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.UseSound = new SoundStyle($"mahouSyoujyo/Radio/Sound/clocksound");
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        Item.consumable = true; // 设定为消耗品（但不消耗）
        Item.accessory = true;
        //Item.lifeRegen = 5;
        Item.defense = 5;
        Item.shopCustomPrice =Item.buyPrice(20, 0, 0, 0);
    }
    public override void NetSend(BinaryWriter writer)
    {
        //writer.Write(user_name);
        base.NetSend(writer);
    }
    public override void NetReceive(BinaryReader reader)
    {
        //user_name = reader.ReadString();
        base.NetReceive(reader);
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        Texture2D tex = TextureAssets.Item[Type].Value;
        int height = tex.Height / 3;
        int width = tex.Width;
        Rectangle rect = new Rectangle(0, ((int)Main.time % 12 /4)*height, width, height);
        spriteBatch.Draw(
            tex, Item.Center-Main.screenPosition,
            rect, Color.White, rotation,
            new Vector2(width / 2, height / 2),
            new Vector2(0.75f, 0.75f),SpriteEffects.None,0
            );
        return false;
    }
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        //spriteBatch.End();
        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        return true;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-32,-64);
    }
    public override void UpdateInventory(Player player)
    {
        //if (user_name == "" || user_name != player.name) return;
        if (!player.GetModPlayer<TimeStop>().bind)
        {
            player.GetModPlayer<TimeStop>().bind = true;
            if (Main.netMode == NetmodeID.MultiplayerClient) player.GetModPlayer<TimeStop>().SyncPlayer(-1, player.whoAmI, false);
        }
    }
    public override void UpdateEquip(Player player)
    {
        base.UpdateEquip(player);
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        //if (user_name == "" || user_name != player.name) return;
        // if (user_name == player.name)
        if (!player.GetModPlayer<TimeStop>().bind) 
        {
            player.GetModPlayer<TimeStop>().bind = true;
            if (Main.netMode == NetmodeID.MultiplayerClient) player.GetModPlayer<TimeStop>().SyncPlayer(-1, player.whoAmI, false);
        }
        //关闭了视觉效果就不时停
        if (hideVisual) return;
        MGPlayer mgplayer1 = player.GetModPlayer<MGPlayer>();
        if (!mgplayer1.magia) return;
        mgplayer1.polluted_time += ((mgplayer1.relief) ? 1 : 2)*60;
        if (!mgplayer1.notInDespair()) return;
        TimeStopSystem.StopTimeLeft = (TimeStopSystem.StopTimeLeft>30) ? TimeStopSystem.StopTimeLeft : 30;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)mahouSyoujyo.MessageType.TimeStopSubmit);
            packet.Write((int)TimeStopSystem.StopTimeLeft);
            packet.Send(-1, -1);
        }
    }
    public override void OnSpawn(IEntitySource source)
    {
    }
    private bool notequiped(Player player)
    {
        foreach (Item item in player.armor)
            if (item.type == this.Type) return false;
        return true;
    }

    public override bool CanEquipAccessory(Player player, int slot, bool modded)
    {
        //if (Main.hardMode && notequiped(player)) CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y-32, player.width, player.height), new Color(192, 45, 192, 255),
        //    this.GetLocalizedValue("equiptip").FormatWith(player.name), dramatic: true, dot: true);
        return (Main.hardMode);
    }
    public override bool CanUseItem(Player player)
    {
        return true;
    }
    public override bool AltFunctionUse(Player player)
    {
        return false;
    }
    public override bool? UseItem(Player player)
    {
        MGPlayer mgplayer = player.GetModPlayer<MGPlayer>();
        if (!mgplayer.magia)
        {
            if (ModContent.GetInstance<ClientConfigs>().ShowTimeStopTips) CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y-32, player.width, player.height), new Color(192, 45, 192, 255),
                this.GetLocalizedValue("nomagiatip").FormatWith(player.name), dramatic: true);
            return true;
        }
            
        mgplayer.polluted_time += ((mgplayer.relief) ? 1 : 2)*60*60;
        if (!mgplayer.notInDespair())
        {
            if (ModContent.GetInstance<ClientConfigs>().ShowTimeStopTips) CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y-32, player.width, player.height), new Color(192, 45, 192, 255),
                this.GetLocalizedValue("despairtip").FormatWith(player.name), dramatic: true);
            return true;
        }
        TimeStopSystem.StopTimeLeft = (TimeStopSystem.StopTimeLeft>180) ? TimeStopSystem.StopTimeLeft : 180;
        if (ModContent.GetInstance<ClientConfigs>().ShowTimeStopTips) CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y-32, player.width, player.height), new Color(192,45,192, 255),
            this.GetLocalizedValue("usetip").FormatWith(player.name), dramatic: true);
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)mahouSyoujyo.MessageType.TimeStopSubmit);
            packet.Write((int)TimeStopSystem.StopTimeLeft);
            packet.Send(-1, -1);
        }
        return true;
    }
    public override bool ConsumeItem(Player player)
    {
        return false; 
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
    public override void LoadData(TagCompound tag)
    {

        //if (tag.TryGet<string>(this.ToString(), out string name)) user_name = name;
        //else user_name = "";

        base.LoadData(tag);
    }
    public override void SaveData(TagCompound tag)
    {
        //tag.Add(this.ToString(), user_name);
        base.SaveData(tag);
    }
    public override void PreReforge()
    {
        base.PreReforge();
    }
    public override void PostReforge()
    {
    }
    public override bool CanRightClick()
    {
        //Main.player[Item.playerIndexTheItemIsReservedFor].ClearBuff(ModContent.BuffType<MagicGirlPover>());
        return false;
    }
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {

        // Here we give the item name a rainbow effect.
        foreach (TooltipLine line in tooltips)
        {
            //Main.NewText(line.Name);
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                line.OverrideColor = Main.DiscoColor;
            }
            if (line.Mod == "Terraria" && line.Name == "Consumable")
            {
                line.Hide();
            }
        }
        tooltips.Add(new TooltipLine(Mod, "MagicGirlTips", this.GetLocalizedValue("MagicGirlTips")) { OverrideColor = Main.DiscoColor });

    }

    // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    public override void AddRecipes()
    {
       // CreateRecipe()
            //.AddIngredient(ItemID.DirtBlock, 1)
            //.AddTile(TileID.WorkBenches)
       //     .AddCondition(new Condition("hasPower", () => Main.LocalPlayer.HasBuff<MagicGirlPover>()))
       //     .AddCustomShimmerResult(ModContent.ItemType<DespairSoul>(), 1)
       //     .Register();
    }
}