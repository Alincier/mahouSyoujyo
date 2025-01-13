sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
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
float4 MajoShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (any(color)) return color;
    //输入uShaderSpecificData，uSecondaryColor，uColor
    float2 center=float2(uSecondaryColor.x,uSecondaryColor.y) /2 + uShaderSpecificData.ba;//中心位移
    float sr=uShaderSpecificData.x / 2; //内宽度
    float lr=uShaderSpecificData.y / 2; //内高度
    float drawwidth =uSecondaryColor.z;//渐变宽度
    float2 target =coords*(uSecondaryColor.x,uSecondaryColor.y/6)-center;
    float theta = acos(dot(target,float2(1,0)) / 2 /length(target));
    float2 edge=(sr*cos(theta),lr*sin(theta));
    float tr=length(edge);
    float tlen = length(target); 
    float fincolor = smoothstep(tr,tr+drawwidth,tlen);
    return (uColor,1)+(float4(0,0,0,0)-float4(uColor,1))*fincolor;
    
}

technique SmoothBackground
{
    pass MajoConsciousnessShader
    {
        PixelShader = compile ps_2_0 MajoShader();
    }
}