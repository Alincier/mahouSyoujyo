using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria;

public class MyScreenShaderData : ScreenShaderData
{
    public MyScreenShaderData(string passName) : base(passName)
    {
    }
    public MyScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
    {
    }
    public override void Apply()
    {
        base.Apply();
    }
}