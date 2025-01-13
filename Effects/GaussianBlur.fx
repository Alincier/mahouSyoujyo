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
float gauss[3][3] = {
    0.075, 0.124, 0.075,
    0.124, 0.204, 0.124,
    0.075, 0.124, 0.075
};

float gaussculTD(float u, float v, float sigma)
{
    float r2 = u * u + v * v;							//模糊半径
    float exponent = -r2 / (2 * sigma*sigma);			//指数
    float denominator = 2 * 3.1415926 * sigma*sigma;	//分母
    return pow(2.7182818, exponent) / denominator;		//结果
}
float gausscul(float x,  float sigma) //一维
{
    float r2 = x * x;							//模糊半径
    float exponent = -r2 / (2 * sigma*sigma);			//指数
    float denominator =sqrt( 2 * 3.1415926) * sigma;	//分母
    return pow(2.7182818, exponent) / denominator;		//结果
}

float4 OnePixelGS(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    float dx = 2 / uScreenResolution.x;
    float dy = 2 / uScreenResolution.y;
    color = float4(0, 0, 0, 0);
    for(int i = -1; i <= 1; i++) {
        for(int j = -1; j <= 1; j++) {
            color += gauss[i + 1][j + 1] * tex2D(uImage0, float2(coords.x + dx * i, coords.y + dy * j));
        }
    }
    return color;
    
}
float4 CustomOnePixelGS(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
     //输入uIntensity标准差sigma
    float dx = 2 / uScreenResolution.x;
    float dy = 2 / uScreenResolution.y;
    float amount = 0;
    color = float4(0, 0, 0, 0);
    float weight = 0;
    for(int i = -1; i <= 1; i++) {
        for(int j = -1; j <= 1; j++) {
            weight = gaussculTD((float)i, (float)j, uIntensity);
            color += weight * tex2D(uImage0, float2(coords.x + dx * i, coords.y + dy * j));
            amount += weight;
        }
    }

    return color / amount;
    
}
float4 CustomPixelHorizonGS(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    //uColor.x为1则取空白区域，0则取有色区域，其他则都取
    if (!any(color) && uColor.x == 0)
        return color;
    if (any(color) && uColor.x == 1)
        return color;
     //输入宽度uColor.y<6,uColor.z标准差sigma
    float dx = 2 / uScreenResolution.x;
    float amount = 0;
    color = float4(0, 0, 0, 0);
    float weight = 0;
    for(int i = -5; i <= 5; i++) {
    if (uColor.y*uColor.y >= i*i)
    {
        weight = gausscul((float)i, uColor.z);
        color += weight * tex2D(uImage0, float2(coords.x + i * dx, coords.y));
        amount += weight;
    }
    }

    return color / amount;
    
}
float4 CustomPixelVerticalGS(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    //uColor.x为1则取空白区域，0则取有色区域，其他则都取
    if (!any(color) && uColor.x == 0)
        return color;
    if (any(color) && uColor.x == 1)
        return color;
     //输入宽度uColor.y<6,uColor.z标准差sigma
    float dy = 2 / uScreenResolution.y;
    float amount = 0;
    color = float4(0, 0, 0, 0);
    float weight = 0;
    for(int j = -5; j <= 5; j++) {
    if (uColor.y*uColor.y >= j*j)
    {
        weight = gausscul((float)j, uColor.z);
        color += weight * tex2D(uImage0, float2(coords.x, coords.y + j * dy));
        amount += weight;
    }
    }

    return color / amount;
    
}
technique GaussianBlur
{
    pass OnePixelGaussian
    {    
        PixelShader = compile ps_2_0 OnePixelGS();
    }
    pass CustomPixelGaussian
    {
        PixelShader = compile ps_2_0 CustomOnePixelGS();
    }
    pass CustomPixelhorizonGS
    {
        PixelShader = compile ps_2_0 CustomPixelHorizonGS();
    }
    pass CustomPixelVerticalGS
    {
        PixelShader = compile ps_2_0 CustomPixelVerticalGS();
    }
}