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

float4 NormalGrayScale(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
    return color;
    // �Ҷ� = r*0.3 + g*0.59 + b*0.11
    float gs = dot(float3(0.3, 0.59, 0.11), color.rgb);
    return float4(gs, gs, gs, color.a);
}

float4 DynamicGrayScale(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
    return color;
    // �Ҷ� = r*0.3 + g*0.59 + b*0.11
    float gs = dot(float3(0.3, 0.59, 0.11), color.rgb);
    return float4(color.r+(gs-color.r)*uProgress, color.g+(gs-color.g)*uProgress, color.b+(gs-color.b)*uProgress, color.a);
}

float4 CustomedGrayScale(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
    return color;
    // �Ҷ� = r*0.3 + g*0.59 + b*0.11
    float gs = dot(float3(0.3, 0.59, 0.11), color.rgb);
    //uIntensity����ȫ�����
    if (uIntensity == 1) 
    return float4(color.r+(gs-color.r)*uProgress, color.g+(gs-color.g)*uProgress, color.b+(gs-color.b)*uProgress, color.a);
    //uColor����(��Բ�뾶����Բ�뾶����Ե�����뾶)
    //���벢����target��[0,1]����
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
    //�����ƽ��
    float dotField = dot(centreCoords, centreCoords);
    float innerdotRadias=(uColor.x / uScreenResolution.y) * (uColor.x / uScreenResolution.y);
    float outerdotRadias=(uColor.y / uScreenResolution.y) * (uColor.y / uScreenResolution.y);
    float solidr = (uColor.y-uColor.z<0)?  0 : uColor.y-uColor.z;
    float soliddotRadias=( solidr / uScreenResolution.y) * ( solidr / uScreenResolution.y);
    float degree = 1;
    if (dotField>outerdotRadias) degree = 0;
    else if (dotField>soliddotRadias)
    {
        degree = (outerdotRadias - dotField) / (outerdotRadias - soliddotRadias);
    }
    else if (dotField <= innerdotRadias) degree = 0;
    return float4(color.r+(gs-color.r)*uProgress*degree, color.g+(gs-color.g)*uProgress*degree, color.b+(gs-color.b)*uProgress*degree, color.a);
}
technique GrayScaleTechnique
{
    pass NormalGrayScale
    {    
        PixelShader = compile ps_2_0 NormalGrayScale();
    }
    pass DynamicGrayScale
    {
        PixelShader = compile ps_2_0 DynamicGrayScale();
    }
    pass CustomedGrayScale
    {
        PixelShader = compile ps_2_0 CustomedGrayScale();
    }
}