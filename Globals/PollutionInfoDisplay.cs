using Humanizer;
using mahouSyoujyo.Content;
using mahouSyoujyo.Globals;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace mahouSyoujyo.Globals
{
    public class PollutionInfoDisplay : InfoDisplay
    {
        public static Color RedInfoTextColor => new(255, 19, 19, Main.mouseTextColor);

        // By default, the vanilla circular outline texture will be used. 
        // This info display has a square icon instead of a circular one, so we need to use a custom outline texture instead of the vanilla outline texture.
        // You will only need to use a custom hover texture if your info display icon doesn't perfectly match the shape that vanilla info displays use
        //public override string HoverTexture => Texture + "_Hover";
        public override string HoverTexture => Texture + "_Hover";
        // This dictates whether or not this info display should be active
        public override bool Active()
        {
            return Main.LocalPlayer.GetModPlayer<InfoDisplayPlayer>().showPollution;
        }

        // Here we can change the value that will be displayed in the game
        public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
        {
            MGPlayer mgplayer = Main.LocalPlayer.GetModPlayer<MGPlayer>();
            bool magia = mgplayer.magia;
            double time = mgplayer.polluted_time;
            double deadline = mgplayer.deadline;


            double PollutionDegree = Math.Round((time/(deadline*3600)*100), 2);
            double LeftTime = Math.Round(deadline * ( 1- Math.Clamp(time/(deadline*3600),0,1) ), 2);

            if (PollutionDegree < 50)
            {
                displayColor = Color.Green;
            }
            else if (PollutionDegree < 80)
            {
                displayColor = GoldInfoTextColor;
            }
            else if (PollutionDegree < 100)
            {
                displayColor = Color.OrangeRed;
            }
            else 
            {
                displayColor = RedInfoTextColor;
            }

            return magia ? this.GetLocalizedValue("ToolTip").FormatWith(PollutionDegree, LeftTime) :
                this.GetLocalizedValue("Notmagia");
                //Language.GetTextValue("Mods.mahouSyoujyo.Items.TimePlate.nomagiatip");
        }
    }

}
