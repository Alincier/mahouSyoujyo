using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace mahouSyoujyo.Content.Items.Consumables.StatIncreaseItem
{
    public class BlessOfCircles : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            //Item.CloneDefaults(ItemID.LifeFruit);
            Item.maxStack = 1;
            Item.consumable = true;
            Item.width = 60;
            Item.height = 60;
            Item.useStyle = 4;
            Item.noUseGraphic = true;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item4;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 50, 0, 0);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D tex = TextureAssets.Item[Type].Value;
            int height = tex.Height;
            int width = tex.Width;
            Rectangle rect = new Rectangle(0, 0, width, height);
            spriteBatch.Draw(
                tex, Item.Center-Main.screenPosition,
                rect, Color.White, rotation,
                new Vector2(width / 2, height / 2),
                new Vector2(1f, 1f), SpriteEffects.None, 0
                );
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return DownedBossSystem.downedConciousBoss;
        }

        public override bool? UseItem(Player player)
        {

            if (player.GetModPlayer<MGPlayer>().relief)
            {
                return null;
            }
            player.GetModPlayer<MGPlayer>().relief = true;
            return true;
        }
    }
}
