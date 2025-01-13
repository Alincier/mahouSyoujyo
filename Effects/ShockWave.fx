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


float4 Scalewave(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
    return color;
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
    //uIntensity输入不渐变与否
    if (uIntensity == 1) 
    return tex2D(uImage0, targetCoords+(coords - targetCoords)*uOpacity);
    //uColor输入(内圆半径，外圆半径，边缘扭曲半径)
    //输入并计算target的[0,1]坐标
    //距离的平方
    float dotField = dot(centreCoords, centreCoords);
    float innerdotRadias=(uColor.x / uScreenResolution.y) * (uColor.x / uScreenResolution.y);
    float outerdotRadias=(uColor.y / uScreenResolution.y) * (uColor.y / uScreenResolution.y);
    float solidr = (uColor.y-uColor.z<0)?  0 : uColor.y-uColor.z;
    float soliddotRadias=( solidr / uScreenResolution.y) * ( solidr / uScreenResolution.y);
    float degree = 1;
    if (dotField>outerdotRadias) degree = 1;
    else if (dotField>soliddotRadias)
    {
        degree = ((outerdotRadias - dotField) / (outerdotRadias - soliddotRadias) )* (1-uOpacity) +uOpacity;
    }
    else if (dotField <= innerdotRadias) degree = 1;
    return tex2D(uImage0, targetCoords+(coords - targetCoords)*degree);
}

float4 Shockwave(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
    float dotField = dot(centreCoords, centreCoords);
    float ripple = dotField * uColor.y * 3.1415 - uProgress * uColor.z;

    if (ripple < 0 && ripple > uColor.x * -2 * 3.1415)
    {
        ripple = saturate(sin(ripple));
    }
    else
    {
        ripple = 0;
    }

    float2 sampleCoords = coords + ((ripple * uOpacity / uScreenResolution) * centreCoords);

    return tex2D(uImage0, sampleCoords);
}



technique ShockWaveTechnique
{
    pass NormalScale
    {    
        PixelShader = compile ps_2_0 Scalewave();
    }
    pass NormalShockWave
    {    
        PixelShader = compile ps_2_0 Shockwave();
    }
}
