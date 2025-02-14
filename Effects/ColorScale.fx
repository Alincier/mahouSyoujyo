sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float4 uShaderSpecificData;

float4 NormalColorScale(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
    return color;
    return float4(color.rgb * uColor / 255.0 * uIntensity, color.a);
}

float4 DynamicColorScale(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    return float4(lerp(color.rgb , color.rgb * uColor / 255.0 * uIntensity, uProgress), color.a);
}

technique ColorScaleTechnique
{
    pass NormalColorScale
    {    
        PixelShader = compile ps_2_0 NormalColorScale();
    }
    pass DynamicColorScale
    {
        PixelShader = compile ps_2_0 DynamicColorScale();
    }
}